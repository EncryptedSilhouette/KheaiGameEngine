This is a personal project I have been working on. I wanted to create my own game engine in order to learn game development and computer science. 
This engine is not intended to be used by anyone outside myself. However, anyone who wishes to use it may as long as its for non-malicious and legal purposes.
My design for this engine to provide a simple framework for a 2d application. It's meant to be modular, so that others may use their own implementation.

Some of the major design concepts I wish to achive with this engine are as follows:
The engine has 2 update stages: The first stage is the logic update for each IKEngineObject. The second stage is the frame update that processes any pre-draw logic.
There is no delta-time variable or equivilent. I intend for the engine to be tick based for consistency and simplicity. That said, the framerate is capped by the update rate. 
The engine prioritizes the logic update. If the engine is lagging behind it will loop the logic update until it is caught up.
