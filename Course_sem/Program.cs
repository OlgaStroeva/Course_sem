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
            for aa1 ass 10; to (aa1 > 10) step 1
                bb2 ass ((-7) + (bb2 *bb2));
            next;
        }
        ";

            representation output = new representation(codeToAnalyze);
            output.getData();

        }
    }
    
    
}