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
using System.Reflection;
namespace ConsoleApp1
{   public class Security
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
                                if (temp_2.Contains(x) && temp_2.Contains("Main") == false)
                                {
                                    temp_2 = temp_2.Trim();
                                    string[] split = temp_2.Split(' ', '(');
                                    if (split[0] == "abstract" && (split[1] == "public" || split[1] == "private"))
                                    {
                                        all_classified_methods.Add(split[3]);
                                        break;
                                    }
                                    else
                                    {
                                        all_classified_methods.Add(split[2]);
                                        break;
                                    }
                                }
                            }
                            //buraya ikinci olarak bide constructorlar yada getter setterlar için bir case eklenebilir. Sonuçta onlarda değerleri
                            //manipule edebiliyor.
                        }
                    }
                }
                return all_classified_methods.Distinct().ToList();
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
                    //Console.WriteLine(value);
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
                                    for (int i = 0; i < words.Count() - 1; i++)
                                    {
                                        if (words[i] == "class")
                                        {
                                            all_critical_classes.Add(words[i + 1].TrimEnd());
                                            break;
                                        }
                                    }
                                }
                                else if (words[1] == "sealed")
                                {
                                    all_critical_classes.Add(words[3].TrimEnd());
                                }
                                break;
                            }
                        }
                    }
                }
                foreach (var x in random)
                {
                    words = x.ToFullString().Trim().Split(' ', '{');
                    if (words[3] == ":" && all_critical_classes.Contains(words[4]))
                    {
                        all_critical_classes.Add(words[2]);
                    }
                }
                return all_critical_classes.Distinct().ToList();
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
        public class RPB
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
        public class CMAI
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
        public class CAAI
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
        public class UACA
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
                        main = value.ToString();
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
                    if (x.ToFullString().Contains("class Program") || x.ToFullString().Contains("Main")) { continue; }
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
                    //Console.WriteLine("Key: {0}, Value: {1}", item_3.Key, item_3.Value);
                    if (item_3.Value == 0)
                    {
                        not_used_variables++;
                    }
                }
                double result = not_used_variables / classified_values.Count();
                return System.Math.Round(result, 1);
            }
        }
        public class UCAM
        {
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_methods)
            {
                IDictionary<string, int> dict = new Dictionary<string, int>();
                var random = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
                double not_used_methods = 0;
                foreach (var method in classified_methods)
                {
                    dict.Add(method, 0);
                }
                foreach (var item in random)
                {
                    foreach (var x in classified_methods)
                    {
                        if (item.ToFullString().Contains(x))
                        {
                            for (int i = 0; i < dict.Count; i++)
                            {
                                if (dict.Keys.ElementAt(i) == x)
                                {
                                    dict[dict.Keys.ElementAt(i)] = dict[dict.Keys.ElementAt(i)] + 1;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < dict.Count; i++)
                {
                    dict[dict.Keys.ElementAt(i)] = dict[dict.Keys.ElementAt(i)] - 1;
                }
                foreach (KeyValuePair<string, int> item_3 in dict)
                {
                    //.WriteLine("Key: {0}, Value: {1}", item_3.Key, item_3.Value);
                    if (item_3.Value == 0)
                    {
                        not_used_methods++;
                    }
                }
                double result = not_used_methods / classified_methods.Count();
                return System.Math.Round(result, 1);
            }
        }
        public class UCAC
        {
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> critical_classes)
            {
                double not_used_classes = 0;
                IDictionary<string, int> dict = new Dictionary<string, int>();
                var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                foreach (var item in critical_classes)
                {
                    dict.Add(item, 0);
                }
                foreach (var value in random)
                {
                    if (value.ToFullString().Contains("class Program")) { continue; }
                    else
                    {
                        foreach (var x in critical_classes)
                        {
                            if (value.ToFullString().Contains(x))
                            {
                                for (int i = 0; i < dict.Count; i++)
                                {
                                    if (dict.Keys.ElementAt(i) == x)
                                    {
                                        dict[dict.Keys.ElementAt(i)] = dict[dict.Keys.ElementAt(i)] + 1;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < dict.Count; i++)
                {
                    dict[dict.Keys.ElementAt(i)] = dict[dict.Keys.ElementAt(i)] - 1;
                }
                foreach (KeyValuePair<string, int> item_3 in dict)
                {
                    //Console.WriteLine("Key: {0}, Value: {1}", item_3.Key, item_3.Value);
                    if (item_3.Value == 0)
                    {
                        not_used_classes++;
                    }
                }
                double result = not_used_classes / critical_classes.Count();
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
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> critical_classes)
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
                //Console.WriteLine("serilizable_ct value"+Serializable_ct);
                //Console.WriteLine("critical_classes.Count" + critical_classes.Count);
                try
                {
                    result = Serializable_ct / ((double)critical_classes.Count);
                }
                catch (ArithmeticException e)
                {
                    Console.WriteLine(e);
                    result = Serializable_ct / ((double)critical_classes.Count + 0.001);
                }
                return System.Math.Round(result, 1);
            }
        }
        public class CSP
        {
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> critical_classes)
            {
                var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                List<string> all_super_classes = new List<string>();
                List<string> all_super_classes2 = new List<string>();
                double Critical_superclass = 0;
                string[] words;
                foreach (var x in random)
                {
                    var temp = x.ToFullString();
                    if (temp.Contains("class Program")) { continue; }
                    if (temp.Contains(":"))
                    {
                        words = temp.Trim().Split(' ');
                        all_super_classes.Add(words[4].TrimEnd());
                        //Console.WriteLine(words[4]);
                    }
                }
                foreach (var x in critical_classes)
                {
                    foreach (var y in all_super_classes)
                    {
                        if (x.Contains(y))
                        {
                            Critical_superclass++;
                            all_super_classes2.Add(y);
                        }
                    }
                }
                //Console.WriteLine(Critical_superclass);
                /*foreach (var x in all_super_classes2)
                {
                    Console.WriteLine(x);
                }*/
                double result = 0;
                result = Critical_superclass / (double)(critical_classes.Count);
                return System.Math.Round(result, 1);
            }
        }
        public class CSI
        {
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> critical_classes)
            {
                var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                List<string> parent_classes = new List<string>();
                List<string> child_classes = new List<string>();
                double inherit_child_class_ct = 0;
                double not_possible_critical_class_ct = 0;
                List<string> all_super_classes2 = new List<string>();
                string[] words;
                foreach (var x in random)
                {
                    var temp = x.ToFullString();
                    if (temp.Contains("class Program")) { continue; }
                    if (temp.Contains(":"))
                    {
                        words = temp.Trim().Split(' ', '{');
                        if (words[3] == ":")
                        {
                            parent_classes.Add(words[4].TrimEnd());
                        }
                    }
                }
                foreach (var x in critical_classes)
                {
                    foreach (var y in parent_classes)
                    {
                        if (x.Contains(y))
                        {
                            all_super_classes2.Add(y);
                        }
                    }
                }
                foreach (var x in random)
                {
                    var temp = x.ToFullString();
                    if (temp.Contains("class Program")) { continue; }
                    if (temp.Contains(":"))
                    {
                        words = temp.Trim().Split(' ');
                        if (words[3] == ":")
                        {
                            child_classes.Add(words[2].TrimEnd());
                            inherit_child_class_ct++;
                        }
                    }
                }
                /*foreach (var x in child_classes)
                {
                    Console.WriteLine("child class list :" + x );//find child classes
                }
                foreach (var x in critical_classes)
                {
                    Console.WriteLine("critical class list :" + x);//get critical classes
                }*/
                foreach (var critclas in critical_classes)
                {
                    foreach (var childclas in child_classes)
                    {
                        if (critclas.Contains(childclas))
                        {
                            not_possible_critical_class_ct++;
                        }
                    }
                }
                double possible_critical_class = 0;
                double result = 0;
                /*Console.WriteLine("not possible" + not_possible_critical_class_ct);           
                Console.WriteLine("possible_critical_class: " + possible_critical_class);
                Console.WriteLine(inherit_child_class_ct);*/
                possible_critical_class = (critical_classes.Count()) - not_possible_critical_class_ct;
                try
                {
                    result = inherit_child_class_ct / possible_critical_class;
                }
                catch (ArithmeticException e)
                {
                    Console.WriteLine(e);
                    result = inherit_child_class_ct / (possible_critical_class + 0.001);
                }
                return System.Math.Round(result, 1);
            }
        }
        public class CMI
        {
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_methods, List<string> critical_classes)
            {
                var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                List<string> temp = new List<string>();
                double result;
                foreach (var item in random)
                {
                    foreach (var item_2 in critical_classes)
                    {
                        if (item.ToFullString().Contains(item_2) && item.ToFullString().Contains("sealed") == false)
                        {
                            foreach (var item_3 in classified_methods)
                            {
                                if (item.ToFullString().Contains(item_3)) { temp.Add(item_3); }
                            }
                        }
                    }
                }
                result = (double)(temp.Distinct().Count()) / (classified_methods.Count());
                return System.Math.Round(result, 1);
            }
        }
        public class CAI
        {
            public static double Calculate(Microsoft.CodeAnalysis.SyntaxNode root, List<string> classified_attributes, List<string> critical_classes)
            {
                var random = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                List<string> temp = new List<string>();
                double result;
                foreach (var item in random)
                {
                    foreach (var item_2 in critical_classes)
                    {
                        if (item.ToFullString().Contains(item_2) && item.ToFullString().Contains("sealed") == false)
                        {
                            foreach (var item_3 in classified_attributes)
                            {
                                if (item.ToFullString().Contains(item_3)) { temp.Add(item_3); }
                            }
                        }
                    }
                }
                result = (double)(temp.Distinct().Count()) / (classified_attributes.Count());
                return System.Math.Round(result, 1);
            }
        }
        public class RandW_Security_Metrics
        {
            public static double RCA(double CIDA, double CCDA, double CAI)
            {
                double result = 0;
                result = (CIDA + CCDA + CAI) * 4;
                return System.Math.Round(result, 1);
            }
            public static double WCA(double CMAI, double CAAI, double UACA)
            {
                double result = 0;
                result = (CMAI + CAAI + UACA) * 4;
                return System.Math.Round(result, 1);
            }
            public static double RCM(double COA, double CMI)
            {
                double result = 0;
                result = (COA + CMI) * 3;
                return System.Math.Round(result, 1);//CME yazılmadı o yok 
            }
            public static double WCM(double CAIW, double CMW, double CWMP, double UCAM)
            {
                double result = 0;
                result = (CAIW + CMW + CWMP + UCAM) * 3;
                return System.Math.Round(result, 1);
            }
            public static double RCC(double RPB, double CDP, double CSP)
            {
                double result = 0;
                result = (RPB + CDP + CSP) * 2;//CPCC,CCE yazılmadı.
                return System.Math.Round(result, 1);
            }
            public static double WCC(double CCC, double UCAC, double CSCP, double CSI)
            {
                double result = 0;
                result = (CCC + UCAC + CSCP + CSI) * 2;
                return System.Math.Round(result, 1);
            }
            public static double SAM(double CAT, double CMT, double CCT)
            {
                double result = 0;
                result = (CAT + CMT + CCT) * 1;
                return System.Math.Round(result, 1);
            }
        }
        public class Security_Design_Principle_Metrics
        {
            public static double PLP(double WCA, double WCM, double WCC)
            {
                double result = (WCA + WCM + WCC);
                return System.Math.Round(result, 1);
            }
            public static double PRAS(double RCA, double RCM, double RCC)
            {
                double result = (RCA + RCM + RCC);
                return System.Math.Round(result, 1);
            }
            public static double PSWL(double WCA, double WCM)
            {
                double result = (WCA + WCM);
                return System.Math.Round(result, 1);
            }
            public static double PFSD(double RCA, double RCM)
            {
                double result = (RCA + RCM);
                return System.Math.Round(result, 1);
            }
            public static double PLCM(double RCC, double WCC)
            {
                double result = (RCC + WCC);
                return System.Math.Round(result, 1);
            }
            public static double PI(double WCC)
            {
                double result = WCC;
                return System.Math.Round(result, 1);
            }
            public static double PEM(double SAM)
            {
                double result = SAM;
                return System.Math.Round(result, 1);
            }
        }
    }
    public class Technique_Used
    {
        public static Boolean Check_Loop_Usage(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            bool Loop_result = false;
            var For_each_list = root.DescendantNodes().OfType<ForEachStatementSyntax>().ToList();
            var for_list = root.DescendantNodes().OfType<ForStatementSyntax>().ToList();
            var While_list = root.DescendantNodes().OfType<WhileStatementSyntax>().ToList();
            /*foreach (var x in While_list)
            {
                Console.WriteLine(x);
            }*/
            if (For_each_list.Count() >= 1 || for_list.Count() >= 1 || While_list.Count() >= 1)
            {
                Loop_result = true;
            }
            return Loop_result;
        }
        public static Boolean Check_tryCatch_Usage(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            bool tryCatch_result = false;
            var tryCatch_list = root.DescendantNodes().OfType<TryStatementSyntax>().ToList();
            /*foreach (var x in tryCatch_list)
            {
                Console.WriteLine(x);
            }*/
            if (tryCatch_list.Count() >= 1)
            {
                tryCatch_result = true;
            }
            return tryCatch_result;
        }
        public static Boolean Check_ifElse_usage(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            bool ifElse_result = false;
            var ifElse_list = root.DescendantNodes().OfType<IfStatementSyntax>().ToList();
            /*foreach (var x in ifElse_list)
            {
                Console.WriteLine(x);
            }*/
            if (ifElse_list.Count() >= 1)
            {
                ifElse_result = true;
            }
            return ifElse_result;
        }
        public static Boolean Check_Switch_Case_usage(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            bool switch_case_result = false;
            var switch_case_list = root.DescendantNodes().OfType<SwitchStatementSyntax>().ToList();
            /*foreach (var x in switch_case_list)
            {
                Console.WriteLine(x);
            }*/
            if (switch_case_list.Count() >= 1)
            {
                switch_case_result = true;
            }
            return switch_case_result;
        }
    }
    public class OOP_Metrics
    {
        public static Boolean Check_polymorphism(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            var All_classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            string[] words;
            List<string> parent_classes = new List<string>();
            List<string> inherited_classes = new List<string>();
            foreach (var x in All_classes)
            {
                var temp = x.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                if (temp.Contains(":"))
                {
                    words = temp.Trim().Split(' ');
                    parent_classes.Add(words[4].Trim());
                    //Console.WriteLine(words[4]);                                    
                }
            }
            /*foreach (var y in parent_classes)
            {
                Console.WriteLine(y);//all parent classes list properly(no problem)
            }*/
            foreach (var x in All_classes)
            {
                var temp = x.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                if (temp.Contains(":"))
                {
                    inherited_classes.Add(temp);
                    //Console.WriteLine(temp);                                    
                }
            }
            /*foreach (var y in inherited_classes)
            {
                Console.WriteLine(y);//all inherited classes list properly(no problem)
            }*/
            //var interactions = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
            foreach (var kvp in inherited_classes)
            {
                words = kvp.Trim().Split(' ');
                kvpList.Add(new KeyValuePair<string, string>(words[2], words[4]));//child ve parent classsı liste koyma işlemi [child1,parent]
            }
            foreach (var y in kvpList)
            {
                //Console.WriteLine(y);
            }
            double poly_ct = 0;
            Boolean poly_ct2 = false;
            foreach (var y in kvpList)
            {
                foreach (var x in kvpList)
                {
                    if (x.Value.Contains(y.Value))
                    {
                        poly_ct++;
                    }
                }
                if (poly_ct > 1)
                {
                    poly_ct2 = true;
                }
                //Console.WriteLine(poly_ct);
                poly_ct = 0;
            }
            return poly_ct2;
        }
        public static Boolean Check_Constractor(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            var all_constructor = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
            Boolean result = false;
            /*foreach (var y in all_constructor)
            {
                Console.WriteLine(y);
            }*/
            if (all_constructor.Count() >= 1)
            {
                result = true;
            }
            return result;
        }
        public static Boolean Check_Enum(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            bool result = false;
            var Enum_list = root.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();
            if (Enum_list.Count() >= 1)
            {
                result = true;
            }
            return result;
        }
        public static Boolean Check_Inheritance(Microsoft.CodeAnalysis.SyntaxNode root)
        {
            var All_classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            List<string> inherited_classes = new List<string>();
            List<string> parent_classes = new List<string>();
            string[] words;
            bool result = false;
            foreach (var x in All_classes)
            {
                var temp = x.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                if (temp.Contains(":"))
                {
                    words = temp.Trim().Split(' ');
                    parent_classes.Add(words[4].Trim());
                    //Console.WriteLine(words[4]);                                    
                }
            }
            /*foreach (var y in parent_classes)
            {
                Console.WriteLine(y);
            }*/
            foreach (var x in All_classes)
            {
                var temp = x.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                if (temp.Contains(":"))
                {
                    inherited_classes.Add(temp);
                    //Console.WriteLine(temp);                                    
                }
            }
            /*foreach (var y in inherited_classes)
            {
                Console.WriteLine(y);
            }*/
            foreach (var i_c in inherited_classes)
            {
                foreach (var p_c in parent_classes)
                {
                    if (i_c.Contains(p_c))
                    {
                        result = true;
                    }
                }
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
            var printable = Security.CAT.Calculate(root); //CAT metricini çağırıp değerini almadan ikinci metrici çağıramıyoruz
            var printable_2 = Security.CMT.Calculate(root, printable); // CAT'den gelen değer buraya passlanıyor list olarak.
            Console.WriteLine("Number of classified data: " + printable.Count());
            Console.WriteLine("Number of classified methods: " + printable_2.Count());
            var printable_3 = Security.CCT.Calculate(root, printable, printable_2); //Yukardakilerin döndürdüğü listeleri alıp işlem yapıyor bu arkadaş.
            Console.WriteLine("Number of critical classes: " + printable_3.Count());
            /*foreach (var x in printable_3)
            {
                Console.WriteLine("Critical class: "+ x);
            }*/
            /*foreach (var item in printable_2)
            {
                Console.WriteLine(item);
            }*/
            var printable_40 = Security.CCDA.Calculate(root);
            Console.WriteLine("Number of Non-Private Class Attributes: " + printable_40.Count());
            var printable_50 = Security.CIDA.Calculate(root, printable_40); // Calismasi icin CCDA'dan gelen list gerekli.
            Console.WriteLine("Number of Non-Private Instance Attributes: " + printable_50.Count());
            var printable_60 = Security.COA.Calculate(root, printable);
            Console.WriteLine("Number of Non-Private Methods: " + printable_60.Count());
            var printable_70 = Security.CIDA.ratio(printable_50.Count(), printable.Count());
            Console.WriteLine("Ratio of CIDA: " + printable_70);
            var printable_80 = Security.CCDA.ratio(printable_40.Count(), printable.Count());
            Console.WriteLine("Ratio of CCDA: " + printable_80);
            var printable_90 = Security.COA.ratio(printable_2.Count(), printable_60.Count());
            Console.WriteLine("Ratio of COA: " + printable_90);
            bool printable_4 = Security.RPB.Calculate(tree);
            double printRPB = 0;
            if (printable_4)
            {
                printRPB += 1.0;
            }
            else
            {
                printRPB += 0.0;
            }
            Console.WriteLine("RPB: "+printable_4);
            var printable_12 = Security.CMAI.Calculate(root, printable);
            Console.WriteLine("CMAI: " + printable_12);
            var printable_11 = Security.CAAI.Calculate(root, printable);
            Console.WriteLine("CAAI: " + printable_11);
            var printable_15 = Security.CAIW.Calculate(root, printable, readContents); //Classified attributeların olduğu listeyi ve tüm txt orijinal kodu buna passlıyoruz
            Console.WriteLine("CAIW: "+printable_15);
            var printable_13 = Security.CMW.Calculate(root, printable_2);
            Console.WriteLine("cmw: " + printable_13);
            var printable_16 = Security.CWMP.Calculate(root, printable, printable_2, readContents);
            Console.WriteLine("CWMP: " + printable_16);
            var printable_14 = Security.CCC.Calculate(root, printable, printable_3.Count());
            Console.WriteLine("CCC: "+printable_14);
            var printable_17 = Security.UACA.Calculate(root, printable, printable_3);
            Console.WriteLine("UACA: "+printable_17);
            var printUCAM = Security.UCAM.Calculate(root, printable_2);
            Console.WriteLine("UCAM ratio:" + printUCAM);
            var printUCAC = Security.UCAC.Calculate(root, printable_3);
            Console.WriteLine("UACA ratio:" + printUCAC);
            var printCDP = Security.CDP.Calculate(root, printable_3.Count());
            Console.WriteLine("cdp: " + printCDP);
            var printCSCP = Security.CSCP.Calculate(root, printable_3);
            Console.WriteLine("CSCP ratio:" + printCSCP);
            var printCSP = Security.CSP.Calculate(root, printable_3);
            Console.WriteLine("CSP ratio:" + printCSP);
            var printCSI = Security.CSI.Calculate(root, printable_3);
            Console.WriteLine("CSI ratio: " + printCSI);
            var printCMI = Security.CMI.Calculate(root, printable_2, printable_3);
            Console.WriteLine("CMI ratio:" + printCMI);
            var printCAI = Security.CAI.Calculate(root, printable, printable_3);
            Console.WriteLine("CAI ratio:" + printCAI);
            //-------------------------------------------------------
            //table 2 
            var printRCA = Security.RandW_Security_Metrics.RCA(printable_70, printable_80, printCAI);//CAI eklenecek
            var printWCA = Security.RandW_Security_Metrics.WCA(printable_12, printable_11, printable_17);
            var printRCM = Security.RandW_Security_Metrics.RCM(printable_90, printCMI);
            var printWCM = Security.RandW_Security_Metrics.WCM(printable_15, printable_13, printable_16, printUCAM);
            var printRCC = Security.RandW_Security_Metrics.RCC(printRPB, printCDP, printCSP);
            var printWCC = Security.RandW_Security_Metrics.WCC(printable_14, printable_17, printCSCP, printCSI);
            var printSAM = Security.RandW_Security_Metrics.SAM(printable.Count, printable_2.Count, printable_3.Count);
            var RandW_Security_Metrics_result = printRCA + printWCA + printRCM + printWCM + printRCC + printWCC + printSAM;
            Console.WriteLine("Read & Write Security Metric Score is: " + System.Math.Round(RandW_Security_Metrics_result, 1));
            //TABLE 3---------------------------------------------------------------
            var printPLP = Security.Security_Design_Principle_Metrics.PLP(printWCA, printWCM, printWCC);
            var printPRAS = Security.Security_Design_Principle_Metrics.PRAS(printRCA, printRCM, printRCC);
            var printPSWL = Security.Security_Design_Principle_Metrics.PSWL(printWCA, printWCM);
            var printPFSD = Security.Security_Design_Principle_Metrics.PFSD(printRCA, printRCM);
            var printPLCM = Security.Security_Design_Principle_Metrics.PLCM(printRCC, printWCC);
            var printPI = Security.Security_Design_Principle_Metrics.PI(printWCC);
            var printPEM = Security.Security_Design_Principle_Metrics.PEM(printSAM);
            var Total_security_Index = printPLP + printPRAS + printPSWL + printPFSD + printPLCM + printPI + printPEM;//TSI METRIC
            Console.WriteLine("Security Design Principle Metric Score is : " + System.Math.Round(Total_security_Index, 1));
            //others--------------------------------------------------------------------------
            Console.WriteLine("----------------------------------------------------------------");
            bool check_Poly = OOP_Metrics.Check_polymorphism(root);//true veya false dönüyor bunu 1 0 yap 
            Console.WriteLine("does program contains polymorphism: " + check_Poly);
            bool check_Constructor = OOP_Metrics.Check_Constractor(root);//true veya false dönüyor bunu 1 0 yap 
            Console.WriteLine("does program contains constructor: " + check_Constructor);
            bool check_Enum = OOP_Metrics.Check_Enum(root);//true veya false dönüyor bunu 1 0 yap 
            Console.WriteLine("does program contains Enum: " + check_Enum);
            bool check_Inheritance = false;//true veya false dönüyor bunu 1 0 yap            
            if (check_Poly)
            {
                check_Inheritance = true;
            }
            else
            {
                check_Inheritance = OOP_Metrics.Check_Inheritance(root);
            }
            Console.WriteLine("does program contains Inheritance: " + check_Inheritance);
            bool check_Loop_usage = Technique_Used.Check_Loop_Usage(root);//true veya false dönüyor bunu 1 0 yap 
            Console.WriteLine("Loop usage result:" + check_Loop_usage);
            bool check_tryCatch_usage = Technique_Used.Check_tryCatch_Usage(root);
            Console.WriteLine("Try_Catch usage:" + check_tryCatch_usage);
            bool check_ifElse_usage = Technique_Used.Check_ifElse_usage(root);
            Console.WriteLine("İf else usage:" + check_ifElse_usage);
            bool check_Switch_Case_usage = Technique_Used.Check_Switch_Case_usage(root);
            Console.WriteLine("Switch Case usage:" + check_Switch_Case_usage);
        }
    }
}