using Labb3.Dialogs;
using Labb3.Model;
using Labb3.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Labb3
{
    public class DialogService : IDialogService
    {
        
        public QuestionPack ShowCreateNewPackDialog()
        {
            var dialog = new CreateNewPackDialog();

            if (Application.Current.MainWindow != null)
                dialog.Owner = Application.Current.MainWindow;

            if (dialog.ShowDialog() == true)
            {
                return new QuestionPack(dialog.PackName)
                {
                    Difficulty = dialog.Difficulty,
                    TimeLimit = dialog.TimeLimit,
                    Questions = new List<Question>()
                };
            }

            return null;
        }

        public void ShowPackOptionsDialog(QuestionPack pack)
        {
            var dialog = new PackOptionsDialog(pack);

            if (Application.Current.MainWindow != null)
                dialog.Owner = Application.Current.MainWindow;

            dialog.ShowDialog();
        }

    }
}
