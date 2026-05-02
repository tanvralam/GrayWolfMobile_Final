using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class InactivityTimeoutEventArgs : EventArgs
    {
        public string DeviceId { get; }
        public string DeviceName { get; }

        public InactivityTimeoutEventArgs(string deviceId, string deviceName)
        {
            DeviceId = deviceId;
            DeviceName = string.IsNullOrWhiteSpace(deviceName) ? deviceId : deviceName;
        }
    }

    public class InactivityService
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, CancellationTokenSource> _timers = new Dictionary<string, CancellationTokenSource>();
        private readonly HashSet<string> _timedOutDevices = new HashSet<string>();

        // Use 3 minutes for production. Temporarily change to seconds only while testing.
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(40);

        public event EventHandler<InactivityTimeoutEventArgs> OnTimeout;

        public void ResetTimer(string deviceId, string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            CancellationTokenSource cts;

            lock (_syncRoot)
            {
                CancelTimerNoLock(deviceId);
                _timedOutDevices.Remove(deviceId);

                cts = new CancellationTokenSource();
                _timers[deviceId] = cts;
            }

            _ = StartTimerAsync(deviceId, deviceName, cts.Token);
        }

        public void EnsureTimerRunning(string deviceId, string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            lock (_syncRoot)
            {
                // If a timer is already running, do nothing.
                // If this device already timed out, do nothing until the user presses OK.
                if (_timers.ContainsKey(deviceId) || _timedOutDevices.Contains(deviceId))
                {
                    return;
                }
            }

            ResetTimer(deviceId, deviceName);
        }

        public void RestartTimerAfterAcknowledgement(string deviceId, string deviceName)
        {
            ResetTimer(deviceId, deviceName);
        }

        public void StopTimer(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            lock (_syncRoot)
            {
                CancelTimerNoLock(deviceId);
                _timedOutDevices.Remove(deviceId);
            }
        }

        public void StopAllTimers()
        {
            lock (_syncRoot)
            {
                foreach (var cts in _timers.Values)
                {
                    try
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }
                    catch
                    {
                    }
                }

                _timers.Clear();
                _timedOutDevices.Clear();
            }
        }

        private async Task StartTimerAsync(string deviceId, string deviceName, CancellationToken token)
        {
            try
            {
                await Task.Delay(_timeout, token);

                lock (_syncRoot)
                {
                    _timers.Remove(deviceId);
                    _timedOutDevices.Add(deviceId);
                }

                OnTimeout?.Invoke(this, new InactivityTimeoutEventArgs(deviceId, deviceName));
            }
            catch (TaskCanceledException)
            {
                // Expected when data arrives, the device reconnects, or the timer is stopped manually.
            }
        }

        private void CancelTimerNoLock(string deviceId)
        {
            if (_timers.TryGetValue(deviceId, out var existingCts))
            {
                try
                {
                    existingCts.Cancel();
                    existingCts.Dispose();
                }
                catch
                {
                }

                _timers.Remove(deviceId);
            }
        }
    }
}
