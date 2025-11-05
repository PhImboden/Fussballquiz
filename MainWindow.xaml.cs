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

namespace Fussballquiz
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                Username = UsernameTextbox.Text;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Bitte einen Benutzernamen eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}