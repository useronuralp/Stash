using System;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                        
                        if (tmp_2.Contains("Classified") || tmp_2.Contains("classified")) { 
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
                                string[] split = temp_2.Split(' ', '(' );
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
            List<string> temp_2 = new List<string>();
            temp_2.AddRange(classified_values);
            temp_2.AddRange(classified_methods);
            
            foreach (var value in random)
            {
                var temp = value.ToFullString();
                if (temp.Contains("class Program")) { continue; }
                else
                {
                    foreach (var item in temp_2)
                    {
                        if (temp.Contains(item)) { number_of_critical_classes++; break; }
                    }
                }
            }
            return number_of_critical_classes;
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
            return System.Math.Round(total_number_of_classified_attribute_interactions / total_number_of_interactions, 2);
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
            var printable_4 = CAIW.Calculate(root, printable, readContents); //Classified attributeların olduğu listeyi ve tüm txt orijinal kodu buna passlıyoruz
            Console.WriteLine(printable_4);
            /*foreach (var x in printable)
            {
                Console.WriteLine("Calssified attributes:"+ x);
            }/*
            foreach (var item in printable_2)
            {
                Console.WriteLine(item);
            }*/
        }
    }
}
