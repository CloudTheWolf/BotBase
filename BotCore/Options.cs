using System.Collections.Generic;
using System.ComponentModel;

namespace BotCore
{
    public class Options
    {
        [Description("Command Prefix (Eg !)")]
        public static IEnumerable<string> Prefix { get; set; }


        public static bool EnableDms { get; set; }

        public static bool EnableMentionPrefix { get; set; }

        public static bool DmHelp { get; set; }

        public static bool DefaultHelp { get; set; }


    }
}
