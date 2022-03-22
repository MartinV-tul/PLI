using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ukol4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "TEXTCZ1";
            string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + @"\" + fileName + ".txt", Encoding.GetEncoding(28592));
            int n = 2;
            NgramModel ngramModel = new NgramModel(lines, n);
            List<Ngram> ngrams = ngramModel.GetNgrams();
            ngrams.Sort();
            using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + @"\" + fileName + "-" + n + "gram" + ".txt"))
            {
                foreach (Ngram ngram in ngrams)
                {
                    writer.WriteLine(ngram.ToString());
                }
            }
        }

        private class Ngram : IComparable
        {
            public double value;
            string[] words;

            public Ngram(double value, params string[] words)
            {
                this.value = value;
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
                text += ") = " + Math.Log10(value);
                return text;
            }
        }

        private class NgramModel
        {
            private Dictionary<string, int> nLessOneGrams = new Dictionary<string, int>();
            private Dictionary<string, int> ngrams = new Dictionary<string, int>();
            private HashSet<string> vocabulary = new HashSet<string>();
            private Dictionary<string, int> precursorCount = new Dictionary<string, int>();
            private char[] separator = { '|' };
            private int length;
            public NgramModel(string[] lines, int size)
            {
                if (size == 0) return;
                length = lines.Length;
                for (int i = 0; i < length; i++)
                {
                    vocabulary.Add(lines[i]);
                    if (i > size - 3)
                    {
                        int index = i - size + 2;
                        string nLessOneGram = lines[index];
                        index++;
                        for (int j = index; j <= i; j++)
                        {
                            nLessOneGram = lines[j] + "|" + nLessOneGram;
                        }
                        if (size > 1)
                        {
                            if (!nLessOneGrams.ContainsKey(nLessOneGram))
                            {
                                nLessOneGrams.Add(nLessOneGram, 0);
                            }
                            nLessOneGrams[nLessOneGram]++;
                        }
                        if (i < length - 1)
                        {
                            string ngram = lines[i + 1] + "|" + nLessOneGram;
                            if (!ngrams.ContainsKey(ngram))
                            {
                                if (!precursorCount.ContainsKey(nLessOneGram))
                                {
                                    precursorCount.Add(nLessOneGram, 0);
                                }
                                precursorCount[nLessOneGram]++;
                                ngrams.Add(ngram, 0);
                            }
                            ngrams[ngram]++;
                        }
                    }
                }
            }

            public double GetProbability(string ngram)
            {
                if (!ngrams.ContainsKey(ngram)) return 0;
                string[] words = ngram.Split(separator, 2);
                if (words.Length == 1)
                {
                    return (double)ngrams[ngram] / length;
                }
                string nLessOneGram = words[1];
                double ngramCount = ngrams[ngram];
                double nLessOneGramCount = nLessOneGrams[nLessOneGram];
                return ngramCount / nLessOneGramCount;
            }

            public double GetWittenBellProbability(string ngram)
            {
                string[] words = ngram.Split(separator, 2);
                if (words.Length == 1)
                {
                    if (!ngrams.ContainsKey(ngram)) return 0;
                    return (double)ngrams[ngram] / length;

                }

                if (!precursorCount.ContainsKey(words[1])) return 0;

                double T = precursorCount[words[1]];
                double Z = vocabulary.Count - T;
                double N = nLessOneGrams[words[1]];

                if (!ngrams.ContainsKey(ngram)) return T / (Z * N + T);
                return ngrams[ngram] / (N + T);
            }

            public List<Ngram> GetNgrams()
            {
                List <Ngram> ngramsValue = new List<Ngram>();
                foreach (string n in ngrams.Keys)
                {
                    Ngram ngram = new Ngram(GetWittenBellProbability(n), n.Split('|'));
                    ngramsValue.Add(ngram);
                }
                return ngramsValue;
            }

            public double GetPerplexity()
            {
                double perplexity = 0;
                foreach(string ngram in ngrams.Keys)
                {
                    perplexity += Math.Log(GetProbability(ngram), 2) * ngrams[ngram];
                }
                perplexity *= -1;
                perplexity /= length;
                perplexity = Math.Pow(2, perplexity);
                return perplexity;
            }

            public double GetWittenBellPerplexity()
            {
                double perplexity = 0;
                foreach (string ngram in ngrams.Keys)
                {
                    perplexity += Math.Log(GetWittenBellProbability(ngram), 2) * ngrams[ngram];
                }
                perplexity *= -1;
                perplexity /= length;
                perplexity = Math.Pow(2, perplexity);
                return perplexity;
            }
        }
    }
}
