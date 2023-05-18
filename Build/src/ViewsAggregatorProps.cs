using Amazon.CDK.AWS.DynamoDB;

namespace Build;

public class ViewsAggregatorProps
{
    public ITable Table { get; set; }
}