using System;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;


namespace ConsoleApp1
{


    public class CAT
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            List<string> all_classified_variables = new List<string>();
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            int ct_classified = 0;
            foreach (var value in random)
            {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
                {
                    var temp_tree = CSharpSyntaxTree.ParseText(temp);
                    var temp_root = temp_tree.GetRoot();
                    var random_2 = temp_root.DescendantNodes().OfType<BaseFieldDeclarationSyntax>().ToList();

                    foreach (var tmp in random_2)
                    {
                        SyntaxFactory.Comment(tmp.ToFullString());
                        var tmp_2 = tmp.ToFullString();

                        if (tmp_2.Contains("Classified") || tmp_2.Contains("classified"))
                        {
                            ct_classified++;
                            tmp_2 = tmp_2.Trim();
                            string[] words = tmp_2.Split(' ');
                            all_classified_variables.Add(words[2]);
                        }
                    }
                }
            }
            return all_classified_variables;
        }
    }

    public class CMT
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
        {
            List<string> all_classified_methods = new List<string>();
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            foreach (var value in random)
            {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
                {
                    var temp_tree = CSharpSyntaxTree.ParseText(temp);
                    var temp_root = temp_tree.GetRoot();
                    var random_2 = temp_root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

                    foreach (var item in random_2)
                    {
                        var temp_2 = item.ToFullString();
                        foreach (var x in classified_values)
                        {
                            if (temp_2.Contains(x))
                            {
                                temp_2 = temp_2.Trim();
                                string[] split = temp_2.Split(' ', '(');
                                all_classified_methods.Add(split[2]);
                                break;
                            }

                        }
                        //buraya ikinci olarak bide constructorlar yada getter setterlar için bir case eklenebilir. Sonuçta onlarda değerleri
                        //manipule edebiliyor.
                    }
                }
            }
            return all_classified_methods;
        }
    }

    public class CCT
    {
        public static int Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values, List<string> classified_methods)
        {
            int number_of_critical_classes = 0;
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            classified_values.AddRange(classified_methods);

            foreach (var value in random)
            {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
                {
                    foreach (var item in classified_values)
                    {
                        if (temp.Contains(item)) { number_of_critical_classes++; break; }
                    }
                }
            }
            return number_of_critical_classes;
        }
    }
    //----
    class RPB
    {

        internal static bool Calculate(SyntaxTree tree)
        {
            var root2 = tree.GetCompilationUnitRoot();
            var element = root2.Usings;
            var temp = element.ToFullString();
            if (temp.Contains("System.Reflection"))
            {
                return true;
            }
            else
            {

                return false;
            }
            throw new NotImplementedException();
        }
    }
    class CMAI
    {
        public static float Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
        {
            double total_mutator_number = 0.0;
            double interact_set_ct = 0.0;
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            foreach (var value in random)
            {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
                {
                    var temp_tree = CSharpSyntaxTree.ParseText(temp);
                    var temp_root = temp_tree.GetRoot();
                    //var random_2 = root.DescendantNodes().OfType<AccessorDeclarationSyntax>().Count();
                    var random_3 = root.DescendantNodes().OfType<AccessorDeclarationSyntax>().ToList();

                    foreach (var item_2 in random_3)
                    {
                        var temp_2 = item_2.ToFullString();

                        if (temp_2.Contains("set"))
                        {
                            total_mutator_number++;
                            foreach (var x in classified_values)
                            {
                                if (temp_2.Contains(x))
                                {
                                    interact_set_ct++;
                                }
                            }
                        }
                    }

                }
            }


            double result = 0.0;
            result = interact_set_ct / total_mutator_number;
            return (float)result;
        }
    }
    class CAAI
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
        {
            double total_accessor_number = 0.0;
            double interact_get_ct = 0.0;
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            foreach (var value in random)
             {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
             {
                var temp_tree = CSharpSyntaxTree.ParseText(temp);
                var temp_root = temp_tree.GetRoot();
                //var random_2 = root.DescendantNodes().OfType<AccessorDeclarationSyntax>().Count();
                var random_3 = root.DescendantNodes().OfType<AccessorDeclarationSyntax>().ToList();
                    /*foreach (var x in random_3)
                    {
                        Console.WriteLine(x);

                    }*/
                
                    foreach (var item_2 in random_3)
                    {
                        var temp_2 = item_2.ToFullString();

                            if (temp_2.Contains("get"))
                            {                                
                                total_accessor_number++;
                                foreach (var x in classified_values)
                                {
                                    if (temp_2.Contains(x))
                                    {
                                        interact_get_ct++;
                                    }
                                }
                            }                        
                    }
                    
                }
            }


            double result=0.0;
            result = interact_get_ct / total_accessor_number;
            return (double)result;
        }
    }
    public class CAIW // bu metric geliştirilebilir 
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values, string input)
        {
            var random = root.DescendantNodes().OfType<VariableDeclarationSyntax>().ToList();
            string[] words;
            List<string> all_attributes = new List<string>();
            double total_number_of_interactions = 0;
            double total_number_of_classified_attribute_interactions = 0;

            foreach (var item in random) //bütün variable'ları burada buluyor.
            {
                words = item.ToFullString().Trim().Split(' ');
                all_attributes.Add(words[1]);

            }

            foreach (var x in all_attributes)
            {
                if (x.Length == 1) //tek harfli değişkenler için özel case bu olmazsa sapıtıyor.
                {
                    total_number_of_interactions += Regex.Matches(input, x + ' ').Count();                   

                    if (classified_values.Contains(x))
                    {
                        total_number_of_classified_attribute_interactions += Regex.Matches(input, x + ' ').Count();
                    }
                }
                else //tek harften daha uzun isimli değişkenler için case.
                {
                    total_number_of_interactions += Regex.Matches(input, x).Count();

                    if (classified_values.Contains(x))
                    {
                        total_number_of_classified_attribute_interactions += Regex.Matches(input, x).Count();
                    }
                }
            }
            return System.Math.Round(total_number_of_classified_attribute_interactions / total_number_of_interactions, 2);
        }
    }
    public class CMW
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> all_classified_methods)
        {
            double Total_Method_Number = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Count();
            /*var random_2 = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            Console.WriteLine(random);
            foreach (var x in random_2)
            {
                Console.WriteLine(x);
            }*/
            double All_classified_methods_number = all_classified_methods.Count();
            //Console.WriteLine(All_classified_methods_number);
            double result = 0.0;
            try
            {
                result = All_classified_methods_number / Total_Method_Number;

            }
            catch (ArithmeticException e)
            {
                Console.WriteLine("Division by zero not possible");
            }
            

            return result;
        }
    }
    class Program
    {

        static void Main()
        {
            string readContents;
            using (StreamReader streamReader = new StreamReader("exampleC#.txt", Encoding.UTF8)) //Öğrenci ödevini okuma kısmı
            {
                readContents = streamReader.ReadToEnd(); 
            }
            var tree = CSharpSyntaxTree.ParseText(readContents);
            //Metriclerin içine passlenecek olan arkadaş burdaki root.
            var root = tree.GetRoot();
           
            //Alttaki satırdaki gibi de metricleri çağırıp içlerindeki 'Calculate' ile ne yapıyorlarsa onları hesaplattıracağız. 
            var printable = CAT.Calculate(root); //CAT metricini çağırıp değerini almadan ikinci metrici çağıramıyoruz
            var printable_2 = CMT.Calculate(root, printable); // CAT'den gelen değer buraya passlanıyor list olarak.
            Console.WriteLine("Number of classified data:" + printable.Count());
            Console.WriteLine("Number of classified methods:" + printable_2.Count());
            int printable_3 = CCT.Calculate(root, printable, printable_2); //Yukardakilerin döndürdüğü listeleri alıp işlem yapıyor bu arkadaş.
            Console.WriteLine("Number of critical classes:" + printable_3);
            /*foreach (var x in printable)
            {
                Console.WriteLine(x);
            }
            foreach (var item in printable_2)
            {
                Console.WriteLine(item);
            }*/
            //bool printable_4= RPB.Calculate(tree);
            //Console.WriteLine(printable_4);
            //var printable_5 = CAAI.Calculate(root, printable);
            //Console.WriteLine("accessors ratio: " + printable_5);
            //var printable_6 = CMAI.Calculate(root, printable);
            //Console.WriteLine("mutator ratio"+printable_6);
            //var printable_7 = CMW.Calculate(root,printable_2);
            //Console.WriteLine("cmw"+printable_7);

        }
    }
}
