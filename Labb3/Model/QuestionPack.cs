using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.Model
{
    public enum Difficulty { Easy, Medium, Hard };
    public class QuestionPack
    {
        public QuestionPack()
        {
            Name = string.Empty;
            Questions = new List<Question>();
        }

        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimit = timeLimitInSeconds;
            Questions = new List<Question>();
        }

        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimit { get; set; }
        public List<Question> Questions { get; set; } = new();
    }
}
