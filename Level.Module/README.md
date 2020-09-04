# Level.Module

## About

This module allows server members to gain EXP for being active in the server. (Both Voice and Text Activity)


## Pre-Reqs
1. MySql Database Config [See Here For Details](https://github.com/CloudTheWolf/BotBase/blob/master/README.md)

2. `levels` and `levelsExpToLevel` tables in your database [Get Schema Here](https://github.com/CloudTheWolf/BotBase/blob/master/Sql_Schema/MySql/Tables)

3. All `Levels_` Procedures added to your database [Get Schema Here](https://github.com/CloudTheWolf/BotBase/blob/master/Sql_Schema/MySql/Procedures)

## How to use

In your config.json you will need the following
```
	"Level": {
    "MsgExp": 0.012,
    "VoiceExp": 0.012,
    "PurgeExpOnBan": false
  }
```

Change the MsgExp and VoiceExp to be how much EXP per Message / Minute of voice chat

If you want to purge a Users exp on being banned set `PurgeExpOnBan` to true

In the `levelsExpToLevel` table set the exp required for each level and add an optional level image.

Currently this module does not support Level Roles 