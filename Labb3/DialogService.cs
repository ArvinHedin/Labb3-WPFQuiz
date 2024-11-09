using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3
{
    public class DialogService : IDialogService
    {
        
        public QuestionPack ShowCreateNewPackDialog()
        {
            // Show a dialog to get the new pack name, difficulty, and time limit
            // and return the new QuestionPack instance
            return null;
        }

        public void ShowPackOptionsDialog(QuestionPack pack)
        {
            // Show a dialog to edit the current pack's options
        }

        public void ShowAddQuestionDialog(QuestionPack pack)
        {
            // Show a dialog to add a new question to the current pack
        }
        
        public void ShowRemoveQuestionDialog(QuestionPack pack)
        {

        }
    }
}
