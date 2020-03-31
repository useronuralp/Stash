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
using System.Collections;


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
                            all_classified_variables.Add(words[2].TrimEnd(';'));
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
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values, List<string> classified_methods)
        {
            int number_of_critical_classes = 0;
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            List<string> temp_2 = new List<string>();
            temp_2.AddRange(classified_values);
            temp_2.AddRange(classified_methods);
            string[] words;
            List<string> all_critical_classes = new List<string>();
            foreach (var value in random)
            {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
                {
                    foreach (var item in temp_2)
                    {
                        if (temp.Contains(item))
                        {
                            number_of_critical_classes++;
                            words = temp.Trim().Split(' ');
                            all_critical_classes.Add(words[2]);
                            break;
                        }
                    }
                }
            }

            return all_critical_classes;
        }
    }

    public class NonPrivateClassAttributes
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            List<string> all_nonPrivateClassified_variables = new List<string>();
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            int ct_nonClassified = 0;
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
                        if (tmp.ToFullString().Contains("private")) { continue; }
                        else
                        {
                            SyntaxFactory.Comment(tmp.ToFullString());
                            var tmp_2 = tmp.ToFullString();

                            if (tmp_2.Contains("Classified") || tmp_2.Contains("classified"))
                            {
                                ct_nonClassified++;
                                tmp_2 = tmp_2.Trim();
                                string[] words = tmp_2.Split(' ');
                                all_nonPrivateClassified_variables.Add(words[2]);
                            }
                        }
                    }
                }
            }
            return all_nonPrivateClassified_variables;
        }
    }

    public class NonPrivateInstanceAttributes
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> nonPrivateClassified_values)
        {
            List<string> all_npinstance_attributes = new List<string>();
            var random = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            foreach (var value in random)
            {
                var temp = value.ToFullString();
                //Console.WriteLine(temp);
                if (temp.Contains("static void Main")) { continue; }
                else
                {
                    var temp_tree = CSharpSyntaxTree.ParseText(temp);
                    var temp_root = temp_tree.GetRoot();
                    var random_2 = temp_root.DescendantNodes().OfType<BaseFieldDeclarationSyntax>().ToList();

                    foreach (var item in random_2)
                    {
                        var temp_2 = item.ToFullString();
                        Console.WriteLine(temp_2);
                        foreach (var x in nonPrivateClassified_values)
                        {
                            if (temp_2.Contains(x))
                            {
                                temp_2 = temp_2.Trim();
                                string[] split = temp_2.Split(' ', '(');
                                all_npinstance_attributes.Add(split[2]);
                                break;
                            }

                        }
                    }
                }
            }
            return all_npinstance_attributes;
        }
    }

    public class NonPrivateMethods
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
        {
            List<string> all_npclassified_methods = new List<string>();
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
                            if (x.Contains("private")) { continue; }
                            else
                            {
                                if (temp_2.Contains(x))
                                {
                                    temp_2 = temp_2.Trim();
                                    string[] split = temp_2.Split(' ', '(');
                                    all_npclassified_methods.Add(split[2]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return all_npclassified_methods;
        }
    }

    public class CIDA
    {
        public static double ratio(int number_of_npia, int number_of_cat) //npia = NonPrivateInstanceAttributes
        {
            double result = 0.0;
            try
            {
                result = number_of_npia / number_of_cat;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = number_of_npia / (number_of_cat + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }

    public class CCDA
    {
        public static double ratio(int number_of_npca, int number_of_cat) //npca = NonPrivateClassAttributes
        {
            double result = 0.0;
            try
            {
                result = number_of_npca / number_of_cat;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = number_of_npca / (number_of_cat + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }

    public class COA
    {
        public static double ratio(int number_of_cmt, int number_of_npm)
        {
            double result = 0.0;
            try
            {
                result = number_of_cmt / number_of_npm;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = number_of_cmt / (number_of_npm + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }

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
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
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
            try
            {
                result = interact_set_ct / total_mutator_number;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = interact_set_ct / (total_mutator_number + 0.001);
            }
            return System.Math.Round(result, 1);
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
            double result = 0.0;
            try
            {
                result = interact_get_ct / total_accessor_number;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = interact_get_ct / (total_accessor_number + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }
    public class CAIW
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
                if (x.Length == 1 || x.Length == 2 || x.Length == 3) //tek, çift yada üç harfli değişkenler için özel case bu olmazsa sapıtıyor.
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
            try
            {
                return System.Math.Round(total_number_of_classified_attribute_interactions / total_number_of_interactions, 1);
            }
            catch (ArithmeticException)
            {
                return 0.0;
            }
        }
    }
    public class CMW
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> all_classified_methods)
        {
            double Total_Method_Number = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Count();
            
            var random_2 = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            //Console.WriteLine(random);
            /*foreach (var x in random_2)
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
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = All_classified_methods_number / (Total_Method_Number + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }
    public class CCC
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> all_classified_variables, double number_of_critical_classes)
        {
            double All_classified_attribute_numbers = all_classified_variables.Count();
            //Console.WriteLine(All_classified_attribute_numbers);           
            //Console.WriteLine(number_of_critical_classes);
            double result = 0.0;
            try
            {
                result = number_of_critical_classes/ All_classified_attribute_numbers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = number_of_critical_classes / (All_classified_attribute_numbers+0.001);
            }
            return System.Math.Round(result, 1);
        }
    }
    public class CWMP
    {
        public static Boolean Check(String method_classified, Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
        {
            //Console.WriteLine(method_classified);
            Boolean flag = false;
            HashSet<string> hset2 = new HashSet<string>();
            
            var unaryexp=root.DescendantNodes().OfType<PostfixUnaryExpressionSyntax>().ToList();
            var asg = root.DescendantNodes().OfType<AssignmentExpressionSyntax>().ToList();

            foreach (var x in asg)
            {
                var temp = x.ToFullString();
                if (method_classified.Contains(temp))
                {
                    if (classified_values.Contains(temp))
                    {
                        if (temp.Contains("+=")|| temp.Contains("==") || temp.Contains("-="))
                        {
                            flag = true;
                        }
                    }
                }
            }
            
            foreach (var temp in unaryexp)
            {
                var value = temp.ToFullString();
                hset2.Add(value);
            }
            List<String> list = hset2.ToList();
            
            foreach (var temp in list)
            {
                if (method_classified.Contains(temp))
                {
                    flag = true;
                }

            }


            return flag;


            
        }
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values, List<string> all_classified_methods, string input)
        {
            double classified_assigment_ct = 0;
            HashSet<string> hset = new HashSet<string>();
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
                                hset.Add(temp_2);    
                            }

                        }                        
                    }
                }
            }

            List<String> Method_classified = hset.ToList();

            HashSet<string> hset2 = new HashSet<string>();
            
            foreach (var method in Method_classified)
            {
                if (CWMP.Check(method, root,classified_values))
                {
                    classified_assigment_ct++;
                }   
            }
            //Console.WriteLine(classified_assigment_ct);
            //Console.WriteLine(all_classified_methods.Count());
            double result = 0;
            try
            {
                result = classified_assigment_ct /(double) (all_classified_methods.Count());
            }
            catch(ArithmeticException e)
            {
                Console.WriteLine(e);
            }


            return result;
        }
    }
    public class CDP
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, double number_of_critical_classes)
        {
            double Total_Class_Number = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count();
            //Console.WriteLine("total class"+Total_Class_Number);

            double result = 0.0;
            try
            {
                result = number_of_critical_classes / Total_Class_Number;
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine(e);
               
            }
            return System.Math.Round(result, 1);
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

            var printable_3 = CCT.Calculate(root, printable, printable_2); //Yukardakilerin döndürdüğü listeleri alıp işlem yapıyor bu arkadaş.
            Console.WriteLine("Number of critical classes:" + printable_3.Count());
            /*foreach (var x in printable)
            {
                Console.WriteLine("Calssified attributes:"+ x);
            }
            foreach (var item in printable_2)
            {
                Console.WriteLine(item);
            }*/

            var printable_4 = NonPrivateClassAttributes.Calculate(root); // Calisma mantigi CAT ile ayni.
            Console.WriteLine("Number of NonPrivateClassAttributes:" + printable_4.Count());
            var printable_5 = NonPrivateInstanceAttributes.Calculate(root, printable_4); // Calismasi icin NonPrivateClassAttributes'dan gelen list gerekli.
            Console.WriteLine("Number of NonPrivateInstanceAttributes:" + printable_5.Count());
            var printable_6 = NonPrivateMethods.Calculate(root, printable);
            Console.WriteLine("Number of NonPrivateMethods:" + printable_6.Count()); // Calisma mantigi CMT ile ayni.
            var printable_7 = CIDA.ratio(printable_5.Count(), printable.Count());
            Console.WriteLine("Ratio of CIDA:" + printable_7);
            var printable_8 = CCDA.ratio(printable_4.Count(), printable.Count());
            Console.WriteLine("Ratio of CCDA:" + printable_8);
            var printable_9 = COA.ratio(printable_2.Count(), printable_6.Count());
            Console.WriteLine("Ratio of COA:" + printable_9);

            bool printable_5= RPB.Calculate(tree);
            Console.WriteLine(printable_5);

            var printable_6 = CAAI.Calculate(root, printable);
            Console.WriteLine("accessors ratio: " + printable_6);

            var printable_7 = CMAI.Calculate(root, printable);
            Console.WriteLine("mutator ratio"+printable_7);

            var printable_8 = CMW.Calculate(root,printable_2);
            Console.WriteLine("cmw"+printable_9);

            var printable_9 = CCC.Calculate(root,printable,printable_3.Count());
            Console.WriteLine(printable_9);

            var printable_10 = CAIW.Calculate(root, printable, readContents); //Classified attributeların olduğu listeyi ve tüm txt orijinal kodu buna passlıyoruz
            Console.WriteLine(printable_10);

            var printable_temp = CDP.Calculate(root,printable_3.Count());
            Console.WriteLine("cdp"+ printable_temp);

            var printable_11 = CWMP.Calculate(root,printable,printable_2,readContents);
            Console.WriteLine("CWMP: "+ printable_11);
            
        }
    }
}