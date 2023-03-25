using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
using DungeonDraws.Scripts.Systems.LevelGeneration.Renderer;
using DungeonDraws.Scripts.Utils.Attributes;
using GameJamKit.Scripts.Utils.Logging;
using UnityEngine;

namespace DungeonDraws.Scripts.Systems.LevelGeneration
{
    public class LevelGeneratorBehaviour : MonoBehaviour
    {
        public LevelData levelData;

        public bool _devMode = false;
        public bool _devLog = false;
        public bool _randomSeed = false;

        public GameObject _floorPrefab;
        public GameObject _wallPrefab;
        public GameObject _wallSeparatorPrefab;
        public GameObject _cornerInPrefab;
        public GameObject _cornerOutPrefab;

        [SerializeField]
        private int[,] _tilesMap;
        
        // TODO: Implement renderer
        private LevelGenerator _generator;
        private LevelRenderer _renderer;

        void Awake()
        {
            _generator = new LevelGenerator(10);
            _renderer = LevelRenderer.newInstance(this);
        }

        void Start()
        {
            if (_devMode)
            {
                devMode();
                return;
            }
        }

        private void devMode()
        {
            if (_randomSeed)
            {
                levelData._seed = Time.time.ToString().GetHashCode();
            }
            generateDungeon();
        }

        void Update()
        {
            if (_devMode && Input.GetMouseButtonDown(0))
            {
                devMode();
            }
        }
        
        [Button]
        private void generateDungeon() {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = Vector3.one;

            _generator.SetMapSize(levelData._mapHeight, levelData._mapWidth);
            _generator.SetRoomsNumberRange(levelData._roomsNumberMin, levelData._roomsNumberMax);
            _generator.SetRoomSizeRange(levelData._roomSizeMin, levelData._roomSizeMax);
            _generator.SetCorridorLengthRange(levelData._corridorLengthMin, levelData._corridorLengthMax);
            _generator.SetCorridorWidthRange(levelData._corridorWidthMin, levelData._corridorWidthMax);
            _generator.SetPlotter(new DetailedTilesPlotter());
            if (_devLog) _generator.SetLogger(new UnityEngineLogger());
            _generator.SetSeed(levelData._seed);

            _tilesMap = _generator.AsMatrix();
            _renderer.convertToMeshes(_tilesMap);

            transform.rotation = Quaternion.Euler(0, 90, 0);
            transform.localScale = new Vector3(-1, 1, -1);
            transform.position = new Vector3(0.5f, 0, 0.5f);
        }
    }
}
