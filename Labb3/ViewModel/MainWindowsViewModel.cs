using Labb3.Command;
using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Windows.Input;

namespace Labb3.ViewModel
{
    public class MainWindowsViewModel : ViewModelBase
    {
        public PlayerViewModel PlayerViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }

		private QuestionPack _currentPack;

        public QuestionPack CurrentPack
        {
            get => _currentPack;
            set
            {
                _currentPack = value;
                RaisePropertyChanged();
            }
        }
		private ViewModelBase _currentView;

		public ViewModelBase CurrentView
		{
			get => _currentView;
			set 
			{ 
				_currentView = value;
				RaisePropertyChanged();
			}	
		}
        private ConfigurationViewModel _configurationViewModel;
        private PlayerViewModel _playerViewModel;

        

        private readonly IDialogService _dialogService;
		private readonly IFileService _fileService;


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


        public MainWindowsViewModel()
        {

        }



        public MainWindowsViewModel(IDialogService dialogservice, IFileService fileService)
        {
            _dialogService = dialogservice;
            _fileService = fileService;

            _configurationViewModel = new ConfigurationViewModel();
            _playerViewModel = new PlayerViewModel();

            
            
            
            PlayerViewModel = new PlayerViewModel(this);
			ConfigurationViewModel = new ConfigurationViewModel();
            CurrentView = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
            SelectPackCommand = new RelayCommand(param => SelectPack());
            NewPackCommand = new RelayCommand(param => NewPack());
            RemovePackCommand = new RelayCommand(param => RemovePack());
            ExitCommand = new RelayCommand(param => Exit());
            AddQuestionCommand = new RelayCommand(param => AddQuestion());
            RemoveQuestionCommand = new RelayCommand(param => RemoveQuestion());
            PackOptionsCommand = new RelayCommand(param => OpenPackOptions());
            SwitchToConfigurationCommand = new RelayCommand(param => SwitchToConfiguration());
            SwitchToPlayerCommand = new RelayCommand(param => SwitchToPlayer());
            ToggleFullScreenCommand = new RelayCommand(parma => ToggleFullScreen());

        }


        private void SelectPack()
        {
            var pack = _fileService.OpenQuestionPack();
            
            if (pack != null)
            {
                _currentPack = pack;
                // Switch to config view
            }
        }

        private void NewPack()
        {
            var newPack = _dialogService.ShowCreateNewPackDialog();
            if (newPack != null)
            {
                _currentPack = newPack;
                // Switch to config view
            }
        }

        private void RemovePack()
        {
            _currentPack = null;
        }

        private void Exit()
        {
            Application.Exit();
        }

        private void AddQuestion()
        {
            _dialogService.ShowAddQuestionDialog(_currentPack);
        }

        private void RemoveQuestion()
        {
            _dialogService.ShowRemoveQuestionDialog(_currentPack);
        }

        private void OpenPackOptions()
        {
            _dialogService.ShowPackOptionsDialog(_currentPack);
        }
        private void SwitchToConfiguration()
        {
            // Switch the main view to the configuration view
            CurrentView = _configurationViewModel;
        }

        private void SwitchToPlayer()
        {
            // Switch the main view to the player view
            CurrentView = _playerViewModel;
        }

        private void ToggleFullScreen()
        {
            // Toggle the main window between fullscreen and windowed mode
            // (implementation omitted for brevity)
        }

    }
}
