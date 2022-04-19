using System;

namespace Confined.Stages.Json2
{
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
    public class PatrolPoint
    {
        public float x;
        public float y;
    }

    [Serializable]
    public class Spawn
    {
        public string prefab;
        public float entryDelay;
        public float entrySpeed;
        public float entryHoverTime;
        public PatrolPoint startingPosition;
        public EntryPath entryPath;
        public PatrolPoint[] patrolPoints;
    }

    [Serializable]
    public class Action
    {
        public string type;
        public int group;
        public Spawn[] spawns;
        public float delayTime;

        // public override string ToString()
        // {
        //     return $"type = {type}; group = {group}; delayTime = {delayTime};";// spawns = {spawns.Length}";
        // }
    }

    [Serializable]
    public class Root
    {
        public string levelName;
        public Action[] actions;

        // public override string ToString()
        // {
        //     var returnString = $"levelName = {levelName}; actions =>";
        //     for (int i = 0; i < actions.Length; i++)
        //     {
        //         returnString += $" [{i}] = {actions[i].ToString()}";
        //     }

        //     return returnString;
        // }
    }
}
