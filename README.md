# Lightstreamer - Basic Demo - Unity Client

<!-- START DESCRIPTION lightstreamer-example-basic2-client-unity -->

This project includes a demo client showing the integration between [Lightstreamer server](https://www.lightstreamer.com/) and the [Unity 6](https://unity.com/).<br>

## Live Demo

[![Demo ScreenShot](screen_demo_large2.png)](https://demos.lightstreamer.com/UnityDemo/BasicUnityDemo.zip)<br>

### [![](http://demos.lightstreamer.com/site/img/play.png) View live demo](http://demos.lightstreamer.com/UnityDemo2/BasicUnityDemo.zip)<br>
(for Windows systems: download BasicUnityDemo.zip; unzip it; open example-basic2-client-unity_mono directory; launch example-basic2-client-unity.exe)<br>
(for Linux systems: download BasicUnityDemo.zip; unzip it; open example-basic2-client-unity_linux_IL2CPP; launch example-basic2-client-unity.x86_64)<br>


## Details

The demo is designed to illustrate a reference pattern for using the [Lightstreamer Client API](https://github.com/Lightstreamer/Lightstreamer-lib-client-haxe) within a Unity 3D project, in order to add real-time data sources that game objects in a scene can react to.
Specifically, the demo features three 3D `cube` objects that change their size and color based on the real-time updates received through the StockList Data Adapter, the same one used in our well-known [StockList demo](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript).
In addition, a few `TextMeshPro` objects are used as labels, with some of them being updated in real time.
Alongside stock list data, the demo also displays an Item from the [Round-Trip demo](https://github.com/Lightstreamer/Lightstreamer-example-RoundTrip-client-javascript), used here to demonstrate instant messaging functionality.

All the objects who want to communicate with the Lightstreamer server should be children of the same parent object, in this demo called 'World', and be tagged with a label that starts with the string 'lightstreamer'.
To 'World' has been added the `LighstreamerClientAsset` component, which will take care of opening a Lightstreamer session with the information provided in the parameters: `Push Url` and `Adapters Set`.

All the child objects can ask to open a [Subscription](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/com.lightstreamer.client.Subscription.html) versus the Lightstreamer server by setting these parameters of the `LighstreamerAsset` component: ItemName, Schema, DataAdapter.
The LightstreamerClientAsset component of the World object will perform all the subscriptions specified by child objects, and will communicate with them through two types of messages :

* `RTStatus` - a message of this type provides information about the status of communication with the Lightstreamer server.
* `RTUpdates` - this type of messages brings an [ItemUpdate](https://lightstreamer.com/api/ls-dotnetstandard-client/latest/api/com.lightstreamer.client.ItemUpdate.html) object of the Lightstreamer .NET client library.

### Dig the Code

The Lightstreamer components used in this demo are implemented as C# scripts located in the `Assets/LS` directory.  
The source files include:  

* **LightstreamerClientAsset.cs** – Manages the connection to the Lightstreamer Server and all subscriptions. It redistributes received updates by broadcasting messages to all child objects.  

* **LightstreamerAsset.cs** – A base class that should be extended by any component that consumes real-time events from the Lightstreamer Server.  

* **LightstreamerCubeAsset.cs** – A Lightstreamer component, extending the `LightstreamerAsset` base class, that can be associated with a Cube object. Upon receiving a message, it verifies that the Item matches the one specified in the `ItemName` parameter, then changes the cube’s color (green for positive values and red for negative ones, with intensity reflecting the magnitude of the value) and adjusts the cube’s size (y-axis only) based on the `pct_change` field value.  

* **LightstreamerLabelAsset.cs** – A Lightstreamer component, extending the `LightstreamerAsset` base class, that can be associated with a `TextMeshPro` object to display the `pct_change` field value.  

* **LightstreamerMsgAsset.cs** – A Lightstreamer component, extending the `LightstreamerAsset` base class, that can be associated with a `TextMeshPro` object to display the message of an Item from the Round-Trip demo specified in the `ItemName` parameter. In addition, this class reads user input and sends the typed message to the server.  

* **ConnectionListener.cs** – An implementation of the `ClientListener` interface that handles connection activity and error notifications.  

* **RTQuoteListener.cs** – An implementation of the `SubscriptionListener` interface that handles data updates and subscription termination notifications.  

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

* A Unity 6 Development platform must be installed to build and run this demo. Download and Install Unity 3D from: [https://unity3d.com/get-unity/download](https://unity3d.com/get-unity/download).
* Clone this project: `> git clone https://github.com/Weswit/Lightstreamer-example-basic2-client-unity`.
* Install the Lightstreamer client library. You have two options:

  ### 1. Manual installation of DLLs
  - Download the Lightstreamer .NET Standard Client binaries from [NuGet](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Client/) (version 6).  
  - You will need the following files:  
    - `Lightstreamer.DotNetStandard.Client.Api.dll`  
    - `Lightstreamer.DotNetStandard.Client.dll`  
    - `Lightstreamer.DotNetStandard.Client.Haxe.dll`  
    - `Lightstreamer.DotNetStandard.Client.Net.dll`  
    - plus the dependency [Microsoft.AspNetCore.JsonPatch.dll](https://www.nuget.org/packages/Microsoft.AspNetCore.JsonPatch/)  
  - Copy all these DLLs into the folder `Assets\LS\Lightstreamer_client_library`.  
  - In Unity, import them as new Assets (drag & drop into the **Project** view or via `Assets → Import New Asset...`).

  ### 2. Using NuGet for Unity
  - Download and import the [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) package:
    1. Get the latest `.unitypackage` release from [NuGetForUnity Releases](https://github.com/GlitchEnzo/NuGetForUnity/releases).  
    2. In Unity, go to `Assets → Import Package → Custom Package...` and select the downloaded file.  
    3. Confirm the import.
  - After installation, a new menu `NuGet` will appear in Unity.  
  - Go to `NuGet → Manage NuGet Packages`.  
  - Search for **Lightstreamer.DotNetStandard.Client** and select the latest version (**6.2.1**).  
  - Click **Install**.  
  - NuGetForUnity will automatically download the required DLLs into a `Packages` folder inside your Unity project (e.g., `Packages/Lightstreamer.DotNetStandard.Client/lib/netstandard2.0/`).  
  - Verify in the **Project** view that Unity recognizes the assemblies.
  
* Open `SampleScene.unity` file in `Asset` subfolder double clicking on it. The Unity Development Environment should open.
* You can then build and run the project by menu `File` and then `Build & Run`.
* Please note that in the inspector of the `World` object you can choose the Lightstreamer server targeted by the demo; you can change the `Push Url` parameter to 'http://localhost:8080' or 'https://push.lightstreamer.com' depending you want to use your local instance of Lightstremaer server or our public installations.

### Mono or IL2CPP

The demo was built and tested with both Mono and IL2CPP backend scripting.
To build a project with IL2CPP, you need to have the backend installed in your Unity installation. You can change the scripting backend Unity uses to build your application through the Player Settings following theese steps:

	1. Go to Edit > Project Settings.
	2. Navigate to the Configuration section heading under the Other Settings sub-menu.
	3. Click on the Scripting Backend dropdown menu, then select IL2CPP.

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
