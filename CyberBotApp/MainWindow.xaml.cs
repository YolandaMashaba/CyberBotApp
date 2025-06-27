using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CyberBotApp
{
    public partial class MainWindow : Window
    {
        private string userName;
        private CyberAiManager aiManager = new CyberAiManager();
        private MemoryManager memoryManager = new MemoryManager();
        private QuestionHandler questionHandler;
        private AudioImageHandler mediaHandler = new AudioImageHandler();

        public MainWindow()
        {
            InitializeComponent();
            questionHandler = new QuestionHandler(memoryManager);
            aiManager.LoadNextQuestion(QuizOptionsComboBox, QuizQuestionText);
            AiLogoImage.Source = mediaHandler.GetAiLogoImage();
        }

        private void SubmitNameButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UserNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || !System.Text.RegularExpressions.Regex.IsMatch(name, @"^[A-Za-z\s]+$"))
            {
                MessageBox.Show("Please enter a valid name (letters only).");
                return;
            }

            userName = name;
            WelcomeMessage.Text = $"Welcome, {userName}! Get ready to learn about cybersecurity.";
            WelcomeMessage.Visibility = Visibility.Visible;
            AiLogoImage.Visibility = Visibility.Visible;

            try
            {
                mediaHandler.PlayWelcomeAudio();
            }
            catch { }

            ChatTab.IsEnabled = true;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AppendToChat($"You: {input}");
            RespondToUser(input);
            UserInputBox.Clear();
        }

        private void RespondToUser(string input)
        {
            var responses = questionHandler.GetResponse(input);
            foreach (var response in responses)
            {
                AppendToChat(response);
            }
        }

        private void AppendToChat(string message)
        {
            ChatHistoryBox.Document.Blocks.Add(new Paragraph(new Run(message))
            {
                Foreground = Brushes.White
            });
            ChatHistoryBox.ScrollToEnd();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) =>
            ChatHistoryBox.Document.Blocks.Clear();

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var history = memoryManager.GetConversationHistory();
            if (history.Count == 0)
            {
                AppendToChat("No conversation history yet.");
            }
            else
            {
                AppendToChat("Previous conversation history:");
                foreach (var line in history)
                {
                    AppendToChat(line);
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text;
            string desc = TaskDescBox.Text;
            DateTime? reminder = ReminderPicker.SelectedDate;

            aiManager.AddTask(title, desc, reminder, TaskListBox, TaskTitleBox, TaskDescBox, ReminderPicker, ActivityLogTextBox);
        }

        private void MarkDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is CyberAiManager.TaskItem selectedTask)
            {
                selectedTask.IsCompleted = true;
                TaskListBox.Items.Refresh();
                aiManager.LogAction($"Marked task as completed: {selectedTask.Title}", ActivityLogTextBox);
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is CyberAiManager.TaskItem selectedTask)
            {
                TaskListBox.Items.Remove(selectedTask);
                aiManager.Tasks.Remove(selectedTask);
                aiManager.LogAction($"Deleted task: {selectedTask.Title}", ActivityLogTextBox);
            }
        }

        private void SubmitQuizButton_Click(object sender, RoutedEventArgs e)
        {
            int selected = QuizOptionsComboBox.SelectedIndex;
            aiManager.SubmitAnswer(selected, QuizOptionsComboBox, QuizFeedbackText, ActivityLogTextBox);
        }

        private void ResetQuizButton_Click(object sender, RoutedEventArgs e)
        {
            aiManager.ResetQuiz();
            QuizFeedbackText.Text = string.Empty;
            aiManager.LoadNextQuestion(QuizOptionsComboBox, QuizQuestionText);
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            ActivityLogTextBox.Clear();
        }

        private void ActivityLogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActivityLogTextBox.ScrollToEnd();
        }
    }

    public class MemoryManager
    {
        private List<string> conversationHistory = new List<string>();

        public void SaveConversation(List<string> entries)
        {
            conversationHistory.AddRange(entries);
        }

        public List<string> GetConversationHistory()
        {
            return new List<string>(conversationHistory);
        }

        public void ClearHistory()
        {
            conversationHistory.Clear();
        }
    }
}