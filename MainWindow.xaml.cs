using RichIZ.ViewModels;
using System.Windows;

namespace RichIZ
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
