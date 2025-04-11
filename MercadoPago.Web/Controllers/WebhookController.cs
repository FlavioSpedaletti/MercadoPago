using Microsoft.AspNetCore.Mvc;

namespace MercadoPago.Web.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        //[HttpPost("Pix")]
        //public async Task<IActionResult> Pix(string orderNumber)
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
