using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CyberBotApp
{
    public class CyberAiManager
    {
        public class TaskItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderDate { get; set; }
            public bool IsCompleted { get; set; }

            public override string ToString() =>
                $"{Title} - {(IsCompleted ? "✔ Completed" : "Pending")}" +
                (ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value.ToShortDateString()})" : "");
        }

        public class ActivityLogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Description { get; set; }

            public override string ToString() => $"{Timestamp:T}: {Description}";
        }

        public class QuizQuestion
        {
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public int CorrectIndex { get; set; }
            public string Explanation { get; set; } = "";
        }

        public List<TaskItem> Tasks { get; private set; } = new List<TaskItem>();
        public List<ActivityLogEntry> ActivityLog { get; private set; } = new List<ActivityLogEntry>();
        public List<QuizQuestion> QuizQuestions { get; private set; }

        public int QuizIndex { get; private set; } = 0;
        public int Score { get; private set; } = 0;

        public CyberAiManager()
        {
            LoadQuizData();
        }

        private void LoadQuizData()
        {
            QuizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What is the most secure way to create a password?",
                    Options = new List<string> { "Use your pet's name", "Use a mix of cases, numbers, symbols", "Use your birthday", "Use the same password everywhere" },
                    CorrectIndex = 1,
                    Explanation = "Strong passwords mix upper/lowercase letters, numbers, and symbols."
                },
                new QuizQuestion
                {
                    Question = "Which is a sign of a phishing attempt?",
                    Options = new List<string> { "A secure HTTPS link", "An email asking for your credentials", "An email from your boss", "A bookmarked site" },
                    CorrectIndex = 1,
                    Explanation = "Phishing emails often ask for login info or personal data."
                },
                new QuizQuestion
                {
                    Question = "What should you do with a suspicious email?",
                    Options = new List<string> { "Open attachments", "Reply with password", "Report as spam/phishing", "Click all links to check" },
                    CorrectIndex = 2,
                    Explanation = "Never engage; always report suspicious emails."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a good way to protect your computer?",
                    Options = new List<string> { "Disable antivirus", "Regularly update software", "Use open Wi-Fi without VPN", "Share passwords with friends" },
                    CorrectIndex = 1,
                    Explanation = "Keeping software updated patches security vulnerabilities."
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string> { "A backup password", "Using two passwords", "A second layer of security with SMS/app codes", "Changing password twice" },
                    CorrectIndex = 2,
                    Explanation = "2FA adds an extra verification step like SMS or authenticator apps."
                },
                new QuizQuestion
                {
                    Question = "Which is safest for storing passwords?",
                    Options = new List<string> { "Writing on paper", "In your browser only", "A secure password manager", "Your phone notes app" },
                    CorrectIndex = 2,
                    Explanation = "Password managers encrypt and securely store your credentials."
                },
                // True/False
                new QuizQuestion
                {
                    Question = "Using '123456' as your password is secure.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "This is one of the most common and insecure passwords."
                },
                new QuizQuestion
                {
                    Question = "You should always install updates when prompted.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "Updates fix security holes; always install them."
                },
                new QuizQuestion
                {
                    Question = "It's safe to click on unknown links if they look professional.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Even professional-looking links can be phishing."
                }
            };
        }

        public void LoadNextQuestion(ComboBox comboBox, TextBlock questionText)
        {
            if (QuizIndex >= QuizQuestions.Count)
            {
                questionText.Text = $"Quiz Complete! 🎉 You scored {Score} out of {QuizQuestions.Count}.";
                comboBox.ItemsSource = null;
                return;
            }

            var q = QuizQuestions[QuizIndex];
            questionText.Text = q.Question;
            comboBox.ItemsSource = q.Options;
            comboBox.SelectedIndex = -1;
        }

        public void SubmitAnswer(int selectedIndex, ComboBox comboBox, TextBlock feedbackText, TextBox activityLog)
        {
            if (QuizIndex >= QuizQuestions.Count || selectedIndex < 0)
            {
                feedbackText.Text = "Please select an answer.";
                return;
            }

            var q = QuizQuestions[QuizIndex];
            if (selectedIndex == q.CorrectIndex)
            {
                Score++;
                feedbackText.Text = $"✅ Correct! {q.Explanation}";
                activityLog.AppendText($"Answered correctly: {q.Question}\n");
            }
            else
            {
                feedbackText.Text = $"❌ Incorrect. Correct: {q.Options[q.CorrectIndex]}. {q.Explanation}";
                activityLog.AppendText($"Incorrect: {q.Question}\n");
            }

            QuizIndex++;
            LoadNextQuestion(comboBox, feedbackText);
        }

        public void ResetQuiz()
        {
            QuizIndex = 0;
            Score = 0;
        }

        public void AddTask(string title, string desc, DateTime? reminder, ListBox taskListBox, TextBox titleBox, TextBox descBox, DatePicker reminderPicker, TextBox chatOutput)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = desc,
                ReminderDate = reminder,
                IsCompleted = false
            };

            Tasks.Add(task);
            taskListBox.Items.Add(task);
            LogAction($"Task added: {task.Title}", chatOutput);

            titleBox.Clear();
            descBox.Clear();
            reminderPicker.SelectedDate = null;

            chatOutput.AppendText($"Task '{task.Title}' added.\n");
        }

        public void LogAction(string description, TextBox chatOutput)
        {
            var log = new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Description = description
            };

            ActivityLog.Add(log);
            if (ActivityLog.Count > 10)
                ActivityLog.RemoveAt(0);

            chatOutput.AppendText($"{log}\n");
        }
    }
}
