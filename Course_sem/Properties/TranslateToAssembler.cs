using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            main_code = "", label = "", step;
        private List<string> Labels = new List<string>();
        private int i = 0, s_count = 1;
        private bool complex = false;

        public string GetData()
        {
            string REZ = "";
            REZ += _data + formal + main_code;
            foreach (var lab in Labels)
            {
                REZ += '\n' + lab;
            }
            return REZ;
        }
        
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
                    _data += id + " dd 0\n";
                }
            } else if (stack.Count == 2 && stack.Peek() == "for")
            {
                id = IDs.Pop();
                if (number == 3)
                {
                    id2 = IDs.Pop();
                    step = id2;
                }
                else
                {
                    step = id;
                    main_code += TranslateToAssembly(expressions.Pop());
                }
                main_code += assembley[number];
            }
            else
            {
                if (s_count == stack.Count)
                {
                    label += assembley[number];
                } else if (s_count < stack.Count)
                {
                    
                    s_count = stack.Count;
                    SoComplicated(line, ref IDs, ref expressions);
                    i += 1;
                    main_code += $"\nlabel" + i + ":\n";
                    //new conditional action was added; create a new label, labels.Add(label)
                }
                else
                {
                    Labels.Append(label);
                    i += 1;
                    label = $"label" + i +":\n";
                    //here s_count < stack.Count so one of condition ended
                    s_count = stack.Count;
                    //
                    
                    //here we analyze, which construction is it. If "startsWith("If")"
                    //then analyze whole line from the end (cause their expressions stored according rules of the Stack)
                    //if it's loop - analyze the whole, but remember - they all have same sense in general
                }
            }
        }

        private void SoComplicated(string line, ref Stack<string> IDs, ref Stack<string> expressions)
        {
            string temp = "";
            Console.WriteLine(line);
            if (line.StartsWith("if"))
            {
                List<string> expressionsAndIds = line.Split(new[] { "elseif", "else", "endif" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim('(', ')'))
                    .Reverse()
                    .ToList();
                bool hasElse = line.Contains("else");
                int exp = expressionsAndIds.Count, I = 1;
                foreach (var word in expressionsAndIds)
                {
                    if (word == expression)
                    {
                        temp = TranslateToAssembly(expressions.Pop()) + $"\n cmp eax, 0\njnz .label{i-exp+I - (hasElse ? 1 : 0)}" + temp;
                        Labels[Labels.Count - exp + I - (hasElse ? 1 : 0)] += $"\njmp label{i+1}";
                    }
                    else if (word == "Id")
                    {
                        id = IDs.Pop();
                        temp = $"\n cmp {id}, 0\njnz .label{i-exp+I - (hasElse ? 1 : 0)}" + temp;
                        Labels[Labels.Count - exp + I - (hasElse ? 1 : 0)] += $"\njmp label{i+1}";
                    }
                    I += 1;
                }
                main_code += temp;
                

                if (hasElse) temp += $"\njmp .label" + i ;
                main_code += temp;
            }
            else if (line.StartsWith("for") || line.StartsWith("when"))
            {
                List<string> expressionsAndIds = line.Split(new[] { "forto", "step", "next", "when", "do", "doelsebreak" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim('(', ')'))
                    .ToList();
                foreach (var word in expressionsAndIds)
                {
                    if (word == "express")
                    {
                        temp += '\n' + TranslateToAssembly(expressions.Pop()) + $"\njnz .label{i}\n";
                    }
                    else
                    {
                        if (expressionsAndIds.Count == 1) id = IDs.Pop();
                        else id = IDs.Peek();
                        temp += $"\ncmp" + id + ", 0\njnz .label" + i + "\n";
                    }
                }
                main_code += temp;
                if (line.Contains("step")) temp = $"\nmov eax, {step}\nadd {IDs.Pop()}\nmov step, eax\n" + temp;
                temp += $"jmp .label" + i+1 +"\n";
                Labels[Labels.Count-1] += temp;
                

            }
        }
 
    static string TranslateToAssembly(string expression)
    {
        StringBuilder assemblyCode = new StringBuilder();
        Stack<string> operators = new Stack<string>();

        // Split the expression into tokens
        string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (token == "(")
            {
                // Push opening parenthesis onto the stack
                operators.Push(token);
            }
            else if (token == ")")
            {
                // Process operators until the corresponding opening parenthesis is found
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    ProcessOperator(operators.Pop(), assemblyCode);
                }

                // Pop the opening parenthesis from the stack
                operators.Pop();
            }
            else if (IsOperator(token))
            {
                // Process operators with higher or equal precedence on top of the stack
                while (operators.Count > 0 && GetPrecedence(operators.Peek()) >= GetPrecedence(token))
                {
                    ProcessOperator(operators.Pop(), assemblyCode);
                }

                // Push the current operator onto the stack
                operators.Push(token);
            }
            else
            {
                // If the token is a number, load it into a register
                assemblyCode.AppendLine($"    mov eax, {token}");
            }
        }

        // Process remaining operators on the stack
        while (operators.Count > 0)
        {
            ProcessOperator(operators.Pop(), assemblyCode);
        }

        // Store the final result in a variable
        assemblyCode.AppendLine("    mov [result], eax");

        return assemblyCode.ToString();
    }

    static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    static int GetPrecedence(string op)
    {
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
                return 2;
            default:
                return 0;
        }
    }

    static void ProcessOperator(string op, StringBuilder assemblyCode)
    {
        // Perform operation based on the operator
        switch (op)
        {
            case "+":
                assemblyCode.AppendLine("    add ebx, eax");
                break;
            case "-":
                assemblyCode.AppendLine("    sub ebx, eax");
                break;
            case "*":
                assemblyCode.AppendLine("    imul ebx, eax");
                break;
            case "/":
                assemblyCode.AppendLine("    idiv ebx, eax");
                break;
        }
    }
    }
}