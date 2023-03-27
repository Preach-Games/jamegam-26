using DungeonDraws.Scripts.Utils.Logging;
using DungeonDraws.Scripts.Utils.Singleton;
using UnityEngine;

namespace DungeonDraws.Level
{
    public class LevelManager: Singleton<LevelManager>
    {
        [SerializeField]
        private bool devLog;

        [SerializeField]
        private Loglevel logLevel;
        private IXLogger _logger;

        private void Awake()
        {
            SetParams();
            _logger.info("LevelManager awake");
        }
        
        private void OnValidate()
        {
            SetParams();
        }

        private void SetParams()
        {
            _logger = devLog ? new UnityEngineLogger() : new NullLogger();
            _logger.setLogLimit(logLevel);
        }

        // TODO: Implement method that returns a map texture of the generated dungeon
        // public Texture2D GetMap(int width, int height)
        // {
        //     return new Texture2D(width, height);
        // }
        
        
    }
}