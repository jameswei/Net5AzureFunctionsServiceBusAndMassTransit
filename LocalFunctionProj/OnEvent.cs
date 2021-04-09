using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LocalFunctionProj
{
    public class OnEvent
    {
        //[Function("OnEvent")]
        public void Run([CosmosDBTrigger("databaseName",
                "collectionName",
                ConnectionStringSetting = "",
                LeaseCollectionName = "leases")]
            IReadOnlyList<MyDocument> input,
            FunctionContext context)
        {
            var logger = context.GetLogger("OnEvent");
            if (input != null && input.Count > 0)
            {
                logger.LogInformation("Documents modified: " + input.Count);
                logger.LogInformation("First document Id: "  + input[0].Id);
            }
        }
    }

    public class MyDocument
    {
        public bool Boolean { get; set; }
        public string Id { get; set; }

        public int Number { get; set; }

        public string Text { get; set; }
    }
}