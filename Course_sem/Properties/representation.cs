using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Course_sem.Properties.CodeUtils;

namespace Course_sem.Properties
{
    public class representation
    {
        private Queue<string> words;
        
        public Stack<string> keywords = new Stack<string>();
        private List<string> separators = new List<string>(), constants = new List<string>();
        private HashSet<string> IDs = new HashSet<string>();

        private string ResultSem, ResultSin, text;
        
        public representation(string code)
        {
            words = SplitCodeIntoWords(code + ';');
            var semantical = new Semantical(words);
            CodeAnalyzer analyzer = new CodeAnalyzer(words);
            semantical.GetDataLex(ref keywords, ref separators, ref constants, ref IDs, ref text);
            ResultSem = semantical.GetSemanticalResult();
            ResultSin = analyzer.AnalyzeCode();
        }

        public void getData()
        {
            foreach (var token in keywords)
            {
                Console.WriteLine(token);
            }
            foreach (var token in separators)
            {
                Console.WriteLine(token);
            }
            foreach (var token in constants)
            {
                Console.WriteLine(token);
            }
            foreach (var token in IDs)
            {
                Console.WriteLine(token);
            }
            Console.WriteLine(ResultSem);
            Console.WriteLine(ResultSin);
            Console.WriteLine(text);
        }
        
    }
}