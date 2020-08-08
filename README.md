# BotBase

![CodeNotary](https://github.com/CloudTheWolf/BotBase/workflows/CodeNotary/badge.svg) ![CodeQL](https://github.com/CloudTheWolf/BotBase/workflows/CodeQL/badge.svg)

Bot Base is a basic wrapper / framework for DSharp+ https://github.com/DSharpPlus/DSharpPlus

This project was created as a base for getting started, and has no affiliation with DShap+.

## Configuration

Rename config.json.example to config.json, set token to your bot token and prefix as the required bot prefix.

You can now also toggle settings related to disabling Default Help and enable/disable DMs

```
{
  "token": "{{DISCORD_TOKEN}}",
  "prefix": "!",
  "enableDms": true,
  "enableMentionPrefix": false,
  "dmHelp": false,
  "enableDefaultHelp": false
}
```

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

## How to debug your modules

To Debug you need to add the following to your CSProj file (Replace `{{PATH_TO_REPO}}` with your actual path (Eg C:\Repo\BotBase)

```

  <PropertyGroup>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>
	<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>

  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <OutputPath>{{PATH_TO_REPO}}\BotBase\bin\Debug\netcoreapp3.1\win10-x64\Plugins\</OutputPath>
    <Optimize>false</Optimize>
  </PropertyGroup>

  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|AnyCPU'">
    <OutputPath>{{PATH_TO_REPO}}\BotBase\bin\Release\netcoreapp3.1\win10-x64\Plugins\</OutputPath>
  </PropertyGroup>


```

Then in the Debug Settings you will need to set it to Launch an executable
```
{{PATH_TO_REPO}}\BotBase\bin\Debug\netcoreapp3.1\win10-x64\BotBase.exe
```

And set the Working Directory to be the location of the exe (eg)
```
{{PATH_TO_REPO}}\BotBase\bin\Debug\netcoreapp3.1\win10-x64\
```
