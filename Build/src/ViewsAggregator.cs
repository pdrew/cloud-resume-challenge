using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Constructs;

namespace Build;

public class ViewsAggregator : Construct
{
    public ViewsAggregator(Construct scope, string id, ViewsAggregatorProps props) : base(scope, id)
    {
        var function = new Function(this, "Function", new FunctionProps()
        {
            Runtime = Runtime.DOTNET_6,
            MemorySize = 256,
            LogRetention = RetentionDays.ONE_DAY,
            Handler = "ViewsAggregator::ViewsAggregator.Function::FunctionHandler",
            Timeout = Duration.Seconds(30),
            Code = Code.FromAsset("../Helpers/ViewsAggregator/dist/viewsaggregator-function.zip"),
            Description = "ViewsAggregatorFunction",
            Environment = new Dictionary<string, string>
            {
                { "DYNAMODB_TABLE", props.Table.TableName }
            }
        });

        var eventSource = new DynamoEventSource(props.Table, new DynamoEventSourceProps()
        {
            StartingPosition = StartingPosition.TRIM_HORIZON,
            BatchSize = 1,
            Filters = new []
            {
                FilterCriteria.Filter(new Dictionary<string, object>()
                {
                    { "eventName", FilterRule.Or("INSERT", "MODIFY") },
                    {
                        "dynamodb",  new Dictionary<string, object>()
                        {
                            { 
                                "NewImage", new Dictionary<string, object>()
                                {
                                    {
                                        "pk", new Dictionary<string, object>()
                                        {
                                            { "S", FilterRule.Or("VISITOR") }
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
            }
        });

        function.AddEventSource(eventSource);
        
        function.AddDynamoPolicies(props.Table);
    }
}