using UnityEngine;

namespace DungeonDraws.Scripts.Utils
{
    public static class GizmoExtensions
    {
        // Only draws flat on ground doesn't handle rotation.
        public static void GridGizmo(float width, float height, int horizontalCellCount, int verticalCellCount,
            Vector3 position)
        {
            for (float x = 0 + position.x; x <= width; x += width / horizontalCellCount)
            {
                Gizmos.DrawLine(new Vector3(x, position.y, position.z), new Vector3(x, 0, height));
            }

            for (float z = 0 + position.z; z <= height; z += height / verticalCellCount)
            {
                Gizmos.DrawLine(new Vector3(position.x, position.y, z), new Vector3(width, 0, z));
            }
        }
    }
}