//using MercadoPago.Client;
//using MercadoPago.Client.Payment;
//using MercadoPago.Config;
//using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;

namespace MercadoPago.Controllers
{
    [Route("api/[controller]")]
    public class PagamentoController : Controller
    {
        //public PagamentoController(IConfiguration configuration)
        //{
        //    MercadoPagoConfig.AccessToken = configuration.GetValue<string>("MercadoPago__AccessToken");
        //}

        //[HttpPost("CartaoCredito")]
        //public async Task<IActionResult> CartaoCredito(PaymentCreateRequest paymentRequest)
        //{
        //    try
        //    {
        //        var requestOptions = new RequestOptions();
        //        requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

        //        var client = new PaymentClient();
        //        Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

        //        return Ok(payment.Status);

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
