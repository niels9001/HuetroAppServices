Huetro App Services [PREVIEW]
=========

With Huetro 7.1, other Windows 10 apps can leverage the power of Huetro to integrate Hue lighting effects into their own apps. With the App Service other apps can send simple LightCommands through Huetro to control Philips Hue lights. You only need a couple lines of code and Huetro will do all the heavy lifting (e.g. authentication, distributing lightcommands!).

## How to use?
Some basic usage examples

### Opening the AppService connection
Before you can send LightCommands to Huetro, a connection need to be opened. you can communicate with the Philips Hue Bridge, you need to find the bridge and register your application:

```cs
  AppServiceConnection huetroService = new AppServiceConnection();
  huetroService.AppServiceName = "com.huetro.appservice";
  huetroService.PackageFamilyName = "27078NielsLaute.HuetroforHue_91se88q2mhfz2";
  
  await huetroService.OpenAsync();
```
	

### List current lights
To control the lights, you need to send a specific commands with a list of lightbulb IDs that you want to target. To do this, Huetro recommends to show the user a list of light bulbs that can be selected.

To do this, the following command can be used to get the list of connected lights of the Bridge that is currently selected in Huetro.

```cs
  ValueSet Message = new ValueSet();
  Message.Add("Command", "GetLights");
  AppServiceResponse response = await huetroService.SendMessageAsync(Message);
```
This will return a ValueSet where each Key is the light ID and the Value is the name of the light (to display to the user).


### Sending light commands
Once you have a list of selected IDs you can start sending requests. Please note that you do not send more than ~10 light commands per seconds, to not overload the Hue Bridge.

To send light commands, we'll have to use the same ValueSet objects to communicate. However, for the commands we'll use the excellent Q42.HueApi LightCommands that provide a nice typed way to set up commands.

Turning on a blue light:
```cs
	//Turn the light on and set the color to Blue
  new LightCommand().TurnOn().SetColor(new RGBColor(0, 0, 255));
```

Set color or brightness
```cs
	//Turn light red
  new LightCommand().SetColor(new RGBColor("ff0000"));
	
	//Turn the light to full Brightness (0-255)
  new LightCommand() { Brightness = 255 }
```

Turn off (with a transition time of 3 seconds)
```cs
   new LightCommand() { On = false, TransitionTime = new TimeSpan(0, 0, 3));
```


Once you have this LightCommand we need to serialize it (since AppService communication only allows for strings) and we can then send the command to Huetro:



```cs
  ValueSet Message = new ValueSet();
  Message.Add("Command", "SendLightCommand");
  Message.Add("LightCommand", JsonConvert.SerializeObject(new LightCommand().TurnOn(), Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
  Message.Add("LightIDs", JsonConvert.SerializeObject(new List<string>() { "1", "2" }, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
  AppServiceResponse response = await huetroService.SendMessageAsync(Message);
```


### Open Source Project Credits

* Q42.HueApi is used for LightCommands
* Newtonsoft.Json is used for object serialization
