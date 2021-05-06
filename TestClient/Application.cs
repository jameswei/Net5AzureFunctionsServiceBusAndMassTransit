using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using IHost = Microsoft.Extensions.Hosting.IHost;

namespace TestClient
{
    // 实现成 hosted service
    public class Application : IHostedService
    {
        readonly IBusControl _busControl;
        readonly ICommandFactory _commandFactory;
        readonly Microsoft.Extensions.Hosting.IHost _host;

        public Application(IBusControl busControl, ICommandFactory commandFactory, IHost host)
        {
            _busControl = busControl;
            _commandFactory = commandFactory;
            _host = host;
        }

        // 实现 service start
        public async Task StartAsync(CancellationToken ct)
        {
            await _busControl.StartAsync(ct);
            while (!ct.IsCancellationRequested)
            {
                Console.Write(">");
                var cmd = parse(Console.ReadLine());
                if (cmd == default) Console.WriteLine("Unknown command");
                else if (cmd is ExitCommand)
                {
                    Console.WriteLine("...exiting");
                    break;
                }
                else await cmd.ExecuteAsync();
            }

            await _host.StopAsync(CancellationToken.None);

            ICommand parse(string line)
            {
                var (cmd, arg) = split(line);
                return cmd.ToLowerInvariant() switch
                {
                    "bye" => _commandFactory.Exit(),
                    "cmd" => _commandFactory.Cmd(arg),
                    "not" => _commandFactory.Not(arg),
                    "req" => _commandFactory.Req(toNumber(arg)),
                    _ => default
                };
            }

            int toNumber(string what) => int.TryParse(what, out var result) ? result : 0;

            (string cmd, string arg) split(string line)
            {
                line = line.Trim();
                if (line.Length == 0) return (string.Empty, string.Empty);
                var index = line.IndexOf(' ');
                if (index < 0) return (line, string.Empty);
                var cmd = line[..index];
                var arg = line[(index + 1)..];
                return (cmd, arg);
            }
        }

        // 实现 service stop
        public Task StopAsync(CancellationToken ct)
        {
            return this._busControl.StopAsync(ct);
        }
    }
}