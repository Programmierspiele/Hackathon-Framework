Hackathon-Framework
===================

A framework that can be reused for multiple hackathons for clients as well as servers.

API-Specification
=================

You have to extend either the Server or the Client class dependent on your purpose. And pass the an instance to a Connection (for clients) or a Network (for servers). And finally clients have to call the startClientLoop-Method on the connection to get the game rolling.

###### Ruby
```ruby
class MyAI < Client
  ...
end
...
client = Network.connect(hostname, port)
connection = Connection.new(client, MyAI.new, -1)
connection.start_client_loop
connection.join
```

###### C-Sharp

```csharp
class Sample : Client
{
    static void Main(string[] args)
    {
        // Define hostname and port.
        string hostname = "localhost";
        int port = 25555;
        // Open up a tcp connection.
        TcpClient client = Network.Connect(hostname, port);
        Connection connection = new Connection(client, new Sample(), -1, true);
        // Join the connection since it's callback based.
        connection.Join();
    }
    override public void UpdateScene(List<GameObject> scene)
    { ... }
    ...
}
```

###### Java

```java
public class Sample extends Client {
    public static void main(String[] args) throws IOException, InterruptedException {
        // Define hostname and port.
        String hostname = "localhost";
        int port = 25555;
        // Open up a tcp connection.
        Socket client = new Socket(hostname, port);
        Connection connection = new Connection(client, new Sample(), -1);
        // Start the client loop.
        connection.startClientLoop();
        // Join the connection since it's callback based.
        connection.join();
    }
    @Override
    public void updateScene(List<GameObject> scene) { ... }
    ...
}
```

It's super simple just look at the complete samples, to see what methods you have to override.

Java Sample: https://github.com/penguinmenac3/Hackathon-Framework/blob/master/java-impl/src/test/java/hackathonlib/Sample.java

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
