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

namespace ScientificCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
   
    public partial class MainWindow : Window
    {
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
                case ",": KommaEingeben(); break;
                case "+":
                case "-":
                case "*":
                case "/": OperatorEingeben(aktion); break;
                default:
                    if (aktion.Length == 1 && char.IsDigit(aktion[0]))
                        Eingabe.Text += aktion;
                    break;
            }
        }

        private void OperatorEingeben(string op)
        {
            if (Eingabe.Text == "") return;                 // kein Operator am Anfang
            char letztes = Eingabe.Text[Eingabe.Text.Length - 1];
            if ("+-*/".Contains(letztes))                   // zwei Operatoren hintereinander?
                Eingabe.Text = Eingabe.Text.Substring(0, Eingabe.Text.Length - 1);
            Eingabe.Text += op;
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
            int pos = Eingabe.Text.LastIndexOfAny(new[] { '+', '-', '*', '/' });
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
    }
}
