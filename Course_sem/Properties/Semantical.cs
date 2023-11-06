using System;
using System.Collections.Generic;

namespace Course_sem.Properties
{
    class Semantical
    {
        
        private LexicalAnalyze beginLexicalAnalyze = new LexicalAnalyze();
        private bool Correct = true;
        public Semantical(Queue<string> words)
        {
            foreach (var word in words)
            {
                Correct &= beginLexicalAnalyze.SeparateThem(word);

            }
            beginLexicalAnalyze.ShowAll();
            if(Correct) Console.WriteLine("Everything fine! Good for you");
        }
    }
}