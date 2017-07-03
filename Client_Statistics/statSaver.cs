//----------------------------------------------------------------------
// Title:   Client - Stat Saver
// Author:  LakeYS
// Version: 1
//----------------------------------------------------------------------
// Allows the client to receive and store user data from servers.
// This contains all of the 'background' client functionality, allowing
// the game to receive and store data.
//----------------------------------------------------------------------
// NOTE: Currently in development; this script is non-functional.

////// # Client-Server Communication

// clientCmdReceiveStat(name, group, data);
// Set a stat that will be saved on the client side.
// This data is for the user's reference ONLY; it cannot be read back by the server.
/// %name: Name of the variable/stat to modify.
/// %group: Optional. Group that the specified stat should be placed under.
/// %data: Data to set under %name
////// Unimplemented standards (For future-proofing):
////// - Multiple columns on one line should be separated by a semicolon and a space, like "dataA; dataB"
////// - To set a group's header, use "_header" as the name, then use the aforementioned "A; B" format.
////// - To mark a group as a leaderboard, set "_leaderboard" to 1. This will allow the user to see a 'global' leaderboard of all the servers they've joined.
////// - Any variables starting with "_" will be ignored as a reservation for special tags.
////// - Beware: %name and %group will have character limits for the GUI. Stick to reasonably short titles.
//TO DO: Need a way to 'archive' old values. Consider making a function for renaming groups.
//TO DO: CHECK FOR BLANK INPUT
//TO DO: Tailored achievement support.
//TO DO: Find a way to support exponential level scales (for servers with a levelling system)
function clientCmdReceiveStat(%name, %group, %type, %data, %string)
{
	// TODO: CLEAN TABS FROM ALL INPUT (convert to spaces?)

	if(%name $= "" || %data $= "") // Cancel if input is blank.
		return;

	if(!%group) // If no group name, fall back to 'General'
		%group = "General";

	// Max of 255 total variables. Prevents server from flooding the client with nonsense.
	if($Stat::Count[$Stat::Server] > 255)
		return;

 	$Stat::c[$Stat::Server,$Stat::GameMode]++;

	$Stat::a[$Stat::Server,%group,%name] = %data;
	$Stat::str[$Stat::Server,%group,%name] = %string;

	$Stat::b[%Stat::Count] = %name TAB %group TAB %type TAB %data TAB %string;

	echo("Received stat; Group: " @ %group @ "; Name:" @ %name @ "; Data: '" @ %data @ "'");
}

function clientCmdRenameStatGroup(%old, %new)
{
	// TODO: NOT IMPLEMENTED
}

function clientCmdDeleteStat(%name)
{
	// TODO: NOT IMPLEMENTED
}

function clientCmdDeleteStatGroup(%group)
{
	// TODO: NOT IMPLEMENTED
}

////// # Data Handling
function Stat::CleanUp()
{
	// TODO: NOT IMPLEMENTED
}

////// # File Handling
function Stat::ReadStats(%gamemode, %host)
{
	// TODO: PARTIALLY IMPLEMENTED

	%file = new FileObject();

	//Use .log format?
	%file.openForRead("config/client/stats/" @ %gamemode @ "/" @ %host @ ".log"); //%host should be by bl-id if possible

	while(!%file.isEOF())
	{
		// TODO: Make sure lines start at 1 instead of 0
		%line[%lines++] = %file.readLine();

		%name = getField(%line,0);
		%group = getField(%line,1);

		$Stat::a[$Stat::Server,%group,%name] = %data;
		$Stat::str[$Stat::Server,%group,%name] = %string;

		$Stat::b[%line] = %line;
	}

	$Stat::c = %lines;
}

// Some notes:
// We'll need to strip non-file-friendly characters from a host's name.
// If a host's name consists entirely of special characters, we'll have to throw a warning and cancel logging.
// stripChars(%str, "\/:*?<>|\"")

function Stat::WriteStats(%gamemode, %host)
{
	// TODO: PARTIALLY IMPLEMENTED

	%file = new FileObject();

	%file.openForWrite("config/client/stats/" @ %gamemode @ "/" @ %host @ ".log"); //%host should be by bl-id if possible

	// TODO: Make sure %i should start at 0
	for(%i = 0; %i >= $Stat::Variables; %i++)
	{
		%file.writeLine(
			$Stat::vName[%i]
			TAB $Stat::vGroup[%i]
			TAB $Stat::vType[%i]
			TAB $Stat::vData[%i]
			TAB $Stat::vString[%i]
		);
	}
}

// # Packaged
deactivatePackage("StatSaver");
package StatSaver
{
	// The function connectToServer is used to determine the current server.
	// Stats are separated by game-mode and host.
  function connectToServer(%ip, %a, %b, %c)
  {
		%server = JS_ServerList.getRowTextByID(JS_ServerList.getSelectedID());
		%serverIP = getField(%server, 9); // Get the ip+port

		//echo(%ip SPC %serverIP);
		if(%ip !$= %serverIP)
		{
			error("Stat Saver - Server does not match! Stats will not be logged for this session.");
			return Parent::connectToServer(%ip, %a, %b, %c);
		}

		$Stat::NextGameMode = getField(%server, 8);

		// All data is tied to a server's host username and game-mode.
		// Note that having a "s' " or "s' " in a username can interfere with this check.
		%field = getField(%server, 2);
		%pos = strPos(%field, "'s");

		// If there is no "'s" then try "s'"
		if(%pos == -1)
			%pos = strpos(%field, "s'");

		// If there's still nothing, whoops.
		if(%pos == -1)
		{
			error("Stat Saver - Invalid host field! Stats will not be logged for this session.");
			return Parent::connectToServer(%ip, %a, %b, %c);
		}

		$Stat::NextHost = getSubStr(%field, 0, %pos);
		$Stat::NextServer = $Stat::GameMode @ "_" @ $Stat::Host;

		$Stat::NextIP = %serverIP;

		return Parent::connectToServer(%ip, %a, %b, %c);
  }

	function GameConnection::onConnectionAccepted(%connection)
	{
		if($Stat::NextIP !$= %connection.getAddress())
		{
			error("Stat Saver - Server's IP does not match connection! Stats will not be logged for this session.");
			return Parent::onConnectionAccepted(%a);
		}

		if($Stat::Connected == 1)
		{
			error("Stat Saver - Failed to disconnect from previous server. Cleaning up now...");
			Stat::CleanUp();
		}

		$Stat::Connected = 1;

		$Stat::Host = $Stat::NextHost;
		$Stat::Server = $Stat::NextServer;
		$Stat::GameMode = $Stat::NextGameMode;

		return Parent::onConnectionAccepted(%a);
	}

	function disconnectedCleanup(%a)
	{
		if($Stat::Connected == 0)
		{
			error("Stat Saver - Disconnected from uninitialized server! No data has been exported.");
			Parent::disconnectedCleanup(%a);
		}
		$Stat::Connected = 0;

		Parent::disconnectedCleanup(%a);
	}

	function onServerCreated()
	{
		if($Stat::Connected == 1)
		{
			error("Stat Saver - Failed to disconnect from previous server. Cleaning up now...");
			Stat::CleanUp();
		}
		$Stat::Connected = 1;

		// TODO: Fill host info when server is started.

		Parent::onServerCreated();
	}

	// Record kills
	function PackagePlaceholder::onKillPlayer()
	{
		// To do: Record this as a stat
	}

	// Record Deaths
	function PackagePlaceholder::onDeath()
	{
		// To do: Record this as a stat
	}
};
activatePackage("StatSaver");
