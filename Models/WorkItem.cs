
public class WorkItem
{
    public string DetailsUrl { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Name: {Name}, State: {State}, Job: {Job}, DetailsUrl: {DetailsUrl}";
    }
}