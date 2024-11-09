using Labb3.Command;
using Labb3.Model;
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

        private DispatcherTimer _timer;
        private int _remainingTime;
        private double _timerPercentage;
        private bool _isTimerRunning;
        private QuestionPack _currentPack;
        private Question _currentQuestion;
        private int _currentQuestionIndex;

        

        public int RemainingTime
        {
            get => _remainingTime;
            set 
            { 
                _remainingTime = value;
                RaisePropertyChanged();
                TimerPrecentage = (_remainingTime / (double)_currentPack.TimeLimit) * 100;
            }
        }

        public double TimerPrecentage
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

        public string TimerDisplay => $"{RemainingTime:D2}";

        public ICommand AnswerCommand { get; }

        public RelayCommand UpdateButtonCommand { get; }

        public PlayerViewModel(MainWindowsViewModel? mainWindowsViewModel)
        {
            this.mainWindowsViewModel = mainWindowsViewModel;

            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromSeconds(1);
            //_timer.Tick += Timer_Tick;
            ////timer.Start();

            //UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);
        }

        //        private string _testData = "Start Value";
        //        public string TestData 
        //        { 
        //            get => _testData;
        //            private set 
        //            {
        //                _testData = value;
        //                RaisePropertyChanged();
        //            }
        //        }
        //        private bool CanUpdateButton(object? arg)
        //        {
        //            return TestData.Length < 20;
        //        }

        //        private void UpdateButton(object obj)
        //        {
        //            TestData += "x";
        //            UpdateButtonCommand.RaiseCanExecuteChanged();
        //        }

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
            if (_currentPack == null) return;
            RemainingTime = _currentPack.TimeLimit;
            IsTimerRunning = true;
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
            IsTimerRunning = false;
        }

        private void TimeUp()
        {
            StopTimer();
            HandleAnswer(-1);
        }
        
        private void HandleAnswer(int answerIndex)
        {
            StopTimer();

            bool isCorrect = answerIndex == _currentQuestion.CorrectAnswer.Length;

            ShowAnswerFeedback(isCorrect);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Task.Delay(1500).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(MoveToNextQuestion);
                });
            }));
        }

        private void ShowAnswerFeedback(bool isCorrect)
        {

        }

        private void MoveToNextQuestion()
        {

        }
    }
}
