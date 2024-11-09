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
        public ObservableCollection<Question> Questions { get; }

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
        }

       

    }
}
