using System;
using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
using DungeonDraws.Scripts.Systems.LevelGeneration.Renderer;
using DungeonDraws.Scripts.Utils;
using DungeonDraws.Scripts.Utils.Attributes;
using DungeonDraws.Scripts.Utils.Logging;
using UnityEngine;

namespace DungeonDraws.Scripts.Systems.LevelGeneration
{
    public class LevelGeneratorBehaviour : MonoBehaviour
    {
        public LevelData levelData;

        public bool _devMode = false;
        public bool _devLog = false;
        public bool _randomSeed = false;
        public bool _drawGrid = false;
        public bool _drawTiles = false;

        public GameObject _floorPrefab;
        public GameObject _wallPrefab;
        public GameObject _wallSeparatorPrefab;
        public GameObject _cornerInPrefab;
        public GameObject _cornerOutPrefab;

        [SerializeField] private int[,] _tilesMap;

        private int _seed;

        // TODO: Implement renderer
        private LevelGenerator _generator;
        private LevelRenderer _renderer;

        void Awake()
        {
            _generator = new LevelGenerator(10);
            _renderer = LevelRenderer.newInstance(this);
            _seed = levelData._seed;
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
            generateDungeon();
        }

        private void generateSeed()
        {
            _seed = Time.time.ToString().GetHashCode();
        }

        void Update()
        {
            if (_devMode && Input.GetMouseButtonDown(0))
            {
                devMode();
            }
        }

        [Button]
        private void generateDungeon()
        {
            if (_randomSeed)
            {
                generateSeed();
            }

            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = Vector3.one;
            _generator.SetMapSize(levelData._mapHeight, levelData._mapWidth);
            _generator.SetRoomsNumberRange(levelData._roomsNumberMin, levelData._roomsNumberMax);
            _generator.SetRoomSizeRange(levelData._roomSizeMin, levelData._roomSizeMax);
            _generator.SetCorridorLengthRange(levelData._corridorLengthMin, levelData._corridorLengthMax);
            _generator.SetCorridorWidthRange(levelData._corridorWidthMin, levelData._corridorWidthMax);
            _generator.SetPlotter(new DetailedTilesPlotter());
            if (_devLog) _generator.SetLogger(new UnityEngineLogger());
            _generator.SetSeed(_seed);

            _tilesMap = _generator.AsMatrix();
            _renderer.convertToMeshes(_tilesMap);
        }

        public void OnDrawGizmos()
        {
            if (_floorPrefab && _drawGrid)
            {
                float tileWidth = _floorPrefab.GetComponent<MeshRenderer>().bounds.size.x;
                float mapWidth = levelData._mapWidth * tileWidth;
                float mapHeight = levelData._mapHeight * tileWidth;
                Gizmos.color = Color.red;
                GizmoExtensions.GridGizmo(mapWidth, mapHeight, levelData._mapWidth,
                    levelData._mapHeight,
                    new Vector3(transform.position.x, 0, transform.position.z));

                if (_tilesMap != null && _drawTiles)
                {
                    DrawTilesMap();
                }
            }
        }

        public void DrawTilesMap()
        {
            float floorSpan = _floorPrefab.GetComponent<MeshRenderer>().bounds.size.x;
            for (int row = 0; row < _tilesMap.GetLength(0); row++)
            {
                for (int col = 0; col < _tilesMap.GetLength(1); col++)
                {
                    float xPos = col * floorSpan;
                    float zPos = row * floorSpan;
                    int value = _tilesMap[row, col];
                    DetailedTileType type = (DetailedTileType)value;

                    switch (value)
                    {
                        case (int)DetailedTileType.Floor:
                            Gizmos.color = Color.white;
                            break;
                        case (int)DetailedTileType.Wall_N:
                        case (int)DetailedTileType.Wall_E:
                        case (int)DetailedTileType.Wall_S:
                        case (int)DetailedTileType.Wall_W:
                            Gizmos.color = Color.black;
                            break;
                        case (int)DetailedTileType.Corner_INN_NE:
                        case (int)DetailedTileType.Corner_INN_NW:
                        case (int)DetailedTileType.Corner_INN_SW:
                        case (int)DetailedTileType.Corner_INN_SE:
                            Gizmos.color = Color.green;
                            break;
                        case (int)DetailedTileType.Corner_OUT_NE:
                        case (int)DetailedTileType.Corner_OUT_NW:
                        case (int)DetailedTileType.Corner_OUT_SW:
                        case (int)DetailedTileType.Corner_OUT_SE:
                            Gizmos.color = Color.blue;
                            break;
                    }

                    if (value != (int)DetailedTileType.Empty)
                    {
                        Gizmos.DrawCube(new Vector3(xPos + floorSpan / 2, 0, zPos + floorSpan/2),
                            new Vector3(floorSpan, 1, floorSpan));
                    }
                }
            }
        }
    }
}