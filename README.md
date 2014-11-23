Universal-Hackathon-Api
=======================

Try to define a minimal but just powerfull enough json protocol for common types of hackathon.


Specification
=============

Transfer the current gamestate.
> {"scene":[GAME_OBJECT]}

A simplified gameobject. Extra can be an array containing any simple data type like STRING, INT, DOUBLE, NULL.
> GAME_OBJECT = {"name":STRING, "x":DOUBLE,"y":DOUBLE,"z":DOUBLE,"rotation":[0,2*PI],"extra":[]}

A command that can be send from server to client or vice versa. E.g subscribe, jump, punch, ...
> {"command":STRING}

The speed at which you want to move. Positive means forwards and negative backwards.
> {"speed":DOUBLE}

The rotation that is requested, can be relative or absolute dependant on the game.
> {"rotation":DOUBLE}

Set your name. This can only be send once per connection.
> {"name":STRING}

Ping the server...
> {"ping":"ping"}

Answer a ping...
> {"ping":"pong"}
