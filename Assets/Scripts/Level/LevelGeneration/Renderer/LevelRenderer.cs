using System;
using DungeonDraws.Level;
using DungeonDraws.Scripts.Systems.LevelGeneration.Plotters;
using UnityEngine;

namespace DungeonDraws.Scripts.Systems.LevelGeneration.Renderer
{
    public class LevelRenderer : ScriptableObject
    {
        private LevelManager _manager;
        private GameObject _boardHolder;

        private LevelRenderer(LevelManager manager, GameObject boardHolder)
        {
            _manager = manager;
            _boardHolder = boardHolder;
        }

        public void convertToMeshes(int[,] map)
        {
            GameObject oldTileParent = _boardHolder.transform.childCount > 0
                ? _boardHolder.transform.GetChild(0).gameObject
                : null;
            if (oldTileParent != null && oldTileParent.name == "TileParent")
            {
                Destroy(oldTileParent);
            }

            GameObject tileParent = new GameObject("TileParent");
            tileParent.transform.parent = _boardHolder.transform;
            addMainMashes(tileParent, map);
            Debug.Log(oldTileParent);
            //overlapWallSerators(boardHolder, map);
        }

        private void addMainMashes(GameObject boardHolder, int[,] map)
        {
            Vector3 floorSize = _manager._floorPrefab.GetComponentInChildren<MeshRenderer>().bounds.size;
            float floorSpan = floorSize.x;
            float halfFloorSpan = floorSpan * 0.5f;

            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    int value = map[row, col];
                    DetailedTileType type = (DetailedTileType)value;
                    float xPos = col * floorSpan + halfFloorSpan;
                    float zPos = row * floorSpan + halfFloorSpan;

                    if (type == DetailedTileType.Floor)
                    {
                        GameObject prefab = _manager._floorPrefab;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Wall_N)
                    {
                        GameObject prefab = _manager._wallPrefab;
                        // zPos += floorSpan;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Wall_E)
                    {
                        GameObject prefab = _manager._wallPrefab;
                        // xPos += floorSpan;
                        // Wall cell need to be shifted to cover floor
                        float yRot = 270f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Wall_S)
                    {
                        GameObject prefab = _manager._wallPrefab;
                        // Wall cell need to be shifted to cover floor
                        float yRot = 180f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Wall_W)
                    {
                        GameObject prefab = _manager._wallPrefab;
                        // Wall cell need to be shifted to cover floor
                        float yRot = 90f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_INN_NW)
                    {
                        GameObject prefab = _manager._cornerInPrefab;
                        float yRot = 180f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_INN_NE)
                    {
                        GameObject prefab = _manager._cornerInPrefab;
                        // xPos -= floorSpan;
                        // zPos += floorSpan;
                        float yRot = 270f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_INN_SE)
                    {
                        GameObject prefab = _manager._cornerInPrefab;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_INN_SW)
                    {
                        GameObject prefab = _manager._cornerInPrefab;
                        float yRot = 90f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_OUT_NW)
                    {
                        GameObject prefab = _manager._cornerOutPrefab;
                        float yRot = 180f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_OUT_NE)
                    {
                        GameObject prefab = _manager._cornerOutPrefab;
                        float yRot = 90f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_OUT_SW)
                    {
                        GameObject prefab = _manager._cornerOutPrefab;
                        float yRot = 270f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                    else if (type == DetailedTileType.Corner_OUT_SE)
                    {
                        GameObject prefab = _manager._cornerOutPrefab;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, type.ToString());
                    }
                }
            }
        }

        public static LevelRenderer newInstance(LevelManager levelManager, GameObject boardHolder)
        {
            LevelRenderer renderer = ScriptableObject.CreateInstance<LevelRenderer>();
            renderer.setManager(levelManager);
            renderer.setBoard(boardHolder);
            return renderer;
        }

        private void setManager(LevelManager levelManager)
        {
            _manager = levelManager;
        }

        private void setBoard(GameObject boardHolder)
        {
            _boardHolder = boardHolder;
        }

        private void overlapWallSerators(GameObject boardHolder, int[,] map)
        {
            GameObject prefab = _manager._wallSeparatorPrefab;
            String objectName = "Wall_Separator";
            Vector3 floorSize = _manager._floorPrefab.GetComponentInChildren<MeshRenderer>().bounds.size;

            float floorSpan = floorSize.x;
            float oneQuarterFloorSpan = floorSpan * 0.25f;
            float threeQuartersFloorSpan = floorSpan * 0.75f;

            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    int value = map[row, col];
                    DetailedTileType type = (DetailedTileType)value;

                    if (type == DetailedTileType.Wall_N)
                    {
                        float xPos = col * floorSpan;
                        float zPos = row * floorSpan + oneQuarterFloorSpan;
                        float yRot = 180f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Wall_E)
                    {
                        float xPos = col * floorSpan - threeQuartersFloorSpan;
                        float zPos = row * floorSpan;
                        float yRot = 270f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Wall_S)
                    {
                        float xPos = col * floorSpan - floorSpan;
                        float zPos = row * floorSpan + threeQuartersFloorSpan;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Wall_W)
                    {
                        float xPos = col * floorSpan - oneQuarterFloorSpan;
                        float zPos = row * floorSpan + floorSpan;
                        float yRot = 90f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_INN_NW)
                    {
                        float xPos = col * floorSpan + floorSpan;
                        float zPos = row * floorSpan + oneQuarterFloorSpan;
                        float yRot = 180f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_INN_NE)
                    {
                        float xPos = col * floorSpan - threeQuartersFloorSpan;
                        float zPos = row * floorSpan - floorSpan;
                        float yRot = 270f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_INN_SE)
                    {
                        float xPos = col * floorSpan - 2 * floorSpan;
                        float zPos = row * floorSpan + threeQuartersFloorSpan;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_INN_SW)
                    {
                        float xPos = col * floorSpan - oneQuarterFloorSpan;
                        float zPos = row * floorSpan + 2 * floorSpan;
                        float yRot = 90f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_OUT_NW)
                    {
                        float xPos = col * floorSpan - floorSpan;
                        float zPos = row * floorSpan + threeQuartersFloorSpan;
                        float yRot = 0f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_OUT_NE)
                    {
                        float xPos = col * floorSpan - threeQuartersFloorSpan;
                        float zPos = row * floorSpan;
                        float yRot = 270f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_OUT_SW)
                    {
                        float xPos = col * floorSpan - oneQuarterFloorSpan;
                        float zPos = row * floorSpan + floorSpan;
                        float yRot = 90f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                    else if (type == DetailedTileType.Corner_OUT_SE)
                    {
                        float xPos = col * floorSpan;
                        float zPos = row * floorSpan + oneQuarterFloorSpan;
                        float yRot = 180f;
                        _instantiate(prefab, xPos, zPos, yRot, boardHolder, row, col, objectName);
                    }
                }
            }
        }

        private void _instantiate(GameObject prefab, float xPos, float zPos, float yRot, GameObject parent, int mapX,
            int mapZ, String name)
        {
            Vector3 position = new Vector3(xPos, 0, zPos) - parent.transform.position;
            GameObject instance = (GameObject)Instantiate(prefab, position, Quaternion.identity);
            instance.transform.Rotate(0, yRot, 0);
            instance.name = "(" + mapX + "," + mapZ + ") " + name;
            instance.transform.parent = parent.transform;
        }
    }
}