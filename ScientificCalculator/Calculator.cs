using System;
using System.Collections.Generic;
using System.Globalization;

namespace ScientificCalculator
{
    public class Calculator
    {
        private readonly CultureInfo deutsch = new CultureInfo("de-DE");

        // true = Grad (DEG), false = Bogenmaß (RAD)
        public bool Gradmass { get; set; } = true;

        // Alle bekannten Funktionen, nachschlagbar über ihren Namen
        private readonly Dictionary<string, Funktion> funktionen = new Dictionary<string, Funktion>();

        public Calculator()
        {
            Funktion[] alle =
            {
                new Sinus(), new Kosinus(), new Tangens(),
                new Arkussinus(), new Arkuskosinus(), new Arkustangens(),
                new Logarithmus(), new NatuerlicherLogarithmus(),
                new Wurzel(), new Kubikwurzel(), new Exponentialfunktion(),
                new Fakultaet(), new Prozent()
            };
            foreach (Funktion f in alle)
                funktionen[f.Name] = f;
        }

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

        // Tokenizer
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
                if ((char.IsLetter(zeichen) && zeichen != 'π') || zeichen == '√' || zeichen == '∛')
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
            return funktionen.ContainsKey(token);
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

        // Shunting-yard
        private List<string> ToRpn(List<string> tokens)
        {
            var ausgabe = new List<string>();
            var stapel = new Stack<string>();

            foreach (string token in tokens)
            {
                // Postfix-Funktion (z. B. "!") gilt für das, was direkt davor steht
                if (IstFunktion(token) && funktionen[token].IstPostfix)
                {
                    ausgabe.Add(token);
                }
                else if (token == "(" || token == "neg" || IstFunktion(token))
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

        // UPN auswerten
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
                    // Welche Klasse dahintersteckt, entscheidet sich erst hier zur Laufzeit
                    stapel.Push(funktionen[token].Berechne(stapel.Pop(), Gradmass));
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
    }

    // Eigene Exception für verständliche Rechenfehler
    public class RechenFehler : Exception
    {
        public RechenFehler(string meldung) : base(meldung) { }
    }
}
