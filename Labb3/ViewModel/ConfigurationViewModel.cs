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
using Labb3.Command;
using System.Collections.Specialized;
using System.ComponentModel;


namespace Labb3.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowsViewModel? mainWindowsViewModel;
        private QuestionPack? _currentPack;
        private Question? _selectedQuestion;
        private ObservableCollection<QuestionPack>? _questionPacks;
        private ObservableCollection<Question>? _questions;
        private IDialogService? _dialogService;
        private CommandContainer _commandContainer;

        public CommandContainer CommandContainer
        {
            get { return _commandContainer; }
            set
            {
                _commandContainer = value;
                RaisePropertyChanged(nameof(CommandContainer));
            }
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


        public ConfigurationViewModel(MainWindowsViewModel? mainWindowsViewModel)
        {
            this.mainWindowsViewModel = mainWindowsViewModel;
        }

        public ConfigurationViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            QuestionPacks = new ObservableCollection<QuestionPack>();
            Questions = new ObservableCollection<Question>();
            CurrentPack = new QuestionPack("New Pack");

            AddQuestionCommand = new RelayCommand(param => AddQuestion(), param => CurrentPack != null);
            RemoveQuestionCommand = new RelayCommand(param => RemoveQuestion(), param => SelectedQuestion != null);
            PackOptionsCommand = new RelayCommand(param => OpenPackOptions(), param => CurrentPack != null);

            QuestionPackMessenger.QuestionPackUpdated += OnQuestionPackUpdated;

            LoadQuestionPacks();
        }
       
        private void OnQuestionPackUpdated(object sender, UpdatedEventArgs e)
        {
            QuestionPacks = e.UpdatedPacks;
            CurrentPack = e.NewPack;

            LoadQuestions();
            if (Questions != null && Questions.Any())
            {
                SelectedQuestion = Questions.First();
            }
            
            RaisePropertyChanged(nameof(QuestionPacks));
            RaisePropertyChanged(nameof(CurrentPack));
        }
       
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

                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                string filePath = Path.Combine(appDataPath, "questionpacks.json");
                string json = JsonSerializer.Serialize(QuestionPacks.ToList());
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving question packs: {ex.Message}");
            }
        }

        private void LoadQuestions()
        {
            if (CurrentPack != null)
            {
                Questions = new ObservableCollection<Question>(CurrentPack.Questions);
                Questions.CollectionChanged += Questions_CollectionChanged;

                foreach (var question in Questions)
                {
                    question.PropertyChanged += Question_PropertyChanged;
                }
            }
            else
            {
                Questions?.Clear();
            }
        }

        private void AddQuestion()
        {
            var newQuestion = new Question
            {
                Text = "New Question"
            };
            Questions.Add(newQuestion);
            CurrentPack.Questions = Questions.ToList();
            SelectedQuestion = newQuestion;
            SaveQuestionPacks();
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
        public void RemoveSelectedQuestion()
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
                    Questions.Remove(SelectedQuestion);
                    CurrentPack.Questions = Questions.ToList();

                    if (Questions.Any())
                    {
                        SelectedQuestion = Questions.First();
                    }

                    SaveQuestionPacks();
                }
            }
        }

        private void OpenPackOptions()
        {
            if (CurrentPack != null)
            {
                _dialogService.ShowPackOptionsDialog(CurrentPack);
                SaveQuestionPacks();
                RaisePropertyChanged(nameof(CurrentPack));
            }
            else
            {
                MessageBox.Show(
                    "Please select a question pack first.",
                    "No Pack Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        private void Questions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Question question in e.NewItems)
                {
                    question.PropertyChanged += Question_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Question question in e.OldItems)
                {
                    question.PropertyChanged -= Question_PropertyChanged;
                }
            }

            if (CurrentPack != null)
            {
                CurrentPack.Questions = Questions.ToList();
                SaveQuestionPacks();
            }
        }

        private void Question_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SaveQuestionPacks();
        }
    }
}

