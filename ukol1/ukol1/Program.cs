using System.Text.RegularExpressions;
using Jace;

namespace ukol1
{
    internal class Program
    {
        static CalculationEngine engine = new CalculationEngine();
        static void Main(string[] args)
        {
            engine.AddFunction("log", (Func<double, double>)Math.Log, false);
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.txt"))
            {
                string text = File.ReadAllText(file);
                Regex rx = new Regex(@"[\w+\/\*\-\+\(\)]+ *= *[\w+\/\*\-\+\(\)]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(text);
                foreach (Match match in matches)
                {
                    string[] expresion = match.Value.Split("=");
                    Console.WriteLine(match.Value + "   ->  " + (Evaluate(expresion[0], expresion[1]) == true ? "Správně" : "Špatně"));
                }
            }
        }

        static bool Evaluate(string leftSide, string rightSide)
        {
            return engine.Calculate(leftSide) == engine.Calculate(rightSide);
        }
    }
}