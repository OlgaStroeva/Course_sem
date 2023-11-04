using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Course_sem
{    
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            string codeToAnalyze = @"
        {
            dim aa1, bb2, cc3 %;
            /* kottt */
            if (aa1 > 0) begin
                aa1 ass (aa1 - 1) * cc3;
                bb2 ass bb2;
            end;
            elseif(aa1>bb2)
                aa1 ass (aa1 - bb2);
            elseif (aa1<bb2)
                bb2 ass aa1 + 10;
            else
                aa1 ass 1;
            endif;
            for aa1 to (aa1 > 10) step 1
                bb2 ass ((-7) + (bb2 *bb2));
            next;
            read(aa1 bb2);
            output(aa1+bb2);
        }
        ";

            CodeAnalyzer analyzer = new CodeAnalyzer(codeToAnalyze);

            Console.WriteLine("Analysis Result:");
            Console.WriteLine("---------------");
            if(analyzer.AnalyzeCode()) Console.WriteLine("You did a great job! Good for you, continue working)");
            else Console.WriteLine("You're a good kitten. Work harder, be better");
            
        }
    }
    

    class CodeAnalyzer
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
        

        // Implement the IsKeyword, IsSeparator, and IsConstantValue functions as described in previous responses.
        // The IsCorrectId function remains the same as well.

        private bool IsCorrectId(string word)
        {
            // Check if the word is a correct ID based on your grammar rules.
            // Correct IDs should match the pattern "<letter><letter><non-empty sequence of digits>".
            if (word.Length >= 3 && IsLetter(word[0]) && IsLetter(word[1]))
            {
                for (int i = 2; i < word.Length; i++)
                {
                    if (!IsDigit(word[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsConstantValue(string word)
        {
            // Check if the word is a constant (number or boolean value) based on your grammar.
            // In this case, constants can be numbers (e.g., 123) or '1' or '0'.
            return IsNumberConstant(word);
        }

        private bool IsNumberConstant(string word)
        {
            // Implement logic to check if the word is a numeric constant.
            if (double.TryParse(word, out _))
            {
                return true;
            }
            return false;
        }

        private HashSet<string> keywords = new HashSet<string>() { "dim", "begin", "end", "for", "if", "elseif", 
            "else", "endif", "step", "next", "when", "read", "output", "%", "!", "$"};

        private HashSet<string> separators = new HashSet<string>() { "{", "}", ";", ",", "(", ")" };
        
        private bool IsOperator(string word)
        {
            // Define a list of operator patterns in your language.
            List<string> operatorPatterns = new List<string>
            {
                // Add operator patterns for your language here.
                "+", "-", "*", "/", "=", "<", ">", "<=", ">=", "==", "!=", "&", "|" };

            // Check if the word matches any of the operator patterns.
            return operatorPatterns.Contains(word);
        }
        
        private bool IsKeyword(string word)
        {
            // Check if the word is a keyword based on your grammar.
            return keywords.Contains(word);
        }

        private bool IsSeparator(string word)
        {
            // Check if the word is a separator based on your grammar.
            return separators.Contains(word);
        }
        private bool IsLetter(char c)
        {
            // Helper function to check if a character is a letter.
            return char.IsLetter(c);
        }

        private bool IsDigit(char c)
        {
            // Helper function to check if a character is a digit.
            return char.IsDigit(c);
        }

    }
    
}