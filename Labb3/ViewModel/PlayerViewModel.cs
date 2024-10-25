using Labb3.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Labb3.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowsViewModel? mainWindowsViewModel;

        private DispatcherTimer timer;
        private string _testData = "Start Value";
        public DelegateCommand UpdateButtonCommand { get; }

        public string TestData 
        { 
            get => _testData;
            private set 
            {
                _testData = value;
                RaisePropertyChanged();
            }
        }


        public PlayerViewModel(MainWindowsViewModel? mainWindowsViewModel)
        {
            this.mainWindowsViewModel = mainWindowsViewModel;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //timer.Start();

            UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);
        }

        private bool CanUpdateButton(object? arg)
        {
            return TestData.Length < 20;
        }

        private void UpdateButton(object obj)
        {
            TestData += "x";
            UpdateButtonCommand.RaiseCanExecuteChanged();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TestData += "x";
        }
    }
}
