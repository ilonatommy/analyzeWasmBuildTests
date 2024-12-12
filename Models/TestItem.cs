public class TestItem
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public bool State { get; set; }
    public string Details { get; set; } = string.Empty;
    public int NumberOfTests { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, Duration: {Duration}, State: {State}, Details: {Details}, NumberOfTests: {NumberOfTests}";
    }
}