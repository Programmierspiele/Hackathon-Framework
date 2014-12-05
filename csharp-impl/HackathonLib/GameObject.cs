using System.Collections.Generic;

namespace HackathonLib
{
    public class GameObject
    {
        public double x, y, z;
        public double rotation;
        public string name;
        public List<object> extra;

        public GameObject(string name, double x, double y, double z, double rotation, List<object> extra)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.z = z;
            this.rotation = rotation;
            this.extra = extra;
        }
    }
}
