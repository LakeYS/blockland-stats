# Blockland Game Statistics
[Project Summary]

# Work-in-Progress Notice
**Do not develop using this documentation.** This currently serves as an outline for the project's feature set. It is very much subject to change.

# Developer Documentation
The Game Statistics client allows server-side add-ons to send user data to clients.
This allows users to view their progress offline and see their overall stats across multiple servers.

All data saved with this is for the user's viewing ONLY and cannot be read back by the server. Server-side data storage needs to be done separately.

# Sending Data to Clients
Data is sent to a user's game using a client command called `clientCmdReceiveStat`.
This command is called using commandToClient, like this: `commandToClient(%client, 'ReceiveStat', "Name", "Group", "Data Type", Data, "String");`
Data is stored and displayed in the order it is sent, so it needs to be initialized in the desired display order.
## Parameters
These are the parameters used by the function:
### Name
The name of the variable you want to set. This will be displayed in the GUI.
This is used to identify your variable within the game-mode.
### Group
The group that the variable will be displayed under in the GUI. If '0' or blank, this defaults to "General".
Multiple variables can use the same name as long as they are in different groups.
### Type
The type of data to be saved. This determines how the variable is organized. See 'Data Types' below.
### Data
This is the actual value. For organization purposes, this must be in numeric form, even if it is to be replaced with a string.
### String (Optional)
If specified, will display in place of the 'data' parameter in the GUI.
For example, with a boolean variable, you probably want to show something like "Yes" instead of "1".

## Data Types
- Additive: A number that increases over time. Examples: Kills, deaths, rounds won/lost
- High: The highest overall value attained for this stat. Example: High score (Works for true/false (1/0) values)
- Low: The lowest overall value attained for this stat. Example: High score (...if you're playing golf) (Works for true/false (1/0) values)
- Achievement: Same as 'high', but displays differently in the interface. Intended for boolean values. Use this to represent unlockables, achievements, etc. See 'Init Handshake'

More data types may be added in the future. If an unrecognized data type is detected, the variable will save but it won't display in the interface.

## Deleting Data and Renaming Sections
When resetting all user data, consider renaming the old groups instead of removing them entirely. This way, users can still view their old data.
The following function renames a category:
`[ Not yet implemented ]`

If all else fails, data can be entirely removed using the following functions:
Remove a single variable: `[ Not yet implemented ]`
Remove an entire section: `[]`

# Init Handshake
It is best practice to initialize all of your data. If data is not initialize

Clients display data in the order it was received.

The best way to ensure data is displayed in the desired order is to initialize it.

Data can be initialized by packaging the command: `serverCmdStatHandshake(init,a,b,c)`
Clients send this handshake each time they join, setting `%init` to 1 if they have never received data from the server. %a, %b, and %c are currently unused; make sure to pass them through in packages for future use.
Package this to send initial data such as locked achievements, or to generally ensure everything displays in the desired order.

Beware: Users can manually trigger this command.
