using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Level.Module
{
    class LevelOptions
    {
        public static decimal ExpPerMessage { get; set; }

        public static decimal ExpPerVoiceMin { get; set; }
        
        [Description("Remove all EXP from banned users")]
        public static bool PurgeExpOnBan { get; set; }
    }
}
