By połączyć się z bazą danych, potrzebne jest skonfigurowanie połączenia (connectionString) w pliku config.app

1: Wchodzimy do pliku config.App, następnie szukamy w eksploratorze rozwiązań pliku Profisys_Zadanie.mdf - klikamy na niego dwa razy
2: Po otworzeniu przechodzimy do właściwości owej bazy danych i następnie szukamy pozycji "Connection String" - którą kopiujemy
3: Skopiowaną zawartość, wklejamy do App.config w znacznikach <connectionStrings> - jako definicje zmiennej connectionString.

UWAGA: czasami baza danych może powodować błąd z szyfrowaniem, wtedy przy końcówce (w wklejonym connection String) należy zmienić z Encrypt=True na Encrypt=False;