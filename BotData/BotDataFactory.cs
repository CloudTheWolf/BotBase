using System;
using System.Collections.Generic;
using System.Text;

namespace BotData
{
    abstract class BotDataFactory
    {

        public abstract string dbhost { get; set; }
        public abstract string dbuser { get; set; }
        public abstract string dbpass { get; set; }
        public abstract string dbname { get; set; }

    }
}
