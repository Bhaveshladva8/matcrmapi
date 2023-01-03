using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace matcrm.api.Controllers
{
    [ApiController]
    // [Route("[controller]")]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[] {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> List()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
                .ToArray();
        }

        [HttpGet("TestApi")]
        public string TestApi()
        {
            return "TestApi";
        }

        [HttpPost, Route("paymentstatus", Name = nameof(NotifyPaymentStatusChange))]
        public async Task<string> NotifyPaymentStatusChange([FromForm] string id)
        {
            // Use the received id value to continue the payment verification process ...

            Console.WriteLine("Call webhookUrl from mollie");
            Console.WriteLine(id);
            return id;
        }
    }
}