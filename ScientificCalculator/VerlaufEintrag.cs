using System.Globalization;

namespace ScientificCalculator
{
    public class VerlaufEintrag
    {
        public string Rechnung { get; }
        public double Ergebnis { get; }

        public VerlaufEintrag(string rechnung, double ergebnis)
        {
            Rechnung = rechnung;
            Ergebnis = ergebnis;
        }

        // Legt fest, wie der Eintrag in der Liste angezeigt wird
        public override string ToString()
        {
            return Rechnung + " = " + Ergebnis.ToString(new CultureInfo("de-DE"));
        }
    }
}
