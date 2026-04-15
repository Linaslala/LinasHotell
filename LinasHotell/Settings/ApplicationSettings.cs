using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.Settings
{
    public class ApplicationSettings
    {
        public DatabaseSettings Database { get; set; } = new();
    }
}
