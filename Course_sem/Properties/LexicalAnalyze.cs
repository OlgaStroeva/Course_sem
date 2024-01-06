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

        private string Result = "", text = "";

        public bool SeparateThem(string word)
        {
            if (IsKeyword(word))
            {
                keywords.Push(word);
                text += "( Keys, " + keywords.Count + " ) ";
            }
            else if (IsSeparator(word))
            {
                separators.Add(word);
                text += "( Separator, " + separators.Count + " ) ";
            }
            else if (IsConstantValue(word))
            {
                text += "( Constant, " + constants.Count + " ) ";
                constants.Add(word);
            }
            else if (IsCorrectId(word))
            {
                text += "( ID, " + IDs.Count + " ) ";
                if (!CheckID(word))
                {
                    text += "( Incorrect ID ) ";
                    Result += "Syntax Error because of "+ word +"! Id here isn't exist! Fix it.\n";
                    return false;
                }
            }
            else if (!IsOperator(word))
                return false;
            return true;
        }
        
        public string GetResult()
        {
            return Result;
        }

        public Stack<string> GetKeywords()
        {
            return keywords;
        }

        public List<string> Getseparators()
        {
            return separators;
        }

        public List<string> Getconstants()
        {
            return constants;
        }

        public HashSet<string> GetIDs()
        {
            return IDs;
        }

        public string GetText()
        {
            return text;
        }

    }
    
}