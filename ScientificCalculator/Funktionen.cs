using System;

namespace ScientificCalculator
{
    // Basisklasse: Jede Funktion hat einen Namen und kann berechnet werden
    public abstract class Funktion
    {
        public abstract string Name { get; }

        // false = steht vor der Zahl, z. B. sin(30); true = dahinter, z. B. 5!
        public virtual bool IstPostfix => false;

        public abstract double Berechne(double x, bool gradmass);
    }

    // Winkelfunktionen: die EINGABE ist ein Winkel
    public abstract class WinkelFunktion : Funktion
    {
        public override double Berechne(double x, bool gradmass)
        {
            double rad = gradmass ? x * Math.PI / 180 : x;
            return BerechneBogenmass(rad);
        }

        protected abstract double BerechneBogenmass(double rad);
    }

    public class Sinus : WinkelFunktion
    {
        public override string Name => "sin";
        protected override double BerechneBogenmass(double rad) => Math.Sin(rad);
    }

    public class Kosinus : WinkelFunktion
    {
        public override string Name => "cos";
        protected override double BerechneBogenmass(double rad) => Math.Cos(rad);
    }

    public class Tangens : WinkelFunktion
    {
        public override string Name => "tan";
        protected override double BerechneBogenmass(double rad)
        {
            if (Math.Abs(Math.Cos(rad)) < 1e-12)
                throw new RechenFehler("tan ist bei 90° (bzw. π/2) nicht definiert.");
            return Math.Tan(rad);
        }
    }

    // ---------- Arkusfunktionen: das ERGEBNIS ist ein Winkel ----------
    public abstract class ArkusFunktion : Funktion
    {
        public override double Berechne(double x, bool gradmass)
        {
            double rad = BerechneBogenmass(x);
            return gradmass ? rad * 180 / Math.PI : rad;
        }

        protected abstract double BerechneBogenmass(double x);
    }

    public class Arkussinus : ArkusFunktion
    {
        public override string Name => "asin";
        protected override double BerechneBogenmass(double x)
        {
            if (x < -1 || x > 1)
                throw new RechenFehler("asin ist nur für Werte von -1 bis 1 definiert.");
            return Math.Asin(x);
        }
    }

    public class Arkuskosinus : ArkusFunktion
    {
        public override string Name => "acos";
        protected override double BerechneBogenmass(double x)
        {
            if (x < -1 || x > 1)
                throw new RechenFehler("acos ist nur für Werte von -1 bis 1 definiert.");
            return Math.Acos(x);
        }
    }

    public class Arkustangens : ArkusFunktion
    {
        public override string Name => "atan";
        protected override double BerechneBogenmass(double x) => Math.Atan(x);
    }

    // ---------- Übrige Funktionen ----------
    public class Logarithmus : Funktion
    {
        public override string Name => "log";
        public override double Berechne(double x, bool gradmass)
        {
            if (x <= 0)
                throw new RechenFehler("Logarithmus ist nur für Zahlen größer 0 definiert.");
            return Math.Log10(x);
        }
    }

    public class NatuerlicherLogarithmus : Funktion
    {
        public override string Name => "ln";
        public override double Berechne(double x, bool gradmass)
        {
            if (x <= 0)
                throw new RechenFehler("Logarithmus ist nur für Zahlen größer 0 definiert.");
            return Math.Log(x);
        }
    }

    public class Wurzel : Funktion
    {
        public override string Name => "√";
        public override double Berechne(double x, bool gradmass)
        {
            if (x < 0)
                throw new RechenFehler("Wurzel aus einer negativen Zahl ist nicht definiert.");
            return Math.Sqrt(x);
        }
    }

    public class Kubikwurzel : Funktion
    {
        public override string Name => "∛";
        public override double Berechne(double x, bool gradmass) => Math.Cbrt(x);
    }

    public class Exponentialfunktion : Funktion
    {
        public override string Name => "exp";
        public override double Berechne(double x, bool gradmass) => Math.Exp(x);
    }

    public class Fakultaet : Funktion
    {
        public override string Name => "!";
        public override bool IstPostfix => true;   // steht HINTER der Zahl

        public override double Berechne(double x, bool gradmass)
        {
            if (x < 0 || x != Math.Floor(x))
                throw new RechenFehler("Fakultät ist nur für natürliche Zahlen definiert.");
            if (x > 170)
                throw new RechenFehler("Ergebnis ist zu groß.");

            double ergebnis = 1;
            for (int i = 2; i <= x; i++)
                ergebnis *= i;
            return ergebnis;
        }
    }

    public class Prozent : Funktion
    {
        public override string Name => "%";
        public override bool IstPostfix => true;   // steht HINTER der Zahl
        public override double Berechne(double x, bool gradmass) => x / 100;
    }
}
