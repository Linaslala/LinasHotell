using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.UIMenus
{
    internal class RoomMenu
    {
        public static int ShowRoomMenu()
        {
            Console.WriteLine("=== Rum ===\n");
            Console.WriteLine("1. Skapa rum");
            Console.WriteLine("2. Lista rum");
            Console.WriteLine("3. Uppdatera rum");
            Console.WriteLine("4. Ta bort rum");
            Console.WriteLine("0. Tillbaka");
            Console.Write("\nVälj: ");

            return int.TryParse(Console.ReadLine(), out var select) ? select : -1;
        }
    }
}
