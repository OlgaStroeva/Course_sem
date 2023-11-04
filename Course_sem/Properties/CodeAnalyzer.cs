using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Course_sem.Properties.CodeUtils;

namespace Course_sem.Properties
{
    internal class CodeAnalyzer
    {
        private Queue<string> words;
        private Stack<string> wordStack = new Stack<string>();
        private Stack<int> numberOfWords = new Stack<int>();
        private HashSet<string> set = new HashSet<string>()
        {
            "{}", "dimIDtype", "dimIdtype", "beginend", "Idassexpress", "IdassId", "if(express)endif", "if(Id)endif",
            "forIdtoexpressnext", "forIDtoIDnext", "whenexpressdo", "read(ID)", "output(express)"
        };
        
        readonly private HashSet<string> keys = new HashSet<string>()
        {
            "{", "}", "dim", "begin", "end", "if", "endif", "ass", "for",
            "to", "next", "when", "do", "read", "output"
        }, types = new HashSet<string>() {"%", "$", "!" };

        private readonly Dictionary<string, int> constructionTemplates = new Dictionary<string, int>
            {{"{", 2}, {"dim", 3}, {"begin", 2}, {"ass", 3}, {"if", 5}, {"for", 5}, {"when", 3},
                {"read", 0}, {"output", 0} };

        public CodeAnalyzer(string code)
        {
            // Constructor: Initialize the class with the input code.
            // Split the code into words and process them.
            words = SplitCodeIntoWords(code + ';');
        }
        
        private Queue<string> SplitCodeIntoWords(string code)
        {
            // Define a regular expression pattern to split the code based on space, comma, semicolon, parentheses, operators, "read," and comments.
            string pattern = @"(/\*[^*]*\*+(?:[^/*][^*]*\*+)*/)|\s+|(?<=[;,()=<>+\-*/])|(?=[;,()=<>+\-*/])|(?<=read)|(?=read)";
            // Split the code using the regular expression pattern.
            var words = Regex.Split(code, pattern)
                .Where(word => !string.IsNullOrWhiteSpace(word) && !word.StartsWith("/*"))
                .Select(word => word.Trim()) // Remove leading/trailing spaces.
                .ToList();
            return new Queue<string>(words);
        }
        
        public bool AnalyzeCode()
        {
            string word;
            while(words.Count != 0)
            {
                word = words.Dequeue();
                if (word == ";") CorrectConstruction();
                else if(types.Contains(word)) wordStack.Push("type");
                else if (IsConstantValue(word) || IsCorrectId(word) ||
                         (word=="(" && !bracket.Contains(wordStack.Peek())))
                    wordStack.Push(ProcessTheExpression(word));
                else if (word == "step") words.Dequeue(); 
                else if (keys.Contains(word) || IsKeyword(word) || IsSeparator(word))
                {
                    wordStack.Push(word);
                    try
                    {
                        numberOfWords.Push(constructionTemplates[word]);
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                } 
                //else if (keywords.Contains(word)) SpecialCase(word);
                
            }
            return wordStack.Count == 0;
        }

        
        private string ProcessTheExpression(string word) //add for negative numbers, only get '-'
        {
            var typeOfExpression = 0;// this var means "how construction should look like" (so it's impossible if there is something like '1+1, 2+2'
            Stack<string> tmp = new Stack<string>();
            //if (word == "(") tmp.Push("(");
            string nextOne = words.Peek(), result = "Id"; //result var display, which world will we return
            if (IsOperator(nextOne) || word == "(") //if it's expression (so there is any operator, after ID or constant var word)
                typeOfExpression = 1;
            else if (nextOne == ",")
            {
                typeOfExpression = 2;
                result = "ID";
            }
            if (typeOfExpression != 0) //if we know, which type of construction it is
            {
                while (((IsOperator(nextOne) || word == "(") && typeOfExpression==1)
                    || (nextOne == "," && typeOfExpression==2)) // = while match conditions for types
                {
                    while (word == "(")
                    {
                        tmp.Push("(");
                        word = words.Dequeue();
                    }
                    nextOne = words.Dequeue(); // remove checked ',' or operator or ID from main queue
                    word = words.Dequeue(); //take new word (ID or constant)
                    if(word == "-") word = words.Dequeue();
                    while (words.Peek() == ")" && tmp.Count>0 && tmp.Peek() == "(")
                    {
                        words.Dequeue();
                        tmp.Pop();
                    }
                    nextOne = words.Peek(); // peek ',' or operator or ID from main queue
                }
                if (typeOfExpression == 1) result = "express";
            }
            return result;
        }

        private HashSet<string> bracket = new HashSet<string>() { "read", "output", "if", "elseif" };

        private void CorrectConstruction()
        {
            int lenta = numberOfWords.Pop();
            string temp = "";
            for (int i = 0; i < lenta; i++)
                temp = wordStack.Pop() + temp;
            if (!(set.Contains(temp)))
            {
                if (!LastChance(ref temp))
                {
                    Console.WriteLine("Incorrect construction " + temp + ". Fix it now or forever hold your peace!");
                    throw new Exception();
                }
            }
        }
        
        private bool LastChance(ref string temp)
        {
            while (!IsKeyword(wordStack.Peek()))
                temp = wordStack.Pop() + temp;
            if (wordStack.Peek() == "elseif")
            {
                while (!keys.Contains(wordStack.Peek()))
                {
                    temp = wordStack.Pop() + temp;
                }
            }
            temp = wordStack.Pop() + temp;
            Console.WriteLine(temp);
            string readRegex = @"read\((?:Id)+\)",
                outputRegex = @"output\((?:(?:Id|express)+)\)";
            string ifRegex = @"if\(\w+\)(?:elseif\(\w+\))*(?:else)?endif";
            return Regex.IsMatch(temp, outputRegex) 
                   || Regex.IsMatch(temp, readRegex)
                   || Regex.IsMatch(temp, ifRegex);
        }
        
    }
}