# BotBase

Bot Base is a basic wrapper / framework for DSharp+ https://github.com/DSharpPlus/DSharpPlus

This project was created as a base for getting started, and has no affiliation with DShap+.

## Configuration

Rename config.json.example to config.json, set token to your bot token and prefix as the required bot prefix.

## SQL Support 

Add the following to enable MySql Support

```
	"sql": 
		{
			"host": "127.0.0.1",
			"user": "bots_user",
			"pass": "bots_password",
			"name": "bots_database"
		}
```

## Creating your own modules

Copy and rename the Examle.Module project in ./Example

Rename the project to be YOURMODLE.Module and apply the change to the Namespace

Rename Example.cs to your required class name. 

Update Name, Description and Version as requiruired

In ./Commands create a new class (Eg FunCommands.CS)

Import the following 
```
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
```

Your new class needs to extend BaseCommandModule.

You can now create your own commands in this class!

To learn how to make commands and use DShap+ please check out their repo for the latest 
