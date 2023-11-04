using System.Collections.Generic;

namespace Course_sem.Properties
{
    public class LexicalAnalyze
    {
        private Stack<string> keywords = new Stack<string>();
        private HashSet<string> IDs = new HashSet<string>();
        public LexicalAnalyze(Queue<string> words)
        {
        }
    }
}