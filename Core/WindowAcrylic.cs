using System;
using System.Windows;
using System.Windows.Media;

namespace Pierre.Wpf.Fluent
{
    public class WindowAcrylic : Freezable
    {
        public static readonly DependencyProperty WindowAcrylicProperty = DependencyProperty.RegisterAttached("WindowAcrylic", typeof(WindowAcrylic), typeof(Window), new PropertyMetadata(null, OnAcrylicChanged));

        public static WindowAcrylic GetWindowAcrylic(Window window) => window.GetValue(WindowAcrylicProperty) as WindowAcrylic;

        public static void SetWindowAcrylic(Window window, WindowAcrylic windowAcrylic) => window.SetValue(WindowAcrylicProperty, windowAcrylic);

        private static void OnAcrylicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        public static readonly DependencyProperty IsEnableProperty = DependencyProperty.Register("IsEnable", typeof(bool), typeof(WindowAcrylic), new PropertyMetadata(true, _OnAcrylicPropertyChanged));

        public static readonly DependencyProperty AttachColorProperty = DependencyProperty.Register("AttachColor", typeof(Color), typeof(WindowAcrylic), new PropertyMetadata(Colors.White, _OnAcrylicPropertyChanged));

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(WindowAcrylic), new PropertyMetadata((double)1, _OnAcrylicPropertyChanged), value => (double)value is >= 0 and <= 1);

        public static readonly DependencyProperty ContinuousModeProperty = DependencyProperty.Register("ContinuousMode", typeof(ContinuousMode), typeof(WindowAcrylic), new PropertyMetadata(ContinuousMode.None, (_, __) => { }));

        public static readonly DependencyProperty ContinuousDelayMsProperty = DependencyProperty.Register("ContinuousDelayMs", typeof(double), typeof(WindowAcrylic), new PropertyMetadata((double)0, (_, __) => { }));


        private static void _OnAcrylicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WindowAcrylic)d).OnAcrylicPropertyChanged(d, e);
        }

        internal event EventHandler AcrylicPropertyChanged;
        private void OnAcrylicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AcrylicPropertyChanged?.Invoke(this, EventArgs.Empty);
        }

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
}
