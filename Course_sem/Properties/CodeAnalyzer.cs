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
        private Stack<string> wordStack = new Stack<string>(), 
            Identifify = new Stack<string>(),
            expressions = new Stack<string>();
        private Stack<int> numberOfWords = new Stack<int>();
        private HashSet<string> set = new HashSet<string>()
        {
            "{}", "dimIDtype", "dimIdtype", "beginend", "Idassexpress", "IdassId", "if(express)endif", "if(Id)endif",
            "fortoexpressnext", "fortoexpressstepIdnext", "forIdtoIdnext", "whenexpressdo", "whenIddo",
            "read(ID)", "read(Id)", "output(express)", "output(Id)", "output(ID)"
        };
        
        readonly private HashSet<string> keys = new HashSet<string>()
        {
            "{", "}", "dim", "begin", "end", "if", "endif", "ass", "for",
            "to", "next", "when", "do", "read", "output", "step"
        }, types = new HashSet<string>() {"%", "$", "!" };

        private readonly Dictionary<string, int> constructionTemplates = new Dictionary<string, int>
            {{"{", 2}, {"dim", 3}, {"begin", 2}, {"ass", 3}, {"if", 5}, {"for", 4}, {"when", 3},
                {"read", 4}, {"output", 0} };

        public CodeAnalyzer(Queue<string> code)
        {
            words = code;  //SplitCodeIntoWords(code + ';');
            //var wow = new Semantical(words);
        }

        private string Result = ""; 
        
        public string AnalyzeCode()
        {
            string word;
            if (words.Peek() != "{") return "I can't see where is your program. Didn't you forget about { ?";
            while(words.Count != 0)
            {
                word = words.Dequeue();
                if (word == ";") CorrectConstruction();
                else if(types.Contains(word)) wordStack.Push("type");
                else if (IsConstantValue(word) || IsCorrectId(word) ||
                         (word=="(" && !bracket.Contains(wordStack.Peek())))
                    wordStack.Push(ProcessTheExpression(word));
                else if (keys.Contains(word) || IsKeyword(word) || IsSeparator(word))
                {
                    wordStack.Push(word);
                    try
                    {
                        numberOfWords.Push(constructionTemplates[word]);
                    }
                    catch (Exception e)
                    {
                        if (word == "step")
                        {
                            numberOfWords.Push(numberOfWords.Pop() + 2);
                            
                        }
                        // ignored
                    }
                }
                //else if (keywords.Contains(word)) SpecialCase(word);
                else
                {
                    return "You write something strange!" + word;
                }
            }

            if (wordStack.Count == 0 && Result == "") return "Everything is correct!";
            return Result;

        }

        public string GetAssembleyRezult()
        {
            return _assembler.GetData();
        }

        
        private string ProcessTheExpression(string word) //add for negative numbers, only get '-'
        {
            string REZ = word;
            var typeOfExpression = 0;// this var means "how construction should look like" (so it's impossible if there is something like '1+1, 2+2'
            Stack<string> tmp = new Stack<string>();
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
                    REZ += " " + nextOne;
                    while (word == "(")
                    {
                        tmp.Push("(");
                        word = words.Dequeue();
                        if(word == "-") REZ += " " + word + " " + words.Peek();
                        else if (IsOperator(word))
                        {
                            REZ += " " + nextOne;
                        }
                        //Here is a problem!
                    }
                    
                    nextOne = words.Dequeue(); // remove checked ',' or operator or ID from main queue
                    word = words.Dequeue(); //take new word (ID or constant)
                    
                    
                    if (nextOne == "-")
                    {
                        REZ += " " + nextOne + " " + word;
                    }else if (IsOperator(nextOne) && !REZ.EndsWith(nextOne))
                        REZ += " " + nextOne + " " + word;
                    else REZ += " " + word;
                    while (words.Peek() == ")" && tmp.Count>0 && tmp.Peek() == "(")
                    {
                        REZ += " )";
                        words.Dequeue();
                        tmp.Pop();
                    }
                    
                    nextOne = words.Peek(); // peek ',' or operator or ID from main queue
                }
                if (typeOfExpression == 1) result = "express";
            }
            if(typeOfExpression == 1) expressions.Push(REZ);
            else Identifify.Push(REZ);
            //Console.WriteLine(REZ);
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
                    Result += "Incorrect construction " + temp + ". Fix it now or forever hold your peace!\n";
                    return;
                    
                } else _assembler.TranslateFromCode(temp,wordStack,ref Identifify, ref expressions);
            }
            else
            {
                _assembler.TranslateFromCode(temp,wordStack,ref Identifify, ref expressions);
            }
        }

        private TranslateToAssembler _assembler = new TranslateToAssembler();
        private bool LastChance(ref string temp)
        {
            while (!IsKeyword(wordStack.Peek()))
                temp = wordStack.Pop() + temp;
            if (wordStack.Peek() == "elseif")
            {
                while (!keys.Contains(wordStack.Peek()) && wordStack.Count > 1)
                {
                    temp = wordStack.Pop() + temp;
                }
            }
            if(!IsSeparator(wordStack.Peek())) temp = wordStack.Pop() + temp;
            string outputRegex = @"output\((?:(?:Id|express)+)\)";
            string ifRegex = @"if\(\w+\)(?:elseif\(\w+\))*(?:else(?!if)[\w.]*)?endif";
            return Regex.IsMatch(temp, outputRegex) 
                   || Regex.IsMatch(temp, ifRegex);
        }
        
    }
}