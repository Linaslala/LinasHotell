using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.UIMenus
{
    public static class MainMenu
    {
        public static int ShowMainMenu()
        {
            Console.WriteLine("=== Hotel Cozy Inn ===\n");
            Console.WriteLine("1. Bokningar");
            Console.WriteLine("2. Gäster");
            Console.WriteLine("3. Rum");
            Console.WriteLine("4. Fakturor");
            Console.WriteLine("0. Avsluta");
            Console.Write("\nVälj: ");

            return int.TryParse(Console.ReadLine(), out var select) ? select : -1;
        }
    }
}
