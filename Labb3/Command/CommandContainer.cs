using Labb3.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Labb3.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Diagnostics;

namespace Labb3.Command
{

    public class CommandContainer
    {
        private readonly IDialogService _dialogService;
        private readonly QuestionPackViewModel _questionPackViewModel;
        private ConfigurationViewModel _configurationViewModel;
        private PlayerViewModel _playerViewModel;
        private ObservableCollection<QuestionPack>? _questionPacks;
        private QuestionPack _currentPack;
        public PlayerViewModel PlayerViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public ICommand SelectPackCommand { get; }
        public ICommand NewPackCommand { get; }
        public ICommand RemovePackCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand AddQuestionCommand { get; }
        public ICommand RemoveQuestionCommand { get; }
        public ICommand PackOptionsCommand { get; }
        public ICommand SwitchToConfigurationCommand { get; }
        public ICommand SwitchToPlayerCommand { get; }
        public ICommand ToggleFullScreenCommand { get; }
        public ICommand RestartQuizCommand { get; }

        public ObservableCollection<QuestionPack> QuestionPacks
        {
            get => _questionPacks;
            set
            {
                _questionPacks = value;
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
            }
        }
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                RaisePropertyChanged(nameof(CurrentView));
            }
        }       

        public CommandContainer(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _questionPackViewModel = new QuestionPackViewModel(dialogService);
            
            QuestionPacks = new ObservableCollection<QuestionPack>();
            LoadQuestionPacks();

            _playerViewModel = new PlayerViewModel();
            _configurationViewModel = new ConfigurationViewModel(dialogService);

            PlayerViewModel = _playerViewModel;
            ConfigurationViewModel =_configurationViewModel;

            SelectPackCommand = new RelayCommand(param =>
            {
                if (param is QuestionPack selectedPack)
                {
                    CurrentPack = selectedPack;
                    QuestionPackMessenger.NotifyUpdated(selectedPack, QuestionPacks);
                    CurrentView = new View.ConfigurationView();
                }
            });
            NewPackCommand = new RelayCommand(param => NewPack());
            RemovePackCommand = new RelayCommand(param => RemovePack());
            ExitCommand = new RelayCommand(param => Exit());
            AddQuestionCommand = new RelayCommand(param => AddQuestion());
            RemoveQuestionCommand = new RelayCommand(param => RemoveQuestion());
            PackOptionsCommand = new RelayCommand(param => OpenPackOptions());
            SwitchToConfigurationCommand = new RelayCommand(param => SwitchToConfiguration());
            SwitchToPlayerCommand = new RelayCommand(param => SwitchToPlayer());
            ToggleFullScreenCommand = new RelayCommand(parma => ToggleFullScreen());
            RestartQuizCommand = new RelayCommand(param => RestartQuiz());

            CurrentView = new View.ConfigurationView();

            QuestionPackMessenger.QuestionPackUpdated += OnQuestionPackUpdated;
        }

        private void OnQuestionPackUpdated(object sender, UpdatedEventArgs e)
        {
            QuestionPacks = e.UpdatedPacks;
            CurrentPack = e.NewPack;

            
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
                if (QuestionPacks == null || !QuestionPacks.Any())
                {
                    return;
                }

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
                MessageBox.Show($"Error saving question packs: {ex.Message}\nStack trace: {ex.StackTrace}");
            }
        }

        private void SelectPack(object parameter)
        {
            if (parameter is QuestionPack selectedPack)
            {
                if (CurrentPack != null)
                {
                    SaveQuestionPacks();
                }

                CurrentPack = selectedPack;
                QuestionPackMessenger.NotifyUpdated(selectedPack, QuestionPacks);
                CurrentView = new View.ConfigurationView();
            }
        }
        private void NewPack()
        {
            var newPack = _dialogService.ShowCreateNewPackDialog();
            if (newPack != null)
            {
                var defualtQuestion = new Question
                {
                    Text = "New Question",
                    CorrectAnswer = "",
                    IncorrectAnswer1 = "",
                    IncorrectAnswer2 = "",
                    IncorrectAnswer3 = ""
                };

                newPack.Questions = new List<Question> { defualtQuestion };
                
                if (QuestionPacks == null)
                {
                    QuestionPacks = new ObservableCollection<QuestionPack>();
                }
                QuestionPacks.Add(newPack);
                CurrentPack = newPack;

                QuestionPackMessenger.NotifyUpdated(newPack, QuestionPacks);

                SaveQuestionPacks();

                RaisePropertyChanged(nameof(QuestionPacks));
                RaisePropertyChanged(nameof(CurrentPack));
            }
        }

        private void RemovePack()
        {
            if (CurrentPack != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove '{CurrentPack.Name}'?",
                    "Remove Question Pack",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    QuestionPacks.Remove(CurrentPack);
                    CurrentPack = null;
                    SaveQuestionPacks();
                    QuestionPackMessenger.NotifyUpdated(null, QuestionPacks);
                }
            }
            else
            {
                MessageBox.Show("Please select a question pack to remove.",
                    "No question pack selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void Exit()
        {
            var result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveQuestionPacks();
                Application.Current.Shutdown();
            }
        }

        private void AddQuestion()
        {
            if (CurrentPack != null)
            {
                var newQuestion = new Question
                {
                    Text = "New Question",
                    CorrectAnswer = "",
                    IncorrectAnswer1 = "",
                    IncorrectAnswer2 = "",
                    IncorrectAnswer3 = ""
                };

                CurrentPack.Questions.Add(newQuestion);
                SaveQuestionPacks();

                QuestionPackMessenger.NotifyUpdated(CurrentPack, QuestionPacks);
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

        private void RemoveQuestion()
        {
            if (CurrentPack == null)
            {
                MessageBox.Show(
                    "Please select a question pack first.",
                    "No Pack Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (CurrentPack.Questions.Count <= 1)
            {
                MessageBox.Show(
                    "Cannot remove the last question. Question packs must have at least one question.",
                    "Cannot Remove Question",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            ConfigurationViewModel.RemoveSelectedQuestion();
        }

        private void OpenPackOptions()
        {
            if (CurrentPack == null)
            {
                MessageBox.Show(
                    "Please select a question pack first.",
                    "No Pack Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            _dialogService.ShowPackOptionsDialog(CurrentPack);
            SaveQuestionPacks();
            QuestionPackMessenger.NotifyUpdated(CurrentPack, QuestionPacks);
        }
        private void SwitchToConfiguration()
        {
            if (PlayerViewModel.IsTimerRunning)
            {
                PlayerViewModel.StopTimer();
            }

            CurrentView = new View.ConfigurationView();

            if (CurrentPack != null)
            {
                QuestionPackMessenger.NotifyUpdated(CurrentPack, QuestionPacks);
            }
        }

        private void SwitchToPlayer()
        {
            if (CurrentPack == null)
            {
                MessageBox.Show(
                    "Please select a question pack first.",
                    "No Pack Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (!CurrentPack.Questions.Any())
            {
                MessageBox.Show(
                    "The selected pack has no questions. Please add questions before playing.",
                    "No Questions",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                PlayerViewModel.CurrentPack = CurrentPack;
                PlayerViewModel.InitializeGame();
                var playerView = new View.PlayerView();
                playerView.DataContext = PlayerViewModel;

                CurrentView = playerView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error switching to player view: {ex.Message}");
            }
        }

        private void ToggleFullScreen()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                if (mainWindow.WindowState != WindowState.Maximized)
                {
                    mainWindow.WindowStyle = WindowStyle.None;
                    mainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                    mainWindow.WindowState = WindowState.Normal;
                }
            }
        }
        private void RestartQuiz()
        {
            if (CurrentPack != null)
            {
                PlayerViewModel.CurrentPack = CurrentPack;
                PlayerViewModel.InitializeGame();
                CurrentView = new View.PlayerView { DataContext = PlayerViewModel };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
