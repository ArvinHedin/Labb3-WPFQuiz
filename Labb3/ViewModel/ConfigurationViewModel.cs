using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;


namespace Labb3.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowsViewModel? mainWindowsViewModel;

        private QuestionPack _currentPack;
        private Question _selectedQuestion;
        private ObservableCollection<QuestionPack> _questionPacks;
        private ObservableCollection<Question> _questions;
        private IDialogService _dialogService;
       

        public ConfigurationViewModel(MainWindowsViewModel? mainWindowsViewModel)
        {
            this.mainWindowsViewModel = mainWindowsViewModel;
        }

        public ConfigurationViewModel()
        {
            QuestionPacks = new ObservableCollection<QuestionPack>();
        }


        public ObservableCollection<QuestionPack> QuestionPacks
        {
            get => _questionPacks;
            set
            {
                _questionPacks = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                RaisePropertyChanged();
            }
        }

        public QuestionPack CurrentPack
        {
            get => _currentPack;
            set
            {
                _currentPack = value;
                RaisePropertyChanged();
                LoadQuestions();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                RaisePropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }


        public ICommand AddQuestionCommand { get; }
        public ICommand RemoveQuestionCommand { get; }
        public ICommand PackOptionsCommand { get; }
        public ICommand NewPackCommand { get; }
        public ICommand RemovePackCommand { get; }

        private void LoadQuestionPacks()
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "QuizConfigurator");

                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                    return;
                }

                string filePath = Path.Combine(appDataPath, "questionpacks.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var packs = JsonSerializer.Deserialize<List<QuestionPack>>(json);
                    QuestionPacks = new ObservableCollection<QuestionPack>(packs);
                }
            }
            catch (Exception ex)
            {
                // Handle loading errors
                MessageBox.Show($"Error loading question packs: {ex.Message}");
            }
        }

        private void SaveQuestionPacks()
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "QuizConfigurator");

                string filePath = Path.Combine(appDataPath, "questionpacks.json");
                string json = JsonSerializer.Serialize(QuestionPacks.ToList());
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Handle saving errors
                MessageBox.Show($"Error saving question packs: {ex.Message}");
            }
        }

        private void LoadQuestions()
        {
            if (CurrentPack != null)
            {
                Questions = new ObservableCollection<Question>(CurrentPack.Questions);
            }
            else
            {
                Questions.Clear();
            }
        }

        private void AddQuestion()
        {
            var newQuestion = new Question
            {
                Text = "New Question"
            };
            Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
        }

        private void RemoveQuestion()
        {
            if (SelectedQuestion != null)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to remove this question?",
                    "Confirm Remove",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    CurrentPack.Questions.Remove(SelectedQuestion);
                    Questions.Remove(SelectedQuestion);
                    SaveQuestionPacks();
                }
            }
        }

        private void OpenPackOptions()
        {
            //var dialog = _dialogService.ShowPackOptionsDialog(_currentPack);
            //if (dialog.ShowDialog() == true)
            //{
            //    // Pack properties are updated in the dialog
            //    RaisePropertyChanged(nameof(CurrentPack));
            //    SaveQuestionPacks();
            //}
        }

        private void CreateNewPack()
        {
            //var dialog = new CreateNewPackDialog();
            //if (dialog.ShowDialog() == true)
            //{
            //    var newPack = new QuestionPack
            //    {
            //        Name = dialog.PackName,
            //        Difficulty = dialog.Difficulty,
            //        TimeLimit = dialog.TimeLimit,
            //        Questions = new List<Question>()
            //    };

            //    QuestionPacks.Add(newPack);
            //    CurrentPack = newPack;
            //    SaveQuestionPacks();
            //}
        }

        private void RemovePack()
        {
            if (CurrentPack != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove the pack '{CurrentPack.Name}'?",
                    "Confirm Remove",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    QuestionPacks.Remove(CurrentPack);
                    CurrentPack = null;
                    SaveQuestionPacks();
                }
            }
        }
    }
}

