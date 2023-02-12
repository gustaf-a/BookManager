# BookManager  

Den här readme-filen fokuserar på processen jag gick igenom för att lösa uppgiften.
Den är som en dagbok och skrevs löpande när jag löste uppgiften.
Något som inte är med är pauser som jag medvetet tog för att få perspektiv på problem, men de fanns där.

## 1. Förstå problemet ordentligt  

Det första steget var att gå igenom uppgiften.
- Läsa och förstå kraven
- Skriva ner idéer löpande och börja skapa en informell backlog
- Hitta eventuella dolda problem med kraven
- Försöka förstå vilka krav som kan komma att ändras ofta och vilka som kanske kommer ändras mindre ofta
- Hitta inspiration och läs på 5 minuter online om böcker (Google Books API om deras objekt och data samt kolla på goodreads.com hur deras böcker presenteras).


### Book-objektet  

Jag gissar att Book-objektet kommer att förändras i framtiden. Det är viktigt att veta för det påverkar designen av programmet.
De här förändringarna känns viktigast:

- Change: En enda författare/author är problematiskt då många böcker har flera, men det borde gå att lösa med ; separator
- Change: En enda genre är problematiskt då många böcker har flera, samma lösning som för författare möjligt
- Add: Språk/language är viktigt att veta om böcker
- Add: Priset/price saknar valuta vilket känns som en möjlig felfaktor för framtida användare av API:et
- Add: Type - böcker kan vara en ebok, pocket, ljudbok eller annat vilket kan vara viktigt
- Add: Publisher saknas, men publish_date finns. Det kan vara viktigt om den ges ut olika år av olika förlag i olika format
- Add: ISBN (International Standard Book Number) skulle kunna vara bra då det är internationell standard.


## 2. Definiera de ansvarområdena och försök hitta en MVP, Minimal Viable Product  

Nu med massor av idéer började jag försöka hitta snabbast möjliga sättet att leverera någonting fungerande.
Det här innebar att modellera eller översätta kraven till design. Först tänkte jag att programmet behövde:

- En controller med routes
- Databas eller lagring (i första versionen kan det vara .json-filen)
- Repository som hanterar databasen för att skapa och hämta böcker
- En service som lager mellan controllern och repot för att undvika att repot eller controllern får för många uppgifter
- Sortering av data (kan vara del av databasen)
- IdGenerator som skapar ID för nya böcker (kan vara del av databasen)
- Validering av data (kan vara del av databasen)
- Unit tester
- Service tester
- Backlog med features att implementera senare
- Dokumentation av process (den här readme-filen och bilder på mina fysiska anteckningar)


## 2.5 Skapa grundläggande test-case

Use case-examplen i uppgiften är en perfekt startpunkt för testerna. 
Jag hade lagt tid på att skapa test-case om inte de funnits.


## 3. Skapa projekten BookApi, BookApiServiceTests och BookApiUnitTests och första röda testerna 

- Började med att skapa BookApi-projektet med solution, sen xUnitprojekten BookApiServiceTests och BookApiUnitTests
- Testkörde BookApi för att säkerställa att det fungerade med OpenApi och Swagger för manuella tester
- Jag skapade en tom BookController för BookApi-projektet och en Startup-fil för att kunna köra service-test
- ServiceTests: Skapade BookController_should med GetAllBooks_WhenBaseRouteCalled och såg till att det fungerade att köra.


## 4. Version control design

Jag valde att gå med ett förenklat Git flow med main, develop och sen feature-branches för att:
- Versionskontroll känns viktigare med ett API än små kontinuerliga releaser (t.ex. GitHub Flow med bara master-branch)
- Kunna visa releases tydligt
- Visa hur jag är van vid att jobba från Sinch

Jag valde också att använda GitHub Projects för att skapa en backlog då det blir enkelt att dela med er.
Jag är mer van vid JIRA, Trello och Notion, men det kändes kul att få testa på GitHub Projects.


## 5. Första service testet från rött till grönt till rött igen: GetAllBooks_WhenBaseRouteCalled

- Döpte om master till main och skapade develop-branchen i git 
- Skapade en Issue i GitHub med ID BM1 (BookManager1), flyttade den till Doing och skapade sen en branch med namnet "bm1_base_url_returns_all_unsorted" från develop-branchen i git
- Jag arbetade sen utifrån servicetestet jag skapade i steg 3 för det första testcaset
- Hårdkodade först returvärden från controllern
- När jag fick respons OK i service testet med värdena från controllern så skapade jag TestDataHelper-klassen för jag visste att jag skulle skapa många tester.
- Jsonkonverteringen tog lite tid då jag skapade en extra JsonConverterare för DateOnly i onödan. Det var ett tag som det inte fungerade, men sen gick det till slut utan det jag hade skapat. Oavsett så var det kul att använda DateOnly och lära mig göra en custom JsonConverter för det.
- Nu lade jag till den sista Asserten så att testen blev rött igen tills jag får rätt värden från API:et.


## 6. Grov implementering av delarna i BookApi: Service och Repository

- Skapade IBookService, BookService, IBookRepository och JsonFileBookRepository, med samma json-konvertering som i ServiceTestet och första testet blev grönt
- Skapade en pull request från BM1-branchen till develop. Tog en paus från koden och kollade inte på Pull Requesten förrän dagen efter då jag kollade igenom och mergade.


## 7. Beslut kring databas, modellering och design

Först tänkte jag ta nästa use case och skriva test bara på det, men designen kändes nu beroende av hur jag skulle fortsätta hantera böckerna. 
Jag ville inte riskera att bygga vidare på JsonFileBookRepository och tvingas bygga egen sortering etc etc för att sen ändå i slutet byta till en riktig databas.

All data är välstrukturerad med förutsägbar struktur, så det finns ingen anledning jag kan se till att NoSQL skulle passa för böckerna.
SQL har också mycket av funktionaliteten som behövs inbyggd. Det känns som ett klassiskt databas-problem där SQL är lösningen.


## 8. BM2 - As a user of the API I can get books sorted by any field  

### Testskapande  

- Skapade InlineData-drivna Theory tester för att få datan sorted på de olika endpoints/routes
- Använde en online JSON till CSV converter och kopierade in böckerna i ett spreadsheet för att skapa rätt sekvenser för testerna

### SQLite 

- Skapade en SQLite databas med namnet books.db manuellt med books_seeding.sql
- Skapade IDatabaseAccess-interface och en SqliteDatabaseAccess-klass för att komma åt databasen ifrån repositories
- Skapade SqliteBookRepository och lade in det som default i projektet.
- Sökte efter alternativ för hur jag skulle skicka kommandon till databasen och bestämde mig för att använda SQL queries som jag skapade i koden
- Skapade IDatabaseQueryCreator och SqliteDatabaseQueryCreator som ger tillbaka ett Query-objekt med parametrar
- Utgick från unit test för att skapa minsta möjliga logik i SqliteDatabaseQueryCreator för att sen testa att koppla upp mig till databasen
- Fick problem med dependencies till sqlite batteries, och rekommenderad ominstallation av nuget fungerade inte. Jag löste det med den enklaste lösningen och installerade Microsoft.Data.Sqlite istället för .Core (som har problem med en dependency, kanske pga jag började med en ASP.NET Core Web API template)
- Lade till logiken i query creator för select all queries för de olika fälten. Nu kunde jag manuellt få ut böcker från databasen via Swagger eller Postman

### Tillbaka till service testerna för att få data sorterad på alla fält

Nu jobbade jag för att få service tester som täckte att läsa all data sorterad rätt.
- Skapade Controller endpoints för de olika fälten då service testerna såklart visade NotFound 404
- Skapade en GitHub issue for att senare lägga till global exception handling för controller endpoints

PublishDate orsakade problem då C# namnkonventionen inte passar med SQLite namnkonventionen "publish_date"
- För att se till att inte behöva ändra C# objekt om jag vill byta databas i framtiden så skapade jag extra objekt BookSqlite och hanterade det i SqliteDatabaseQueryCreator

### Sorteringen av ID (nu blev det B1, B10, B11 istället för B1, B2, B3)  

När mappningen mellan Book och BookSqlite var klar fungerade allting bra förutom sortering av ID-fältet (B1, B10, B11 istället för B1, B2, B3).

- Skapade en exempel SQL query för id och skapade separate unit test för ID-fältet
- Lade in specialhantering av ID-fältet i query creator
- Testerna gröna och service tester med! Sortering klar och alla tester grön! 

### Avslutade BM2  

- Skapade git commits och gjorde en pull request
- Åkte och tränade + var ute i solen för att få en paus
- Kolla på Pull Requesten och merga.

## BM5 - Get books filtered by text field values

- Skapade service-test för alla use case där böcker filtreras från text-värden
- Valde att börja med ett (inte ID) värde för att implementera enklast möjliga. Började med description-testen så kommenterade bort de andra testfallen
- Skapade en controller endpoint
- Refactored till att använda StringBuilder och gradvis bygga SQL queries
- Använde DB Browser for SQLite (ett användbart litet program för att öppna SQLite-databaser) för att testa SQL Queries
- Skapade unit tester
- När unit testerna var gröna och service testerna gröna skapade jag resten av controller endpointsen för text fält.


## BM8 - Get books filtered by price

- Skapade service-test för use case med filtrering från price/pris
- Skapade två endpoints för price för att kunna använda double variabler för att undvika att själv hantera input som string
- Gick in i DB Browser för att skapa SQL Queryn, skapade sen unit tester mot query creator
- Implementerade logiken i query creator

## BM9 - Get books filtered by date


## BM10 - Add exception handling to controller



## Extra

### Fel i test instruktionerna

- I use case för publish_date så skrivs det felaktigt som "published_date".