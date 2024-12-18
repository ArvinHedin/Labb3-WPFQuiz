using Labb3.Command;
using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.ViewModel
{
    public class QuestionPackViewModel  : ViewModelBase
    {
        private readonly QuestionPack _model;

        private ObservableCollection<QuestionPack> _questionPacks;
        private QuestionPack _currentPack;
        private readonly IDialogService _dialogService;
        private CommandContainer _commandContainer;
        public ObservableCollection<Question> Questions { get; }
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

        public QuestionPack CurrentPack
        {
            get => _currentPack;
            set
            {
                _currentPack = value;
                RaisePropertyChanged();
            }
        }

        public string Name 
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                RaisePropertyChanged();
            } 
        }
        public Difficulty Difficulty 
        { 
            get => _model.Difficulty;
            set
            {
                _model.Difficulty = value;
                RaisePropertyChanged();
            }
        }
        public int TimeLimitInSeconds 
        { 
            get => _model.TimeLimit;
            set
            {
                _model.TimeLimit = value;
                RaisePropertyChanged();
            } 
        }

        public QuestionPackViewModel(QuestionPack model)
        {
            this._model = model;
            this.Questions = new ObservableCollection<Question>(model.Questions);
            QuestionPackMessenger.QuestionPackUpdated += OnQuestionPackUpdated;
        }
        public QuestionPackViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            QuestionPacks = new ObservableCollection<QuestionPack>();
            QuestionPackMessenger.QuestionPackUpdated += OnQuestionPackUpdated;
        }

        private void OnQuestionPackUpdated(object sender, UpdatedEventArgs e)
        {
            QuestionPacks = e.UpdatedPacks;
            CurrentPack = e.NewPack;

            RaisePropertyChanged(nameof(QuestionPacks));
            RaisePropertyChanged(nameof(CurrentPack));
        }
    }
}
