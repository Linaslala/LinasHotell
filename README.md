# LinasHotell

LinasHotell är en konsolbaserad hotellapplikation skriven i C#.  
Systemet används för att hantera gäster, rum och bokningar i ett mindre hotell.

Applikationen är byggd med Entity Framework Core för datalagring och Spectre.Console för ett användarvänligt gränssnitt i konsolen.

------------------------------------------------------

Funktioner:

Gäster
- Visa alla gäster
- Registrera, uppdatera och radera gäster
- Checka in och checka ut gäster  
- Gäster kan inte raderas om de är incheckade eller har bokningar

Rum
- Visa alla rum
- Skapa och uppdatera rum
- Inaktivera rum så att de inte längre går att boka

Bokningar
- Visa alla bokningar
- Skapa nya bokningar
- Uppdatera befintliga bokningar
- Ta bort bokningar
- Systemet räknar automatiskt ut:
  - antal nätter  
  - totalpris

------------------------------------------------------

Arkitektur:

Applikationen är uppdelad i tydliga lager där varje del har ett eget ansvar:

- Menyer: navigation och användarval  
- Controllers: programflöde och validering  
- Services: affärslogik och regler  
- Repositories: databasåtkomst  
- Models: datamodeller och relationer  

------------------------------------------------------

Datumval:

Vid bokning används ett interaktivt datumval direkt i konsolen:
- Kalendern navigeras med piltangenter
- Datum väljs med Enter
- Bokningar tillåter inte datum bakåt i tiden

Datumlogiken är uppdelad i separata hjälparklasser för rendering, navigering och flöde.

------------------------------------------------------

Start & konfiguration:

- Databasanslutning konfigureras i `appsettings.json`
- Vid start av programmet:
  - sätts alla beroenden upp med Dependency Injection
  - databasen seedas automatiskt med testdata
  - huvudmenyn startas

------------------------------------------------------

Nugetpaket:

Microsoft.EntityFrameworkCore.Design(10.0.6)

Microsoft.EntityFrameworkCore.SqlServer(10.0.6)

Microsoft.EntityFrameworkCore.Tools(10.0.6)

Mocrosoft.Extensions.Configuration.Json(10.0.6)

Microsoft.Extensions.DependencyInjection(10.0.6)

Spectre.Console(0.55.0)

------------------------------------------------------

ERD:

Databasens struktur beskrivs i filen `LinasHotell_ERD.drawio`. 

Öppna det via:
1. https://app.diagrams.net
2. File → Open From → GitHub / URL
3. Välj eller klistra in filens GitHub-länk

Relationer:

Room kan ha många Bookings. 

En Booking kan ha ett, och bara ett, Room.

Guest kan ha många Bookings. En Booking kan ha en, och bara en, Guest.


