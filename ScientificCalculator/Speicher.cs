namespace ScientificCalculator
{
    public class Speicher
    {
        public double Wert { get; private set; } = 0;

        public bool IstBelegt => Wert != 0;

        public void Loeschen() { Wert = 0; }
        public void Speichern(double x) { Wert = x; }
        public void Addieren(double x) { Wert += x; }
    }
}
