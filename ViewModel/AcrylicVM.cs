using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

namespace WindowAcrylicDemo.ViewModel
{
    public class AcrylicVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool enableWindowChrome = true;
        public bool EnableWindowChrome
        {
            get => enableWindowChrome;
            set
            {
                enableWindowChrome = value;
                Notify(nameof(EnableWindowChrome));
            }
        }

        private double captionHeight = SystemParameters.CaptionHeight;
        public double CaptionHeight
        {
            get => captionHeight;
            set
            {
                captionHeight = value;
                Notify(nameof(CaptionHeight));
            }
        }

        private bool useAeroCaptionButtons = true;
        public bool UseAeroCaptionButtons
        {
            get => useAeroCaptionButtons;
            set
            {
                useAeroCaptionButtons = value;
                Notify(nameof(UseAeroCaptionButtons));
            }
        }

        private bool isHitTestVisibleInChrome;
        public bool IsHitTestVisibleInChrome
        {
            get => isHitTestVisibleInChrome;
            set
            {
                isHitTestVisibleInChrome = value;
                Notify(nameof(IsHitTestVisibleInChrome));
            }
        }

        #region GlassFrameThickness
        private bool enableGlassFrame = true;
        private static readonly Thickness DisableGlassFrameThickness = new Thickness(-1);
        public bool EnableGlassFrame
        {
            get => enableGlassFrame;
            set
            {
                enableGlassFrame = value;
                Notify(nameof(GlassFrameThickness));
                Notify(nameof(GlassFrameLeft));
                Notify(nameof(GlassFrameTop));
                Notify(nameof(GlassFrameRight));
                Notify(nameof(GlassFrameBottom));
            }
        }

        private Thickness glassFrameThickness = new Thickness(0, SystemParameters.WindowNonClientFrameThickness.Top, 0, 0); //SystemParameters.WindowNonClientFrameThickness;
        public Thickness GlassFrameThickness
        {
            get => enableGlassFrame ? glassFrameThickness : DisableGlassFrameThickness;
            set
            {
                glassFrameThickness = value;
                Notify(nameof(GlassFrameThickness));
                Notify(nameof(GlassFrameLeft));
                Notify(nameof(GlassFrameTop));
                Notify(nameof(GlassFrameRight));
                Notify(nameof(GlassFrameBottom));
            }
        }

        public double GlassFrameLeft
        {
            get => GlassFrameThickness.Left;
            set
            {
                glassFrameThickness.Left = value;
                Notify(nameof(GlassFrameThickness));
                Notify(nameof(GlassFrameLeft));
            }
        }

        public double GlassFrameTop
        {
            get => GlassFrameThickness.Top;
            set
            {
                glassFrameThickness.Top = value;
                Notify(nameof(GlassFrameThickness));
                Notify(nameof(GlassFrameTop));
            }
        }

        public double GlassFrameRight
        {
            get => GlassFrameThickness.Right;
            set
            {
                glassFrameThickness.Right = value;
                Notify(nameof(GlassFrameThickness));
                Notify(nameof(GlassFrameRight));
            }
        }

        public double GlassFrameBottom
        {
            get => GlassFrameThickness.Bottom;
            set
            {
                glassFrameThickness.Bottom = value;
                Notify(nameof(GlassFrameThickness));
                Notify(nameof(GlassFrameBottom));
            }
        }
        #endregion

        #region ResizeBorderThickness
        private bool enableResizeBorder = true;
        private static readonly Thickness DisableResizeBorderThickness = new Thickness(0);
        public bool EnableResizeBorder
        {
            get => enableResizeBorder;
            set
            {
                enableResizeBorder = value;
                Notify(nameof(ResizeBorderThickness));
                Notify(nameof(ResizeBorderLeft));
                Notify(nameof(ResizeBorderTop));
                Notify(nameof(ResizeBorderRight));
                Notify(nameof(ResizeBorderBottom));
            }
        }

        private Thickness resizeBorderThickness = SystemParameters.WindowResizeBorderThickness;
        public Thickness ResizeBorderThickness
        {
            get => enableResizeBorder ? resizeBorderThickness : DisableResizeBorderThickness;
            set
            {
                resizeBorderThickness = value;
                Notify(nameof(ResizeBorderThickness));
                Notify(nameof(ResizeBorderLeft));
                Notify(nameof(ResizeBorderTop));
                Notify(nameof(ResizeBorderRight));
                Notify(nameof(ResizeBorderBottom));
            }
        }

        public double ResizeBorderLeft
        {
            get => ResizeBorderThickness.Left;
            set
            {
                resizeBorderThickness.Left = value;
                Notify(nameof(ResizeBorderThickness));
                Notify(nameof(ResizeBorderLeft));
            }
        }

        public double ResizeBorderTop
        {
            get => ResizeBorderThickness.Top;
            set
            {
                resizeBorderThickness.Top = value;
                Notify(nameof(ResizeBorderThickness));
                Notify(nameof(ResizeBorderTop));
            }
        }

        public double ResizeBorderRight
        {
            get => ResizeBorderThickness.Right;
            set
            {
                resizeBorderThickness.Right = value;
                Notify(nameof(ResizeBorderThickness));
                Notify(nameof(ResizeBorderRight));
            }
        }

        public double ResizeBorderBottom
        {
            get => ResizeBorderThickness.Bottom;
            set
            {
                resizeBorderThickness.Bottom = value;
                Notify(nameof(ResizeBorderThickness));
                Notify(nameof(ResizeBorderBottom));
            }
        }
        #endregion

        #region NonClientFrameEdges
        private NonClientFrameEdges nonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Top | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom;
        public NonClientFrameEdges NonClientFrameEdges
        {
            get => nonClientFrameEdges;
            set
            {
                nonClientFrameEdges = value;
                Notify(nameof(NonClientFrameEdges));
                Notify(nameof(NonClientFrameLeft));
                Notify(nameof(NonClientFrameTop));
                Notify(nameof(NonClientFrameRight));
                Notify(nameof(NonClientFrameBottom));
            }
        }
        public int NonClientFrameEdgesValue
        {
            get => (int)nonClientFrameEdges;
        }

        public bool NonClientFrameLeft
        {
            get => (nonClientFrameEdges & NonClientFrameEdges.Left) == NonClientFrameEdges.Left;
            set
            {
                if (value)
                {
                    nonClientFrameEdges |= NonClientFrameEdges.Left;
                }
                else
                {
                    nonClientFrameEdges &= ~NonClientFrameEdges.Left;
                }

                Notify(nameof(NonClientFrameEdges));
                Notify(nameof(NonClientFrameEdgesValue));
                Notify(nameof(NonClientFrameLeft));
            }
        }
        public bool NonClientFrameTop
        {
            get => (nonClientFrameEdges & NonClientFrameEdges.Top) == NonClientFrameEdges.Top;
            set
            {
                if (value)
                {
                    nonClientFrameEdges |= NonClientFrameEdges.Top;
                }
                else
                {
                    nonClientFrameEdges &= ~NonClientFrameEdges.Top;
                }

                Notify(nameof(NonClientFrameEdges));
                Notify(nameof(NonClientFrameEdgesValue));
                Notify(nameof(NonClientFrameTop));
            }
        }

        public bool NonClientFrameRight
        {
            get => (nonClientFrameEdges & NonClientFrameEdges.Right) == NonClientFrameEdges.Right;
            set
            {
                if (value)
                {
                    nonClientFrameEdges |= NonClientFrameEdges.Right;
                }
                else
                {
                    nonClientFrameEdges &= ~NonClientFrameEdges.Right;
                }

                Notify(nameof(NonClientFrameEdges));
                Notify(nameof(NonClientFrameEdgesValue));
                Notify(nameof(NonClientFrameRight));
            }
        }

        public bool NonClientFrameBottom
        {
            get => (nonClientFrameEdges & NonClientFrameEdges.Bottom) == NonClientFrameEdges.Bottom;
            set
            {
                if (value)
                {
                    nonClientFrameEdges |= NonClientFrameEdges.Bottom;
                }
                else
                {
                    nonClientFrameEdges &= ~NonClientFrameEdges.Bottom;
                }

                Notify(nameof(NonClientFrameEdges));
                Notify(nameof(NonClientFrameEdgesValue));
                Notify(nameof(NonClientFrameBottom));
            }
        }
        #endregion

        #region CornerRadius
        private CornerRadius cornerRadius = SystemParameters.WindowCornerRadius;
        public CornerRadius CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                Notify(nameof(CornerRadius));
            }
        }

        public double CornerRadiusTopLeft
        {
            get => cornerRadius.TopLeft;
            set
            {
                cornerRadius.TopLeft = value;
                Notify(nameof(CornerRadius));
                Notify(nameof(CornerRadiusTopLeft));
            }
        }
        public double CornerRadiusTopRight
        {
            get => cornerRadius.TopRight;
            set
            {
                cornerRadius.TopRight = value;
                Notify(nameof(CornerRadius));
                Notify(nameof(CornerRadiusTopRight));
            }
        }
        public double CornerRadiusBottomRight
        {
            get => cornerRadius.BottomRight;
            set
            {
                cornerRadius.BottomRight = value;
                Notify(nameof(CornerRadius));
                Notify(nameof(CornerRadiusBottomRight));
            }
        }
        public double CornerRadiusBottomLeft
        {
            get => cornerRadius.BottomLeft;
            set
            {
                cornerRadius.BottomLeft = value;
                Notify(nameof(CornerRadius));
                Notify(nameof(CornerRadiusBottomLeft));
            }
        }
        #endregion

        private bool enableWindowAcrylice = true;
        public bool EnableWindowAcrylice
        {
            get => enableWindowAcrylice;
            set
            {
                enableWindowAcrylice = value;
                Notify(nameof(EnableWindowAcrylice));
            }
        }

        private double opacity = 0.3;
        public double Opacity
        {
            get => opacity;
            set
            {
                opacity = value;
                Notify(nameof(Opacity));
            }
        }

        private Color attachColor = Colors.SkyBlue;
        public Color AttachColor
        {
            get => attachColor;
            set
            {
                attachColor = value;
                Notify(nameof(AttachColor));
                Notify(nameof(AttachColorBrush));
            }
        }
        public SolidColorBrush AttachColorBrush
        {
            get => new SolidColorBrush(attachColor);
        }

        private void Notify(string pName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }
    }
}
