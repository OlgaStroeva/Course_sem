using System;
using System.Collections.Generic;
using System.Linq;

namespace Course_sem.Properties
{
    public class TranslateToAssembler
    {

        private HashSet<string> conditional = new HashSet<string>()
                { "begin", ")", "else", "express", "Id", "do", "if", "elseif"},
            ignore = new HashSet<string>() {"{}", "beginend"};
        
        public TranslateToAssembler()
        {
        }

        private Dictionary<string, int> templates = new Dictionary<string, int>()
        {
            {"dimIDtype", 1}, {"dimIdtype", 1}, {"Idassexpress", 2}, {"IdassId", 3}, {"if(express)endif", 4}, {"if(Id)endif", 5},
            {"fortoexpressnext", 6}, {"fortoexpressstepIdnext", 7}, {"forIdtoIdnext", 8}, {"fortoIdstepIdnext", 9},
            {"whenexpressdo", 10}, {"whenIddo", 11},{"read(ID)", 12}, {"read(Id)", 13}, {"output(express)", 14}, {"output(Id)", 14},{"output(ID)", 14}
        };
        
        private static string id, type, id2, expression;
        private Dictionary<int, string> assembley = new Dictionary<int, string>()
        {
            {1, $"{id} dd 0\n"},  {2, $"mov {id}, eax"}, {3, $"mov {id2}, {id}"},
            {4, $"cmp {id}, 1\nje IfTrueLabel\njmp IfEndLabel\nIfTrueLabel:\n"},
            {13, $"mov eax, 3\nmov ebx, 0\nmov ecx, user_input\n mov edx, 4\nint 0x80 ; make syscall\nmov eax, [user_input]\nsub eax, '0'\nmov {id}, eax\n"},
            
        };

        private string _data = "section .data\ninput_format db \"%d\", 0\noutput_format db \"%d\", 10\n ", 
            formal = "section .bss\n    user_input resd 1 ; Variable to store the user input\n\nsection .text\n    global _start\n",
            main_code = "", label = "";
        private Stack<string> Labels = new Stack<string>();
        private int i = 0, s_count = 1;
        private bool complex = false;
            
        public void TranslateFromCode(string line, Stack<string> stack, 
            ref Stack<string> IDs, ref Stack<string> expressions)
        {
            if(ignore.Contains(line)) return;
            int number;
            try
            {
                number = templates[line];
                
            }
            catch (Exception e)
            {
                number = 0;
            }
            if (number == 1)
            {
                id2 = IDs.Pop();
                foreach (var ID in id2.Split(','))
                {
                    id = ID;
                    _data += assembley[number];
                }
            } else if (stack.Count == 2 && stack.Peek() == "for" && number<4)
            {
                id = IDs.Pop();
                if (number == 3) id2 = IDs.Pop();
                else main_code += TranslateExpression(expressions.Pop());
                main_code += assembley[number];
            }
            else
            {
                if (s_count == stack.Count)
                {
                    label += assembley[number];
                } else if (s_count > stack.Count)
                {
                    Labels.Append(label);
                    i += 1;
                    label = "";
                    s_count = stack.Count;
                    //new conditional action was added; create a new label, labels.Add(label)
                }
                else
                {
                    //here s_count < stack.Count so one of condition ended
                    s_count = stack.Count;
                    //here we analyze, which construction is it. If "startsWith("If")"
                    //then analyze whole line from the end (cause their expressions stored according rules of the Stack)
                    //if it's loop - analyze the whole, but remember - they all have same sense in general
                }
            }
        }

        private string SoComplicated(string line)
        {
            string temp = "";
            if (line.StartsWith("if"))
            {
                
            }
            return "";
        }
 
    private static readonly Dictionary<string, string> OperatorTranslations = new Dictionary<string, string>
    { { "+", "ADD" }, { "-", "SUB" }, { "*", "MUL" }, { "/", "DIV" }};

    public static string TranslateExpression(string expression)
    {
        var tokens = TokenizeExpression(expression);
        var postfixExpression = ConvertToPostfix(tokens);
        var result = EvaluatePostfix(postfixExpression);
        return result;
    }

    private static List<string> TokenizeExpression(string expression)
    {
        return expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static List<string> ConvertToPostfix(List<string> infixExpression)
    {
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<string>();
        foreach (var token in infixExpression)
        {
            if (IsOperand(token))
            {
                outputQueue.Enqueue(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                if (operatorStack.Count == 0 || operatorStack.Peek() != "(")
                {
                    throw new InvalidOperationException("Mismatched parentheses");
                }
                operatorStack.Pop(); // Pop the "("
            }
            else if (IsOperator(token))
            {
                while (operatorStack.Count > 0 && OperatorPrecedence(operatorStack.Peek()) >= OperatorPrecedence(token))
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
        }

        while (operatorStack.Count > 0)
        {
            if (operatorStack.Peek() == "(" || operatorStack.Peek() == ")")
            {
                throw new InvalidOperationException("Mismatched parentheses");
            }
            outputQueue.Enqueue(operatorStack.Pop());
        }
        return outputQueue.ToList();
    }

    private static bool IsOperand(string token)
    {
        return !OperatorTranslations.ContainsKey(token);
    }

    private static bool IsOperator(string token)
    {
        return OperatorTranslations.ContainsKey(token);
    }

    private static int OperatorPrecedence(string op)
    {
        // Define operator precedence (higher value means higher precedence)
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
                return 2;
            default:
                return 0; // Default precedence for non-operators
        }
    }

    private static string EvaluatePostfix(List<string> postfixExpression)
    {
        var operandStack = new Stack<string>();

        foreach (var token in postfixExpression)
        {
            if (IsOperand(token))
            {
                operandStack.Push(token);
            }
            else if (IsOperator(token))
            {
                if (operandStack.Count < 2)
                {
                    throw new InvalidOperationException("Invalid expression");
                }
                var operand2 = operandStack.Pop();
                var operand1 = operandStack.Pop();
                var result = PerformOperation(operand1, operand2, token);
                operandStack.Push(result);
            }
        }

        if (operandStack.Count != 1)
        {
            throw new InvalidOperationException("Invalid expression");
        }

        return operandStack.Pop();
    }

    private static string PerformOperation(string operand1, string operand2, string op)
    {
        // Perform the arithmetic operation
        // This part needs to be adapted based on your assembler instructions
        return $"{operand1} {OperatorTranslations[op]} {operand2}";
    }
    }
}