using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ukol2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            Dictionary<string, int> pairsDictionary = new Dictionary<string, int>();
            string fileName = "TEXTEN1";
            string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + @"\" + fileName + ".txt", Encoding.GetEncoding(28592));
            foreach (string line in lines)
            {
                if (!dictionary.ContainsKey(line))
                {
                    dictionary.Add(line, 0);
                }
                dictionary[line]++;
            }
            int length = lines.Length;

            bool v = false;
            int size;
            if (v)
            {
                int width = 51;

                for (int i = 0; i < length; i++)
                {
                    string word1 = lines[i];
                    for (int j = 1; j < width; j++)
                    {
                        int index = i - j;
                        if (index >= 0)
                        {
                            string word2 = lines[index];
                            string pair = word2 + "|" + word1;
                            if (!pairsDictionary.ContainsKey(pair))
                            {
                                pairsDictionary.Add(pair, 0);
                            }
                            pairsDictionary[pair]++;
                        }
                        index = i + j;
                        if (index < length)
                        {
                            string word2 = lines[index];
                            string pair = word1 + "|" + word2;
                            if (!pairsDictionary.ContainsKey(pair))
                            {
                                pairsDictionary.Add(pair, 0);
                            }
                            pairsDictionary[pair]++;
                        }
                    }
                }
                size = length * (width - 1) * 2 - width * (width - 1);
            }
            else
            {
                for (int i = 1; i < length; i++)
                {
                    string pair = lines[i - 1] + "|" + lines[i];
                    if (!pairsDictionary.ContainsKey(pair))
                    {
                        pairsDictionary.Add(pair, 0);
                    }
                    pairsDictionary[pair]++;
                }
                size = length - 1;
            }
            List<Pair> pairs = new List<Pair>();
            foreach (string pair in pairsDictionary.Keys)
            {
                string[] words = pair.Split('|');
                double A = (double)dictionary[words[0]];
                double B = (double)dictionary[words[1]];

                if (A < 10 || B < 10)
                {
                    continue;
                }
                A /= length;
                B /= length;
                double AB = (double)pairsDictionary[pair] / size;

                double value = Math.Log(AB / (A * B), 2);
                pairs.Add(new Pair(words[0], words[1], value));
            }
            pairs.Sort();
            string text = "<!DOCTYPE html><html><style>table,th,td {border: 1px solid black;}</style><body><table><tr><th>Dvojice</th><th>PMI</th></tr>";
            for (int i = 0; i < 100; i++)
            {
                text += "<tr><td>" + pairs[i].word1 + " " + pairs[i].word2 + "</td><td>" + pairs[i].value + "</td></tr>";
            }
            text += "</table></body></html>";
            if (v)
            {
                File.WriteAllText(Environment.CurrentDirectory + @"\" + fileName + "-50.html", text);
            }
            else
            {
                File.WriteAllText(Environment.CurrentDirectory + @"\" + fileName + "-1.html", text);
            }
        }

        private class Pair : IComparable
        {
            public string word1;
            public string word2;
            public double value;

            public Pair(string word1, string word2, double value)
            {
                this.word1 = word1;
                this.word2 = word2;
                this.value = value;
            }

            public int CompareTo(object obj)
            {
                Pair other = obj as Pair;
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
        }
    }
}
