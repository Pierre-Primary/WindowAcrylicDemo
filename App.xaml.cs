using System.Windows;
using WindowAcrylicDemo.ViewModel;

namespace WindowAcrylicDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AcrylicVM AcrylicVM { get; } = new AcrylicVM();
    }
}
