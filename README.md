# Streamflix Project

Dit document biedt een overzicht van het Streamflix-project, de architectuur en hoe de verschillende services te benaderen zijn. 

## Inhoudsopgave
1.  [Project Starten](#1-project-starten)
2.  [Overzicht van Services en Poorten](#2-overzicht-van-services-en-poorten)
3.  [Database Toegang (pgAdmin)](#3-database-toegang-pgadmin)
4.  [API Documentatie (Swagger)](#4-api-documentatie-swagger)
5.  [Architectuur Overzicht](#5-architectuur-overzicht)
    *   [Frontend (React)](#frontend-react)
    *   [Backend API (.NET)](#backend-api-net)
    *   [Infrastructuur (Database)](#infrastructuur-database)
6.  [Testaccounts](#6-testaccounts)

---

### 1. Project Starten

Het gehele project is gecontaineriseerd met Docker en kan eenvoudig worden gestart met één commando.

**Vereiste:**
*   **Docker Desktop**: Zorg ervoor dat Docker Desktop draait.

**Start-commando:**
Voer het volgende commando uit in de root van het project:
```bash
docker-compose up --build
```
Dit commando bouwt de images voor de frontend en backend, en start alle benodigde containers: de webapplicatie (client), de API, de PostgreSQL database en een pgAdmin-interface voor databasebeheer.

---

### 2. Overzicht van Services en Poorten

Nadat `docker-compose up` is uitgevoerd, zijn de volgende services beschikbaar op `localhost`:

| Service              | Technologie     | URL op localhost                  | Poort | Beschrijving                                       |
|----------------------|-----------------|-----------------------------------|-------|----------------------------------------------------|
| **Frontend**         | React, Vite     | `http://localhost:5173`           | 5173  | De web-interface voor de eindgebruiker.              |
| **Backend API**      | .NET 9          | `http://localhost:5001`           | 5001  | De REST-API die de logica en data afhandelt.       |
| **Database Admin**   | pgAdmin 4       | `http://localhost:8080`           | 8080  | Web-interface om de PostgreSQL-database te beheren.|
| **Database Server**  | PostgreSQL 16   | `localhost:5432`                  | 5432  | De database-server (directe verbinding).           |

---

### 3. Database Toegang (pgAdmin)

`pgAdmin` is een web-based tool om de PostgreSQL database te inspecteren.

*   **URL**: `http://localhost:8080`
*   **Login-gegevens** (uit `docker-compose.yml`):
    *   **Email**: `admin@example.com`
    *   **Wachtwoord**: `admin`

**Database server verbinden (eerste keer):**

1.  In pgAdmin, rechtermuisknop op **Servers** → **Register** → **Server…**.
2.  **General** tabblad:
    *   **Name**: `streamflix_db` (of een andere herkenbare naam).
3.  **Connection** tabblad:
    *   **Host name/address**: `db` (Dit is de naam van de database-service binnen het Docker-netwerk).
    *   **Port**: `5432`
    *   **Maintenance database**: `streamflixdb`
    *   **Username**: `postgres`
    *   **Password**: `postgres`
4.  Klik op **Save**.

De `streamflixdb` database is nu zichtbaar. De tabellen zijn te vinden onder **Databases → streamflixdb → Schemas → public → Tables**.

---

### 4. API Documentatie (Swagger)

De API is volledig gedocumenteerd met Swagger (OpenAPI), wat een interactieve UI biedt om alle endpoints te testen.

*   **Swagger UI URL**: [http://localhost:5001](http://localhost:5001)

Hier kunt u alle beschikbare endpoints zien, de vereiste parameters en de structuur van de responses. U kunt vanuit deze interface ook direct requests naar de API sturen.

---

### 5. Architectuur Overzicht

Het project is opgedeeld in drie hoofdonderdelen: de frontend, de backend API en de infrastructuurlaag.

#### Frontend (React)
*   **Locatie**: `/client/client-app`
*   **Technologie**: React met TypeScript, gebouwd met Vite.
*   **Beschrijving**: Dit is de gebruikersinterface van Streamflix. Het stelt gebruikers in staat om door content te bladeren, profielen te beheren, de kijkgeschiedenis in te zien en abonnementen te beheren. De frontend communiceert via HTTP-requests met de Backend API om data op te halen en acties uit te voeren.

#### Backend API (.NET)
*   **Locatie**: `/src/Api`
*   **Technologie**: .NET 9 Web API (C#).
*   **Beschrijving**: De API vormt het hart van de applicatie en bevat alle bedrijfslogica.
    *   **`Controllers`**: Vangen HTTP-verzoeken op en sturen de juiste services aan. Elk domein (bv. `AccountController`, `ContentController`) heeft zijn eigen controller.
    *   **`Services`**: Bevatten de kernlogica. Ze coördineren de interactie met de database en voeren berekeningen of validaties uit.
    *   **`DTOs` (Data Transfer Objects)**: Specifiek gedefinieerde objecten voor de input en output van de API. Dit zorgt voor een duidelijke scheiding tussen de interne databasestructuur en de data die over het netwerk wordt gestuurd, wat de veiligheid en onderhoudbaarheid ten goede komt.
    *   **`Program.cs`**: Hier wordt de applicatie geconfigureerd, inclusief de databaseverbinding, dependency injection en het automatisch toepassen van database-migraties bij het opstarten.

#### Infrastructuur (Database)
*   **Locatie**: `/src/Infrastructure`
*   **Technologie**: Entity Framework Core 8 met een PostgreSQL-database.
*   **Beschrijving**: Deze laag is verantwoordelijk voor alle data-opslag en -beheer.
    *   **`Entities`**: C#-klassen die de databasestructuur definiëren. Elke `Entity` (bv. `Movie.cs`, `Account.cs`) representeert een tabel in de database. De relaties tussen tabellen (bv. een `Account` heeft een `Watchlist`) worden hier ook gedefinieerd.
    *   **`Data/ApplicationDbContext.cs`**: De `DbContext` van Entity Framework Core. Het fungeert als een brug tussen de C#-code en de database, en maakt het mogelijk om data op te vragen (queryen) en op te slaan.
    *   **`Migrations`**: Code-first migraties die door EF Core worden gegenereerd. Elke migratie beschrijft een verandering in de databasestructuur. Bij het opstarten van de API worden openstaande migraties automatisch op de database toegepast.
    *   **`Data/DbSeeder.cs`**: Een utility-klasse die de database vult met initiële, essentiële data (zoals filmgenres) wanneer de database voor het eerst wordt aangemaakt.

---

### 6. Testaccounts

Voor testdoeleinden zijn er 10 gebruikersaccounts beschikbaar.

*   **E-mailadressen**: `user1@streamflix.com` tot `user10@streamflix.com`
*   **Wachtwoord**: `Password123!`


### 7. Postman Tests

Om de api te testen in postman moet u de file Streamflix_tests.json in de map tests importeren in postman. Klik dan op Run en dan op Run Streamflix API Tests
