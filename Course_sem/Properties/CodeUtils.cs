using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Course_sem.Properties
{
    using System;
    // Implement the IsKeyword, IsSeparator, and IsConstantValue functions as described in previous responses.
    // The IsCorrectId function remains the same as well.
    public static class CodeUtils
    {
        
        public static Queue<string> SplitCodeIntoWords(string code)
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
        
        public static bool IsCorrectId(string word)
        {
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

        public static bool IsConstantValue(string word)
        {
            return IsNumberConstant(word);
        }

        public static bool IsNumberConstant(string word)
        {
            return double.TryParse(word, out _);
        }

        public static bool IsOperator(string word)
        {
            string[] operatorPatterns = { "no", "+", "-", "*", "/", "=", "<", ">", "<=", ">=", "==", "!=", "&", "|" };
            return Array.Exists(operatorPatterns, pattern => pattern == word);
        }

        public static bool IsKeyword(string word)
        {
            var keywords = new HashSet<string> { "dim", "ass", "to", "begin", "end", "for", "if", "elseif",
                "else", "endif", "step", "next", "when", "read", "output", "%", "!", "$" };
            return keywords.Contains(word);
        }

        public static bool IsSeparator(string word)
        {
            HashSet<string> separators = new HashSet<string> { "{", "}", ";", ",", "(", ")" };
            return separators.Contains(word);
        }

        private static bool IsLetter(char c)
        {
            return char.IsLetter(c);
        }

        private static bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }
    }

}