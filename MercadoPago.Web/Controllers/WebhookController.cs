using MercadoPago.Config;
using MercadoPago.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MercadoPago.Web.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IConfiguration _configuration;
        private readonly static ConcurrentDictionary<string, string> _webhookData = new();

        public WebhookController(ILogger<WebhookController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var at = _configuration.GetValue<string>("MercadoPago:AccessToken");
            MercadoPagoConfig.AccessToken = at;
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive([FromBody] WebhookDto request)
        {
            try
            {
                if (request is null)
                {
                    return new OkResult();
                }

                _logger.LogInformation($"Webhook recebido: {JsonSerializer.Serialize(request)}");

                if (request.Action is not null && request.Action != "payment.updated" ||
                    request.Action is null)
                {
                    _logger.LogInformation($"Webhook ignorado porque não é atualização de pagamento");
                    return new OkResult();
                }

                var id = request.Data.Id;
                _webhookData.TryAdd(id, "none");

                _logger.LogInformation($"Chamando api payments");

                var http = new HttpClient();
                http.BaseAddress = new Uri("https://api.mercadopago.com");
                string bearer = "Bearer " + MercadoPagoConfig.AccessToken;
                http.DefaultRequestHeaders.Add("Authorization", bearer);

                string requestUri = $"v1/payments/{id}";
                _logger.LogInformation($"Chamando api de consulta de pagamento: {requestUri}");

                var result = await http.GetFromJsonAsync<PaymentResponseDto>(requestUri);

                _logger.LogInformation($"Sucesso api payments. Status {result.Status}");

                _webhookData.AddOrUpdate(id, result.Status, (key, oldValue) => result.Status);

                return Ok(result.Status == "approved");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar webhook. {JsonSerializer.Serialize(request)}. Erro: {ex.Message}");

                throw;
            }
        }

        [HttpGet("PaymentStatus/{id}")]
        public JsonResult OnGetStatus(string id)
        {
            //logger.LogInformation($"Consultando status do pagamento. Id {id}");

            _webhookData.TryGetValue(id, out string status);

            //logger.LogInformation($"Status do pagamento {status}");

            return new JsonResult(new { pago = status == "approved" });
        }

        [HttpGet("DictStatus/{id}")]
        public IActionResult OnGetDictStatus(string id)
        {
            //logger.LogInformation($"Consultando status do pagamento. Id {id}");

            _webhookData.TryGetValue(id, out string status);

            //logger.LogInformation($"Status do pagamento {status}");

            return Ok(status);
        }
    }

    public class PaymentResponseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("date_approved")]
        public string Date_Approved { get; set; }

        [JsonPropertyName("date_last_updated")]
        public string Date_Last_Updated { get; set; }

        [JsonPropertyName("payment_type_id")]
        public required string Payment_Type_Id { get; set; }

        [JsonPropertyName("status")]
        public required string Status { get; set; }

        [JsonPropertyName("status_detail")]
        public required string Status_Detail { get; set; }
    }

    public class WebhookDto
    {
        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("live_mode")]
        public bool Live_Mode { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date_created")]
        public string Date_Created { get; set; }

        [JsonPropertyName("api_version")]
        public string Api_Version { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("data")]
        public DataWebhookDto Data { get; set; }
    }

    public class DataWebhookDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
