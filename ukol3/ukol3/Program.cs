using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ukol3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> unigrams = new Dictionary<string, int>();
            Dictionary<string, int> bigrams = new Dictionary<string, int>();
            Dictionary<string, int> trigrams = new Dictionary<string, int>();
            string fileName = "TEXTEN1";
            string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + @"\" + fileName + ".txt", Encoding.GetEncoding(28592));
            int length = lines.Length;
            for (int i = 0; i < length; i++)
            {
                string line = lines[i];
                if (!unigrams.ContainsKey(line))
                {
                    unigrams.Add(line, 0);
                }
                unigrams[line]++;
                if (i > 0)
                {
                    string bigram = lines[i - 1] + "|" + line;
                    if (!bigrams.ContainsKey(bigram))
                    {
                        bigrams.Add(bigram, 0);
                    }
                    bigrams[bigram]++;
                    if (i > 1)
                    {
                        string trigram = lines[i - 2] + "|" + bigram;
                        if (!trigrams.ContainsKey(trigram))
                        {
                            trigrams.Add(trigram, 0);
                        }
                        trigrams[trigram]++;
                    }
                }
            }
            List<Ngram> unigramModel = new List<Ngram>();
            List<Ngram> bigramModel = new List<Ngram>();
            List<Ngram> trigramModel = new List<Ngram>();

            foreach(string unigram in unigrams.Keys)
            {
                unigramModel.Add(new Ngram(unigrams[unigram], length, unigram));
            }
            foreach(string bigram in bigrams.Keys)
            {
                string[] words = bigram.Split('|');
                bigramModel.Add(new Ngram(bigrams[bigram], unigrams[words[0]], words[1], words[0]));
            }
            foreach (string trigram in trigrams.Keys)
            {
                string[] words = trigram.Split('|');
                string bigram = words[0] + "|" + words[1];
                trigramModel.Add(new Ngram(trigrams[trigram], bigrams[bigram], words[2], words[1], words[0]));
            }

            double perplexity = 0;
            foreach(Ngram ngram in unigramModel)
            {
                perplexity += Math.Log(ngram.value, 2) * ngram.count;
            }
            perplexity *= -1;
            perplexity /= length;
            perplexity = Math.Pow(2, perplexity);
            System.Console.WriteLine(perplexity);

            perplexity = 0;
            foreach (Ngram ngram in bigramModel)
            {
                perplexity += Math.Log(ngram.value, 2) * ngram.count;
            }
            perplexity *= -1;
            perplexity /= length;
            perplexity = Math.Pow(2, perplexity);
            System.Console.WriteLine(perplexity);

            perplexity = 0;
            foreach (Ngram ngram in trigramModel)
            {
                perplexity += Math.Log(ngram.value, 2) * ngram.count;
            }
            perplexity *= -1;
            perplexity /= length;
            perplexity = Math.Pow(2, perplexity);
            System.Console.WriteLine(perplexity);
            System.Console.ReadLine();

        }

        private class Ngram : IComparable
        {
            public double value;
            public int count;
            string[] words;

            public Ngram(int count, int count2, params string[] words)
            {
                this.count = count;
                this.value = (double)count / count2;
                this.words = words;
            }

            public int CompareTo(object obj)
            {
                Ngram other = obj as Ngram;
                if (other == null)
                {
                    return -1;
                }
                if (other.value < this.value)
                {
                    return -1;
                }
                if (other.value == this.value)
                {
                    return 0;
                }
                return 1;
            }

            public override string ToString()
            {
                if (words.Length == 1)
                {
                    return "P(" + words[0] + ") = " + value;
                }
                string text = "P(" + words[0] + " | " + words[1];
                for (int i = 2; i < words.Length; i++)
                {
                    text += ", " + words[i];
                }
                text += ") = " + value;
                return text;
            }
        }

    }
}
