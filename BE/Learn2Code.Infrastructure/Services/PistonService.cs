using System.Net.Http.Json;
using Learn2Code.Infrastructure.DTOs;
using Learn2Code.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learn2Code.Infrastructure.Services;

public interface IPistonService
{
    Task<PistonExecuteResponse?> ExecuteAsync(PistonExecuteRequest request, CancellationToken cancellationToken = default);
}

public class PistonService : IPistonService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PistonService> _logger;

    public PistonService(HttpClient httpClient, IOptions<PistonOptions> options, ILogger<PistonService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var settings = options.Value;
        _httpClient.BaseAddress = new Uri(settings.BaseUrl);
    }

    public async Task<PistonExecuteResponse?> ExecuteAsync(PistonExecuteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("/api/v2/execute", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Piston execute failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, errorBody);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PistonExecuteResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Piston execute API");
            return null;
        }
    }
}
