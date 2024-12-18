using Labb3.Command;
using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Windows.Input;

namespace Labb3.ViewModel
{
    public class MainWindowsViewModel : ViewModelBase
    {
        private ConfigurationViewModel _configurationViewModel;
        private PlayerViewModel _playerViewModel;
        private ObservableCollection<QuestionPack> _questionPacks;
        private readonly IDialogService _dialogService;
        private QuestionPack _currentPack;
		private object _currentView;
        private CommandContainer _commandContainer;

        public PlayerViewModel PlayerViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }

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

		public object CurrentView
		{
			get => CommandContainer?.CurrentView;
			set 
			{ 
				if (CommandContainer != null)
                {
                    CommandContainer.CurrentView = value;
                    RaisePropertyChanged(nameof(CurrentView));
                }
			}	
		}

        public CommandContainer CommandContainer
        {
            get { return _commandContainer; }
            set
            {
                _commandContainer = value;
                RaisePropertyChanged();

                if (_commandContainer != null)
                {
                    _commandContainer.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(CommandContainer.CurrentView))
                        {
                            RaisePropertyChanged(nameof(CurrentView));
                        }
                    };
                }
            }
        }

        public MainWindowsViewModel(IDialogService dialogservice, CommandContainer commandContainer)
        {
            _dialogService = dialogservice;
            _commandContainer = commandContainer;
            CommandContainer = commandContainer;

            _configurationViewModel = new ConfigurationViewModel(dialogservice);
            _playerViewModel = new PlayerViewModel();

            LoadQuestionPacks();

            QuestionPacks = new ObservableCollection<QuestionPack>();
            PlayerViewModel = new PlayerViewModel(this);
			ConfigurationViewModel = new ConfigurationViewModel(this);
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

    }
}
