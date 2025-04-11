using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MercadoPago.Web.Pages
{
    public class PixModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public required string PublicKey { get; set; }

        public PixModel(IConfiguration configuration)
        {
            _configuration = configuration;
            var at = _configuration.GetValue<string>("MercadoPago:AccessToken");
            MercadoPagoConfig.AccessToken = at;
        }

        public void OnGet()
        {
            PublicKey = _configuration.GetValue<string>("MercadoPago:PublicKey");
        }

        public async Task<JsonResult> OnPostPix([FromBody] PaymentCreateRequest paymentRequest)
        {
            try
            {
                var orderNumber = Guid.NewGuid().ToString();

                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", orderNumber);

                var notificationUrl = Url.Page(
                    pageName: null,
                    pageHandler: "WebhookPix",
                    values: new { orderumber = orderNumber },
                    protocol: Request.Scheme
                );
                paymentRequest.NotificationUrl = notificationUrl;

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

                return new JsonResult(new
                {
                    succes = true,
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

        public void OnPostWebhookPix(string orderNumber)
        {
            var a = orderNumber;
        }
    }
}
