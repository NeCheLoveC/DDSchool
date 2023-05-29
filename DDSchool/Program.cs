using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TextParser;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DDSchool
{
    class Program
    {
        //private static Dictionary<String, long> dictionary = new Dictionary<string, long>();

        private static String PATH = Directory.GetCurrentDirectory();
        private static String FILE_NAME = "text.txt";

        static void Main(string[] args)
        {
            String fullPathName = PATH + "\\" + FILE_NAME;
            if (System.IO.File.Exists(fullPathName))
            {
                String text = File.ReadAllText(fullPathName);
                Type type1 = typeof(WordCalculator);
                MethodInfo methodInfo1 = type1.GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Static);
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                Dictionary<String, int> dictionary = (Dictionary<String, int>)methodInfo1.Invoke(null, parameters: new Object[] { text });
                stopwatch1.Stop();
                Console.WriteLine("Время выполнение статического приватного метода - " + stopwatch1.ElapsedMilliseconds);

                

                Stopwatch stopwatch2 = new Stopwatch();
                WordCalculatorThread wordCalculatorThread = new WordCalculatorThread();
                stopwatch2.Start();
                Dictionary<String, int> dictionary2 = wordCalculatorThread.Parse(text);
                
                stopwatch2.Stop();
                Console.WriteLine("Время выполнение нестатического публичного метода - " + stopwatch2.ElapsedMilliseconds);

                writeDictionaryIntoFile(dictionary2);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Файл не найден... Для выполнения программы необходим файл в " +
                    "корневой папке проекта (" + PATH + ")" + " - text.txt");
            }
        }

        private static void writeDictionaryIntoFile(Dictionary<string, int> dictionary)
        {
            FileStream file = File.Create("result.txt");
            StreamWriter writer = new StreamWriter(file, Encoding.Default);
            foreach (KeyValuePair<String, int> pair in dictionary)
            {
                writer.WriteLine(pair.Key + " : " + pair.Value);
            }
            writer.Close();
            file.Close();
        }
    }
}
