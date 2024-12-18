using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.Command
{
    public class UpdatedEventArgs : EventArgs
    {
        public QuestionPack NewPack { get; set; }
        public ObservableCollection<QuestionPack> UpdatedPacks { get; set; }
    }

    public static class QuestionPackMessenger
    {
        public static event EventHandler<UpdatedEventArgs> QuestionPackUpdated;
        
        public static void NotifyUpdated(QuestionPack newPack, ObservableCollection<QuestionPack> updatedPacks)
        {
            QuestionPackUpdated?.Invoke(null, new UpdatedEventArgs
            {
                NewPack = newPack,
                UpdatedPacks = updatedPacks
            });
        }
    }
}
