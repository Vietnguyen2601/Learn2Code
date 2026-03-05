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
            
            _logger.LogInformation("PayOS Request JSON: {RequestJson}", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v2/payment-requests", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("PayOS CreatePaymentLink failed: {StatusCode} - {Response}. Request: {Request}", 
                    response.StatusCode, responseBody, json);
                return null;
            }

            var result = JsonSerializer.Deserialize<PayOsCreatePaymentResponse>(responseBody, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (result == null || result.Data == null)
            {
                _logger.LogError("PayOS response deserialization failed or Data is null. Response: {Response}", responseBody);
                return null;
            }

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
        // Try approach 1: Plain values without URL encoding
        var dataToSign = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";

        _logger.LogInformation("Signature input - Amount: {Amount}, CancelUrl: {CancelUrl}, Description: {Description}, OrderCode: {OrderCode}, ReturnUrl: {ReturnUrl}", 
            amount, cancelUrl, description, orderCode, returnUrl);
        _logger.LogInformation("Signature data (PLAIN, NO ENCODING): {DataToSign}", dataToSign);
        _logger.LogInformation("Checksum key: {ChecksumKey}", _options.ChecksumKey);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.ChecksumKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        var signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        
        _logger.LogInformation("Generated signature: {Signature}", signature);
        return signature;
    }
}
