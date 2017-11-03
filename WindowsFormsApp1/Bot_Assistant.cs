using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Bot_Assistant
    {

        private List<Question> questions;

        public Bot_Assistant(string answersPath)
        {
            questions = new List<Question>();
            StreamReader objReader = new StreamReader(answersPath);

            string[] text_array = File.ReadAllLines(answersPath, Encoding.Default);
            objReader.Close();

            if (text_array == null)
                throw new Exception("Empty questions file");

            foreach (string line in text_array)
            {
                if (line != null)
                {
                    string[] data = line.Split('_');
                    questions.Add(new Question(data));
                }
            }            
        }

        public string Answer(string question)
        {
            foreach(Question q in questions)
            {
                if (q.Triggered(question))
                    return q.GetAnswer();
            }

            return "Я чот вас не понял";
        }
    }
}
