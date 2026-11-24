using System;
using System.Collections.Generic;
using System.Globalization;

namespace ScientificCalculator
{
    public class Calculator
    {
        private readonly CultureInfo deutsch = new CultureInfo("de-DE");

        // #7: true = Grad (DEG), false = Bogenmaß (RAD)
        public bool Gradmass { get; set; } = true;

        public double Evaluate(string ausdruck)
        {
            List<string> tokens = Tokenize(ausdruck);
            List<string> upn = ToRpn(tokens);
            return EvaluateRpn(upn);
        }

        // ---------- Tokenizer ----------
        private List<string> Tokenize(string ausdruck)
        {
            var tokens = new List<string>();
            string zahl = "";
            string wort = "";   // für Funktionsnamen wie "sin"

            foreach (char zeichen in ausdruck)
            {
                if (char.IsDigit(zeichen) || zeichen == ',')
                {
                    zahl += zeichen;
                    continue;
                }
                if ((char.IsLetter(zeichen) && zeichen != 'π') || zeichen == '√')
                {
                    wort += zeichen;
                    continue;
                }

                // anderes Zeichen: gesammelte Zahl bzw. gesammeltes Wort abschließen
                if (zahl != "") { tokens.Add(zahl); zahl = ""; }
                if (wort != "") { tokens.Add(wort); wort = ""; }

                if (zeichen == 'π')
                    tokens.Add("π");
                else if (zeichen == '-' && IstVorzeichenPosition(tokens))
                    tokens.Add("neg");
                else
                    tokens.Add(zeichen.ToString());
            }
            if (zahl != "") tokens.Add(zahl);
            if (wort != "") tokens.Add(wort);
            return tokens;
        }

        private bool IstVorzeichenPosition(List<string> tokens)
        {
            if (tokens.Count == 0) return true;
            string davor = tokens[tokens.Count - 1];
            return davor == "(" || davor == "neg" || IstOperator(davor);
        }

        private bool IstOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "^";
        }

        private bool IstFunktion(string token)
        {
            return token == "sin" || token == "cos" || token == "tan" ||
                   token == "log" || token == "ln" || token == "√" || token == "exp";
        }

        private int Rang(string op)
        {
            switch (op)
            {
                case "^": return 4;
                case "neg": return 3;
                case "*":
                case "/": return 2;
                case "+":
                case "-": return 1;
                default: return 0;   // "(" und Funktionen
            }
        }

        // ---------- Shunting-yard ----------
        private List<string> ToRpn(List<string> tokens)
        {
            var ausgabe = new List<string>();
            var stapel = new Stack<string>();

            foreach (string token in tokens)
            {
                if (token == "(" || token == "neg" || IstFunktion(token))
                {
                    stapel.Push(token);
                }
                else if (token == ")")
                {
                    while (stapel.Count > 0 && stapel.Peek() != "(")
                        ausgabe.Add(stapel.Pop());

                    if (stapel.Count == 0)
                        throw new Exception("Es fehlt eine öffnende Klammer \"(\".");
                    stapel.Pop();   // "(" wegwerfen

                    // Gehörte die Klammer zu einer Funktion? Dann jetzt ausgeben
                    if (stapel.Count > 0 && IstFunktion(stapel.Peek()))
                        ausgabe.Add(stapel.Pop());
                }
                else if (IstOperator(token))
                {
                    // "^" ist rechtsassoziativ: bei gleichem Rang NICHT ausgeben
                    while (stapel.Count > 0 &&
                           (Rang(stapel.Peek()) > Rang(token) ||
                           (Rang(stapel.Peek()) == Rang(token) && token != "^")))
                        ausgabe.Add(stapel.Pop());
                    stapel.Push(token);
                }
                else
                {
                    ausgabe.Add(token);   // Zahl oder π
                }
            }

            while (stapel.Count > 0)
            {
                string rest = stapel.Pop();
                if (rest == "(" || IstFunktion(rest))
                    throw new Exception("Es fehlt eine schließende Klammer \")\".");
                ausgabe.Add(rest);
            }
            return ausgabe;
        }

        // ---------- UPN auswerten ----------
        private double EvaluateRpn(List<string> upn)
        {
            var stapel = new Stack<double>();

            foreach (string token in upn)
            {
                if (token == "π")
                {
                    stapel.Push(Math.PI);
                }
                else if (token == "neg")
                {
                    stapel.Push(-stapel.Pop());
                }
                else if (IstFunktion(token))
                {
                    stapel.Push(Funktion(token, stapel.Pop()));
                }
                else if (IstOperator(token))
                {
                    double b = stapel.Pop();
                    double a = stapel.Pop();
                    switch (token)
                    {
                        case "+": stapel.Push(a + b); break;
                        case "-": stapel.Push(a - b); break;
                        case "*": stapel.Push(a * b); break;
                        case "/": stapel.Push(a / b); break;
                        case "^": stapel.Push(Math.Pow(a, b)); break;
                    }
                }
                else
                {
                    stapel.Push(double.Parse(token, deutsch));
                }
            }

            if (stapel.Count != 1)
                throw new Exception("Ungültiger Ausdruck.");
            return stapel.Pop();
        }

        // #6: die wissenschaftlichen Funktionen
        private double Funktion(string name, double x)
        {
            switch (name)
            {
                case "sin": return Math.Sin(InBogenmass(x));
                case "cos": return Math.Cos(InBogenmass(x));
                case "tan": return Math.Tan(InBogenmass(x));
                case "log": return Math.Log10(x);
                case "ln": return Math.Log(x);
                case "√": return Math.Sqrt(x);
                case "exp": return Math.Exp(x);
                default: throw new Exception("Unbekannte Funktion: " + name);
            }
        }

        // #7: Im DEG-Modus vor sin/cos/tan in Bogenmaß umrechnen
        private double InBogenmass(double x)
        {
            return Gradmass ? x * Math.PI / 180 : x;
        }
    }
}
