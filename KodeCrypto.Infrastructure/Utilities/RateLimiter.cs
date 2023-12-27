using System;
namespace KodeCrypto.Infrastructure.Utilities
{
    public class RateLimiter
    {
        private readonly int maxRequestsPerSecond;
        private readonly SemaphoreSlim semaphore;
        private DateTime lastRequestTimestamp;

        public RateLimiter(int maxRequestsPerSecond)
        {
            this.maxRequestsPerSecond = maxRequestsPerSecond;
            semaphore = new SemaphoreSlim(maxRequestsPerSecond, maxRequestsPerSecond);
            lastRequestTimestamp = DateTime.MinValue;
        }

        public async Task WaitAsync()
        {
            await semaphore.WaitAsync();

            try
            {
                var currentTime = DateTime.UtcNow;
                var timeSinceLastRequest = currentTime - lastRequestTimestamp;

                if (timeSinceLastRequest < TimeSpan.FromSeconds(1))
                {
                    var delay = TimeSpan.FromSeconds(1) - timeSinceLastRequest;
                    await Task.Delay(delay);
                }

                lastRequestTimestamp = DateTime.UtcNow;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}

