By połączyć się z bazą danych, potrzebne jest skonfigurowanie połączenia (connectionString) w pliku config.app

1: Wchodzimy do pliku config.App, następnie szukamy w eksploratorze rozwiązań pliku Profisys_Zadanie.mdf - klikamy na niego dwa razy
2: Po otworzeniu przechodzimy do właściwości owej bazy danych i następnie szukamy pozycji "Connection String" - którą kopiujemy
3: Skopiowaną zawartość, wklejamy do App.config w znacznikach <connectionStrings> - jako definicje zmiennej connectionString.

UWAGA: czasami baza danych może powodować błąd z szyfrowaniem, wtedy przy końcówce (w wklejonym connection String) należy zmienić z Encrypt=True na Encrypt=False;

Instalacja .NET Framework 4.8.1 - potrzebna do uruchomienia projektu:

1: Wejść w oficjalny instalator visual studio community 2022
2: Modyfikuj instalacje visual studio community 2022
3: Przejdź do Pojedyńcze składniki
4: Wyszukaj .NET Framework 4.8.1 - zainstaluj
5: Po instalacji wejdź w projekt
6: Kliknij prawym na projekt (nie solution/rozwiązanie)
7: W zakładce Application -> Target Framework 4.8.1
8: Gotowe.

Projekt można uruchomić, w razie pytań - proszę o kontakt