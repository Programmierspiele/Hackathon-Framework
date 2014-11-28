using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hackathonlib
{
    class HackathonPacket
    {
        public List<GameObject> scene = null;
        public string command = null;
        public double? speed = null;
        public double? rotation = null;
        public string name = null;
        public string ping = null;
    }
}
