# TatryExplorer
TatryExplorer to aplikacja internetowa stworzona do dzielenia się szlakami górskimi w Tatrach oraz odkrywania tras dodanych do niej przez innych użytkowników. 
# Wymagania
- .NET 8.0
- Microsoft SQL Server
- Visual Studio 2022
# Uruchomienie
## Krok 1: Pobranie projektu z GitHub
1.	Pobierz archiwum z projektem o nazwie „TatryExplorer.zip” z https://github.com/bkollat/TatryExplorer/releases/latest.
2.	Rozpakuj pobrane archiwum na swoim komputerze.

## Krok 2: Uruchomienie projektu w Visual Studio
1.	Otwórz folder, w którym rozpakowałeś projekt.
2.	Znajdź plik TatryExplorer.sln i podwójnie kliknij, aby otworzyć projekt w Visual Studio. 
Alternatywnie można wybrać go z poziomu otwartego programu Visual Studio.
 
## Krok 3: Aktualizacja bazy danych przed pierwszym uruchomieniem
1.	W Visual Studio otwórz narzędzie Package Manager Console z menu Tools > NuGet Package Manager > Package Manager Console.
2.	W Package Manager Console wykonaj następujące polecenie, aby zaktualizować bazę danych:
 - Update-Database

  Alternatywnie jeżeli wystąpią błędy z aktualizacją bazy to należy stworzyć nową migrację poleceniem:
 - Add-Migration Nazwa_Migracji

  I dopiero potem wykonać polecenie:
 - Update-Database

## Krok 4: Uruchomienie aplikacji
Uruchom projekt w Visual Studio przyciskiem albo klawiszem F5

