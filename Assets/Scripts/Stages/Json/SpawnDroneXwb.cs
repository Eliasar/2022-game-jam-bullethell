using System;
using System.Collections.Generic;

namespace Confined.Stages.Json
{
    [Serializable]
    public class SpawnDroneXwb
    {
        public float entrySpeed;
        public float entryHoverTime;
        public EntryPath entryPath;
        public Coordinate[] patrolPoints;
    }

    [Serializable]
    public class EntryPath
    {
        public float p0x;
        public float p0y;
        public float p1x;
        public float p1y;
        public float p2x;
        public float p2y;
        public float p3x;
        public float p3y;
    }

    [Serializable]
    public class Coordinate
    {
        public float x;
        public float y;
    }
}