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
            double ergebnis;
            try
            {
                List<string> tokens = Tokenize(ausdruck);
                List<string> upn = ToRpn(tokens);
                ergebnis = EvaluateRpn(upn);
            }
            catch (InvalidOperationException)   // Stapel leer, z. B. bei "5+"
            {
                throw new RechenFehler("Ungültiger Ausdruck.");
            }
            catch (FormatException)             // Zahl nicht lesbar
            {
                throw new RechenFehler("Ungültige Zahl.");
            }

            if (double.IsNaN(ergebnis))
                throw new RechenFehler("Ergebnis ist nicht definiert.");
            if (double.IsInfinity(ergebnis))
                throw new RechenFehler("Ergebnis ist zu groß.");

            return ergebnis;
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
                        throw new RechenFehler("Es fehlt eine öffnende Klammer \"(\".");
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
                    throw new RechenFehler("Es fehlt eine schließende Klammer \")\".");
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
                        case "/":
                            if (b == 0) throw new RechenFehler("Division durch 0 ist nicht möglich.");
                            stapel.Push(a / b);
                            break;
                        case "^": stapel.Push(Math.Pow(a, b)); break;
                    }
                }
                else
                {
                    stapel.Push(double.Parse(token, deutsch));
                }
            }

            if (stapel.Count != 1)
                throw new RechenFehler("Ungültiger Ausdruck.");
            return stapel.Pop();
        }

        // #6: die wissenschaftlichen Funktionen
        private double Funktion(string name, double x)
        {
            switch (name)
            {
                case "sin": return Math.Sin(InBogenmass(x));
                case "cos": return Math.Cos(InBogenmass(x));
                case "tan":
                    if (Math.Abs(Math.Cos(InBogenmass(x))) < 1e-12)
                        throw new RechenFehler("tan ist bei 90° (bzw. π/2) nicht definiert.");
                    return Math.Tan(InBogenmass(x));
                case "log":
                case "ln":
                    if (x <= 0)
                        throw new RechenFehler("Logarithmus ist nur für Zahlen größer 0 definiert.");
                    return name == "log" ? Math.Log10(x) : Math.Log(x);
                case "√":
                    if (x < 0)
                        throw new RechenFehler("Wurzel aus einer negativen Zahl ist nicht definiert.");
                    return Math.Sqrt(x);
                case "exp": return Math.Exp(x);
                default: throw new RechenFehler("Unbekannte Funktion: " + name);
            }
        }

        // #7: Im DEG-Modus vor sin/cos/tan in Bogenmaß umrechnen
        private double InBogenmass(double x)
        {
            return Gradmass ? x * Math.PI / 180 : x;
        }
    }

    // Eigene Exception für verständliche Rechenfehler
    public class RechenFehler : Exception
    {
        public RechenFehler(string meldung) : base(meldung) { }
    }
}
