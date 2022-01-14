# NTR21Z-Sygocki-Dawid lab4

Time Reporting System (TRS) React (z bazą) - 30 pkt - termin oddania zadania 28.01.2021 23.59

## Uruchamianie

### Development (serwer i klient na osobnych portach, śledzenie zmian)

```bash
dotnet run
```

### Produkcja (serwer i klient na wspólnym porcie)

```bash
dotnet publish -c Release
cd bin/Release/net6.0/publish
./Trs.exe
```
