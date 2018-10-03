using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using PollyLoggingContextSample.Policies;

namespace PollyLoggingContextSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
        private readonly IHttpClientFactory _clientFactory;

        public ValuesController(ILogger<ValuesController> logger, IReadOnlyPolicyRegistry<string> policyRegistry, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _policyRegistry = policyRegistry;
            _clientFactory = clientFactory;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            try
            {
                var httpClient = _clientFactory.CreateClient();
                var retryPolicy = _policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>(PolicyNames.BasicRetry)
                             ?? Policy.NoOpAsync<HttpResponseMessage>();

                var context = new Context($"GetSomeData-{Guid.NewGuid()}", new Dictionary<string, object>
                {
                    { PolicyContextItems.Logger, _logger }
                });

                var response = await retryPolicy.ExecuteAsync(ctx => 
                    httpClient.GetAsync("http://www.hopefully-this-doesnt-exist-and-will-return-404.com/apaththatdoesntexist"), context);

                var result = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "Error";

                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
