package hackathonlib;

import com.google.gson.JsonElement;

import java.util.List;

/**
 * Models a game object.
 *
 * @author Michael Fürst
 * @version 1.0
 * @since 23.11.2014
 */
public class GameObject {
    public String name;
    public double x;
    public double y;
    public double z;
    public double rotation;
    public List<JsonElement> extra;

    /**
     * Create a new GameObject.
     * @param name The name.
     * @param x The x position.
     * @param y The y position.
     * @param z The z position.
     * @param rotation The rotation.
     * @param extra The extra information.
     */
    public GameObject(String name, double x, double y, double z, double rotation, List<JsonElement> extra) {
        this.name = name;
        this.x = x;
        this.y = y;
        this.z = z;
        this.rotation = rotation;
        this.extra = extra;
    }
}
