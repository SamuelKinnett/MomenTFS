using System.IO;

namespace MomenTFS.MAP.Collision
{
    public class CollisionData
    {
        public static int COLLISION_MAP_GRID_SIZE = 100;

        public CollisionType[,] CollisionMap { get; set; }

        public CollisionData(Stream stream) {
            CollisionMap = new CollisionType[COLLISION_MAP_GRID_SIZE, COLLISION_MAP_GRID_SIZE];

            for (int y = 0; y < COLLISION_MAP_GRID_SIZE; ++y) {
                for (int x = 0; x < COLLISION_MAP_GRID_SIZE; ++x) {
                    CollisionMap[x, y] = (CollisionType)stream.ReadByte();
                }
            }
        }
    }
}
