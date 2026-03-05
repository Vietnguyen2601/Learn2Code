using System.Text.Json.Serialization;

namespace Learn2Code.Application.Base;

public abstract class ServiceResultBase : IServiceResult
{
    [JsonPropertyName("success")]
    public bool Success => Status is >= 200 and < 300;

    [JsonPropertyName("status_code")]
    public int Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("error_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? Data { get; set; }

    protected ServiceResultBase()
    {
        Status = -1;
        Message = "Action failed";
    }
}

public class ServiceResult : ServiceResultBase
{
    public ServiceResult() : base() { }

    public ServiceResult(int status, string message)
    {
        Status = status;
        Message = message;
    }

    public static ServiceResult Ok(string message = "Success")
    {
        return new ServiceResult { Status = 200, Message = message };
    }

    public static ServiceResult Created(object data, string message = "Resource created successfully")
    {
        return new ServiceResult { Status = 201, Message = message, Data = data };
    }

    public static ServiceResult Error(string errorCode, string message, int status = 400)
    {
        return new ServiceResult { Status = status, ErrorCode = errorCode, Message = message };
    }

    public static ServiceResult BadRequest(string message = "Bad request")
    {
        return new ServiceResult { Status = 400, ErrorCode = "BAD_REQUEST", Message = message };
    }

    public static ServiceResult NotFound(string message = "Resource not found")
    {
        return new ServiceResult { Status = 404, ErrorCode = "NOT_FOUND", Message = message };
    }
}

public class ServiceResult<T> : ServiceResultBase
{
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new T? Data
    {
        get => (T?)base.Data;
        set => base.Data = value;
    }

    public ServiceResult() : base() { }

    public static ServiceResult<T> Ok(T data, string message = "Success")
    {
        return new ServiceResult<T> { Status = 200, Message = message, Data = data };
    }

    public static ServiceResult<T> Created(T data, string message = "Resource created successfully")
    {
        return new ServiceResult<T> { Status = 201, Message = message, Data = data };
    }

    public static ServiceResult<T> Error(string errorCode, string message, int status = 400)
    {
        return new ServiceResult<T> { Status = status, ErrorCode = errorCode, Message = message };
    }

    public static ServiceResult<T> BadRequest(string message = "Bad request")
    {
        return new ServiceResult<T> { Status = 400, ErrorCode = "BAD_REQUEST", Message = message };
    }

    public static ServiceResult<T> NotFound(string message = "Resource not found")
    {
        return new ServiceResult<T> { Status = 404, ErrorCode = "NOT_FOUND", Message = message };
    }
}
