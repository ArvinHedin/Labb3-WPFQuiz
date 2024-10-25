using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowsViewModel? mainWindowsViewModel;

        public ConfigurationViewModel(MainWindowsViewModel? mainWindowsViewModel)
        {
            this.mainWindowsViewModel = mainWindowsViewModel;
        }


        public QuestionPackViewModel? ActivePack { get => mainWindowsViewModel.ActivePack; }
    }
}
