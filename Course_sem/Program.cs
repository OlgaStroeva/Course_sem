using System;
using Course_sem.Properties;

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
            read(aa1, bb2);
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
    
    
}