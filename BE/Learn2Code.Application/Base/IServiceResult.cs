namespace Learn2Code.Application.Base;

public interface IServiceResult
{
    int Status { get; set; }
    string? Message { get; set; }
    object? Data { get; set; }
    List<string>? Errors { get; set; }
}
