# TwitchTv.Module

## About

By Request I have made the TwitchTV Notifications Module public

This module allows server memvers to have their stream go live announcement messages get automatically sent.

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
                        "StreamerRole": "StreamerRoleId",
			"VerifiedRole": "VerifiedRoleId",
                        "StreamChannel": "StreamChannelId",
			"LogChannel": "LogChannelId"
		}
```
