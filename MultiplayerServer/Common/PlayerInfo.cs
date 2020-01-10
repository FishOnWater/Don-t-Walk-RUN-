using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class PlayerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rotation { get; set; }
        public int HP { get; set; }
        public float bulletX { get; set; }
        public float bulletY { get; set; }
        public float bulletZ { get; set; }
        public float hitPlayer { get; set; }
        public string HitName { get; set; }
        public int SpawnLocation { get; set; }
    }
}
