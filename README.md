# LinasHotell

A console-based hotel booking application written in C#. The system manages guests, rooms and bookings for a small hotel, with data stored via **Entity Framework Core** and a user-friendly console interface built with **Spectre.Console**.

## Tech stack

- **C# / .NET 10**
- **Entity Framework Core** (Code First with migrations, SQL Server)
- **Spectre.Console** for the console UI
- Dependency Injection (`Microsoft.Extensions.DependencyInjection`)

## Features

**Guests**
- List all guests
- Register, update and delete guests
- Check in / check out guests
- Guests with active bookings cannot be deleted

**Rooms**
- List all rooms
- Create and update rooms
- Disable rooms so they can't be booked

**Bookings**
- List all bookings
- Create, update and delete bookings
- Automatic calculation of number of nights and total price

**Date picker**
- Interactive calendar picked in the console
- Navigate with arrow keys, confirm with Enter
- Dates in the past cannot be selected

## Architecture

The app is split into clear layers, each with a single responsibility:

- `UIMenus` — navigation and user choices
- `Controllers` — program flow and validation
- `Services` — business logic and rules
- `Repositories` — database access
- `Models` — data models and relationships
- `Utilities` — calendar rendering, navigation and flow
- `Settings` — application and database settings

## Getting started

```bash
git clone https://github.com/Linaslala/LinasHotell.git
cd LinasHotell
dotnet restore
dotnet ef database update
dotnet run --project LinasHotell
```

- The database connection is configured in `appsettings.json`.
- On start, dependencies are set up with dependency injection, the database is seeded with test data, and the main menu is shown.

## Database design

The ERD is included in `LinasHotell_ERD.drawio`. Open it via:

1. https://app.diagrams.net
2. File → Open From → GitHub / URL
3. Paste the file's GitHub link

Relationships:
- A `Room` can have many `Bookings`
- A `Booking` belongs to exactly one `Room`
- A `Guest` can have many `Bookings`
- A `Booking` belongs to exactly one `Guest`

## Possible future improvements

- Add price lists and room types via EF Core entities
- Add login/authentication for staff
- Export booking confirmations to PDF
- Add unit tests for services and repositories