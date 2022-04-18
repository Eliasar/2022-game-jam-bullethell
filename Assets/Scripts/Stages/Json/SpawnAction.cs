using System;

namespace Confined.Stages.Json
{
    [Serializable]
    public class SpawnAction
    {
        public int group;

        public SpawnCollection[] spawns;
    }
}
