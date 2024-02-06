using System;
using System.Threading;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using LightPadd.Core.Messaging;

namespace LightPadd.Core.Services
{
    public class ScreenIdleService
    {
        private readonly ScreenBrightnessService _brightnessService;
        private readonly IMessenger _messenger;
        private readonly DispatcherTimer _idleTimer;

        private CancellationTokenSource _dimScreenCts;
        private bool _isIdle = false;
        private byte? _brightnessBeforeDeactivate = null;

        public ScreenIdleService(ScreenBrightnessService brightnessService, IMessenger messenger)
        {
            _brightnessService = brightnessService;
            _messenger = messenger;
            _dimScreenCts = new CancellationTokenSource();
            _idleTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(30),
                DispatcherPriority.Normal,
                OnIdle
            );

            _messenger.Register<AppShutdownMessage>(this, OnShutdown);
        }

        public void ActivityDetected()
        {
            if (_isIdle)
            {
                _dimScreenCts.Cancel();
                _brightnessService.IsScreenOn = true;
                _brightnessService.Brightness = _brightnessBeforeDeactivate ?? 128;
                _isIdle = false;
            }

            _idleTimer.Start();
        }

        private async void OnIdle(object? sender, EventArgs e)
        {
            if (_isIdle)
            {
                return;
            }

            _idleTimer.Stop();
            _isIdle = true;
            _dimScreenCts = new CancellationTokenSource();
            _brightnessBeforeDeactivate = _brightnessService.Brightness;

            // Creating this so that we can do -= the brightness value without doing
            // a file-read-then-file-write roundtrip in the brightnessService.
            byte proxyCurrentBrightness = _brightnessBeforeDeactivate.Value;

            const int totalTicks = 20;
            byte dimPerTick = (byte)(_brightnessBeforeDeactivate.Value / totalTicks);
            int msPerTick = 1000 / totalTicks;
            using PeriodicTimer dimScreenTimer = new(TimeSpan.FromMilliseconds(msPerTick));
            for (int i = 0; i < totalTicks; i++)
            {
                // DON'T pass in the token, or it'll throw an exception on cancellation.
                // We check for cancellation manually, because it's faster.
                await dimScreenTimer.WaitForNextTickAsync();
                if (_dimScreenCts.IsCancellationRequested)
                {
                    return;
                }
                proxyCurrentBrightness -= dimPerTick;
                _brightnessService.Brightness = proxyCurrentBrightness;
            }

            if (_dimScreenCts.IsCancellationRequested)
            {
                return;
            }

            _brightnessService.Brightness = 0;
            _brightnessService.IsScreenOn = false;
        }

        private void OnShutdown(object _, AppShutdownMessage message)
        {
            _idleTimer.Stop();
            _dimScreenCts.Cancel();
            if (_isIdle)
            {
                _brightnessService.IsScreenOn = true;
                _brightnessService.Brightness = _brightnessBeforeDeactivate ?? 128;
            }
        }
    }
}
