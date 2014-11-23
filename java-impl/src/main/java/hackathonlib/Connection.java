package hackathonlib;

import com.google.gson.*;
import jdk.nashorn.api.scripting.JSObject;

import java.io.*;
import java.net.Socket;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.List;

/**
 * Models a connection.
 * @author Michael Fürst
 * @version 1.0
 * @since 23.11.2014
 */
public class Connection implements Runnable {
    private String name;

    // Stop-Flag
    private boolean stop = false;
    // Flag für "Thread läuft"
    private boolean running = false;
    // Die Verbindung zum Client
    private Socket connection = null;

    // Der Ausgabestream
    private PrintWriter outStream;
    // Der Eingabestream
    private BufferedReader inStream;
    // Der Spielkoordinator
    private IGameManager gameManager;
    // The worker thread.
    private Thread thread;

    /**
     * Create a connection.
     * @param connection The socket used for the connection.
     * @param gameManager The gamemanager used for control.
     * @param receiveTimeout The receive timeout. (smaller or equal to 0 means system default)
     */
    public Connection(Socket connection, IGameManager gameManager, int receiveTimeout) {
        this.name = "none";
        this.gameManager = gameManager;
        this.connection = connection;

        // Setze das receive timeout. Für server wichtig...
        if (receiveTimeout > 0)
        {
            try {
                connection.setSoTimeout(receiveTimeout);
            } catch (SocketException e) {
                e.printStackTrace();
            }
        }


        // Hole den Stream für's schreiben
        try {
            outStream = new PrintWriter(connection.getOutputStream(), true);;
            inStream = new BufferedReader(new InputStreamReader(connection.getInputStream()));
        } catch (Exception e) {
            e.printStackTrace();
            outStream.close();
            try {
                inStream.close();
            } catch (IOException e1) {
                e1.printStackTrace();
            }
            return;
        }

        // Initialisiert und startet den Thread
        thread = new Thread(this);
        thread.start();
    }

    /**
     * Start the client loop. Only call this client side.
     */
    public void startClientLoop() {
        write("{\"ping\":\"ping\"}");
    }

    @Override
    public void run() {
        // Setze Flag für "Thread läuft"
        this.running = true;
        boolean loop = true;
        gameManager.connectionStarted(this);
        JsonParser parser = new JsonParser();

        while (loop)
        {
            try
            {
                Thread.sleep(16);
                String result = handle(parser.parse(read()).getAsJsonObject());
                write(result);

                // Wiederhole die Schleife so lange bis von außen der Stopwunsch kommt
                loop = !stop;
            }
            catch (Exception e)
            {
                // oder bis ein Fehler aufgetreten ist
                loop = false;
            }
        }
        gameManager.connectionLost(this);

        // Schließe die Verbindung zum Client
        try {
            outStream.close();
            inStream.close();
            this.connection.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        // Setze das Flag "Thread läuft" zurück
        this.running = false;
    }

    private String read() throws IOException {
        // Zeilenweise lesen von der Eingabe
        return inStream.readLine();
    }

    private void write(String result) {
        outStream.println(result);
    }

    public void quit() {
        stop = true;
    }

    public void join() throws InterruptedException {
        thread.join();
    }

    private String handle(JsonObject jsonObject)
    {
        if (jsonObject.has("name"))
        {
            String tmp = jsonObject.get("name").getAsString();
            if (name == "none" && tmp != "none")
            {
                name = tmp;
                gameManager.connectionReady(this);
            }
        }
        if (jsonObject.has("command"))
        {
            gameManager.executeCommand(this, jsonObject.get("command").getAsString());
        }
        if (jsonObject.has("speed"))
        {
            gameManager.changeSpeed(this, jsonObject.get("speed").getAsDouble());
        }
        if (jsonObject.has("rotation"))
        {
            gameManager.changeRotation(this, jsonObject.get("rotation").getAsDouble());
        }
        if (jsonObject.has("scene"))
        {
            // Calculate a scene object.
            List<GameObject> scene = new ArrayList<GameObject>();
            JsonArray array = jsonObject.get("scene").getAsJsonArray();
            for (int i = 0; i < array.size(); i++) {
                JsonObject obj = array.get(i).getAsJsonObject();
                // Not sure if this works...
                List<JsonElement> extra = new ArrayList<>();
                JsonArray extraArray = obj.get("extra").getAsJsonArray();
                for (int j = 0; j < extraArray.size(); j++) {
                    extra.add(extraArray.get(i));
                }
                // Add the object to the scene.
                scene.add(new GameObject(obj.get("name").getAsString(), obj.get("x").getAsDouble(),
                        obj.get("y").getAsDouble(), obj.get("z").getAsDouble(), obj.get("rotation").getAsDouble(), extra));
            }
            gameManager.updateScene(scene);
        }

        String result = "{";

        boolean topSeparatorRequired = false;

        if (jsonObject.has("ping") && jsonObject.get("ping").getAsString().equals("ping"))
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            result += "\"ping\":\"pong\"";
        }

        if (jsonObject.has("ping") && gameManager.doPing())
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            result += "\"ping\":\"ping\"";
        }

        String command = gameManager.getCommand();
        if (command != null)
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            result += "\"command\":\"" + command + "\"";
        }

        if (gameManager.getName() != null && name == "none" && "none" != gameManager.getName())
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            name = gameManager.getName();
            result += "\"name\":\"" + gameManager.getName() + "\"";
        }

        Double speed = gameManager.getSpeed();
        if (speed != null)
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            result += "\"speed\":\"" + speed + "\"";
        }

        Double rotation = gameManager.getRotation();
        if (rotation != null)
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            result += "\"rotation\":\"" + rotation + "\"";
        }

        List<GameObject> outScene = gameManager.getScene();
        if (outScene != null)
        {
            if (topSeparatorRequired)
            {
                result += ",";
            }
            else
            {
                topSeparatorRequired = true;
            }
            result += "\"scene\":[";
            boolean separatorRequired = false;

            for (int i = 0; i < outScene.size(); i++) {
                GameObject obj = outScene.get(i);
                if (separatorRequired) {
                    result += ",";
                } else {
                    separatorRequired = true;
                }
                result += "{";
                result += "\"name\":\"" + obj.name + "\",";
                result += "\"x\":\"" + obj.x + "\",";
                result += "\"y\":\"" + obj.y + "\",";
                result += "\"z\":\"" + obj.z + "\",";
                result += "\"rotation\":\"" + obj.rotation + "\",";
                result += "\"extra\":[";

                boolean subSeparatorRequired = false;
                for (int j = 0; j < obj.extra.size(); j++) {
                    JsonElement ext = obj.extra.get(i);
                    if (subSeparatorRequired)
                    {
                        result += ",";
                    }
                    else
                    {
                        subSeparatorRequired = true;
                    }
                    result += ext.toString();
                }

                result += "]";
                result += "}";
            }

            result += "]";
        }

        return result + "}";
    }
}
