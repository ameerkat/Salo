Salo is the beginning of a framework written in C# that allows bots to play a game similar to [Neptune's Pride 2: Triton](http://triton.ironhelmet.com). Salo is named after the robotic being in the book "The Siren's of Titan".
## Implementing your own bot with ISaloBot
If you want to write a bot to run on the Salo simulator (or potentially elsewhere) then you need to implement ISaloBot. ISaloBot contains two methods, one is to set the player data and action handler that the bot will be acting as and the other is a continual Run method that gets called for each tick that lets the bot know the game state has been updated and to take actions. There are three action handlers, one for in process games, one for simulator server games, and one for live games. Each one will be set by the program running the bot, this should be transparent to the bot as all are IActionHandler. Take a look at RandomBot for an example of how to implement a bot. The bot client running the bot will be in charge of managing the timing and ensuring the Run method is called at the beginning of each tick. Be sure to tag your bot class with BotName and BotDescription. This is used by the Web interface to inventory bots, if you don't tag your bot with the attributes then the class name and the assembly version will be used instead.
### Example
	[BotName("My Bot", "1.0")]
    [BotDescription("My bot has a super awesome strategy that will beat all the other bots!")]
    public class MyBot : ISaloBot
    { ... }
## About Salo Simulator
### Salo Simulator
Salo simulator attempts to emulate the workings of the actual game Neptune's Pride 2: Triton, allowing bots to interact with one another in the same way that humans interact with one another. Currently the simulator contains a main game loop, which processes production events, industry, fleet position, research, and conflict. IActionHandlers allow the bot (or player via a program) to interact with the simulation e.g. set research, determine visbility and range, upgrade planets etc. Salo simulator is configured through Configuration.ini allowing the user to set the tick rate, production cycle rate, etc. (e.g. you can set games to run at one production a second, much quicker than the online version which is more practical for testing out bots). Salo simulator will eventually become a console application that runs either the in process game or the simulation game server, allowing an eventual bot client to connect to it (or to the live server) with little change. Additional parameter balance work is required to make the simulator behave like the actual game with regards to ship speed, pricing for upgrades, vision range, etc.
#### Web Site
Salo simulator has the beginnings of a web interface that allows players to create matches that consist of 1 or more games between bots. These bots can be either running on the server (the quickest way) or running from a client runner that runs the bot and relay the actions. Bot match history and statistics can be viewed, allowing users to drill into particular games step by step.
### Salo Map Generator
Salo map generator generates maps similar to the maps in the online game which get used by the simulator below. Currently only random hex configurations are allowed. Some more work is required to get maps that are balanced, see distance/scale params in MapGenerator.cs. Random maps are generated in favor of a fixed map pool in order to prevent bots that overfit to the map. Map generator includes a console application that will probable be merged with the simulator application when a full fledged console application is created.
## TODO
* Refactor the game simulator into seperate class (currently in Main() of Simulator project)
* Refactor the actions to make more things client side, using game state
	* Add whatever settings are required to game to get more calculations client side
* Add game support to web interface
* Move over actions from Web to Web2 and remove Web
* Add web based map viewer (pre-requisite to game log viewer)
* Add game log/history viewer
