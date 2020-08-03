using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TwitchTv.Module
{
    class TwitchOptions
    {
        [Description("Tiwtch Client ID")]
        internal static string ClientId { get; set; }
        
        [Description("Tiwtch Access Token")]
        internal static string AccessToken { get; set; }
        
        [Description("Assign Roles To Streamers")]
        internal static bool AutoAssignRoles { get; set; }
        
        [Description("Purge Streams For Members who have left the server")]
        internal static bool AutoPurgeStreams { get; set; }
        
        [Description("Role ID to assign to streamers")]
        internal static ulong StreamerRole { get; set; }
        
        [Description("Role ID to assign to streamers who get @here mentions")]
        internal static ulong VerifiedRole { get; set; }
        
        [Description("Discord Channel Id to send the message to")]
        internal static ulong TargetChannelId { get; set; }

        [Description("Discord Channel Id to send logging messages to")]
        internal static ulong LogChannelId { get; set; }
    }
}
