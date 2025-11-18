using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Fussballquiz
{
    public partial class MainWindow : Window
    {
        private string currentUser = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // LOGIN BUTTON
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Owner = this;
            bool? result = login.ShowDialog();

            if (result == true)
            {
                currentUser = login.Username;
                MessageBox.Show($"Willkommen, {currentUser}!",
                    "Login erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);

                QuizSelectionWindow selector = new QuizSelectionWindow(currentUser);
                selector.Show();
            }
        }

        // OHNE LOGIN STARTEN
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            QuizSelectionWindow selector = new QuizSelectionWindow(currentUser);
            selector.Show();
        }
    }

    // -------------------------------------------------------------------
    // LOGIN WINDOW
    // -------------------------------------------------------------------
    public class LoginWindow : Window
    {
        public string Username { get; private set; } = "";

        public LoginWindow()
        {
            this.Title = "Login";
            this.Width = 300;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Grid grid = new Grid() { Margin = new Thickness(10) };
            this.Content = grid;

            StackPanel stack = new StackPanel();
            grid.Children.Add(stack);

            stack.Children.Add(new TextBlock()
            {
                Text = "Benutzername:",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            });

            TextBox usernameBox = new TextBox() { Height = 25 };
            stack.Children.Add(usernameBox);

            Button loginBtn = new Button()
            {
                Content = "Login",
                Height = 30,
                Margin = new Thickness(0, 10, 0, 0)
            };

            loginBtn.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(usernameBox.Text))
                {
                    Username = usernameBox.Text.Trim();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Bitte einen Benutzernamen eingeben.",
                        "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            stack.Children.Add(loginBtn);
        }
    }

    // -------------------------------------------------------------------
    // QUIZ AUSWAHL FENSTER
    // -------------------------------------------------------------------
    public class QuizSelectionWindow : Window
    {
        private string username;

        public QuizSelectionWindow(string user)
        {
            username = user;

            this.Title = "Quiz Auswahl";
            this.Width = 400;
            this.Height = 300;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            StackPanel stack = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            this.Content = stack;

            stack.Children.Add(new TextBlock()
            {
                Text = $"Wähle dein Quiz, {username}",
                FontSize = 20,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // --- Buttons ---
            stack.Children.Add(CreateButton("Real Madrid Quiz", "Real"));
            stack.Children.Add(CreateButton("FC Barcelona Quiz", "Barca"));
            stack.Children.Add(CreateButton("Champions League Quiz", "CL"));
        }

        private Button CreateButton(string text, string mode)
        {
            Button btn = new Button()
            {
                Content = text,
                Width = 200,
                Height = 40,
                Margin = new Thickness(0, 10, 0, 0)
            };

            btn.Click += (s, e) =>
            {
                QuizWindow qw = new QuizWindow(mode, username);
                qw.Show();
            };

            return btn;
        }
    }

    // -------------------------------------------------------------------
    // QUIZ FENSTER
    // -------------------------------------------------------------------
    public class QuizWindow : Window
    {
        private List<(string Frage, string Antwort)> questions;
        private int index = 0;
        private int points = 0;
        private TextBlock questionBlock;
        private TextBox answerBox;

        public QuizWindow(string mode, string username)
        {
            this.Title = $"{mode} Quiz — {username}";
            this.Width = 600;
            this.Height = 350;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LoadQuestions(mode);

            Grid grid = new Grid() { Margin = new Thickness(20) };
            this.Content = grid;

            RowDefinition row1 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);

            questionBlock = new TextBlock()
            {
                Text = questions[0].Frage,
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(questionBlock, 0);
            grid.Children.Add(questionBlock);

            answerBox = new TextBox() { Height = 30, Margin = new Thickness(0, 20, 0, 20) };
            Grid.SetRow(answerBox, 1);
            grid.Children.Add(answerBox);

            Button next = new Button()
            {
                Content = "Antwort bestätigen",
                Width = 200,
                Height = 40
            };
            next.Click += NextQuestion;
            Grid.SetRow(next, 2);
            grid.Children.Add(next);
        }

        private void LoadQuestions(string mode)
        {
            if (mode == "Real")
            {
                questions = new()
                {
                    ("Wer ist der Rekordtorschütze von Real Madrid?", "Cristiano Ronaldo"),
                    ("Wie viele Champions-League-Titel hat Real Madrid?", "15"),
                    ("Wie heißt das Stadion von Real Madrid?", "Santiago Bernabeu")
                };
            }
            else if (mode == "Barca")
            {
                questions = new()
                {
                    ("Wer ist der beste Spieler der in FC Barcelona gespielt hat?", "Lionel Messi"),
                    ("Wie heißt das Stadion des FC Barcelona?", "Camp Nou"),
                    ("Welche Klubfarben hat Barça?", "Blau Rot")
                };
            }
            else
            {
                questions = new()
                {
                    ("Wer gewann die Champions League 2020?", "Bayern"),
                    ("Wie viele Teams spielen in der Gruppenphase?", "32"),
                    ("Welcher Klub hat die meisten CL Titel?", "Real Madrid")
                };
            }
        }

        private void NextQuestion(object sender, RoutedEventArgs e)
        {
            if (answerBox.Text.Trim().ToLower() ==
                questions[index].Antwort.ToLower())
            {
                points++;
            }

            index++;

            if (index >= questions.Count)
            {
                MessageBox.Show($"Quiz beendet!\n\nPunkte: {points}/{questions.Count}",
                    "Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                return;
            }

            answerBox.Text = "";
            questionBlock.Text = questions[index].Frage;
        }
    }
}
