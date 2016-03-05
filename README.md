# Gridwalls
A real time strategy multiplayer game created for learning DDD, micro services, event sourcing, CQRS and cloud related stuff.

What's what in the current code?

* Game runs with mods. Every mod can be run within a mod runner, so every mod can in effect run as a micro service. The mod runner can run several mods, though. Every mod contains logic for its specific game logic.
* Other than that, see headlines below.

## Communication

Contains the basic needs for network communication, which is connecting, sending messages, broadcasting messages to all connected clients, and disconnect. The network protocol is TCP.

Both the server and client are very simple to use.

The client is written in .NET 3.5 to be able to use it from Unity. The server is written in .NET 4.6. I have googled a bit without finding another simple network library for .NET 3.5. I tried the Lidgren library, but even though it's not too hard, it was still way too complex for my needs.

You also need the Logging solution for this to work.

## Logging

A simple logging library. Suited for my specific needs, which is needing to log in a consistent way to files.