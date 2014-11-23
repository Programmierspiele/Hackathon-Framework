package hackathonlib;

import java.net.ServerSocket;
import java.net.Socket;
import java.util.LinkedList;
import java.util.List;

/**
 * Models a network for servers. Accepts clients on ports.
 * @author Michael Fürst
 * @version 1.0
 * @since 23.11.2014
 */
public class Network implements Runnable{
    private boolean running = false;
    private ServerSocket server;

    private int port;
    private IGameManager gameManager;
    private int receiveTimeout;

    private Thread th = null;
    private List<Connection> threads = new LinkedList<>();

    /**
     * Start a server service on a given port using the game manager.
     * @param port The port.
     * @param gameManager The gameManager.
     * @param receiveTimeout The timeout how often clients must be active before kicked in seconds.
     */
    public void startListen(int port, IGameManager gameManager, int receiveTimeout) {
        if (th == null) {
            this.gameManager = gameManager;
            this.receiveTimeout = receiveTimeout;
            th = new Thread(this);
            th.start();
        }
    }

    @Override
    public void run() {
        running = true;

        try {
            server = new ServerSocket(port);
            server.setSoTimeout(10);
        } catch (Exception e) {
            e.printStackTrace();
        }

        while (running) {
            try {
                Socket c = server.accept();
                if (c == null) continue;
                threads.add(new Connection(c, gameManager, receiveTimeout));
            } catch (Exception e) {
                // Do nothing...
            }
        }
        try {
            server.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
        server = null;
        threads.forEach(t -> t.quit());
        th = null;
    }

    /**
     * Stop listening for clients and disconnect all connected clients.
     */
    public void endListen() {
        running = false;
    }
}
