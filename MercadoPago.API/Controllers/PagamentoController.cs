using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;

namespace MercadoPago.Controllers
{
    [Route("api/[controller]")]
    public class PagamentoController : Controller
    {
        public PagamentoController(IConfiguration configuration)
        {
            var at = configuration.GetValue<string>("MercadoPago:AccessToken");
            MercadoPagoConfig.AccessToken = at;
        }

        [HttpPost("CartaoCredito")]
        public async Task<IActionResult> CartaoCredito([FromBody] PaymentCreateRequest paymentRequest)
        {
            try
            {
                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

                return Ok(payment.Status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Pix")]
        public async Task<IActionResult> Pix([FromBody] PaymentCreateRequest paymentRequest)
        {
            try
            {
                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

                return Ok(payment.Status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
