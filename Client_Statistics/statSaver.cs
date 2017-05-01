//----------------------------------------------------------------------
// Title:   Client - Stat Saver
// Author:  LakeYS
// Version: 1
//----------------------------------------------------------------------
// Allows the client to receive and store user data from servers.
// This contains all of the 'background' client functionality, allowing
// the game to receive and store data.
//----------------------------------------------------------------------

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
function clientCmdReceiveStat(%name, %group, %data)
{
	// TODO: PARTIALLY IMPLEMENTED

	if(%name $= "" || %data $= "") // Cancel if input is blank.
		return;

	if(!%group) // If no group name, fall back to 'General'
		%group = "General";

	// Max of 255 total variables. Prevents server from flooding the client with nonsense.
	if($Stat::Count[$Stat::Server] > 255)
		return;

 $Stat::Count[$Stat::Server]++;
	$Stat::a[$Stat::Server,%group,%name] = %data;

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
		%line[%lines++] = %file.readLine();
		// Convert data to variables
	}
}

function Stat::WriteStats(%gamemode, %host)
{
	// TODO: PARTIALLY IMPLEMENTED

	%file = new FileObject();

	//Use .log format?
	%file.openForWrite("config/client/stats/" @ %gamemode @ "/" @ %host @ ".log"); //%host should be by bl-id if possible

	for(%i = 0; %i >= $Stat::Variables[%placeholderstuff])
	{

	}
}

// # Packaged
// TODO: PARTIALLY IMPLEMENTED
package Client_StatSaver
{
	// Stats are separated by game-mode and host.
  function PackagePlaceholder::joinServer(%host,%gamemode)
  {
		// Different 'games' are identified by host and game-mode name.
    //$Stat::Server = %gamemode @ "_" %host;
    //$Stat::GameMode = %gamemode;
    //$Stat::Host = %host;
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
