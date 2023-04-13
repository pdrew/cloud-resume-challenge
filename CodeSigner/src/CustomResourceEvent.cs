﻿namespace CodeSigner;

public class CustomResourceEvent
{
    public string? RequestType { get; set; }
    public string? StackId { get; set; }
    public string? RequestId { get; set; }
    public string? LogicalResourceId { get; set; }
    public string? ResourceType { get; set; }
    public string? PhysicalResourceId { get; set; }
}

public class CustomResourceResponse
{
    public string Status { get; set; }
    public string Reason { get; set; }
    public string PhysicalResourceId { get; set; }
    public string StackId { get; set; }
    public string RequestId { get; set; }
    public string LogicalResourceId { get; set; }
    public bool NoEcho { get; set; }
}