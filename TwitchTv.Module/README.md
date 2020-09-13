# TwitchTv.Module 

## What's New

In the latest version, multi-server support was added.
This required moving some settings from the Config File to the Database 

## About

By Request I have made the TwitchTV Notifications Module public.

This module combines [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) and [TwitchLib](https://github.com/TwitchLib/TwitchLib)

This module allows server members to have their stream go live announcement messages get automatically sent.

It will create an Embed message with the following:

* Link To Stream
* Stream Title
* Channel Name
* Game Title (Stream Catagory)
* Current Number Of Viewers
* Current Number Of Followers
* Level, Meaning Partner, Affiliate or Normal (Which we show as Road To Affiliate to help the channel stand out more)

Example:
![Example Go Live](https://img.itch.zone/aW1hZ2UvNzE1MTM4LzM5NjM4ODgucG5n/347x500/IVy5Kw.png)

## Pre-Reqs
1. MySql Database Config [See Here For Details](https://github.com/CloudTheWolf/BotBase/blob/master/README.md)

2. `streams` and `settings` tables, along with all `streams_` and `settings_` procedures in your database [Get Schema Demo Here](https://github.com/CloudTheWolf/BotBase/tree/master/Sql_Schema/MySql)

## How to use

1. Login into the [Twitch Developer Portal](https://dev.twitch.tv/)
2. Go to Your Console -> Applications and click Register Your Application
3. Give your Application a name and set the OAuth URL to `http://localhost`
4. Set the Category as "Application Intergration" 
5. Confirm you are not a pesky robot and click Save
6. Go into your Application and grab the Client ID and Application Secret 
7. In your config.json you will need the following
```
	"twitch": 
		{
			"ClientId": "MyClientId",
			"AccessToken": "MyAccessToken",
			"AutoAssign": true,
			"AutoPurge": true,
			"LogChannel": "LogChannelId"
		}
```

8. Create a role called `TTVMod` and assign this to yourself (And anyone you with to moderate stream announcements)
9. Use the `ttv.setup` command with the following parameters to start using the bot
`ttv.setup StreamChannel {{Discord Channel Id}}` - Set the channel for the streams to be pushed to.
`ttv.setup StreamerRole {{Discord Role Id}}` - Set the role to assign to streamers.
`ttv.setup VerifiedRole  {{Discord Role Id}}` - Set the Role for verified streamers. (Streamers who get `@here` mentions)
