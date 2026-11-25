using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScientificCalculator;

namespace ScientificCalculator.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator rechner = null!;

        // Läuft vor JEDEM Test: frischer Rechner im Gradmaß
        [TestInitialize]
        public void Vorbereiten()
        {
            rechner = new Calculator();
        }

        // Grundrechenarten
        [TestMethod]
        [DataRow("2+3*4", 14.0)]
        [DataRow("10-4-3", 3.0)]
        [DataRow("7/2", 3.5)]
        [DataRow("1,5*2", 3.0)]
        [DataRow("8/2*4", 16.0)]
        [DataRow("7,5/2", 3.75)]
        public void Grundrechenarten(string ausdruck, double erwartet)
        {
            Assert.AreEqual(erwartet, rechner.Evaluate(ausdruck), 1e-9);
        }

        // Klammern und Vorzeichen
        [TestMethod]
        [DataRow("(2+3)*4", 20.0)]
        [DataRow("2*(3+4)", 14.0)]
        [DataRow("((1+2)*3)", 9.0)]
        [DataRow("-5+3", -2.0)]
        [DataRow("-(2+3)", -5.0)]
        [DataRow("2*-3", -6.0)]
        [DataRow("5--3", 8.0)]
        public void KlammernUndVorzeichen(string ausdruck, double erwartet)
        {
            Assert.AreEqual(erwartet, rechner.Evaluate(ausdruck), 1e-9);
        }

        // Funktionen (Gradmaß)
        [TestMethod]
        [DataRow("sin(30)", 0.5)]
        [DataRow("cos(60)", 0.5)]
        [DataRow("tan(45)", 1.0)]
        [DataRow("asin(0,5)", 30.0)]
        [DataRow("acos(0,5)", 60.0)]
        [DataRow("atan(1)", 45.0)]
        [DataRow("3^2", 9.0)]
        [DataRow("2^10", 1024.0)]
        [DataRow("2^3^2", 512.0)]        // rechtsassoziativ: 2^(3^2)
        [DataRow("-2^2", -4.0)]
        [DataRow("2^-1", 0.5)]
        [DataRow("√(16)", 4.0)]
        [DataRow("∛(27)", 3.0)]
        [DataRow("log(1000)", 3.0)]
        [DataRow("ln(1)", 0.0)]
        [DataRow("exp(1)", 2.718281828459045)]
        [DataRow("5!", 120.0)]
        [DataRow("0!", 1.0)]
        [DataRow("(2+1)!", 6.0)]
        [DataRow("π", 3.141592653589793)]
        [DataRow("20%", 0.2)]
        [DataRow("50*20%", 10.0)]
        [DataRow("200+10%", 200.1)]
        [DataRow("2^3", 8.0)]
        [DataRow("log(100)", 2.0)]
        [DataRow("ln(exp(1))", 1.0)]
        public void FunktionenImGradmass(string ausdruck, double erwartet)
        {
            Assert.AreEqual(erwartet, rechner.Evaluate(ausdruck), 1e-9);
        }

        // Bogenmaß
        [TestMethod]
        [DataRow("sin(π/2)", 1.0)]
        [DataRow("cos(π)", -1.0)]
        [DataRow("asin(1)", 1.5707963267948966)]
        public void FunktionenImBogenmass(string ausdruck, double erwartet)
        {
            rechner.Gradmass = false;
            Assert.AreEqual(erwartet, rechner.Evaluate(ausdruck), 1e-9);
        }

        // Fehlerbehandlung
        [TestMethod]
        [DataRow("5/0")]
        [DataRow("(2+3")]
        [DataRow("2+3)")]
        [DataRow("√(-4)")]
        [DataRow("log(0)")]
        [DataRow("ln(-1)")]
        [DataRow("tan(90)")]
        [DataRow("asin(2)")]
        [DataRow("2,5!")]
        [DataRow("5+")]
        [DataRow("10^400")]
        [DataRow("(-8)^0,5")]
        public void UngueltigeEingabeWirftRechenFehler(string ausdruck)
        {
            Assert.ThrowsExactly<RechenFehler>(() => rechner.Evaluate(ausdruck));
        }

        // Polymorphismus
        [TestMethod]
        public void FunktionAlsBasisklasseRechnetRichtig()
        {
            Funktion f = new Sinus();          // Variable vom Typ der Basisklasse
            Assert.AreEqual(0.5, f.Berechne(30, true), 1e-9);

            f = new Fakultaet();               // gleiche Variable, andere Klasse
            Assert.AreEqual(24.0, f.Berechne(4, true), 1e-9);
        }

        // Speicher
        [TestMethod]
        public void SpeicherSpeichertAddiertUndLoescht()
        {
            var speicher = new Speicher();
            Assert.IsFalse(speicher.IstBelegt);

            speicher.Speichern(12);
            speicher.Addieren(8);
            Assert.AreEqual(20.0, speicher.Wert);
            Assert.IsTrue(speicher.IstBelegt);

            speicher.Loeschen();
            Assert.AreEqual(0.0, speicher.Wert);
            Assert.IsFalse(speicher.IstBelegt);
        }
    }
}
