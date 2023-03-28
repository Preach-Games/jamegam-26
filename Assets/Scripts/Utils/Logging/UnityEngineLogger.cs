using UnityEngine;

namespace DungeonDraws.Scripts.Utils.Logging
{
    public class UnityEngineLogger : Logger { 
        public override void output(string v)
        {
            switch (_logLevel)
            {
                case Loglevel.Error:
                    Debug.LogError(v);
                    break;
                case Loglevel.Warn:
                    Debug.LogWarning(v);
                    break;
                case Loglevel.Info:
                    Debug.Log(v);
                    break;
            }
        }
    }
}