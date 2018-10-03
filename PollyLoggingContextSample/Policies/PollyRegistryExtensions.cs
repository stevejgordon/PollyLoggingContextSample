using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;

namespace PollyLoggingContextSample.Policies
{
    public static class PollyRegistryExtensions
    {
        public static IPolicyRegistry<string> AddBasicRetryPolicy(this IPolicyRegistry<string> policyRegistry)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(10), (result, timeSpan, retryCount, context) =>
                {
                    if (context.TryGetLogger(out var logger))
                    {
                        if (result.Exception != null)
                        {
                            logger.LogError(result.Exception, "An exception occurred on {RetryAttempt}", retryCount);
                        }
                        else
                        {
                            logger.LogError("A non success code {StatusCode} was received on {RetryAttempt}", (int)result.Result.StatusCode, retryCount);
                        }
                    }
                });

            policyRegistry.Add(PolicyNames.BasicRetry, retryPolicy);

            return policyRegistry;
        }
    }
}
