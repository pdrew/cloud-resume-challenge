namespace SlackNotifier;

public class AlarmMessage
{
    public string? AlarmName { get; set; }

    public string? NewStateValue { get; set; }

    public string? NewStateReason { get; set; }

    public bool IsAlarmState => NewStateValue?.Contains("ALARM") ?? false;
}