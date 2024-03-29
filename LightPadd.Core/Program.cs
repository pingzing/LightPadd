﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Avalonia;

namespace LightPadd.Core
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static int Main(string[] args)
        {
            try
            {
                var builder = BuildAvaloniaApp();
                if (args.Contains("--drm"))
                {
                    SilenceConsole();
                    return builder.StartLinuxDrm(args, null, 1, null);
                }

                return builder.StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: '{ex}");
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                return 1;
            }
        }

        private static void SilenceConsole()
        {
            new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                {
                    Console.ReadKey(true);
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
    }
}
