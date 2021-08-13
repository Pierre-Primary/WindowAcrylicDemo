using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Media;

namespace WindowAcrylicDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickShowColorDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var c = dialog.Color;
                App.AcrylicVM.AttachColor = Color.FromArgb(c.A, c.R, c.G, c.B);
            }
        }

        private void ClickShowAcrylicWindow(object sender, RoutedEventArgs e)
        {
            //foreach (Window win in System.Windows.Application.Current.Windows)
            foreach (Window win in OwnedWindows)
            {
                if (win is AcrylicWindow)
                {
                    win.Activate();
                    return;
                }
            }
            var window = new AcrylicWindow();
            window.Show();
            window.Owner = this;
        }
    }
}
