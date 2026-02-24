# Thread Generator WPF Application

## Aprašymas
WPF aplikacija UVS atrankai

## Funkcionalumas
- **Thread kiekio nustatymas**: 2-15 thread'ų
- **Atsitiktinė eilutės generavimas**: 5-10 simbolių ilgio
- **Atsitiktinis intervalas**: 0.5-2 sekundės
- **Rezultatų saugojimas**: Paskutiniai 20 rezultatų rodomi ListView
- **Duomenų bazė**: SQL Server integracija su ThreadResults lentele

## Struktūra
- `MainWindow.xaml` - Pagrindinis langas su UI
- `MainWindow.xaml.cs` - Code-behind logika
- `ThreadManager.cs` - Thread'ų valdymas
- `DatabaseService.cs` - Duomenų bazės operacijos
- `App.xaml` - Application konfigūracija

## Duomenų bazės lentelė
```sql
CREATE TABLE ThreadResults (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ThreadID INT NOT NULL,
    [Time] DATETIME NOT NULL,
    Data NVARCHAR(100) NOT NULL
);
```



## Connection String
Pakeiskite `DatabaseService.cs` faile connection string pagal savo SQL Server konfigūraciją:
```csharp
_connectionString = "Server=localhost;Database=ThreadGeneratorDB;Trusted_Connection=true;";
```
