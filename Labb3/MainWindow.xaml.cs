using Labb3.ViewModel;
using Labb3.Model;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Labb3.Command;

namespace Labb3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var dialogService = new DialogService();
            var commandContainer = new CommandContainer(dialogService);

            DataContext = new MainWindowsViewModel(dialogService, commandContainer);
        }
    }
}