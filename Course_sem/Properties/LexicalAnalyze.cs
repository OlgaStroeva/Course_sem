using System;
using System.Collections.Generic;
using static Course_sem.Properties.CodeUtils;

namespace Course_sem.Properties
{
    class LexicalAnalyze
    {
        private Stack<string> keywords = new Stack<string>();
        private List<string> separators = new List<string>(), constants = new List<string>();
        
        private HashSet<string> IDs = new HashSet<string>();
        public LexicalAnalyze()
        {
        }
        public bool CheckID(string id)
        {
            if (keywords.Peek() == "dim")
            {
                IDs.Add(id);
            } else if (!IDs.Contains(id))
            {
                return false;
            }
            return true;
        }

        public bool SeparateThem(string word)
        {
            if(IsKeyword(word)) keywords.Push(word);
            else if (IsSeparator(word)) separators.Add(word);
            else if (IsConstantValue(word)) constants.Add(word);
            else if (IsCorrectId(word))
            {
                if (!CheckID(word))
                {
                    Console.WriteLine("Syntax Error because of {0}! Id here isn't exist! Fix it.",
                        word);
                    return false;
                }
            }
            else if (!IsOperator(word))
                return false;
            return true;
        }
        public void ShowAll(){
            Console.WriteLine("IDs:");
            foreach (var word in IDs)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine("Constants:");
            foreach (var word in constants)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine("Separators:");
            foreach (var word in separators)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine("Keywords:");
            foreach (var word in keywords)
            {
                Console.WriteLine(word);
            }
        }
    }
    
}