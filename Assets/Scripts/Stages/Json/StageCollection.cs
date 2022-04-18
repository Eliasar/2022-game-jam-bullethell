using System;

namespace Confined.Stages.Json
{
    [Serializable]
    public class StageCollection
    {
        public string levelName;
        public object[] actions;

        public override string ToString()
        {
            return $"levelName = {levelName}; actions = {actions}";
        }
    }
}
