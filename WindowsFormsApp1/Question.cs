using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Question
    {
        public Regex[] RegexTriggers { get; private set; }
        private string Answer;

        public Question(params string[] list)
        {
            if (list.Length >= 1)
            {
                Answer = list[list.Length-1];
                RegexTriggers = new Regex[list.Length - 1];

                for (int i = 0; i < list.Length-1; i++)
                {
                    RegexTriggers[i] = new Regex(list[i], RegexOptions.IgnoreCase);
                }
            }
            else
                throw new Exception("question err");
        }

        public bool Triggered(string question)
        {
            bool triggered = true;
            foreach (Regex reg in RegexTriggers)
            {
                if (!reg.IsMatch(question))
                    triggered = false;
            }
            return triggered;
        }

        public string GetAnswer()
        {
            return Answer;
        }
    }
}
