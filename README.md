# Bezpieczny Egzamin — Kiosk Guard (Windows Client)

**Bezpieczny Egzamin (Kiosk Guard)** to otwartoźródłowa aplikacja kliencka dla systemu Windows (C# / .NET WinForms), przeznaczona dla uczniów i szkół korzystających z platformy edukacyjnej [egzamin.info.pl](https://egzamin.info.pl/).

Aplikacja uruchamia dedykowane środowisko egzaminacyjne w trybie pełnoekranowym (Kiosk) oraz przeprowadza diagnostykę połączenia ze stanowiskiem egzaminacyjnym.

---

## Informacje o projekcie / Project Information (Certum Open Source Verification)

* **Nazwa projektu / Project Name:** Bezpieczny Egzamin — Kiosk Guard
* **Autor / Author & Maintainer:** Karol Kowalski (Live Production)
* **Strona projektu / Website:** [https://egzamin.info.pl/](https://egzamin.info.pl/)
* **Organizacja GitHub / GitHub Organization:** [https://github.com/LiveProductionPL](https://github.com/LiveProductionPL)
* **Licencja / License:** MIT License (Open Source — see [LICENSE](./LICENSE))
* **Wersja / Version:** 2.1.0

---

## Funkcje programu / Features

1. **Automatyczne wykrywanie przeglądarek Chromium (Microsoft Edge / Google Chrome / Brave)** i uruchamianie natywnego trybu `--app` na pełnym ekranie.
2. **Obsługa kodów PIN sprawdzianów** przekazywanych bezpośrednio z nazwy pliku (np. `BezpiecznyEgzamin_2TI21.exe`) lub wpisanych w oknie startowym.
3. **Kryptograficzny token sesji (`EGZ-Kiosk-Guard/2.1`)** potwierdzający uruchomienie arkusza w dedykowanej aplikacji.

---

## Kompilacja ze źródeł / Building from Source

Wymagania: System Windows 10/11 z wbudowanym kompilatorem `.NET Framework 4.x` (`csc.exe`).

```powershell
powershell -ExecutionPolicy Bypass -File .\build_and_sign.ps1
```
