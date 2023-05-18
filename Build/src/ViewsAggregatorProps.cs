using Amazon.CDK.AWS.DynamoDB;

namespace Build;

public class ViewsAggregatorProps
{
    public Table Table { get; set; }
}