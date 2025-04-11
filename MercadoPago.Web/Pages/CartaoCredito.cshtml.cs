using MercadoPago.Client.Payment;
using MercadoPago.Client;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MercadoPago.Config;

namespace MercadoPago.Web.Pages
{
    public class CartaoCreditoModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public required string PublicKey { get; set; }

        public CartaoCreditoModel(IConfiguration configuration)
        {
            _configuration = configuration;
            var at = _configuration.GetValue<string>("MercadoPago:AccessToken");
            MercadoPagoConfig.AccessToken = at;
        }

        public void OnGet()
        {
            PublicKey = _configuration.GetValue<string>("MercadoPago:PublicKey");
        }

        public async Task<JsonResult> OnPostCartaoCredito([FromBody] PaymentCreateRequest paymentRequest)
        {
            try
            {
                var orderNumber = Guid.NewGuid().ToString();

                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", orderNumber);

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

                return new JsonResult(new
                {
                    succes = true,
                    id = payment.Id,
                    status = payment.Status,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { sucess = false, message = ex.Message });
            }
        }
    }
}
