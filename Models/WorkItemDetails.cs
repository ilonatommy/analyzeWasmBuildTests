public class WorkItemDetails
{
    public DateTime Queued { get; set; }
    public DateTime Started { get; set; }
    public DateTime Finished { get; set; }
    public TimeSpan Delay { get; set; }
    public TimeSpan Duration { get; set; }
    public string Id { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public string ConsoleOutputUri { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<Log> Logs { get; set; } = new();
    public List<File> Files { get; set; } = new();
    public string Job { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Queued: {Queued}, Started: {Started}, Finished: {Finished}, Duration: {Duration}, MachineName: {MachineName}, ExitCode: {ExitCode}, ConsoleOutputUri: {ConsoleOutputUri}";
    }
}