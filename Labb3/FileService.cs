using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3
{
    public class FileService : IFileService
    {
        public QuestionPack OpenQuestionPack()
        {
            // Show a file dialog to select a saved pack
            // and return the loaded QuestionPack instance
            return null;
        }

        public void SaveQuestionPack(QuestionPack pack)
        {
            // Show a file dialog to save the current pack
        }
    }
}
