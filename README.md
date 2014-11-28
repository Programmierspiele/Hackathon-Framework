Universal-Hackathon-Api
=======================

Try to define a minimal but just powerfull enough json protocol for common types of hackathon.

API-Specification
=================

You have to extend either the Server or the Client class dependent on your purpose. And pass the an instance to a Connection (for clients) or a Network (for servers).
```ruby
class MyAI < Client
  ...
end
...
client = Network.connect(hostname, port)
connection = Connection.new(client, MyAI.new, -1)
```

Finally clients have to call the startClientLoop-Method on the connection to get the game rolling.

```ruby
connection.start_client_loop
connection.join
```

It's super simple just look at the complete saples.

Java Sample: https://github.com/penguinmenac3/Hackathon-Framework/blob/master/java-impl/src/test/java/hackathonlib/TestProgram.java

C# Sample: https://github.com/penguinmenac3/Hackathon-Framework/blob/master/csharp-impl/HackathonNetwork/Program.cs

Ruby Sample: https://github.com/penguinmenac3/Hackathon-Framework/blob/master/ruby-impl/Sample.rb

Network-Specification
=====================

If you don't like the framework you can easily write your own client with the following network specification.

There is send one of the following JSON-Objects per line. (Java, C# and Ruby default linefeeds work corretly.)

Transfer the current gamestate.
```json
{"scene":[GAME_OBJECT]}
```

A simplified gameobject. Extra can be an array containing any simple data type like STRING, INT, DOUBLE, NULL.
```json
GAME_OBJECT = {"name":STRING, "x":DOUBLE,"y":DOUBLE,"z":DOUBLE,"rotation":[0,2*PI],"extra":[]}
```

A command that can be send from server to client or vice versa. E.g subscribe, jump, punch, ...
```json
{"command":STRING}
```

The speed at which you want to move. Positive means forwards and negative backwards.
```json
{"speed":DOUBLE}
```

The rotation that is requested, can be relative or absolute dependant on the game.
```json
{"rotation":DOUBLE}
```

Set your name. This can only be send once per connection.
```json
{"name":STRING}
```

Ping the server...
```json
{"ping":"ping"}
```

Answer a ping...
```json
{"ping":"pong"}
```
