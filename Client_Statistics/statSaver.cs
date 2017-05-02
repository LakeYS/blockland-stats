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

// NOTE: To improve performance, this check should be performed before executing. Example:
//if($Library::StatSaver::Ver <= 1)
//	exec("./statSaver.cs");
if($Library::StatSaver::Ver >= 1)
	return;

$Library::StatSaver::Ver = 1;

// # Client-Server Communication

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
	// TODO: CLEAN TABS FROM ALL INPUT (conver to spaces?)
	// TODO: PARTIALLY IMPLEMENTED

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

// # File-Handling Functions

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
// TODO: PARTIALLY IMPLEMENTED
package Client_StatSaver
{
	// Stats are separated by game-mode and host.
  function connectToServer(%ip, %a, %b, %c)
  {
		%server = JS_ServerList.getRowText(JS_ServerList.getSelectedID());
		%serverIP = getField(%server, 9); // Get the ip+port

		if(%ip !$= %serverIP)
		{
			error("Stat Saver - Server does not match! Stats will not be logged for this session.");
			return Parent::connectToServer(%ip, %a, %b, %c);
		}

    $Stat::GameMode = getField(%server, 8);

		// Host is more complicated. Because the server list doesn't include IPs,
		// we have to use an unofficial API: https://bllist.theblackparrot.me/api/

		// TEMPORARY: The server name and host is used for now.
		// This will change before release.
		$Stat::Host = getField(%server, 2);

		$Stat::Server = $Stat::GameMode @ "_" @ $Stat::Host;

		return Parent::connectToServer(%ip, %a, %b, %c);
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
deactivatePackage("Client_StatSaver");
activatePackage("Client_StatSaver");
