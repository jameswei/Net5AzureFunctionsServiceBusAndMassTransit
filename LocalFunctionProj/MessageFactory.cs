using System;
using System.Linq.Expressions;
using System.Reflection;
using GreenPipes.Internals.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace LocalFunctionProj
{
    // 通过反射装配 message
    public static class MessageFactory
    {
        public static Message CreateMessage(byte[] body, FunctionContext context)
        {
            var result = new Message(body);
            result.SetPrimitiveProperty(m => m.MessageId, context);
            result.SetPrimitiveProperty(m => m.ContentType, context);
            result.SetJsonProperty(m => m.UserProperties, context);

            var sysProperties = result.SystemProperties;
            sysProperties.SetPrimitiveProperty(s => s.DeliveryCount, context);
            sysProperties.SetPrimitiveProperty(s => s.SequenceNumber, context);
            sysProperties.SetPrimitiveProperty(s => s.EnqueuedTimeUtc, context);

            // this one we cannot directly set because it's computed from a field
            var lockToken =
                context.GetPrimitiveValue<Message.SystemPropertiesCollection, string>(s => s.LockToken);
            var lockTokenGuid = Guid.Parse(lockToken);
            sysProperties.SetField("lockTokenGuid", lockTokenGuid);

            // this one we need to do indirectly because  ExpiresAtUtc is computed
            // while TTL is settable
            var expiresAtUtc = context.GetPrimitiveValue<Message, DateTime>(m => m.ExpiresAtUtc);
            result.TimeToLive = expiresAtUtc.Subtract(sysProperties.EnqueuedTimeUtc);

            return result;
        }

        static TProperty GetPrimitiveValue<TOwner, TProperty>(this FunctionContext self,
            Expression<Func<TOwner, TProperty>> accessor)
        {
            var rawValue = self.GetRawValue(accessor);
            if (typeof(TProperty) == typeof(DateTime)) rawValue = rawValue?.ToString()?.Trim('"');
            return (TProperty)Convert.ChangeType(rawValue, typeof(TProperty));
        }

        static object GetRawValue<TOwner, TProperty>(this FunctionContext self,
            Expression<Func<TOwner, TProperty>> accessor)
        {
            var property = PropertyInfo(accessor);
            var name = property.Name;
            if (self.BindingContext.BindingData.TryGetValue(name, out var rawValue)) return rawValue;
            return default;
        }

        static PropertyInfo PropertyInfo<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> accessor)
        {
            var result = accessor.GetMemberExpression().Member as PropertyInfo;
            if (result == default)
                throw new ArgumentException("The accessor doesn't access a property", nameof(accessor));
            return result;
        }

        static void SetField(this object self, string fieldName, object value)
        {
            var field = self.GetType()
                .GetField(fieldName,
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);
            if (field == default) throw new ArgumentException("There is no such field", fieldName);
            field.SetValue(self, value);
        }

        static void SetJsonProperty<TOwner, TProperty>(this TOwner self,
            Expression<Func<TOwner, TProperty>> accessor,
            FunctionContext context)
        {
            var json = (string)context.GetRawValue(accessor);
            var value = JsonConvert.DeserializeObject(json, typeof(TProperty));
            self.SetProperty(accessor, value);
        }

        static void SetPrimitiveProperty<TOwner, TProperty>(this TOwner self,
            Expression<Func<TOwner, TProperty>> accessor,
            FunctionContext context)
        {
            var value = context.GetPrimitiveValue(accessor);
            self.SetProperty(accessor, value);
        }

        static void SetProperty<TOwner, TProperty>(this TOwner self,
            Expression<Func<TOwner, TProperty>> accessor,
            object value)
        {
            var property = PropertyInfo(accessor);
            var setter = property.SetMethod;
            if (setter == default)
                throw new ArgumentException("The property does not have any setter", nameof(accessor));

            setter.Invoke(self, new[] { value });
        }
    }
}