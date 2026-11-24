using System;
using System.Collections.Generic;
using System.Globalization;

namespace ScientificCalculator
{
    public class Calculator
    {
        private readonly CultureInfo deutsch = new CultureInfo("de-DE");

        public double Evaluate(string ausdruck)
        {
            List<string> tokens = Tokenize(ausdruck);
            List<string> upn = ToRpn(tokens);
            return EvaluateRpn(upn);
        }

        // Tokenizer – erkennt jetzt auch Minus als Vorzeichen ("neg")
        private List<string> Tokenize(string ausdruck)
        {
            var tokens = new List<string>();
            string zahl = "";

            foreach (char zeichen in ausdruck)
            {
                if (char.IsDigit(zeichen) || zeichen == ',')
                {
                    zahl += zeichen;
                }
                else
                {
                    if (zahl != "")
                    {
                        tokens.Add(zahl);
                        zahl = "";
                    }

                    // Sub-Issue: Minus am Anfang oder nach "(" als Vorzeichen erkennen
                    if (zeichen == '-' && IstVorzeichenPosition(tokens))
                        tokens.Add("neg");
                    else
                        tokens.Add(zeichen.ToString());
                }
            }
            if (zahl != "") tokens.Add(zahl);
            return tokens;
        }

        // Vorzeichen, wenn davor nichts steht, eine "(" oder ein Operator
        private bool IstVorzeichenPosition(List<string> tokens)
        {
            if (tokens.Count == 0) return true;
            string davor = tokens[tokens.Count - 1];
            return davor == "(" || davor == "neg" || IstOperator(davor);
        }

        private bool IstOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        // neg bindet am stärksten, "(" hat Rang 0 und wird daher nie vorzeitig ausgegeben
        private int Rang(string op)
        {
            switch (op)
            {
                case "neg": return 3;
                case "*":
                case "/": return 2;
                case "+":
                case "-": return 1;
                default: return 0;
            }
        }

        // Shunting-yard – jetzt mit Klammern und Vorzeichen
        private List<string> ToRpn(List<string> tokens)
        {
            var ausgabe = new List<string>();
            var stapel = new Stack<string>();

            foreach (string token in tokens)
            {
                if (token == "(" || token == "neg")
                {
                    stapel.Push(token);
                }
                // Sub-Issue: Klammern im Shunting-yard
                else if (token == ")")
                {
                    while (stapel.Count > 0 && stapel.Peek() != "(")
                        ausgabe.Add(stapel.Pop());

                    if (stapel.Count == 0)
                        throw new Exception("Es fehlt eine öffnende Klammer \"(\".");

                    stapel.Pop();   // die "(" selbst wegwerfen
                }
                else if (IstOperator(token))
                {
                    while (stapel.Count > 0 && Rang(stapel.Peek()) >= Rang(token))
                        ausgabe.Add(stapel.Pop());
                    stapel.Push(token);
                }
                else
                {
                    ausgabe.Add(token);   // Zahl
                }
            }

            while (stapel.Count > 0)
            {
                string rest = stapel.Pop();
                if (rest == "(")
                    throw new Exception("Es fehlt eine schließende Klammer \")\".");
                ausgabe.Add(rest);
            }
            return ausgabe;
        }

        private double EvaluateRpn(List<string> upn)
        {
            var stapel = new Stack<double>();

            foreach (string token in upn)
            {
                if (token == "neg")
                {
                    stapel.Push(-stapel.Pop());   // nur EINE Zahl umdrehen
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
    }
}