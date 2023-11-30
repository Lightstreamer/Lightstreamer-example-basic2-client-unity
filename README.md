# Lightstreamer - Basic Demo - Unity Client

<!-- START DESCRIPTION lightstreamer-example-basic2-client-unity -->

New Line.
This project includes a demo client showing the integration between [Lightstreamer server](https://www.lightstreamer.com/) and the [Unity 2021 Development platform](https://unity3d.com/).<br>

## Live Demo

[![Demo ScreenShot](screen_demo_large2.png)](https://demos.lightstreamer.com/UnityDemo/BasicUnityDemo.zip)<br>

### [![](http://demos.lightstreamer.com/site/img/play.png) View live demo](http://demos.lightstreamer.com/UnityDemo2/BasicUnityDemo.zip)<br>
(for Windows systems: download BasicUnityDemo.zip; unzip it; open example-basic2-client-unity_mono directory; launch example-basic2-client-unity.exe)<br>
(for Linux systems: download BasicUnityDemo.zip; unzip it; open example-basic2-client-unity_linux_IL2CPP; launch example-basic2-client-unity.x86_64)<br>
(for Android devices: download BasicUnityDemo.zip; unzip it; install BasicUnityDemo.apk manually on your Android device)

## Details

The demo intends to show a reference pattern for using [Lightstreamer Client API](https://github.com/Lightstreamer/Lightstreamer-lib-client-haxe) in a Unity 3d project in order to add sources of real-time information to which game objects in a scene can react.
Specifically, the demo includes two `cube` 3D ojects that change their size and color according to the real-time information received through the StockList Data Adapter, the same of the [StockList demo](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript).
In addition a `3D Text` object acts as very very simple chat based on the [Round-Trip demo](https://github.com/Lightstreamer/Lightstreamer-example-RoundTrip-client-javascript). 

All the objects who want to communicate with the Lightstreamer server should be children of the same parent object, in this demo called 'World', and be tagged with a label that starts with the string 'lightstreamer'.
To 'World' has been added the `LighstreamerClientAsset` component, which will take care of opening a Lightstreamer session with the information provided in the parameters: `Push Url` and `Adapters Set`.

All the child objects can ask to open a [Subscription](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/com.lightstreamer.client.Subscription.html) versus the Lightstreamer server by setting these parameters of the `LighstreamerAsset` component: ItemName, Schema, DataAdapter.
The LightstreamerClientAsset component of the World object will perform all the subscriptions specified by child objects, and will communicate with them through two types of messages :

* `RTStatus` - a message of this type provides information about the status of communication with the Lightstreamer server.
* `RTUpdates` - this type of messages brings an [ItemUpdate](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/com.lightstreamer.client.ItemUpdate.html) object of the Lightstreamer .NET client library.

### Dig the Code

The Lightstreamer bits in this demo are located inside C# scripts in the `Assets/LS` directory.<br>

In detail the files with the source code are:
* `LighstreamerClientAsset` is the class that manages the connection to the Lightstreamer server and all the subscriptions. Redistributes received updates by broadcasting messages to all the child objects.
* `LighstreamerAsset` is a base class that should be extended by all those components who want to consume real time events from Lightstreamer server. 
* `LightstreamerCubeAsset.cs` is the Lightstreamer component, extension of `LighstreamerAsset` base class, that can be associated to a Cube object. Upon receipt of a message, 
after verifying that the involved Item is the one specified in the ItemName parameter, change the color of the cube by setting parameterized rgb values based on the value of the last_price field (I admit this does not make much sense, it's just an example), and modify the cube size (y axis only) in pase to the pct_change field value.
* `LightstreamerMsgAsset.cs` is the Lightstreamer component, extension of `LighstreamerAsset` base class, that can be associated to a 3D Text object to display the message of an Item of Round-Trip demo specified in ItemName parameter.
In addition this class read the Input and sends the typed message to the server.
* `ConnectionListener.cs` is the implementation of [ClientListener](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/com.lightstreamer.client.ClientListener.html) interface to handle notifications of connection activity and errors. 
* `TableListener.cs` is the implementation of [SubscriptionListener](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/com.lightstreamer.client.SubscriptionListener.html) interface to handle notification of data updates and subscription termination.

Check out the sources for further explanations. The Lightstreamer Documentation is available at [lightstreamer.com/doc](https://lightstreamer.com/doc)<br>

<i>NOTE: Not all the functionalities of the .Net Standard Client API for Lightstreamer are leveraged by the classes listed above. You can easily expand those functionalities using the [Lightstreamer .NET Standard API](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/Index.html) as a reference. </i>

For any inquiry, please email support@lightstreamer.com.

<!-- END DESCRIPTION lightstreamer-example-basic2-client-unity -->

## Install

If you want to install a version of this demo pointing to your local Lightstreamer Server, follow these steps:

* Note that, as prerequisite, the [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Weswit/Lightstreamer-example-Stocklist-adapter-java) and the [Lightstreamer - Round-Trip Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-RoundTrip-adapter-java) has to be deployed, together in the same Adapter Set, on your local Lightstreamer Server instance.
The folder structure of the Adapter set should look like:
``` 
LS_HOME
->\adapters
  ->\BasicUnity2
    ->\calsses
      ->log4j2.xml
    ->\lib
      ->log4j-api-2.17.1.jar
      ->log4j-core-2.17.1.jar
      ->ls-adapter-inprocess-7.3.0.jar
    ->\roundtrip
      ->\lib
        ->example-RoundTrip-adapter-java-0.0.1-SNAPSHOT.jar
    ->\Stocklist
      ->lib
        ->stocklist-adapter-java-1.0.0.jar
    ->adapters.xml
```

and adapters.xml file for the Portfolio Demo, should look like:
```xml 
<?xml version="1.0"?>

<!-- Mandatory. Define an Adapter Set and sets its unique ID. -->
<adapters_conf id="DEMO">
  
    <metadata_adapter_initialised_first>Y</metadata_adapter_initialised_first>
	
	<metadata_provider>

		<install_dir>roundtrip</install_dir>
	
        <adapter_class>roundtrip_demo.adapters.RoundTripMetadataAdapter</adapter_class>

        <!-- Optional for RoundTripMetadataAdapter.
               Configuration file for the Adapter's own logging.
               Logging is managed through log4j. -->
        <param name="log_config">adapters_log_conf.xml</param>
        <param name="log_config_refresh_seconds">10</param>

        <!-- Optional, managed by the inherited LiteralBasedProvider.
               See LiteralBasedProvider javadoc. -->
        <!--
        <param name="max_bandwidth">40</param>
        <param name="max_frequency">3</param>
        <param name="buffer_size">30</param>
        <param name="prefilter_frequency">5</param>
        <param name="allowed_users">user123,user456</param>
        <param name="distinct_snapshot_length">30</param>
        -->

        <!-- Optional, managed by the inherited LiteralBasedProvider.
               See LiteralBasedProvider javadoc. -->
        <param name="item_family_1">roundtrip\d{1,2}</param>
        <param name="modes_for_item_family_1">MERGE</param>
		
        <param name="item_family_2">item.*</param>
        <param name="modes_for_item_family_2">MERGE</param>

    </metadata_provider>

    <!-- Mandatory. Define the Data Adapter. -->
    <data_provider name="ROUNDTRIP_ADAPTER">
	  
	    <install_dir>roundtrip</install_dir>

        <adapter_class>roundtrip_demo.adapters.RoundTripDataAdapter</adapter_class>

    </data_provider>

	<!-- Mandatory. Define the Data Adapter. -->
	<data_provider name="QUOTE_ADAPTER">

	    <install_dir>Stocklist</install_dir>

		<!-- Mandatory. Java class name of the adapter. -->
		<adapter_class>com.lightstreamer.examples.stocklist_demo.adapters.StockQuotesDataAdapter</adapter_class>

    </data_provider>

</adapters_conf>
```

* Launch the Lightstreamer Server.
* Download the `deploy.zip` file, which you can find in the [latest release](https://github.com/Lightstreamer/Lightstreamer-example-basic2-client-unity/releases) of this project and extract the `example-basic2-client-unity_mono_localhost` folder.
* Launch `example-basic2-client-unity.exe`, please note that the demo tries to connect to http://localhost:8080 and a Windows system is required.


## Build

To build your own version of the demo executable, instead of using the one provided in the `deploy.zip` file from the [Install](https://github.com/Lightstreamer/Lightstreamer-example-basic2-client-unity#install) section above, follow these steps:

* A Unity 2021 Development platform must be installed to build and run this demo. Download and Install Unity 3D from: [https://unity3d.com/get-unity/download](https://unity3d.com/get-unity/download).
* Clone this project: `> git clone https://github.com/Weswit/Lightstreamer-example-basic2-client-unity`.
* Get the  binaries files of the library (`Lightstreamer.DotNetStandard.Client.Api.dll`, `Lightstreamer.DotNetStandard.Client.dll`, `Lightstreamer.DotNetStandard.Client.Haxe.dll`, and `Lightstreamer.DotNetStandard.Client.Net.dll`) and the dependency [Microsoft.AspNetCore.JsonPatch.dll](https://www.nuget.org/packages/Microsoft.AspNetCore.JsonPatch/) from NuGet [Lightstreamer.DotNetStandard.Client](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Client/) version 6 and put them in the `Assets\LS\Lightstreamer_client_library` folder; then import all of them as new Assets.
* Open `SndScene.unity` file in `Asset` subfolder double clicking on it. The Unity Development Environment should open.
* You can then build and run the project by menu `File` and then `Build & Run`.
* Please note that in the inspector of the `World` object you can choose the Lightstreamer server targeted by the demo; you can change the `Push Url` parameter to 'http://localhost:8080' or 'https://push.lightstreamer.com' depending you want to use your local instance of Lightstremaer server or our public installations.

### Mono or IL2CPP

The demo was built and tested with both Mono and IL2CPP backend scripting.
To build a project with IL2CPP, you need to have the backend installed in your Unity installation. You can change the scripting backend Unity uses to build your application through the Player Settings following theese steps:

	1. Go to Edit > Project Settings.
	2. Click on the Player Settings button to open the Player settings for the current platform in the Inspector.
	3. Navigate to the Configuration section heading under the Other Settings sub-menu.
	4. Click on the Scripting Backend dropdown menu, then select IL2CPP.

## See Also

### Lightstreamer Adapters Needed by This Demo Client

<!-- START RELATED_ENTRIES -->
* [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Weswit/Lightstreamer-example-Stocklist-adapter-java)
* [Lightstreamer - Round-Trip Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-RoundTrip-adapter-java)

<!-- END RELATED_ENTRIES -->

### Related Projects

* [Lightstreamer .NET Standard Client SDK](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Client/)
* [Lightstreamer - Stock-List Demos - HTML Clients](https://github.com/Weswit/Lightstreamer-example-Stocklist-client-javascript)
* [Lightstreamer - Round-Trip Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-RoundTrip-client-javascript)
* [Lightstreamer - 3D World Demo - HTML (Three.js) Client](https://github.com/Weswit/Lightstreamer-example-3DWorld-client-javascript)

## Lightstreamer Compatibility Notes

* Compatible with Lightstreamer .NET Standard Client Library version 6.
* For Lightstreamer Server version 7.3.2 or newer. Ensure that .NET Standard Client API is supported by Lightstreamer Server license configuration.
* For instructions compatible with .NET Standard Client library version 5.x, please refer to [this tag](https://github.com/Lightstreamer/Lightstreamer-example-basic2-client-unity/tree/for_client_5.x).
