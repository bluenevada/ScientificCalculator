using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace ScientificCalculator
{
    
    /// Interaction logic for MainWindow.xaml
   
    public partial class MainWindow : Window
    {
        private readonly Calculator rechner = new Calculator();
        private readonly Speicher speicher = new Speicher();
        private double? letztesErgebnis = null;   // null = kein gültiges Ergebnis

        public MainWindow()
        {
            InitializeComponent();
        }

        // Sub-Issue: Gemeinsamer Clickhandler für Ziffern und Operatoren
        private void Taste_Click(object sender, RoutedEventArgs e)
        {
            var taste = (Button)sender;
            string aktion = taste.Tag as string ?? taste.Content.ToString();

            switch (aktion)
            {
                case "CE":
                case "Backspace": ZeichenLoeschen(); break;
                case "Clear": AllesLoeschen(); break;
                case "=": Berechnen(); break;              
                case "(":
                case ")": Eingabe.Text += aktion; break;
                case "+/-": VorzeichenWechseln(); break;
                case "sin":
                case "cos":
                case "tan": FunktionEingeben(InvAktiv() ? "a" + aktion : aktion); break;
                case "log": FunktionEingeben(aktion); break;
                case "x^3":
                    if (InvAktiv()) FunktionEingeben("∛");
                    else Eingabe.Text += "^3";
                    break;
                case "n!": Eingabe.Text += "!"; break;
                case "1/x": Eingabe.Text += "^-1"; break;
                case "ln": FunktionEingeben(InvAktiv() ? "exp" : "ln"); break;
                case "x^2":
                    if (InvAktiv()) FunktionEingeben("√");
                    else Eingabe.Text += "^2";
                    break;
                case "x^y": OperatorEingeben("^"); break;
                case "pi": Eingabe.Text += "π"; break;
                case ",": KommaEingeben(); break;
                case "+":
                case "-":
                case "*":
                case "/": OperatorEingeben(aktion); break;
                case "MC":
                case "MR":
                case "MS":
                case "M+": SpeicherAktion(aktion); break;
                default:
                    if (aktion.Length == 1 && char.IsDigit(aktion[0]))
                        Eingabe.Text += aktion;
                    break;
            }
        }

        private void OperatorEingeben(string op)
        {
            string text = Eingabe.Text;

            // Steht schon ein Operator am Ende? Dann ersetzen
            if (text.Length > 0 && "+-*/^".Contains(text[text.Length - 1]))
                text = text.Substring(0, text.Length - 1);

            // Am Anfang oder nach "(" ist nur Minus (als Vorzeichen) erlaubt
            if ((text == "" || text.EndsWith("(")) && op != "-")
            {
                Eingabe.Text = text;
                return;
            }
            Eingabe.Text = text + op;
        }

        // Sub-Issue: Dezimalkomma
        private void KommaEingeben()
        {
            string zahl = LetzteZahl();
            if (zahl.Contains(",")) return;                 // nur ein Komma pro Zahl
            if (zahl == "") Eingabe.Text += "0";            // aus "," wird "0,"
            Eingabe.Text += ",";
        }

        // Hilfsmethode: Teil nach dem letzten Operator, also die Zahl, die gerade getippt wird
        private string LetzteZahl()
        {
            int pos = Eingabe.Text.LastIndexOfAny(new[] { '+', '-', '*', '/', '^', '(', ')' });
            return Eingabe.Text.Substring(pos + 1);
        }

        // Sub-Issue: CE – letztes Zeichen löschen
        private void ZeichenLoeschen()
        {
            if (Eingabe.Text.Length > 0)
                Eingabe.Text = Eingabe.Text.Substring(0, Eingabe.Text.Length - 1);
        }

        // Sub-Issue: C – beide Zeilen leeren
        private void AllesLoeschen()
        {
            Eingabe.Text = "";
            Ergebnis.Text = "0";
        }

        // Sub-Issue: Berechnung
        private void Berechnen()
        {
            letztesErgebnis = null;
            if (Eingabe.Text == "") return;
            try
            {
                rechner.Gradmass = RadioGrad.IsChecked == true;
                double ergebnis = Math.Round(rechner.Evaluate(Eingabe.Text), 10);
                Ergebnis.Text = ergebnis.ToString(new CultureInfo("de-DE"));
                letztesErgebnis = ergebnis;
            }
            catch (RechenFehler fehler)
            {
                Ergebnis.Text = fehler.Message;
            }
            catch (Exception)
            {
                Ergebnis.Text = "Unerwarteter Fehler.";
            }
        }

        // Sub-Issue: ±-Taste – dreht das Vorzeichen der letzten Zahl um
        private void VorzeichenWechseln()
        {
            string text = Eingabe.Text;
            int start = text.LastIndexOfAny(new[] { '+', '-', '*', '/', '^', '(', ')' }) + 1;
            if (start >= text.Length) return;   // am Ende steht keine Zahl

            int minusPos = start - 1;
            bool hatVorzeichen = minusPos >= 0 && text[minusPos] == '-' &&
                                 (minusPos == 0 || "+-*/^(".Contains(text[minusPos - 1]));

            if (hatVorzeichen)
                Eingabe.Text = text.Remove(minusPos, 1);   // "-5" → "5"
            else
                Eingabe.Text = text.Insert(start, "-");    // "5" → "-5"
        }

        // Hilfsmethoden
        private void FunktionEingeben(string name)
        {
            Eingabe.Text += name + "(";
        }

        // Liest die Inv-Checkbox und hakt sie danach ab, wie beim XP-Rechner
        private bool InvAktiv()
        {
            bool aktiv = Inv.IsChecked == true;
            Inv.IsChecked = false;
            return aktiv;
        }

        private void SpeicherAktion(string taste)
        {
            switch (taste)
            {
                case "MC":
                    speicher.Loeschen();
                    break;
                case "MR":
                    WertEinfuegen(speicher.Wert);
                    break;
                case "MS":
                    Berechnen();
                    if (letztesErgebnis != null) speicher.Speichern(letztesErgebnis.Value);
                    break;
                case "M+":
                    Berechnen();
                    if (letztesErgebnis != null) speicher.Addieren(letztesErgebnis.Value);
                    break;
            }
            SpeicherSymbol.Text = speicher.IstBelegt ? "M" : "";
        }

        // Fügt eine Zahl in die Eingabe ein, negative Zahlen in Klammern
        private void WertEinfuegen(double wert)
        {
            string text = wert.ToString(new CultureInfo("de-DE"));
            if (wert < 0) text = "(" + text + ")";
            Eingabe.Text += text;
        }
    }
}
