﻿using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3
{
    public interface IFileService
    {
        QuestionPack OpenQuestionPack();
        void SaveQuestionPack(QuestionPack pack);
    }
}
