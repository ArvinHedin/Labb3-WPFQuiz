using Labb3.Command;
using Labb3.Model;
using Labb3.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Labb3.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowsViewModel? mainWindowsViewModel;

        private QuestionPack? _currentPack;
        private Question? _currentQuestion;
        private int _currentQuestionIndex;
        private DispatcherTimer? _timer;
        private int _remainingTime;
        private double _timerPercentage;
        private bool _isTimerRunning;
        private bool _showAnswerFeedback;
        private int _correctAnswerIndex;
        private CommandContainer _commandContainer;
        private List<string> _randomizedAnswers;
        private int _correctAnswers;
        private int _totalQuestions;

        public string TimerDisplay => $"{RemainingTime:D2}";
        public ICommand AnswerCommand { get; }
        public RelayCommand UpdateButtonCommand { get; }
        public int CorrectAnswers 
        {
            get => _correctAnswers;
            set
            {
                _correctAnswers = value;
                RaisePropertyChanged();
            }
        }

        public int TotalQuestions
        {
            get =>_totalQuestions; 
            set 
            { 
                _totalQuestions = value;
                RaisePropertyChanged();
            }
        }


        public QuestionPack CurrentPack 
        { 
            get => _currentPack;
            set
            {
                _currentPack = value;
                if (_currentPack != null)
                {
                    InitializeGame();
                }
                RaisePropertyChanged();
            }
        }

        public int CorrectAnswerIndex
        {
            get => _correctAnswerIndex;
            set
            {
                _correctAnswerIndex = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowAnswerFeedback
        {
            get => _showAnswerFeedback;
            set
            {
                _showAnswerFeedback = value;
                RaisePropertyChanged();
            }
        }

        public Question CurrentQuestion 
        { 
            get => _currentQuestion;
            set 
            {
                _currentQuestion = value;
                RandomizeAnswers();
                RaisePropertyChanged();
            }
        }
        public int RemainingTime
        {
            get => _remainingTime;
            set 
            { 
                _remainingTime = value;
                RaisePropertyChanged();
                TimerPercentage = (_remainingTime / (double)_currentPack.TimeLimit) * 100;
            }
        }

        public double TimerPercentage
        {
            get => _timerPercentage;
            set
            {
                _timerPercentage = value;
                RaisePropertyChanged();
            }
        }

        public bool IsTimerRunning
        {
            get => _isTimerRunning;
            set
            {
                _isTimerRunning = value;
                RaisePropertyChanged();
            }
        }

        public CommandContainer CommandContainer
        {
            get { return _commandContainer; }
            set
            {
                _commandContainer = value;
                RaisePropertyChanged(nameof(CommandContainer));
            }
        }
        public List<string> RandomizedAnswers
        {
            get => _randomizedAnswers;
            set
            {
                _randomizedAnswers = value;
                RaisePropertyChanged();
            }
        }

        public PlayerViewModel(MainWindowsViewModel? mainWindowsViewModel)
        {
            this.mainWindowsViewModel = mainWindowsViewModel;
        }

        public PlayerViewModel()
        {
            _randomizedAnswers = new List<string>();
            AnswerCommand = new RelayCommand(param => HandleAnswer(Convert.ToInt32(param)));

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

       

        public void InitializeGame()
        {
            if (CurrentPack != null)
            {
                _currentQuestionIndex = 0;
                CorrectAnswers = 0;
                TotalQuestions = CurrentPack.Questions.Count;
                CurrentQuestion = CurrentPack.Questions[_currentQuestionIndex];
                RemainingTime = CurrentPack.TimeLimit;
                IsTimerRunning = false;
                RandomizeAnswers();

                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Interval = TimeSpan.FromSeconds(1);
                    _timer.Tick += Timer_Tick;
                }
                StartTimer();
            }
        }

        private void RandomizeAnswers()
        {
            if (CurrentQuestion == null) return;

            var answers = new List<string>
            {
                CurrentQuestion.CorrectAnswer,
                CurrentQuestion.IncorrectAnswer1,
                CurrentQuestion.IncorrectAnswer2,
                CurrentQuestion.IncorrectAnswer3
            };

            Random random = new Random();
            for (int i = answers.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = answers[i];
                answers[i] = answers[j];
                answers[j] = temp;
            }

            RandomizedAnswers = answers;
            CorrectAnswerIndex = RandomizedAnswers.IndexOf(CurrentQuestion.CorrectAnswer);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (RemainingTime > 0)
            {
                RemainingTime--;
                RaisePropertyChanged(nameof(TimerDisplay));
            }
            else
            {
                TimeUp();
            }
        }
        private void StartTimer()
        {
            if (_timer != null && CurrentPack != null)
            {
                RemainingTime = CurrentPack.TimeLimit;
                TimerPercentage = 100;
                IsTimerRunning = true;
                _timer.Start();
            }
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                IsTimerRunning = false;
            }
        }

        private void TimeUp()
        {
            if (CurrentPack != null)
            {
                StopTimer();
                HandleAnswer(+1);
            }
        }

        private void HandleAnswer(int answerIndex)
        {
            StopTimer();
            bool isCorrect = RandomizedAnswers[answerIndex] == CurrentQuestion.CorrectAnswer;
            ShowAnswerFeedback = true;
            
            if (isCorrect)
            {
                CorrectAnswers++;
            }


            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Task.Delay(1500).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ShowAnswerFeedback = false;
                        MoveToNextQuestion();
                    });
                });
            }));
        }


        private void MoveToNextQuestion()
        {
            _currentQuestionIndex++;
            if (_currentQuestionIndex < CurrentPack.Questions.Count)
            {
                CurrentQuestion = CurrentPack.Questions[_currentQuestionIndex];
                RemainingTime = CurrentPack.TimeLimit;
                StartTimer();
            }
            else
            {
                ShowResults();
            }
        }

        private void ShowResults()
        {
            var commandContainer = (Application.Current.MainWindow?.DataContext as MainWindowsViewModel)?.CommandContainer;
            if (commandContainer != null)
            {
                var resultsView = new ResultsView();
                commandContainer.CurrentView = resultsView;
            }
        }
    }
}
