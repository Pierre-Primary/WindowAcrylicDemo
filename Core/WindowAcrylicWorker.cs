using Pierre.Core;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows;

namespace Pierre.Wpf.Fluent
{
    internal class WindowAcrylicWorker : DependencyObject
    {
        public static readonly DependencyProperty WindowAcrylicWorkerProperty = DependencyProperty.RegisterAttached("WindowAcrylicWorker", typeof(WindowAcrylicWorker), typeof(WindowAcrylicWorker), new PropertyMetadata(null, OnAcrylicWorkerChanged));

        public static WindowAcrylicWorker GetWindowAcrylicWorker(Window window) => window.GetValue(WindowAcrylicWorkerProperty) as WindowAcrylicWorker;

        public static void SetWindowAcrylicWorker(Window window, WindowAcrylicWorker windowAcrylic) => window.SetValue(WindowAcrylicWorkerProperty, windowAcrylic);

        internal static void OnAcrylicWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is WindowAcrylicWorker worker)
            {
                worker.SetWindow(d as Window);
            }
        }

        private Window window;
        private IntPtr hwnd = IntPtr.Zero;

        private WindowAcrylic acrylic;

        DispatcherTimer timer;
        DateTime lastTime = DateTime.MinValue;

        public void SetWindow(Window window)
        {
            UnSubscribeWindowEvents();
            this.window = window;
            this.window.Closed += OnWindowColse;
            hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
            {
                window.SourceInitialized += OnSourceInitialized;
            }
            else
            {
                ApplyWindowAcrylic(true);
            }
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            hwnd = new WindowInteropHelper(sender as Window).Handle;
            ApplyWindowAcrylic(true);
        }

        public void SetWindowAcrylic(WindowAcrylic acrylic)
        {
            if (this.acrylic == acrylic)
                return;
            if (this.acrylic != null)
                this.acrylic.AcrylicPropertyChanged -= OnAcrylicPropertyChanged;
            this.acrylic = acrylic;
            if (this.acrylic != null)
                this.acrylic.AcrylicPropertyChanged += OnAcrylicPropertyChanged;
            ApplyWindowAcrylic();
        }

        internal void ApplyWindowAcrylic(bool immediately = false)
        {
            timer?.Stop();
            if (immediately || acrylic == null)
            {
                UpdateWindowAcrylic();
                lastTime = DateTime.Now;
                return;
            }
            var delay = TimeSpan.FromMilliseconds(acrylic.ContinuousDelayMs);
            if (acrylic.ContinuousMode == ContinuousMode.Debounce)
            {
                timer = new DispatcherTimer { Interval = delay };
                timer.Tick += (_, __) =>
                {
                    timer?.Stop();
                    UpdateWindowAcrylic();
                    lastTime = DateTime.Now;
                };
                timer.Start();

            }
            else if (acrylic.ContinuousMode == ContinuousMode.Throttle)
            {
                var now = DateTime.Now;
                var diff = now - lastTime - delay;
                if (diff >= TimeSpan.Zero)
                {
                    UpdateWindowAcrylic();
                    lastTime = now;
                    return;
                }
                timer = new DispatcherTimer { Interval = -diff };
                timer.Tick += (_, __) =>
                {
                    timer?.Stop();
                    UpdateWindowAcrylic();
                    lastTime = now;
                };
                timer.Start();
            }
            else
            {
                UpdateWindowAcrylic();
            }
        }

        private void UpdateWindowAcrylic()
        {
            if (acrylic != null && acrylic.IsEnable)
            {
                EnableWindowAcrylic();
            }
            else
            {
                RestoreWindowAcrylic();
            }
        }

        private void EnableWindowAcrylic()
        {
            Helper.SetAccentPolicy(hwnd, Helper.AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND, Utils.ColorToBgr(Utils.ColorWithOpacity(acrylic.AttachColor, acrylic.Opacity)));
        }

        private void RestoreWindowAcrylic()
        {
            Helper.SetAccentPolicy(hwnd, Helper.AccentState.ACCENT_INVALID_STATE);
        }


        private void OnAcrylicPropertyChanged(object sender, EventArgs e)
        {
            ApplyWindowAcrylic();
        }

        private void OnWindowColse(object sender, EventArgs e)
        {
            UnSubscribeWindowEvents();
            if (this.acrylic != null)
                this.acrylic.AcrylicPropertyChanged += OnAcrylicPropertyChanged;
        }

        private void UnSubscribeWindowEvents()
        {
            timer?.Stop();
            timer = null;
            if (window != null)
            {
                window.SourceInitialized -= OnSourceInitialized;
                window.Closed -= OnWindowColse;
            }
            hwnd = IntPtr.Zero;
            window = null;
        }

        private static class Helper
        {

            internal static int SetAccentPolicy(IntPtr handle, AccentState accentState, int color = 0, int accentFlags = 0)
            {
                return SetWindowCompositionAttribute(handle, WindowCompositionAttribute.WCA_ACCENT_POLICY, new AccentPolicy
                {
                    AccentState = accentState,
                    GradientColor = color,
                    AccentFlags = accentFlags,
                });
            }

            private static int SetWindowCompositionAttribute(IntPtr handle, WindowCompositionAttribute attribute, object value)
            {
                var accentPolicySize = Marshal.SizeOf(value);
                var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
                Marshal.StructureToPtr(value, accentPtr, false);
                try
                {
                    var data = new WindowCompositionAttributeData
                    {
                        Attribute = attribute,
                        SizeOfData = accentPolicySize,
                        Data = accentPtr,
                    };
                    return SetWindowCompositionAttribute(handle, ref data);
                }
                finally
                {
                    Marshal.FreeHGlobal(accentPtr);
                }
            }

            [DllImport("user32.dll")]
            private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

            [StructLayout(LayoutKind.Sequential)]
            private struct AccentPolicy
            {
                public AccentState AccentState;
                public int AccentFlags;
                public int GradientColor;
                public int AnimationId;
            }

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
                ACCENT_INVALID_STATE = 5,
            }

            [Flags]
            internal enum AccentFlags
            {
                DrawLeftBorder = 0x20,
                DrawTopBorder = 0x40,
                DrawRightBorder = 0x80,
                DrawBottomBorder = 0x100,
                DrawAllBorders = DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct WindowCompositionAttributeData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }

            private enum WindowCompositionAttribute
            {
                // ...
                WCA_ACCENT_POLICY = 19,
                // ...
            }

        }
    }
}
