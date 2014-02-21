Salo is the beginning of a framework that allows bots to play a game similar to [Neptune's Pride 2: Triton](http://triton.ironhelmet.com).
## Salo
The Salo project itsef is the framework that lets the bot act. Salo itself is currently empty.
## Salo Map Generator
Salo map generator generates maps similar to the maps in the online game which get used by the simulator below. Currently only random hex configurations are allowed. Some more work is required to get maps that are balanced, see distance/scale params in MapGenerator.cs. Random maps are generated in favor of a fixed map pool in order to prevent bots that overfit to the map.
## Triton Simulator
Salo simulator attempts to emulate the workings of the actual game, allowing bots to interact with one another in the same way that humans interact with one another. Currently the simulator contains a main game loop, which processes production events, industry, fleet position, research, and conflict. The simulator is currently lacking methods which allow bots to take actions within the simulator (e.g. purchasing planet upgrades, settings research, diplomatic actions, etc.). Salo simulator is configured through Configuration.ini allowing the user to set the tick rate, production cycle rate, etc. (e.g. you can set games to run at one production a second, much quicker than the online version which is more practical for testing out bots). Parameter balance work is required to make the simulator behave like the actual game with regards to ship speed, pricing for upgrades, vision range, etc.
