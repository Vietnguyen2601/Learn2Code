using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Learn2Code.Infrastructure.DTOs;
using Learn2Code.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learn2Code.Infrastructure.Services;

public interface IPayOsService
{
    Task<PayOsCreatePaymentResponse?> CreatePaymentLinkAsync(PayOsCreatePaymentRequest request);
    Task<PayOsPaymentData?> GetPaymentInfoAsync(long orderCode);
    bool VerifyWebhookSignature(string rawData, string signature);
}

public class PayOsService : IPayOsService
{
    private readonly HttpClient _httpClient;
    private readonly PayOsOptions _options;
    private readonly ILogger<PayOsService> _logger;

    public PayOsService(
        HttpClient httpClient,
        IOptions<PayOsOptions> options,
        ILogger<PayOsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        // Setup HttpClient base configuration
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-client-id", _options.ClientId);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
    }

    public async Task<PayOsCreatePaymentResponse?> CreatePaymentLinkAsync(PayOsCreatePaymentRequest request)
    {
        try
        {
            // Generate signature
            var webhookUrlForSignature = request.WebhookUrl ?? string.Empty;
            request.Signature = GenerateSignature(
                request.Amount,
                request.CancelUrl,
                request.Description,
                request.OrderCode,
                request.ReturnUrl,
                webhookUrlForSignature);

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v2/payment-requests", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("PayOS API error: {StatusCode} - {Response}", response.StatusCode, responseBody);
                return null;
            }

            var result = JsonSerializer.Deserialize<PayOsCreatePaymentResponse>(responseBody, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling PayOS CreatePaymentLink");
            return null;
        }
    }

    public async Task<PayOsPaymentData?> GetPaymentInfoAsync(long orderCode)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/v2/payment-requests/{orderCode}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PayOS GetPaymentInfo failed: {StatusCode} - {Response}", response.StatusCode, responseBody);
                return null;
            }

            var result = JsonSerializer.Deserialize<PayOsCreatePaymentResponse>(responseBody, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling PayOS GetPaymentInfo for orderCode {OrderCode}", orderCode);
            return null;
        }
    }

    public bool VerifyWebhookSignature(string rawData, string signature)
    {
        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Webhook signature is empty - skipping verification");
            return false;
        }

        try
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.ChecksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return computedSignature == signature.ToLowerInvariant();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying webhook signature");
            return false;
        }
    }

    private string GenerateSignature(int amount, string cancelUrl, string description, long orderCode, string returnUrl, string webhookUrl)
    {
        // Data must be in alphabetical order: amount, cancelUrl, description, orderCode, returnUrl, webhookUrl
        var dataToSign = string.IsNullOrEmpty(webhookUrl)
            ? $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}"
            : $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}&webhookUrl={webhookUrl}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.ChecksumKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
