Salo is the beginning of a framework that allows bots to play a game similar to [Neptune's Pride 2: Triton](http://triton.ironhelmet.com).
## Salo
The Salo project itsef is the framework that lets the bot act. Salo itself is currently empty.
## Salo Map Generator
Salo map generator generates maps similar to the maps in the online game, currently only random hex configurations are allowed. Some more work is required to get maps that are balanced, see distance/scale params in MapGenerator.cs
## Salo Simulator
Salo simulator attempts to emulate the workings of the actual game, allowing bots to interact with one another in the same way that humans interact with one another in the actual game. Currently the simulator contains a main game loop, but is lacking methods which allow bots to take actions within the simulator (e.g. purchasing planet upgrades, settings research, diplomatic actions, etc.). Salo simulator is configured through Configuration.ini allowing the user to set the tick rate, production cycle rate, etc. (e.g. you can set games to run at 1 production a second, much quicker than the online version which is more practical for testing out bots)
