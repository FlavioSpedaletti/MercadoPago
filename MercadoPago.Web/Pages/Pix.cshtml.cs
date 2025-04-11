using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MercadoPago.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class PixModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PixModel> _logger;
        public required string PublicKey { get; set; }
        public bool Pago { get; set; }

        public PixModel(IConfiguration configuration, ILogger<PixModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var at = _configuration.GetValue<string>("MercadoPago:AccessToken");
            MercadoPagoConfig.AccessToken = at;
        }

        public void OnGet()
        {
            PublicKey = _configuration.GetValue<string>("MercadoPago:PublicKey");
        }

        public JsonResult OnGetStatus()
        {
            return new JsonResult(new { pago = Pago });
        }

        public async Task<JsonResult> OnPostPix([FromBody] PaymentCreateRequest paymentRequest)
        {
            try
            {
                var orderNumber = Guid.NewGuid().ToString();

                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", orderNumber);

                //var notificationUrl = Url.Page(
                //    pageName: "api/webhook/receive",
                //    //pageHandler: "WebhookPixAsync",
                //    pageHandler: null,
                //    //values: new { orderumber = orderNumber },
                //    values: null,
                //    protocol: Request.Scheme
                //);

                var request = HttpContext.Request;
                var notificationUrl = $"{request.Scheme}://{request.Host}/api/webhook/receive";

                //não vou mais setar por aqui, e sim globalmente nas configurações d~e notificações no painel de desenvolvedor do mercado pago
                //paymentRequest.NotificationUrl = notificationUrl;
                //paymentRequest.NotificationUrl = "https://webhook.site/4a03afdc-1b88-4536-ac02-81dad805f732";

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

                return new JsonResult(new
                {
                    succes = true,
                    id = payment.Id,
                    status = payment.Status,
                    ticketUrl = payment.PointOfInteraction.TransactionData.TicketUrl,
                    qrCode = payment.PointOfInteraction.TransactionData.QrCode,
                    qrCodeBase64 = payment.PointOfInteraction.TransactionData.QrCodeBase64,
                    notificationUrl = paymentRequest.NotificationUrl
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { sucess = false, message = ex.Message });
            }
        }
    }
}
