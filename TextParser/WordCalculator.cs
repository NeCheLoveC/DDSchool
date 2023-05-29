using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;

namespace TextParser
{
    public class WordCalculator
    {
        private static Dictionary<string, int> Parse(String text)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            Regex regex = new Regex(@"\b\w+\b");
            MatchCollection matches = regex.Matches(text);
            foreach (Match a in matches)
            {
                addWordIntoDictionary(a.Value, dictionary);
            }
            return dictionary = dictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        private static void addWordIntoDictionary(String str, Dictionary<string, int> dictionary)
        {
            int currentWordFrequency;
            if (dictionary.TryGetValue(str, out currentWordFrequency))
            {
                dictionary[str]++;
            }
            else
            {
                dictionary.Add(str, 1);
            }
        }
    }


    delegate void AddingWordIntoDictionary(String str);



    public class WordCalculatorThread
    {
        private object objLock;
        private Dictionary<string, int> dictionary;
        private ICollection<BagOfWords> bags;
        
        public WordCalculatorThread() { objLock = new object(); }

        public Dictionary<string, int> Parse(String text)
        {
            dictionary = new Dictionary<string, int>();
            Regex regex = new Regex(@"\b\w+\b");
            MatchCollection matches = regex.Matches(text);
            int countOfWords = matches.Count;
            AddingWordIntoDictionary dest = new AddingWordIntoDictionary(addWordIntoDictionary);
            bags = BagOfWords.SplitCollectionIntoBags(matches, dest);
            Task[] tasks = new Task[bags.Count];
            for (int i = 0;i < bags.Count;i++)
            {
                BagOfWords b = bags.ElementAt(i);
                Task task = Task.Run(b.AddWordsFromBagToDictionary);
                tasks[i] = task;
            }
            //Сортировку нужно запустить после завершения ВСЕХ потоков
            foreach (Task t in tasks)
            {
                t.Wait();
            }
            return dictionary = dictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        private void addWordIntoDictionary(String str)
        {
            lock(objLock)
            {
                int currentWordFrequency;
                if (dictionary.TryGetValue(str,out currentWordFrequency))
                {
                    dictionary[str]++;
                }
                else
                {
                    dictionary.Add(str, 1);
                }
                /*
                if (!dictionary.ContainsKey(str))
                {
                    dictionary.Add(str, 1);
                }
                else
                {
                    int currentWordFrequency;
                    dictionary.TryGetValue(str, out currentWordFrequency);
                    int newWordFrequency = currentWordFrequency + 1;
                    dictionary.Remove(str);
                    dictionary.Add(str, newWordFrequency);
                }
                */
            }
        }
    }

    class BagOfWords
    {
        private int startId;//Включая
        private int endId; //Не включая
        private MatchCollection collectionOfWords;
        //Обезопасит 
        private AddingWordIntoDictionary addWord;

        private BagOfWords(int StartId, int EndId, MatchCollection CollectionOfWords, AddingWordIntoDictionary Dest)
        {
            (startId, endId, collectionOfWords, addWord) = (StartId, EndId, CollectionOfWords, Dest);
        }

        public static ICollection<BagOfWords> SplitCollectionIntoBags(MatchCollection matchCollection, AddingWordIntoDictionary dest)
        {
            ICollection<BagOfWords> bagOfWords = new LinkedList<BagOfWords>();
            int countOfWords = matchCollection.Count;
            object objLock = new object();
            //1 BagOfWords
            if (matchCollection.Count < 500)
            {
                BagOfWords bag = new BagOfWords(0, countOfWords, matchCollection, dest);
                bagOfWords.Add(bag);
            }
            //2 BagOfWords
            else if (matchCollection.Count < 1000)
            {
                int halfOfSize = countOfWords / 2;
                BagOfWords bag = new BagOfWords(0, halfOfSize, matchCollection, dest);
                bagOfWords.Add(bag);

                bag = new BagOfWords(bag.endId, countOfWords, matchCollection, dest);
                bagOfWords.Add(bag);
            }
            //4 BagOfWords
            else
            {
                int quarterOfSize = countOfWords / 4;
                BagOfWords bag = new BagOfWords(0, quarterOfSize, matchCollection, dest);
                bagOfWords.Add(bag);

                bag = new BagOfWords(bag.endId, bag.endId + quarterOfSize, matchCollection, dest);
                bagOfWords.Add(bag);

                bag = new BagOfWords(bag.endId, bag.endId + quarterOfSize, matchCollection, dest);
                bagOfWords.Add(bag);

                bag = new BagOfWords(bag.endId, countOfWords, matchCollection, dest);
                bagOfWords.Add(bag);
            }
            return bagOfWords;
        }

        public void AddWordsFromBagToDictionary()
        {
            for (int i = startId; i < endId; i++)
            {
                
                //Вот этот код должен быть безопасным
                
                
                addWord(collectionOfWords[i].Value);
                

            }
        }

    }
}