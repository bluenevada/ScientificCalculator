# Lasten- und Pflichtenheft: Wissenschaftlicher Taschenrechner

Nov 23, 2026

Schulprojekt: ein wissenschaftlicher Taschenrechner in C# mit WPF-Oberfläche, Abgabe am Nov 27, 2026. Das Lastenheft beschreibt, was der Rechner können soll; das Pflichtenheft, wie er umgesetzt wird.

## Lastenheft

### 1. Ausgangssituation

Im Rahmen der Umschulung zur Fachinformatikerin für Anwendungsentwicklung wird innerhalb einer Projektwoche eine Desktop-Anwendung entwickelt. Benötigt wird ein Taschenrechner, der über die Grundrechenarten hinaus wissenschaftliche Funktionen beherrscht.

### 2. Zielsetzung

Die Anwenderin soll mathematische Ausdrücke inklusive Klammern und wissenschaftlicher Funktionen per Mausklick eingeben und korrekt berechnen lassen können. Fehleingaben führen zu einer verständlichen Meldung, nie zum Absturz.

### 3. Muss-Kriterien

| Nr. | Anforderung |
| --- | --- |
| M01 | Eingabe der Ziffern 0–9 und des Dezimaltrennzeichens über Buttons |
| M02 | Grundrechenarten: Addition, Subtraktion, Multiplikation, Division |
| M03 | Punkt-vor-Strich-Rechnung |
| M04 | Klammern |
| M05 | Negative Zahlen und Vorzeichenwechsel (±) |
| M06 | Berechnung mit „=“ und Anzeige des Ergebnisses |
| M07 | Löschen: C (alles) und ⌫ (letztes Zeichen) |
| M08 | Quadrat x² und Potenz xʸ |
| M09 | Quadratwurzel √ |
| M10 | Winkelfunktionen sin, cos, tan |
| M11 | Logarithmen log (Basis 10) und ln |
| M12 | Konstanten π und e |
| M13 | Umschaltung Grad (DEG) / Bogenmaß (RAD) |
| M14 | Verständliche Fehlermeldung bei Division durch 0, ungültigem Ausdruck, fehlender Klammer und ungültigem Definitionsbereich |
| M15 | Übersichtliche Oberfläche: Anzeige oben, Tastenfeld darunter |
| M16 | Lauffähig unter Windows 10 und 11 |

### 4. Kann-Kriterien

Nur bei verbleibender Zeit, in dieser Reihenfolge:

| Nr. | Anforderung |
| --- | --- |
| K01 | Eingabe über die Tastatur |
| K02 | Umkehrfunktionen asin, acos, atan |
| K03 | Fakultät n! |
| K04 | Prozentrechnung |
| K05 | Verlauf der letzten Rechnungen |
| K06 | Speichertasten M+, M−, MR, MC |
| K07 | Automatisierte Unit-Tests der Rechenlogik |
| K08 | Funktionen als Klassenhierarchie (Polymorphismus) |
| K09 | Dunkles Farbschema |

### 5. Abgrenzung

Nicht Bestandteil des Projekts sind: grafische Darstellung von Funktionen, Bruchrechnung, komplexe Zahlen, Zahlensysteme (Binär/Hex) und ein Installationsprogramm.
(Es sei denn, es ist noch Zeit)

### 6. Rahmenbedingungen

Einzelprojekt in einer Projektwoche (Montag bis Freitag), Abgabe am Freitag.

## Pflichtenheft

### 1. Technische Umsetzung

| Bereich | Umsetzung |
| --- | --- |
| Programmiersprache | C# |
| Framework | .NET 10 mit WPF (Windows Presentation Foundation) |
| Entwicklungsumgebung | Visual Studio 2026 |
| Versionsverwaltung | Git mit privatem GitHub-Repository |
| Zielsystem | Windows 11 |

### 2. Aufbau der Anwendung

Die Anwendung trennt Oberfläche und Rechenlogik in zwei Teile:

| Teil | Aufgabe |
| --- | --- |
| `MainWindow.xaml` | Layout der Oberfläche: Anzeige und Tastenfeld |
| `MainWindow.xaml.cs` | Click-Ereignisse der Tasten: Text in die Anzeige schreiben, löschen, Berechnung anstoßen |
| Klasse `Calculator` | Nimmt den eingegebenen Ausdruck als Text entgegen und gibt das Ergebnis zurück. Zerlegt den Ausdruck in Zahlen, Operatoren und Funktionen und berechnet ihn unter Beachtung von Klammern und Rangfolge |

Die Oberfläche enthält keine Rechenlogik. Dadurch lässt sich die Klasse `Calculator` unabhängig testen und erweitern.

### 3. Oberfläche

Das Fenster besteht aus einem Raster (`Grid`) mit 5 Spalten:

- **Anzeige** (oben): kleine Zeile mit der eingegebenen Rechnung, große Zeile mit dem Ergebnis
- **Funktionstasten**: DEG/RAD, π, e, C, ⌫ · sin, cos, tan, (, ) · log, ln, √, x², xʸ
- **Zahlenfeld**: Ziffern 0–9, ±, Komma, die vier Grundrechenarten und eine große „=“-Taste rechts

```
┌─────────────────────────────────────┐
│                     sin(30)+2^3     │
│                              8,5    │
├───────┬───────┬───────┬──────┬──────┤
│  DEG  │   π   │   e   │  C   │  ⌫  │
│  sin  │  cos  │  tan  │  (   │  )   │
│  log  │  ln   │   √   │  x²  │  xʸ  │
│   7   │   8   │   9   │  ÷   │      │
│   4   │   5   │   6   │  ×   │  =   │
│   1   │   2   │   3   │  −   │      │
│   ±   │   0   │   ,   │  +   │      │
└───────┴───────┴───────┴──────┴──────┘
```

### 4. Umsetzung der Anforderungen

| Nr. | Umsetzung |
| --- | --- |

### 5. Zeitplan

| Tag | Arbeitspaket |
| --- | --- |
| Mo | Lasten- und Pflichtenheft, Oberflächenskizze, Projekt und GitHub-Repository anlegen |
| Di | Oberfläche: Layout, Tasten schreiben in die Anzeige, C und ⌫ (M01, M07, M15) |
| Mi | Klasse `Calculator`: Grundrechenarten, Rangfolge, Klammern, Vorzeichen (M02–M06) |
| Do | Wissenschaftliche Funktionen, DEG/RAD, Fehlerbehandlung (M08–M14), danach Kann-Kriterien |
| Fr | Tests nach Testplan, Fehler beheben, Dokumentation und Präsentation, Abgabe |

### 6. Testfälle

Getestet wird manuell über die Oberfläche. Jeder Fall wird am Freitag mit „ok“ oder „nicht ok“ abgehakt.

### 7. Abnahmekriterien

Das Projekt gilt als erfüllt, wenn alle Muss-Kriterien umgesetzt sind, alle Testfälle T01–T14 bestanden sind und die Anwendung bei keiner Eingabe abstürzt.
