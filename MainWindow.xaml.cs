using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic; // Für InputBox

namespace Fussballquiz
{
    public partial class MainWindow : Window
    {
        // ---------------------------------------------------------
        // FELDER
        // ---------------------------------------------------------
        private string currentUser = "";
        private List<(string Frage, string Antwort)> questions = new(); // Zeile 19: Feld initialisiert
        private int index = 0;
        private int points = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // LOGIN BUTTON (Jetzt ohne eigenes Fenster)
        // ---------------------------------------------------------
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // InputBox statt Popup-Fenster
            string username = Interaction.InputBox(
                "Bitte Benutzernamen eingeben:",
                "Login",
                "");

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Kein Benutzername eingegeben!", "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentUser = username.Trim();

            MessageBox.Show($"Willkommen, {currentUser}!",
                "Login erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);

            // StartScreen → Quiz-Auswahl
            StartScreen.Visibility = Visibility.Collapsed;
            QuizSelection.Visibility = Visibility.Visible;

            QuizSelectionTitle.Text = $"Wähle dein Quiz, {currentUser}";
        }

        // ---------------------------------------------------------
        // OHNE LOGIN STARTEN
        // ---------------------------------------------------------
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartScreen.Visibility = Visibility.Collapsed;
            QuizSelection.Visibility = Visibility.Visible;
        }

        // ---------------------------------------------------------
        // QUIZ BUTTONS
        // ---------------------------------------------------------
        private void RealQuiz_Click(object sender, RoutedEventArgs e)
        {
            LoadQuestions("Real");
            ShowQuizScreen();
        }

        private void BarcaQuiz_Click(object sender, RoutedEventArgs e)
        {
            LoadQuestions("Barca");
            ShowQuizScreen();
        }

        private void CLQuiz_Click(object sender, RoutedEventArgs e)
        {
            LoadQuestions("CL");
            ShowQuizScreen();
        }

        // ---------------------------------------------------------
        // QUIZ STARTEN
        // ---------------------------------------------------------
        private void ShowQuizScreen()
        {
            QuizSelection.Visibility = Visibility.Collapsed;
            QuizScreen.Visibility = Visibility.Visible;

            index = 0;
            points = 0;

            QuestionText.Text = questions[index].Frage;
            AnswerBox.Text = "";
        }

        // ---------------------------------------------------------
        // FRAGEN LADEN
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // NÄCHSTE FRAGE
        // ---------------------------------------------------------
        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (AnswerBox.Text.Trim().ToLower() ==
                questions[index].Antwort.ToLower())
            {
                points++;
            }

            index++;

            // Quiz fertig?
            if (index >= questions.Count)
            {
                MessageBox.Show($"Quiz beendet!\n\nPunkte: {points}/{questions.Count}",
                    "Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);

                QuizScreen.Visibility = Visibility.Collapsed;
                QuizSelection.Visibility = Visibility.Visible;
                return;
            }

            // Nächste Frage
            AnswerBox.Text = "";
            QuestionText.Text = questions[index].Frage;
        }
    }
}
