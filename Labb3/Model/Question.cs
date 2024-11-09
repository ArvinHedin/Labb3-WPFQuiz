using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Labb3.Model
{
    public class Question : INotifyPropertyChanged
    {
        private string _text;
        private string _correctAnswer;
        private string _incorrectAnswer1;
        private string _incorrectAnswer2;
        private string _incorrectAnswer3;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        public string CorrectAnswer
        {
            get => _correctAnswer;
            set
            {
                _correctAnswer = value;
                RaisePropertyChanged();
            }
        }

        public string IncorrectAnswer1
        {
            get => _incorrectAnswer1;
            set
            {
                _incorrectAnswer1 = value;
                RaisePropertyChanged();
            }
        }

        public string IncorrectAnswer2
        {
            get => _incorrectAnswer2;
            set
            {
                _incorrectAnswer2 = value;
                RaisePropertyChanged();
            }
        }

        public string IncorrectAnswer3
        {
            get => _incorrectAnswer3;
            set
            {
                _incorrectAnswer3 = value;
                
            }
        }

        public List<string> GetAllAnswers()
        {
            return new List<string>
        {
            CorrectAnswer,
            IncorrectAnswer1,
            IncorrectAnswer2,
            IncorrectAnswer3
        };
        }

        
        public int CorrectAnswerIndex { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
