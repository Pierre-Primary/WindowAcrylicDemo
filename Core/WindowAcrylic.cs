using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pierre.Wpf.Fluent
{
    public class WindowAcrylic : Freezable
    {
        public static readonly DependencyProperty WindowAcrylicProperty = DependencyProperty.RegisterAttached("WindowAcrylic", typeof(WindowAcrylic), typeof(Window), new PropertyMetadata(null, OnAcrylicChanged));

        public static WindowAcrylic GetWindowAcrylic(Window window) => window.GetValue(WindowAcrylicProperty) as WindowAcrylic;

        public static void SetWindowAcrylic(Window window, WindowAcrylic windowAcrylic) => window.SetValue(WindowAcrylicProperty, windowAcrylic);

        internal static void OnAcrylicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                var worker = WindowAcrylicWorker.GetWindowAcrylicWorker(window);
                if (worker == null)
                {
                    worker = new WindowAcrylicWorker();
                    WindowAcrylicWorker.SetWindowAcrylicWorker(window, worker);
                }
                worker.SetWindowAcrylic(e.NewValue as WindowAcrylic);
            }
        }

        public static readonly DependencyProperty IsEnableProperty = DependencyProperty.Register("IsEnable", typeof(bool), typeof(WindowAcrylic), new PropertyMetadata(true));

        public static readonly DependencyProperty AttachColorProperty = DependencyProperty.Register("AttachColor", typeof(Color), typeof(WindowAcrylic), new PropertyMetadata(Colors.White));

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(WindowAcrylic), new PropertyMetadata((double)1));

        public static readonly DependencyProperty ContinuousModeProperty = DependencyProperty.Register("ContinuousMode", typeof(ContinuousMode), typeof(WindowAcrylic), new PropertyMetadata(ContinuousMode.None));

        public static readonly DependencyProperty ContinuousDelayMsProperty = DependencyProperty.Register("ContinuousDelayMs", typeof(double), typeof(WindowAcrylic), new PropertyMetadata((double)0));


        public bool IsEnable
        {
            set => SetValue(IsEnableProperty, value);
            get => (bool)GetValue(IsEnableProperty);
        }

        public Color AttachColor
        {
            set => SetValue(AttachColorProperty, value);
            get => (Color)GetValue(AttachColorProperty);
        }

        public double Opacity
        {
            set => SetValue(OpacityProperty, value);
            get => (double)GetValue(OpacityProperty);
        }

        public ContinuousMode ContinuousMode
        {
            set => SetValue(ContinuousModeProperty, value);
            get => (ContinuousMode)GetValue(ContinuousModeProperty);
        }

        public double ContinuousDelayMs
        {
            set => SetValue(ContinuousDelayMsProperty, value);
            get => (double)GetValue(ContinuousDelayMsProperty);
        }


        protected override Freezable CreateInstanceCore()
        {
            return new WindowAcrylic();
        }
    }

    public enum ContinuousMode
    {
        None = 0,
        Throttle = 1,
        Debounce = 2
    }

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
        private HwndSource hwndSource;

        private WindowAcrylic acrylic;

        DispatcherTimer timer;
        DateTime lastTime = DateTime.MinValue;

        public void SetWindow(Window window)
        {
            UnsetWindow();
            this.window = window;
            this.window.Closed += UnsetWindow;
            hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
            {
                window.SourceInitialized += OnSourceInitialized;
            }
            else
            {
                hwndSource = HwndSource.FromHwnd(hwnd);
                ApplyWindowAcrylic();
            }
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            hwnd = new WindowInteropHelper(sender as Window).Handle;
            hwndSource = HwndSource.FromHwnd(hwnd);
            ApplyWindowAcrylic();
        }

        private void ApplyWindowAcrylic()
        {
            if (hwnd == IntPtr.Zero || hwndSource == null || hwndSource.IsDisposed) return;
            if (acrylic != null && acrylic.IsEnable)
            {
                SetAccentPolicy(hwnd, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND, ColorToBgr(acrylic.AttachColor, acrylic.Opacity));
            }
            else
            {
                SetAccentPolicy(hwnd, AccentState.ACCENT_INVALID_STATE);
            }
        }

        public void SetWindowAcrylic(WindowAcrylic acrylic)
        {
            this.acrylic = acrylic;
            var delay = TimeSpan.FromMilliseconds(acrylic.ContinuousDelayMs);
            if (acrylic.ContinuousMode == ContinuousMode.Debounce)
            {
                timer?.Stop();
                timer = new DispatcherTimer { Interval = delay };
                timer.Tick += (_, __) =>
                {
                    ApplyWindowAcrylic();
                    timer?.Stop();
                };
                timer.Start();

            }
            else if (acrylic.ContinuousMode == ContinuousMode.Throttle)
            {
                timer?.Stop();
                var now = DateTime.Now;
                var diff = now - lastTime - delay;
                if (diff >= TimeSpan.Zero)
                {
                    lastTime = now;
                    ApplyWindowAcrylic();
                    return;
                }
                timer = new DispatcherTimer { Interval = -diff };
                timer.Tick += (_, __) =>
                {
                    lastTime = now;
                    ApplyWindowAcrylic();
                    timer?.Stop();
                };
                timer.Start();
            }
            else
            {
                ApplyWindowAcrylic();
            }
        }

        [SecuritySafeCritical]
        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void UnsetWindow(object sender, EventArgs e)
        {
            UnsetWindow();
        }


        [SecurityCritical]
        private void UnsetWindow()
        {
            if (window != null)
            {
                window.SourceInitialized -= OnSourceInitialized;
                window.Closed -= UnsetWindow;
                hwndSource = null;
                hwnd = IntPtr.Zero;
                window = null;
            }
            timer?.Stop();
            timer = null;
        }

        public static int ColorToBgr(Color color, double opacity = 1)
        {
            return (color.R << 0)
                | (color.G << 8)
                | (color.B << 16)
                | ((byte)(color.A * opacity) << 24);
        }

        private static int SetAccentPolicy(IntPtr handle, AccentState accentState, int color = 0, int accentFlags = 0)
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

        private enum AccentState
        {
            //ACCENT_DISABLED = 0,
            //ACCENT_ENABLE_GRADIENT = 1,
            //ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5,
        }

        [Flags]
        private enum AccentFlags
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
