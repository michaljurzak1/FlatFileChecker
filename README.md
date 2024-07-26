# :card_index_dividers: FlatFileChecker - Wykaz Podatników VAT

Program do pobierania i odczytywania lokalnie za pomocą udostępnianego przez Ministerstwo Finansów [pliku płaskiego](https://www.podatki.gov.pl/vat/bezpieczna-transakcja/wykaz-podatnikow-vat/plik-plaski/).

## Motywacja

Bazując na [limitach użyć](https://www.gov.pl/web/kas/api-wykazu-podatnikow-vat) 10 zapytań na dzień (reset o 00:00), korzystanie z API okazuje się mocno ograniczone.

Program opiera się na instrukcjach w [specyfikacji technicznej](https://www.podatki.gov.pl/media/5745/specyfikacja-techniczna-pliku-plaskiego_20200826.pdf), w której możliwe jest sprawdzenie 
jedynie istnienia podatnika za pomocą **NIP oraz Numeru Rachunku Bankowego**.

## Działanie

W repozytorium istnieją dwa rozwiązania: **Pobieranie** oraz **Sprawdzanie**.   
Wykorzystywana jest lokalna baza danych SQLite, która jest tworzona w przypadku jej braku. Wszystkie rekordy w bazie danych, z wyjątkiem tabeli `Dane` są zamieniane.  
Jest to spowodowane codziennym generowaniem pliku, a także wsadem do funkcji skrótu Sha512 w danych, 
który składa się z konkatenacji łańcuchów: `data + nip + nrb`, dlatego każdego dnia plik ma całkowicie inne dane.

Cały program wypisuje informacje zwrotne.

## :arrow_down: Pobieranie
```
FlatFileDownload.exe
```
Pobiera najnowszy (dzisiejszy) plik płaski z oficjalnej strony. W przypadku braku istnienia bazy danych tworzy ją na potrzeby nowego pliku.
Za każdym razem aktualizuje tabelę `Dane` dodając nowy rekord z datą i godziną pobrania, datą generowania oraz konieczną liczbą transformacji funkcji skrótu Sha512.

W każdy kolejny dzień dane zostaną pobrane, a następnie zastąpią istniejące rekordy w tabelach `SkrotyPodatnikowCzynnych`, `SkrotyPodatnikowZwolnionych` oraz `Maski`.
Na końcu, po dodaniu / zamianie danych ustawia pozostałe flagi z kolumny `deleted` na 1 (usunięte).

![Schemat bazy danych](https://github.com/user-attachments/assets/db74a715-dff3-4808-a569-518a955c8c45)

## :white_check_mark: Sprawdzanie
```
FlatFileCheck.exe <NIP (10 znaków)> <Numer Rachunku Bankowego (26 znaków)>
```
Jeżeli baza danych nie istnieje lub jest nieaktualna z dzisiejszą datą, program pyta czy pobrać aktualne dane - należy potwierdzić.

### Przyjmuje
Numer Identyfikacji Podatkowej oraz Numer Rachunku Bankowego.  

Oba Argumenty nie mogą zawierać odstępów (white space).

### Zwraca
Wypisuje komunikat o typie konta podatnika, a także w której tabeli w bazie się znajduje.
Konto może być rzeczywiste, lub wirtualne:
- rzeczywiste oznacza pojedyncze konto
- konto wirtualne oznacza brak dosłownego istnienia go w bazie danych. **Może, lecz nie musi** istnieć, oznacza możliwe podkonto.

*W przypadku braku istnienia pliku na serwerze, program pyta użytkownika o użycie istniejących danych z ostatnią datą.*
