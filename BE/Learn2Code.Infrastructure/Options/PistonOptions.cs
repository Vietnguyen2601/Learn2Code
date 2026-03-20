namespace Learn2Code.Infrastructure.Options;

public class PistonOptions
{
    public string BaseUrl { get; set; } = "http://localhost:2000";
    public string Version { get; set; } = "*";
    public int RunTimeout { get; set; } = 3000;
    public int CompileTimeout { get; set; } = 10000;
}
