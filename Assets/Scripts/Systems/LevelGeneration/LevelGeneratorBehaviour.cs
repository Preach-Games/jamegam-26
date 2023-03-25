using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
using GameJamKit.Scripts.Utils.Logging;
using UnityEngine;

namespace DungeonDraws.Scripts.Systems.LevelGeneration
{
    public class LevelGeneratorBehaviour : MonoBehaviour
    {
        public int _mapHeight = 100;
        public int _mapWidth = 100;
        public int _roomsNumberMin = 5;
        public int _roomsNumberMax = 15;
        public int _roomSizeMin = 5;
        public int _roomSizeMax = 20;
        public int _corridorLengthMin = 2;
        public int _corridorLengthMax = 7;
        public int _corridorWidthMin = 3;
        public int _corridorWidthMax = 3;
        public int _seed = 123456;

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
        // private LevelRenderer _renderer;

        void Awake()
        {
            _generator = new LevelGenerator(10);
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
                _seed = Time.time.ToString().GetHashCode();
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

        private void generateDungeon() {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = Vector3.one;

            _generator.SetMapSize(_mapHeight, _mapWidth);
            _generator.SetRoomsNumberRange(_roomsNumberMin, _roomsNumberMax);
            _generator.SetRoomSizeRange(_roomSizeMin, _roomSizeMax);
            _generator.SetCorridorLengthRange(_corridorLengthMin, _corridorLengthMax);
            _generator.SetCorridorWidthRange(_corridorWidthMin, _corridorWidthMax);
            _generator.SetPlotter(new DetailedTilesPlotter());
            if (_devLog) _generator.SetLogger(new UnityEngineLogger());
            _generator.SetSeed(_seed);

            _tilesMap = _generator.AsMatrix();
            //TODO: Some sort of mesh generation or tile placement for rendering?
            // _renderer.convertToMeshes(_tilesMap);

            transform.rotation = Quaternion.Euler(0, 90, 0);
            transform.localScale = new Vector3(-1, 1, -1);
            transform.position = new Vector3(0.5f, 0, 0.5f);
        }
    }
}
