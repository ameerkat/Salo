Salo is the beginning of a framework written in C# that allows bots to play a game similar to [Neptune's Pride 2: Triton](http://triton.ironhelmet.com). Salo is named after the robotic being in the book "The Siren's of Titan".
## ISaloBot
ISaloBot is the primary concern for bot writers. ISaloBot contains two methods, one is to set the player data and action handler that the bot will be acting as and the other is a continual Run method that gets called for each tick that lets the bot know the game state has been updated and to take actions. There are three action handlers, one for in process games, one for simulator server games, and one for live games. Each one will be set by the program running the bot, this should be transparent to the bot as all are IActionHandler. Take a look at RandomBot for an example of how to implement a bot.
## Salo Map Generator
Salo map generator generates maps similar to the maps in the online game which get used by the simulator below. Currently only random hex configurations are allowed. Some more work is required to get maps that are balanced, see distance/scale params in MapGenerator.cs. Random maps are generated in favor of a fixed map pool in order to prevent bots that overfit to the map. Map generator includes a console application that will probable be merged with the simulator application when a full fledged console application is created.
## Salo Simulator
Salo simulator attempts to emulate the workings of the actual game Neptune's Pride 2: Triton, allowing bots to interact with one another in the same way that humans interact with one another. Currently the simulator contains a main game loop, which processes production events, industry, fleet position, research, and conflict. IActionHandlers allow the bot (or player via a program) to interact with the simulation e.g. set research, determine visbility and range, upgrade planets etc. Salo simulator is configured through Configuration.ini allowing the user to set the tick rate, production cycle rate, etc. (e.g. you can set games to run at one production a second, much quicker than the online version which is more practical for testing out bots). Salo simulator will eventually become a console application that runs either the in process game or the simulation game server, allowing an eventual bot client to connect to it (or to the live server) with little change. Additional parameter balance work is required to make the simulator behave like the actual game with regards to ship speed, pricing for upgrades, vision range, etc.
