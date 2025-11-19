using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Fussballquiz
{
    public partial class MainWindow : Window
    {
        // FELDER
        private string currentUser = "";
        private List<(string Frage, string Antwort)> questions = new();
        private int index = 0;
        private int points = 0;

        // Highscore-Pfad (AppData)
        private readonly string highscoreDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fussballquiz");
        private readonly string highscoreFile;

        // Sounds (optional)
        private readonly string soundsFolder;
        private MediaPlayer? clickPlayer = null;
        private MediaPlayer? correctPlayer = null;
        private MediaPlayer? wrongPlayer = null;

        public MainWindow()
        {
            InitializeComponent();

            highscoreFile = Path.Combine(highscoreDir, "highscores.json");
            soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

            LoadHighscores();
            PrepareSounds();
        }

        // -----------------------
        // Sounds initialisieren
        // -----------------------
        private void PrepareSounds()
        {
            try
            {
                // Versuche WAVs zu laden; wenn nicht vorhanden, leave players null -> use fallback
                string clickPath = Path.Combine(soundsFolder, "click.wav");
                string correctPath = Path.Combine(soundsFolder, "correct.wav");
                string wrongPath = Path.Combine(soundsFolder, "wrong.wav");

                if (File.Exists(clickPath))
                {
                    clickPlayer = new MediaPlayer();
                    clickPlayer.Open(new Uri(clickPath, UriKind.Absolute));
                }

                if (File.Exists(correctPath))
                {
                    correctPlayer = new MediaPlayer();
                    correctPlayer.Open(new Uri(correctPath, UriKind.Absolute));
                }

                if (File.Exists(wrongPath))
                {
                    wrongPlayer = new MediaPlayer();
                    wrongPlayer.Open(new Uri(wrongPath, UriKind.Absolute));
                }
            }
            catch
            {
                // bei Fehlern: nichts (Falleback auf SystemSounds)
            }
        }

        private void PlayClick()
        {
            if (clickPlayer != null)
            {
                clickPlayer.Position = TimeSpan.Zero;
                clickPlayer.Play();
            }
            else
            {
                SystemSounds.Beep.Play();
            }
        }

        private void PlayCorrect()
        {
            if (correctPlayer != null)
            {
                correctPlayer.Position = TimeSpan.Zero;
                correctPlayer.Play();
            }
            else
            {
                SystemSounds.Asterisk.Play();
            }
        }

        private void PlayWrong()
        {
            if (wrongPlayer != null)
            {
                wrongPlayer.Position = TimeSpan.Zero;
                wrongPlayer.Play();
            }
            else
            {
                SystemSounds.Hand.Play();
            }
        }

        // -----------------------
        // UI-Animation Hilfsmethoden
        // -----------------------
        private void FadeOut(UIElement element, double durationMs = 250, Action? finished = null)
        {
            var sb = new Storyboard();
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(durationMs)) { EasingFunction = new QuadraticEase() };
            Storyboard.SetTarget(anim, element);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
            sb.Children.Add(anim);
            sb.Completed += (s, e) => finished?.Invoke();
            sb.Begin();
        }

        private void FadeIn(UIElement element, double durationMs = 300)
        {
            element.Opacity = 0;
            element.Visibility = Visibility.Visible;
            var sb = new Storyboard();
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(durationMs)) { EasingFunction = new QuadraticEase() };
            Storyboard.SetTarget(anim, element);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
            sb.Children.Add(anim);
            sb.Begin();
        }

        // -----------------------
        // LOGIN / NAVIGATION
        // -----------------------
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();

            string username = UsernameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Bitte gib einen Benutzernamen ein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentUser = username;
            CurrentUserText.Text = currentUser;
            QuizSelectionTitle.Text = $"Wähle dein Quiz, {currentUser}";

            // Animation: StartCard ausblenden, SelectionCard einblenden
            FadeOut(StartCard, 220, () =>
            {
                StartCard.Visibility = Visibility.Collapsed;
                SelectionCard.Visibility = Visibility.Visible;
                FadeIn(SelectionCard);
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            currentUser = "Gast";
            CurrentUserText.Text = currentUser;
            QuizSelectionTitle.Text = $"Wähle dein Quiz, {currentUser}";

            FadeOut(StartCard, 220, () =>
            {
                StartCard.Visibility = Visibility.Collapsed;
                SelectionCard.Visibility = Visibility.Visible;
                FadeIn(SelectionCard);
            });
        }

        // -----------------------
        // QUIZ-Auswahl
        // -----------------------
        private void RealQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Real");
            ShowQuizScreen();
        }

        private void BarcaQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Barca");
            ShowQuizScreen();
        }

        private void CLQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("CL");
            ShowQuizScreen();
        }

        private void BayernQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Bayern");
            ShowQuizScreen();
        }

        private void BundesligaQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Bindesliga");
            ShowQuizScreen();
        }

        private void JuveQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Juve");
            ShowQuizScreen();
        }

        private void LiverpoolQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Liverpool");
            ShowQuizScreen();
        }
        private void PremierQuiz_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            LoadQuestions("Premier");
            ShowQuizScreen();
        }

        // -----------------------
        // Fragen laden
        // -----------------------
        private void LoadQuestions(string mode)
        {
            if (mode == "Real")
            {
                questions = new()
        {
            ("Wer ist der Rekordtorschütze von Real Madrid?", "cristiano ronaldo"),
            ("Wie viele Champions-League-Titel hat Real Madrid?", "15"),
            ("Wie heißt das Stadion von Real Madrid?", "santiago bernabeu"),
            ("Wer ist der aktuelle Trainer von Real Madrid?", "xabi alonso"),
            ("In welcher Stadt spielt Real Madrid?", "madrid")
        };
            }
            else if (mode == "Barca")
            {
                questions = new()
        {
            ("Wer ist der beste Spieler, der für FC Barcelona gespielt hat?", "lionel messi"),
            ("Wie heißt das Stadion des FC Barcelona?", "camp nou"),
            ("Welche Klubfarben hat Barça?", "blau rot"),
            ("Wer war Trainer von Barcelona in 2020?", "ronald koeman"),
            ("In welcher Stadt spielt Barcelona?", "barcelona")
        };
            }
            else if (mode == "Bayern")
            {
                questions = new()
        {
            ("Wer ist der Rekordtorschütze von Bayern München?", "gerd müller"),
            ("Wie viele Bundesliga-Titel hat Bayern gewonnen?", "33"),
            ("Wie heißt das Stadion von Bayern München?", "allianz arena"),
            ("Wer war Trainer von Bayern 2020?", "hansi flick"),
            ("In welcher Stadt spielt Bayern München?", "münchen")
        };
            }
            else if (mode == "Liverpool")
            {
                questions = new()
        {
            ("Wer ist der Rekordtorschütze von Liverpool?", "ian rush"),
            ("Wie heißt das Stadion von Liverpool?", "anfield"),
            ("Wie viele Champions-League-Titel hat Liverpool?", "6"),
            ("Wer war Trainer von Liverpool 2019?", "jurgen klopp"),
            ("Welche Farben hat Liverpool?", "rot weiß")
        };
            }
            else if (mode == "Juve")
            {
                questions = new()
        {
            ("Wer ist der Rekordtorschütze von Juventus?", "alessandro del piero"),
            ("Wie viele Serie-A-Titel hat Juventus gewonnen?", "36"),
            ("Wie heißt das Stadion von Juventus?", "allianz stadium"),
            ("Wer war Trainer von Juventus 2020?", "maurizio sarri"),
            ("Welche Klubfarben hat Juventus?", "schwarz weiß")
        };
            }
            else if (mode == "Premier")
            {
                questions = new()
        {
            ("Wer ist der Rekordtorschütze von Manchester United?", "wayne rooney"),
            ("Wie viele Premier-League-Titel hat Manchester United gewonnen?", "20"),
            ("Wie heißt das Stadion von Arsenal?", "emirates stadium"),
            ("Wer war Trainer von Liverpool 2020?", "jurgen klopp"),
            ("Welche Klubfarben hat Chelsea?", "blau weiß")
        };
            }
            else
            {
                // Champions League Allgemein
                questions = new()
        {
            ("Wer gewann die Champions League 2020?", "bayern"),
            ("Wie viele Teams spielen in der Gruppenphase?", "36"),
            ("Welcher Klub hat die meisten CL-Titel?", "real madrid"),
            ("In welchem Jahr wurde die Champions League eingeführt?", "1955"),
            ("Wer ist Rekordtorschütze der CL-Geschichte?", "cristiano ronaldo")
        };
            }
        }


        // -----------------------
        // Quiz starten / anzeigen mit Animation
        // -----------------------
        private void ShowQuizScreen()
        {
            // hide selection card, show quiz panel
            FadeOut(SelectionCard, 180, () =>
            {
                SelectionCard.Visibility = Visibility.Collapsed;
                FadeIn(QuizPanel);
                QuizPanel.Visibility = Visibility.Visible;
            });

            // WelcomePanel.Visibility = Visibility.Collapsed; // nur, falls vorhanden

            QuizPanel.Visibility = Visibility.Visible;

            index = 0;
            points = 0;

            SetQuestionUI();
        }

        private void SetQuestionUI()
        {
            if (questions == null || questions.Count == 0) return;
            QuestionText.Text = questions[index].Frage;
            AnswerBox.Text = "";
            ProgressText.Text = $"Frage {index + 1} von {questions.Count}";
        }

        // -----------------------
        // Nächste Frage / Auswertung
        // -----------------------
        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();

            string given = AnswerBox.Text.Trim().ToLower();
            string expected = questions[index].Antwort.ToLower();

            if (!string.IsNullOrEmpty(given) && given == expected)
            {
                points++;
                PlayCorrect();
            }
            else
            {
                PlayWrong();
            }

            index++;

            if (index >= questions.Count)
            {
                // Quiz Ende
                PlayClick();
                MessageBox.Show($"Quiz beendet!\n\nPunkte: {points}/{questions.Count}", "Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);

                // Highscore speichern
                SaveHighscore(new HighscoreEntry { Name = currentUser ?? "Gast", Score = points, Date = DateTime.Now });

                // Zurück zur Auswahl (mit Animation)
                FadeOut(QuizPanel, 200, () =>
                {
                    QuizPanel.Visibility = Visibility.Collapsed;
                    SelectionCard.Visibility = Visibility.Visible;
                    FadeIn(SelectionCard);
                });

                return;
            }

            SetQuestionUI();
        }

        // -----------------------
        // Highscore-Handling
        // -----------------------
        private void SaveHighscore(HighscoreEntry entry)
        {
            try
            {
                Directory.CreateDirectory(highscoreDir);

                List<HighscoreEntry> highs = new();

                // Falls Datei existiert → laden
                if (File.Exists(highscoreFile))
                {
                    var json = File.ReadAllText(highscoreFile);
                    highs = JsonSerializer.Deserialize<List<HighscoreEntry>>(json) ?? new List<HighscoreEntry>();
                }

                // Neuen Score anhängen (kein Limit!)
                highs.Add(entry);

                // Nach Score absteigend, danach Datum neu zu alt
                highs = highs
                    .OrderByDescending(h => h.Score)
                    .ThenByDescending(h => h.Date)
                    .ToList();

                File.WriteAllText(
                    highscoreFile,
                    JsonSerializer.Serialize(highs, new JsonSerializerOptions { WriteIndented = true })
                );

                LoadHighscores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern des Highscores:\n" + ex.Message, "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadHighscores()
        {
            try
            {
                if (!File.Exists(highscoreFile))
                {
                    HighscoreList.ItemsSource = new List<HighscoreEntry>();
                    return;
                }

                var json = File.ReadAllText(highscoreFile);
                var highs = JsonSerializer.Deserialize<List<HighscoreEntry>>(json) ?? new List<HighscoreEntry>();

                // Sortierung: beste zuerst
                highs = highs
                    .OrderByDescending(h => h.Score)
                    .ThenByDescending(h => h.Date)
                    .ToList();

                HighscoreList.ItemsSource = highs;
            }
            catch
            {
                HighscoreList.ItemsSource = new List<HighscoreEntry>();
            }
        }


        private void ClearHighscores_Click(object sender, RoutedEventArgs e)
        {
            PlayClick();
            try
            {
                if (File.Exists(highscoreFile)) File.Delete(highscoreFile);
                LoadHighscores();
            }
            catch
            {
                MessageBox.Show("Konnte Highscores nicht löschen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // -----------------------
        // Highscore-Eintrag-Klasse
        // -----------------------
        public class HighscoreEntry
        {
            public string Name { get; set; } = string.Empty;
            public int Score { get; set; }
            public DateTime Date { get; set; }

            public override string ToString()
            {
                return $"{Name} – {Score} Punkte";
            }
        }

    }
}
