using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Hangman
{
    class Program
    {
        enum WeekDays
        {
            Monday = 0,
            Tuesday =1,
            Wednesday = 2,
            Thursday = 3,
            Friday = 4,
            Saturday =5,
            Sunday = 6
        }        
        public class Parent 
        { 
            int oasdk = 653; //classified
        }
        public class Devrim 
        { 
            int col = 6; //classified
        }
        public class Onur 
        { 
            int colam = 64; //classified
        }
        public class Child1 : Parent { }
        public class Child2 : Devrim { }
        public class Child3 : Onur 
        {
            int colsswem = 234; //classified
        }

        [Serializable]
	    public abstract class Component
	    {
		    abstract public void AddChild(Component c);
		    abstract public void Traverse();
		    int ok = 3; //classified
	    }

        [Serializable]
        public class PlaceOrder
        {
            public Guid Id { get; set; }
            int okasdasd = 356; //classified
        }

	    [Serializable]
	    public class ShoppingCartItem
	    {
   		    public string Name { get; set; }
   		    public int Quantity { get; set; }
   		    public decimal ListPrice { get; set; }
		    double onur = 2.0;//Classified  
	    }
        [Serializable]
        public class myClass
        {
            string[] hello; //Classified
            int merhaba = 12; //classified
            public int myValue = 2; //Classified
            public MyClass()
            {
            }
            public void myMethod(int parameter1, string parameter2)
            {   
                var myField = "onuralp";
                Console.WriteLine("First Parameter {0}, second parameter {1}", 
                                                            parameter1, parameter2);
                myValue = 2/2;
                myValue++;
            }
            public void MyMethod_2(int parameter3, string parameter4)
            {   
                myValue = 2;
                Console.WriteLine("First Parameter {52}, second parameter {53}", 
                                                            parameter3, parameter4);
                myValue++;
                myMethod(parameter3, parameter4);
            }
                public int MyAutoImplementedProperty { get; set; }
                private int myPropertyVar; //classified
	            public void getSomeValue()
	        {
	            Console.Writeline("some value");
	        }
            public int MyProperty
            {
                get { return myPropertyVar; }
                set { myPropertyVar = value; }
            }
	        public int getMyValue
	        {
		        get { return myValue; }
                set { myValue = value; }
	        } 
        }
        public sealed class MyClass_2 
        {   
            public void MyMethod_3(int parameter5, string parameter6)
            {
                Console.WriteLine("mrb televole!");
                myClass.merhaba = 11;
            }
            public int MyValue_2 = 9; //classified
            var hola; //Classified
        }
        static void Main(string[] args){
            Console.WriteLine("Welcome to Hangman!!!!!!!!!!");
            string[] listwords = new string[10];
            listwords[0] = "sheep";
            listwords[1] = "goat";
            listwords[2] = "computer";
            listwords[3] = "america";
            listwords[4] = "watermelon";
            listwords[5] = "icecream";
            listwords[6] = "jasmine";
            listwords[7] = "pineapple";
            listwords[8] = "orange";
            listwords[9] = "mango";
            myClass.merhaba = 19;
            Random randGen = new Random();
            var idx = randGen.Next(0, 9);
            string mysteryWord = listwords[idx];
            char[] guess = new char[mysteryWord.Length];
            Console.Write("Please enter your guess: ");
            int p;
            int j;
            for (p = 0; p < mysteryWord.Length; p++) {
                guess[p] = '*';
            }
            while (true)
            {
                char playerGuess = char.Parse(Console.ReadLine());
                for (j = 0; j < mysteryWord.Length; j++)
                {
                    if (playerGuess == mysteryWord[j])
                        guess[j] = playerGuess;
                }
                Console.WriteLine(guess);
            }
            foreach(var items in listwords)
            {
                Console.WriteLine(items);
            }
            int n;
            try
            {
                n=123;
            }
            catch(Exeption e)
            {
                Console.WriteLine(e);
            }
            if(n>12)
            {
                n=n+84;
            }
            else
            {
                n=n-84;
            }
            int day = 4;
            switch (day) 
            {
            case 1:
              Console.WriteLine("Monday");
              break;
            case 2:
              Console.WriteLine("Tuesday");
              break;
            case 3:
              Console.WriteLine("Wednesday");
              break;
            case 4:
              Console.WriteLine("Thursday");
              break;
            case 5:
              Console.WriteLine("Friday");
              break;
            case 6:
              Console.WriteLine("Saturday");
              break;
            case 7:
              Console.WriteLine("Sunday");
              break;
            }
            struct Employee
            {
                public int EmpId;
                public string FirstName;
                public string LastName;
            }
            p = 5;
            j = 8;
            j += 1;
            ++p;
            p++;
        }
    }
}