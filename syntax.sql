# -- Hämta alla gäster --
# SELECT * 
# FROM Guests

# -- Hämta förnamn och efternamn på gäster --
# SELECT FirstName, LastName
# FROM Guests

# -- Hämta alla rum som kostar mer än 1000 kr per natt --
# SELECT *
# FROM Rooms
# WHERE PricePerNight > 1000

# -- Hämta alla bokningar sorterade efter startdatum (senaste först) --
# SELECT *
# FROM Bookings
# ORDER BY CheckInDate DESC

# -- Räkna hur många rum som finns totalt --
# SELECT COUNT(*) AS TotalNumberOfRooms
# FROM Rooms

# -- Visa unika rumstyper --
# SELECT DISTINCT RoomType
# FROM Rooms
