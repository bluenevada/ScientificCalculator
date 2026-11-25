# Projektdokumentation – Wissenschaftlicher Taschenrechner

| | |
|---|---|
| **Projekt** | Wissenschaftlicher Taschenrechner in C# / WPF |
| **Zeitraum** | 23.11.2026 – 27.11.2026 |
| **Technik** | C#, WPF, .NET 10, MSTest, Git / GitHub (Kanban-Board) |
| **Grundlage** | Lasten- und Pflichtenheft im Ordner `docs` |

---

## 1. Umsetzungsstand

### 1.1 Muss-Kriterien

| Nr. | Anforderung | Status | Bemerkung |
|---|---|---|---|
| M01 | Eingabe von Zahlen und Dezimalkomma | ✔ umgesetzt | Nur ein Komma pro Zahl, `,` wird zu `0,` ergänzt |
| M02, M03, M06 | Grundrechenarten mit Punkt-vor-Strich | ✔ umgesetzt | Shunting-yard-Algorithmus |
| M04 | Klammern | ✔ umgesetzt | auch verschachtelt |
| M05 | Negative Vorzeichen | ✔ umgesetzt | Minus am Anfang, nach `(` oder Operator; zusätzlich ±-Taste |
| M07 | Löschen (C, ⌫) | ✔ umgesetzt | C leert beide Zeilen, Backspace/CE löschen das letzte Zeichen |
| M08 | Potenzen x², xʸ | ✔ umgesetzt | Taste `x^2` bzw. `x^y` |
| M09 | Quadratwurzel | ✔ umgesetzt | über **Inv + x^2** (wie beim XP-Vorbild) |
| M10 | sin, cos, tan | ✔ umgesetzt | |
| M11 | log, ln | ✔ umgesetzt | |
| M12 | Konstanten π und e | ✔ umgesetzt | e über **Inv + ln** mit Argument 1 |
| M13 | Umschaltung Grad / Bogenmaß | ✔ umgesetzt | über RadioButtons statt Umschalttaste |
| M14 | Fehlerbehandlung | ✔ umgesetzt | eigene Exception `RechenFehler`, Meldung in der Ergebniszeile |
| M15 | Oberfläche / Layout | ✔ umgesetzt (abweichend) | Layout nach Vorbild Windows-XP-Rechner, siehe Abschnitt 2 |

### 1.2 Kann-Kriterien

| Anforderung | Status | Bemerkung |
|---|---|---|
| Tastatureingabe | ✔ umgesetzt | Tastenkürzel des XP-Rechners (z. B. `s` = sin, `@` = x², Strg+M = MS) |
| asin, acos, atan | ✔ umgesetzt | über **Inv + sin/cos/tan**, Ergebnis im gewählten Winkelmaß |
| Fakultät | ✔ umgesetzt | Postfix-Schreibweise `5!`, nur für natürliche Zahlen bis 170 |
| Prozent | ✔ umgesetzt | Postfix `%` teilt durch 100, auf der früheren Mod-Taste |
| Verlauf | ✔ umgesetzt | zuschaltbar über Ansicht → Verlauf, Doppelklick übernimmt Ergebnis |
| Speichertasten | ✔ umgesetzt | MC, MR, MS, M+ mit Anzeige „M“ |
| Unit-Tests | ✔ umgesetzt | 56 Tests mit MSTest, alle bestanden |
| Dark Mode | ✔ umgesetzt | austauschbare Themes (Hell / Dunkel), Ansicht → Dunkles Design |
| Polymorphismus für Funktionen | ✔ umgesetzt | abstrakte Basisklasse `Funktion`, siehe Abschnitt 4 |

**Zusätzlich umgesetzt** (nicht geplant, durch das XP-Layout naheliegend): x³, Kubikwurzel (Inv + x³), 1/x.

---

## 2. Abweichungen von der Planung

| Geplant | Umgesetzt | Begründung |
|---|---|---|
| Grid mit 5 Spalten × 9 Zeilen | Layout des wissenschaftlichen Windows-XP-Rechners (14 Spalten × 5 Zeilen, Statistik-, Funktions-, Speicher- und Zifferblock) | gestalterische Entscheidung: originalgetreue Nachbildung |
| „=“-Taste über 4 Zeilen (`RowSpan`) | normale Taste | im XP-Layout nicht vorgesehen |
| eigene Tasten für √ und e | √ über Inv + x², e über Inv + ln | Umkehrfunktions-Logik des Originals nachgebaut |
| DEG-Taste, die den Modus anzeigt und umschaltet | RadioButtons Grad / Bogenmaß | entspricht dem XP-Vorbild |
| ⌫-Taste | Backspace **und** CE löschen das letzte Zeichen | beide Tasten sind im XP-Layout vorhanden |
| — | Mod-Taste in % umgewandelt | XP hatte in der wissenschaftlichen Ansicht keine %-Taste; `%` war dort das Tastaturkürzel für Mod |
| Prozent wie im Windows-Rechner (`200+10%` = 220) | Prozent als „÷ 100“ (`200+10%` = 200,1) | mathematisch eindeutig und passt zum Rechenkern; Sonderregel für + und − bewusst nicht umgesetzt |
| Zeitplan bis Freitag | alle Muss- und Kann-Kriterien bis Mittwoch fertig | Puffer für Tests und Dokumentation |

---

## 3. Nicht umgesetzt

Aus der XP-Vorlage übernommen, aber **bewusst ohne Funktion** (ausgegraut):

| Bereich | Elemente |
|---|---|
| Statistik | Sta, Ave, Sum, s, Dat |
| Funktionen | F-E, dms, Exp |
| Logik / Ganzzahl | And, Or, Xor, Lsh, Not, Int |
| Hexadezimal | A, B, C, D, E, F |
| Zahlensysteme | Hex, Okt, Bin |
| Winkelmaß | Neugrad |
| Hyperbelfunktionen | Hyp |

Ebenfalls ohne Funktion: die Menüpunkte **Bearbeiten → Kopieren / Einfügen**, **Ansicht → Standard** und **? → Info** sowie das zweite Anzeigefeld neben der Speicheranzeige (beim Original die Anzahl offener Klammern).

---

## 4. Aufbau des Programms

### 4.1 Klassen

| Datei | Aufgabe |
|---|---|
| `MainWindow.xaml` / `.cs` | Oberfläche, Tasten- und Tastatureingabe, Anzeige, Verlauf, Theme-Umschaltung |
| `Calculator.cs` | Rechenkern: Tokenizer, Shunting-yard, Auswertung der UPN, `RechenFehler` |
| `Funktionen.cs` | alle Funktionen als eigene Klassen (Polymorphismus) |
| `Speicher.cs` | Speicherfunktion (MC, MR, MS, M+) |
| `VerlaufEintrag.cs` | ein Eintrag im Verlauf (Rechnung und Ergebnis) |
| `Themes/Hell.xaml`, `Themes/Dunkel.xaml` | Farbschemata |
| `ScientificCalculator.Tests` | Unit-Tests |

Oberfläche und Logik sind getrennt: `Calculator`, `Funktionen` und `Speicher` kennen die Oberfläche nicht und lassen sich deshalb unabhängig testen.

### 4.2 Rechenkern

Ein Ausdruck wie `2+3*4` wird in drei Schritten berechnet:

1. **Tokenizer:** zerlegt den Text in Zahlen, Operatoren, Klammern und Funktionsnamen → `2`, `+`, `3`, `*`, `4`. Ein Minus am Anfang, nach `(` oder nach einem Operator wird als Vorzeichen (`neg`) erkannt.
2. **Shunting-yard:** wandelt die Tokens in Umgekehrte Polnische Notation um und berücksichtigt dabei Rangfolge, Klammern und Rechtsassoziativität von `^` → `2 3 4 * +`
3. **Auswertung:** Zahlen kommen auf einen Stack; ein Operator nimmt die obersten Zahlen, verrechnet sie und legt das Ergebnis zurück → `3*4 = 12`, `2+12 = 14`

Rangfolge: `^` (4) > Vorzeichen (3) > `*` `/` (2) > `+` `-` (1)

### 4.3 Polymorphismus

```
Funktion (abstract)
├── WinkelFunktion (abstract)   rechnet die Eingabe ins Bogenmaß um
│   └── Sinus, Kosinus, Tangens
├── ArkusFunktion (abstract)    rechnet das Ergebnis ins Gradmaß um
│   └── Arkussinus, Arkuskosinus, Arkustangens
└── Logarithmus, NatuerlicherLogarithmus, Wurzel, Kubikwurzel,
    Exponentialfunktion, Fakultaet, Prozent
```

Der `Calculator` hält alle Funktionen in einem `Dictionary<string, Funktion>` und ruft nur `Berechne(...)` auf. Welche Klasse dahintersteht, entscheidet sich zur Laufzeit. Fakultät und Prozent überschreiben zusätzlich die Eigenschaft `IstPostfix`. Eine neue Funktion erfordert nur eine neue Klasse und einen Eintrag in der Liste, der Rechenkern bleibt unverändert (Open-Closed-Prinzip).

### 4.4 Themes

Alle Farben sind als `DynamicResource` in den Dateien `Hell.xaml` und `Dunkel.xaml` hinterlegt. Beim Umschalten wird das eingebundene Ressourcenwörterbuch ausgetauscht, und alle Elemente färben sich sofort neu.

---

## 5. Tests

### 5.1 Testfälle aus dem Pflichtenheft

| Nr. | Eingabe | Erwartet | Tatsächlich | Ergebnis | Prüfung |
|---|---|---|---|---|---|
| T01 | 2+3×4 | 14 | 14 | ✔ bestanden | manuell + Unit-Test |
| T02 | (2+3)×4 | 20 | 20 | ✔ bestanden | manuell + Unit-Test |
| T03 | −5+3 | −2 | −2 | ✔ bestanden | manuell + Unit-Test |
| T04 | 7,5÷2 | 3,75 | 3,75 | ✔ bestanden | manuell + Unit-Test |
| T05 | 2^3 | 8 | 8 | ✔ bestanden | manuell + Unit-Test |
| T06 | √(16) | 4 | 4 | ✔ bestanden | manuell + Unit-Test |
| T07 | sin(30), DEG | 0,5 | 0,5 | ✔ bestanden | manuell + Unit-Test |
| T08 | cos(π), RAD | −1 | −1 | ✔ bestanden | manuell + Unit-Test |
| T09 | log(100) | 2 | 2 | ✔ bestanden | manuell + Unit-Test |
| T10 | ln(e) | 1 | 1 | ✔ bestanden | manuell + Unit-Test |
| T11 | 5÷0 | Meldung „Division durch 0“ | „Division durch 0 ist nicht möglich.“ | ✔ bestanden | manuell + Unit-Test |
| T12 | (2+3 | Meldung „Klammer fehlt“ | „Es fehlt eine schließende Klammer ")".“ | ✔ bestanden ¹ | manuell + Unit-Test |
| T13 | √(−4) | Meldung „Ungültige Eingabe“ | „Wurzel aus einer negativen Zahl ist nicht definiert.“ | ✔ bestanden ¹ | manuell + Unit-Test |
| T14 | 12, ⌫, C | erst 1, dann leere Anzeige | erst 1, dann Rechnungszeile leer und Ergebniszeile 0 | ✔ bestanden ² | manuell |

¹ Meldung präziser formuliert als ursprünglich geplant.
² Die Ergebniszeile zeigt nach C wie beim XP-Vorbild `0`.

### 5.2 Unit-Tests

56 automatisierte Tests mit MSTest, alle bestanden. Abgedeckt sind:

- Grundrechenarten und Rangfolge
- Klammern und Vorzeichen
- alle Funktionen im Grad- und Bogenmaß, Potenzen, Fakultät, Prozent, Konstanten
- alle Fehlerfälle (Division durch 0, fehlende Klammern, Definitionsbereich, ungültige Ausdrücke, zu große Ergebnisse)
- Polymorphismus (Aufruf über die Basisklasse `Funktion`)
- Speicher (Speichern, Addieren, Löschen)

Die Oberfläche (Tasteneingabe, Löschen, Verlauf, Theme) wurde manuell getestet.

### 5.3 Während der Entwicklung gefundene Fehler

| Fehler | Ursache | Behebung |
|---|---|---|
| Funktionstasten (sin, cos, tan, log, ln, x², xʸ) ohne Wirkung | Tasten waren im Click-Handler nicht angebunden | `case`-Zweige ergänzt; beim manuellen Test aufgefallen |
| Unit-Tests wurden nicht gefunden | Rechner-Projekt fehlte in der Projektmappe (`.slnx`), Visual Studio war in der Ordneransicht | Projekt zur Projektmappe hinzugefügt |
| Dark Mode: Beschriftungen von RadioButtons/CheckBoxen schwarz, weißer Rand um die Gruppen | Standardvorlagen von WPF setzen feste Farben | eigene Styles für RadioButton/CheckBox, GroupBox durch Border ersetzt |

---

## 6. Bekannte Einschränkungen

- Im Dark Mode bleiben die Kreise der RadioButtons und die Kästchen der CheckBoxen hell (Windows-Standarddarstellung).
- Keine implizite Multiplikation: `2π` oder `2(3+4)` müssen als `2*π` bzw. `2*(3+4)` eingegeben werden.
- Backspace löscht immer nur ein Zeichen, auch innerhalb eines Funktionsnamens (aus `sin(` wird `sin`).
- Klammern werden erst beim Berechnen geprüft, nicht schon bei der Eingabe.
- Sehr große Ergebnisse in Exponentialschreibweise (z. B. `1E+20`) lassen sich nicht über MR oder den Verlauf wieder in eine Rechnung einfügen.
- Theme und Verlauf werden beim Beenden nicht gespeichert.

---

## 7. Zeitplan: Soll und Ist

| Tag | Geplant | Tatsächlich |
|---|---|---|
| Montag | #1 Projekt-Setup | #1 Projekt-Setup |
| Dienstag | #2 Oberfläche, #3 Eingabe und Löschen | #2, #3, außerdem #4 Rechenkern, #5 Klammern/Vorzeichen, #6 Funktionen, #7 DEG/RAD, #8 Fehlerbehandlung |
| Mittwoch | #4 Rechenkern, #5 Klammern/Vorzeichen | alle Kann-Kriterien, Unit-Tests, Testfälle T01–T14, README, Dokumentation |
| Donnerstag | #6 Funktionen, #7 DEG/RAD, #8 Fehlerbehandlung | Puffer |
| Freitag | #9 Test und Abgabe | Abgabe |
