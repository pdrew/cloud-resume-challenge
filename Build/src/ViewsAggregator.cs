﻿using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Constructs;

namespace Build;

public class ViewsAggregator : Construct
{
    public ViewsAggregator(Construct scope, string id, ITable table) : base(scope, id)
    {
        var function = new Function(this, "Function", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "ViewsAggregator::ViewsAggregator.Function::FunctionHandler",
            Timeout = Duration.Seconds(30),
            Code = Code.FromAsset("../Helpers/ViewsAggregator/dist/viewsaggregator-function.zip"),
            Description = "ViewsAggregatorFunction"
        });

        var eventSource = new DynamoEventSource(table, new DynamoEventSourceProps()
        {
            StartingPosition = StartingPosition.TRIM_HORIZON,
            BatchSize = 1
        });
        
        function.AddEventSource(eventSource);
    }
}