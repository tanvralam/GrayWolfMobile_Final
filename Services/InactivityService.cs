using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class InactivityService
    {
        private CancellationTokenSource _cts;

        // ⏱️ SET YOUR TIME HERE (3 minutes)
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(40);

        public event Action OnTimeout;

        public void ResetTimer()
        {
            // Cancel previous timer
            _cts?.Cancel();

            // Create new token
            _cts = new CancellationTokenSource();

            // Start new timer
            _ = StartTimerAsync(_cts.Token);
        }

        private async Task StartTimerAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(_timeout, token);

                // ⛔ No data received for 3 minutes
                OnTimeout?.Invoke();
            }
            catch (TaskCanceledException)
            {
                // Expected when timer is reset
            }
        }

        // Optional: stop timer manually
        public void StopTimer()
        {
            _cts?.Cancel();
        }
    }
}
