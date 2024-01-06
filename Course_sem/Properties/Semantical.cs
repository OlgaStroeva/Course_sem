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
        }

        public string GetSemanticalResult()
        {
            if (Correct) return "Everything fine! Good for you";
            else return beginLexicalAnalyze.GetResult();
        }

        public void GetDataLex(ref Stack<string> keywords,
            ref List<string> separators, ref List<string>constants,
            ref HashSet<string> IDs, ref string text)
        {
            keywords = beginLexicalAnalyze.GetKeywords();
            separators = beginLexicalAnalyze.Getseparators();
            constants = beginLexicalAnalyze.Getconstants();
            IDs = beginLexicalAnalyze.GetIDs();
            text = beginLexicalAnalyze.GetText();
        }
    }
}