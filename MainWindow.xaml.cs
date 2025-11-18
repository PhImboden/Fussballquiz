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
    public partial class MainWindow : Window
    {
        private string currentUser = ""; // aktuell eingeloggter Benutzer
        public string Username { get; private set; } = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Login-Button
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Neues Login-Fenster
            LoginWindow login = new LoginWindow();
            login.Owner = this;
            bool? result = login.ShowDialog();

            if (result == true)
            {
                currentUser = login.Username;
                MessageBox.Show($"Willkommen, {currentUser}!", "Erfolgreich eingeloggt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Ohne Login starten
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentUser))
            {
                MessageBox.Show($"Starte Quiz für {currentUser}...", "Quiz starten", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Starte Quiz ohne Benutzer...", "Quiz starten", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Login-Fenster
    public partial class LoginWindow : Window
    {
        public string Username { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();

            // UI erstellen
            this.Title = "Login";
            this.Width = 300;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Grid grid = new Grid() { Margin = new Thickness(10) };
            this.Content = grid;

            StackPanel stack = new StackPanel() { VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(stack);

            TextBlock tb = new TextBlock() { Text = "Benutzername:", FontSize = 14, Margin = new Thickness(0, 0, 0, 5) };
            stack.Children.Add(tb);

            TextBox usernameBox = new TextBox() { Name = "UsernameTextBox", Height = 25 };
            stack.Children.Add(usernameBox);

            Button loginBtn = new Button() { Content = "Login", Height = 30, Margin = new Thickness(0, 10, 0, 0) };
            loginBtn.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(usernameBox.Text))
                {
                    Username = usernameBox.Text;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Bitte einen Benutzernamen eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
            stack.Children.Add(loginBtn);
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }
}
