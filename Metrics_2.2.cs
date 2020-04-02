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

                            if (words[0] == "public" || words[0] == "private" || words[0] == "protected")
                            {
                                all_classified_variables.Add(words[2].TrimEnd(';'));
                            }
                            else
                            {
                                all_classified_variables.Add(words[1].TrimEnd(';'));
                            }

                        }
                    }
                }
            }

            return all_classified_variables.Distinct().ToList();
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
                            words = temp.Trim().Split(' ', '{');

                            if ((words[0] == "public" || words[0] == "private" || words[0] == "abstract") && (words[1] == "class"))
                            {
                                all_critical_classes.Add(words[2].TrimEnd());
                            }
                            else if ((words[0] == "public" || words[0] == "private") && (words[1] == "abstract"))
                            {
                                all_critical_classes.Add(words[3].TrimEnd());

                            }
                            else if (words[0].Trim() == "[Serializable]")
                            {
                                for (int i = 0; i < words.Count()-1; i++)
                                {
                                    if (words[i] == "class")
                                    {
                                        all_critical_classes.Add(words[i + 1].TrimEnd());
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return all_critical_classes;
        }
    }
    public class CIDA
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> nonPrivateClassified_values)
        {
            List<string> all_npinstance_attributes = new List<string>();
            var random = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            int ct_nonPrivateClassifiedIns = 0;
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
                                ct_nonPrivateClassifiedIns++;
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
        public static double ratio(int ct_nonPrivateClassifiedIns, int cat)
        {
            double result = 0.0;
            try
            {
                result = ct_nonPrivateClassifiedIns / cat;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = ct_nonPrivateClassifiedIns / (cat + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }
    public class CCDA
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            List<string> all_nonPrivateClassified_classVariables = new List<string>();
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            int ct_nonPrivateClassified = 0;
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
                                ct_nonPrivateClassified++;
                                tmp_2 = tmp_2.Trim();
                                string[] words = tmp_2.Split(' ');
                                all_nonPrivateClassified_classVariables.Add(words[2]);
                            }
                        }
                    }
                }
            }
            return all_nonPrivateClassified_classVariables;
        }
        public static double ratio(int ct_nonPrivateClassified, int cat)
        {
            double result = 0.0;
            try
            {
                result = ct_nonPrivateClassified / cat;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = ct_nonPrivateClassified / (cat + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }
    public class COA
    {
        public static List<string> Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values)
        {
            List<string> all_npclassified_methods = new List<string>();
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            int ct_nonPrivateMethods = 0;
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
                                    ct_nonPrivateMethods++;
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
        public static double ratio(int cmt, int ct_nonPrivateMethods)
        {
            double result = 0.0;
            try
            {
                result = cmt / ct_nonPrivateMethods;
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = cmt / (ct_nonPrivateMethods + 0.001);
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
            catch (ArithmeticException)
            {
                Console.WriteLine("Division by zero not possible");
                result = All_classified_methods_number / (Total_Method_Number + 0.001);
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
            var unaryexp = root.DescendantNodes().OfType<PostfixUnaryExpressionSyntax>().ToList();
            var asg = root.DescendantNodes().OfType<AssignmentExpressionSyntax>().ToList();
            foreach (var x in asg)
            {
                var temp = x.ToFullString();
                if (method_classified.Contains(temp))
                {
                    if (classified_values.Contains(temp))
                    {
                        if (temp.Contains("+=") || temp.Contains("==") || temp.Contains("-="))
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
                if (CWMP.Check(method, root, classified_values))
                {
                    classified_assigment_ct++;
                }
            }
            //Console.WriteLine(classified_assigment_ct);
            //Console.WriteLine(all_classified_methods.Count());
            double result = 0;
            try
            {
                result = classified_assigment_ct / (double)(all_classified_methods.Count());
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine(e);
            }
            return result;
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
                result = number_of_critical_classes / All_classified_attribute_numbers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = number_of_critical_classes / (All_classified_attribute_numbers + 0.001);
            }
            return System.Math.Round(result, 1);
        }
    }  
    public class UACA //muhtemelen bug dolu sonra düzelt.
        {

        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_values, List<string> critical_classes)
        {
            var random = root.DescendantNodes().OfType<VariableDeclarationSyntax>().ToList();
            IDictionary<string, int> dict = new Dictionary<string, int>();
            Hashtable ht = new Hashtable();
            double not_used_variables = 0;
            string main = "";
            List<string> temp = new List<string>();

            foreach (var item in random)
            {
                foreach (var item_2 in classified_values)
                {
                    if (item.ToString().Contains(item_2 + " ="))
                    {
                        try
                        {
                            dict.Add(item_2, 0);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

            }
            var random_2 = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            foreach (var value in random_2)
            {
                if (value.ToFullString().Contains("Main"))
                {
                    main = value.ToFullString();
                    break;
                }
            }

            var random_3 = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            foreach (string item_3 in critical_classes)
            {
                if (main.Contains(item_3.Trim()))
                {
                    temp.Add(item_3.Trim());
                }
            }

            foreach (var x in random_3)
            {
                if (x.ToFullString().Contains("class Program")) { continue; }
                else
                {
                    foreach (var y in temp) //mainde kullanılan classlar tempde
                    {
                        if (x.ToFullString().Contains(y))
                        {
                            foreach (var z in classified_values)
                            {
                                if (x.ToFullString().Contains(z))
                                {
                                    for (int i = 0; i < dict.Count; i++)
                                    {
                                        if (dict.Keys.ElementAt(i) == z)
                                        {
                                            dict[dict.Keys.ElementAt(i)] = dict[dict.Keys.ElementAt(i)] + 1;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            foreach (KeyValuePair<string, int> item_3 in dict)
            {
                Console.WriteLine("Key: {0}, Value: {1}", item_3.Key, item_3.Value);
                if (item_3.Value == 0)
                {
                    not_used_variables++;
                }
            }
            double result = not_used_variables / classified_values.Count();
            return System.Math.Round(result, 1);
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
    public class CSCP
    {
        public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root,List<string> critical_classes)
        {
            var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            double Serializable_ct = 0;
            /*foreach (var x in critical_classes)
            {
                Console.WriteLine(x); 
            }*/

            foreach (var x in random)
            {
                var all_classes = x.ToFullString();
                if (all_classes.Contains("class Program")) { continue; }
                foreach (var crit in critical_classes)
                {
                    if (all_classes.Contains(crit))
                    {
                        //Console.WriteLine(all_classes);
                        if (all_classes.Contains("[Serializable]"))
                        {
                            Serializable_ct++;
                        }
                    }
                }
            }
            //Console.WriteLine(Serializable_ct);
            double result = 0.0;
            try
            {
                result = Serializable_ct / ((double)critical_classes.Count);
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine(e);
                result = Serializable_ct / ((double)critical_classes.Count+0.001);
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
            var printable_3 = CCT.Calculate(root, printable, printable_2); //Yukardakilerin döndürdüğü listeleri alıp işlem yapıyor bu arkadaş.
            Console.WriteLine("Number of critical classes:" + printable_3.Count());            
            /*foreach (var x in printable_3)
            {
                Console.WriteLine("Critical class: "+ x);
            }*/
            
            /*foreach (var item in printable_2)
            {
                Console.WriteLine(item);
            }*/
            /*bool printable_4 = RPB.Calculate(tree);
            Console.WriteLine(printable_4);
            var printable_5 = CAAI.Calculate(root, printable);
            Console.WriteLine("accessors ratio: " + printable_5);
            var printable_6 = CMAI.Calculate(root, printable);
            Console.WriteLine("mutator ratio" + printable_6);
            var printable_7 = CMW.Calculate(root, printable_2);
            Console.WriteLine("cmw" + printable_7);
            var printable_8 = CCC.Calculate(root, printable, printable_3.Count());
            Console.WriteLine(printable_8);
            var printable_9 = CAIW.Calculate(root, printable, readContents); //Classified attributeların olduğu listeyi ve tüm txt orijinal kodu buna passlıyoruz
            Console.WriteLine(printable_9);
            var printable_10 =UACA.Calculate(root, printable, printable_3);
            Console.WriteLine(printable_10); 
            var printable_40 = CCDA.Calculate(root);
            Console.WriteLine("Number of Non-Private Class Attributes:" + printable_40.Count());
            var printable_50 = CIDA.Calculate(root, printable_40); // Calismasi icin CCDA'dan gelen list gerekli.
            Console.WriteLine("Number of Non-Private Instance Attributes:" + printable_50.Count());
            var printable_60 = COA.Calculate(root, printable);
            Console.WriteLine("Number of Non-Private Methods:" + printable_60.Count());
            var printable_70 = CIDA.ratio(printable_50.Count(), printable.Count());
            Console.WriteLine("Ratio of CIDA:" + printable_7);
            var printable_80 = CCDA.ratio(printable_40.Count(), printable.Count());
            Console.WriteLine("Ratio of CCDA:" + printable_8);
            var printable_90 = COA.ratio(printable_2.Count(), printable_60.Count());
            Console.WriteLine("Ratio of COA:" + printable_9);
            bool printable_100 = RPB.Calculate(tree);
            Console.WriteLine(printable_100);
            var printable_11 = CAAI.Calculate(root, printable);
            Console.WriteLine("accessors ratio: " + printable_11);
            var printable_12 = CMAI.Calculate(root, printable);
            Console.WriteLine("mutator ratio" + printable_12);
            var printable_13 = CMW.Calculate(root, printable_2);
            Console.WriteLine("cmw" + printable_13);
            var printable_14 = CCC.Calculate(root, printable, printable_3.Count());
            Console.WriteLine(printable_14);
            var printable_15 = CAIW.Calculate(root, printable, readContents); //Classified attributeların olduğu listeyi ve tüm txt orijinal kodu buna passlıyoruz
            Console.WriteLine(printable_15);
            var printable_temp = CDP.Calculate(root, printable_3.Count());
            Console.WriteLine("cdp" + printable_temp);
            var printable_16 = CWMP.Calculate(root, printable, printable_2, readContents);
            Console.WriteLine("CWMP: " + printable_16);*/

            //var printCCE = CCE.Calculate(root,printable_3);
            //var printCME = CME.Calculate(printable_2);

            var printCSCP = CSCP.Calculate(root,printable_3);
            Console.WriteLine("CSCP ratio:" + printCSCP);




        }
    }
}