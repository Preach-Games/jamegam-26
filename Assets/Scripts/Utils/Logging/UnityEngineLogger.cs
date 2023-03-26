using UnityEngine;

namespace DungeonDraws.Scripts.Utils.Logging
{
    public class UnityEngineLogger : IXLogger {
        public void warning(string v) {
            Debug.LogWarning(v);
        }
        public void error(string v) {
            Debug.LogError(v);
        }
        public void info(string v) {
            Debug.Log(v);
        }
    }
}