using System.Collections.Generic;

namespace Course_sem.Properties
{
    using System;
    // Implement the IsKeyword, IsSeparator, and IsConstantValue functions as described in previous responses.
    // The IsCorrectId function remains the same as well.
    public static class CodeUtils
    {
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
            string[] operatorPatterns = { "+", "-", "*", "/", "=", "<", ">", "<=", ">=", "==", "!=", "&", "|" };
            return Array.Exists(operatorPatterns, pattern => pattern == word);
        }

        public static bool IsKeyword(string word)
        {
            var keywords = new HashSet<string> { "dim", "begin", "end", "for", "if", "elseif", "else", "endif", "step", "next", "when", "read", "output", "%", "!", "$" };
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