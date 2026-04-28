namespace LinasHotell.Utilities
{
    public static class CalendarNavigator
    {
        public static DateTime Navigate(DateTime current, ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.RightArrow => current.AddDays(1),
                ConsoleKey.LeftArrow => current.AddDays(-1),
                ConsoleKey.UpArrow => current.AddDays(-7),
                ConsoleKey.DownArrow => current.AddDays(7),
                _ => current
            };
        }
    }
}