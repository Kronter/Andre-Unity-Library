using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using UnityEditor;
using UnityEngine;

namespace Andre.Generation
{
    public class DungeonRoom
    {
        public Vector3 Position;
        public Vector3 Size;
        public float weight;
        public List<DungeonRoom> connections;
    }

    public class GenerateDungeon : MonoBehaviour
    {
        public List<DungeonRoom> Rooms;
        public List<DungeonRoom> bestRooms;
        TriangleNet.Mesh mesh;
        Polygon polygon;
        List<Triangle> triangles;
        List<KruskalsEdge> tmpConnections;
        List<KruskalsEdge> connections;
        public int tileSize = 1;
        public int unitSize = 1;
        public float distanceFactor = 0.5f;
        public Vector2Int RoomMinMaxRangeX;
        public Vector2Int RoomMinMaxRangeY;
        [Range(0.01f, 1.99f)]
        public float cullingPercentage = 1.25f;
        public float connectivityPercentage = 15;
        public bool showConections = false;
        public bool showRoomConections = false;
        public bool ShowBestPointsOnly = false;
        public int NumberRandInCirclePoints = 10;
        public float RadiusRandInCirclePoints = 40;
        float mean = 0;

        // Start is called before the first frame update
        void Start()
        {
            GenDungeon();
        }

        public void GenDungeon()
        {
            DungeonGenPhaseOne();
            DungeonGenPhaseTwo();
            DungeonGenPhaseThree();
        }

        public void DungeonGenPhaseOne()
        {
            CreateRooms();
            SeparateRooms();
        }

        public void DungeonGenPhaseTwo()
        {
            CullRooms();
            DelaunayTriangulation();
        }

        public void DungeonGenPhaseThree()
        {
            KruskalsMinimumSpanningTree();
            SetUpRoomConnections();
        }

        public void CullRooms()
        {
            for (int i = 0; i < NumberRandInCirclePoints; i++)
            {
                if (Rooms[i].weight >= mean * cullingPercentage)
                {
                    bestRooms.Add(Rooms[i]);
                }
            }

            for (int i = 0; i < NumberRandInCirclePoints; i++)
            {
                bool remove = false;
                foreach (var room in bestRooms)
                {
                    if (room != Rooms[i])
                    {
                        if (TooCloseTo(room, Rooms[i]))
                        {
                            remove = true;
                            break;
                        }
                    }
                }
                if (remove)
                    bestRooms.Remove(Rooms[i]);
            }
        }

        public void ConnectAllRooms()
        {
            DelaunayTriangulation();
        }

        void DelaunayTriangulation()
        {
            polygon = new Polygon();
            foreach (var vertex in bestRooms)
            {
                polygon.Add(new TriangleNet.Geometry.Vertex(vertex.Position.x, vertex.Position.z));
            }

            TriangleNet.Meshing.ConstraintOptions options =
            new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = false };
            mesh = (TriangleNet.Mesh)polygon.Triangulate(options);
        }

        public void CullRoomPathways()
        {
            KruskalsMinimumSpanningTree();
        }

        void KruskalsMinimumSpanningTree()
        {
            //triangles = new List<Triangle>();
            tmpConnections = new List<KruskalsEdge>();
            connections = new List<KruskalsEdge>();
            //triangles = DelaunayTriangulation.TriangulateByFlippingEdges(points);
            List<int> vertices = new List<int>();
            int index = 0;
            int index2 = 0;
            foreach (var edge in mesh.Edges)
            {
                TriangleNet.Geometry.Vertex v0 = mesh.vertices[edge.P0];
                TriangleNet.Geometry.Vertex v1 = mesh.vertices[edge.P1];
                Vector3 p0 = new Vector3((float)v0.x, 0.0f, (float)v0.y);
                Vector3 p1 = new Vector3((float)v1.x, 0.0f, (float)v1.y);
                int tmp = 0;
                foreach (var point in Rooms)
                {
                    if (p0 == point.Position)
                    {
                        index = tmp;
                        break;
                    }

                    tmp++;
                }
                tmp = 0;
                foreach (var point in Rooms)
                {
                    if (p1 == point.Position)
                    {
                        index2 = tmp;
                        break;
                    }

                    tmp++;
                }
                tmpConnections.Add(new KruskalsEdge() { Vertex1 = edge.P0, Room1 = Rooms[index], Vertex2 = edge.P1, Room2 = Rooms[index2], Weight = (Vector3.Distance(p0, p1)) * (Rooms[index].weight / 20) });
                if (!vertices.Contains(edge.P0))
                    vertices.Add(edge.P0);
                if (!vertices.Contains(edge.P1))
                    vertices.Add(edge.P1);
            }

            connections = KruskalsAlgorithm.Kruskals_MST(tmpConnections, vertices, false, connectivityPercentage);
        }

        public void CreateRooms()
        {
            tileSize = tileSize * unitSize;
            polygon = new Polygon();
            Rooms = new List<DungeonRoom>();
            bestRooms = new List<DungeonRoom>();
            mean = 0;
            for (int i = 0; i < NumberRandInCirclePoints; i++)
            {
                Vector3 tmpPos = Random.insideUnitCircle * ((RadiusRandInCirclePoints * tileSize) * (distanceFactor / unitSize));
                float weight = Random.Range(1.0f, 16.0f);
                mean = mean + weight;
                DungeonRoom tmpRoom = new DungeonRoom();
                tmpRoom.weight = weight;
                tmpRoom.Size = new Vector3(Random.Range(RoomMinMaxRangeX.x, RoomMinMaxRangeX.y + 1) * tileSize, 1.0f, Random.Range(RoomMinMaxRangeY.x, RoomMinMaxRangeY.y + 1) * tileSize);
                tmpRoom.connections = new List<DungeonRoom>();
                tmpRoom.Position = new Vector3(tmpPos.x, 0.0f, tmpPos.y);
                Rooms.Add(tmpRoom);
            }
            mean = mean / NumberRandInCirclePoints;
        }

        public void SetUpRoomConnections()
        {
            foreach (var connection in connections)
            {
                connection.Room1.connections.Add(connection.Room2);
                connection.Room2.connections.Add(connection.Room1);
            }
        }

        public void SeparateRooms()
        {
            int itirations = 0;
            for (int i = 0; i < Rooms.Count; i++)
            {
                while (itirations < 10000)
                {
                    Vector3 oldPos = Rooms[i].Position;
                    Vector3 separation = computeSeparation(Rooms[i]);
                    Vector3 newPos = new Vector3(RoundToTileSize(oldPos.x += separation.x, tileSize), oldPos.y, RoundToTileSize(oldPos.z += separation.z, tileSize));
                    Rooms[i].Position = newPos;
                    if (separation == Vector3.zero)
                        break;
                    itirations++;
                }
                itirations = 0;
            }
        }

        float RoundToTileSize(float number, int tileSize)
        {
            return Mathf.Floor(((number + tileSize - 1) / tileSize)) * tileSize;
        }

        Vector3 computeSeparation(DungeonRoom r)
        {

            int neighbours = 0;
            Vector3 v = new Vector3();
            int index = 0;
            foreach (DungeonRoom room in Rooms)
            {

                if (room != r)
                {
                    if (TooCloseTo(room, r))
                    {
                        v.x += Difference(room, r, "x");
                        v.z += Difference(room, r, "z");
                        neighbours++;
                    }

                }
                index++;

            }

            if (neighbours == 0)
                return v;

            v.x /= neighbours;
            v.z /= neighbours;
            v.x *= -1;
            v.z *= -1;
            return v;
        }


        public bool TooCloseTo(DungeonRoom room, DungeonRoom currentRoom)
        {
            return currentRoom.Position.x + currentRoom.Size.x / 2 > room.Position.x - room.Size.x / 2 && currentRoom.Position.z + currentRoom.Size.z / 2 > room.Position.z - room.Size.z / 2 &&
                currentRoom.Position.x - currentRoom.Size.x / 2 < room.Position.x + room.Size.x / 2 && currentRoom.Position.z - currentRoom.Size.z / 2 < room.Position.z + room.Size.z / 2;
        }

        float Difference(DungeonRoom room, DungeonRoom currentRoom, string type)
        {
            switch (type)
            {

                case "x":
                    float xBottom = (room.Position.x + room.Size.x / 2) - (currentRoom.Position.x - currentRoom.Size.x / 2);
                    float xTop = (currentRoom.Position.x + currentRoom.Size.x / 2) - (room.Position.x - room.Size.x / 2);
                    return xBottom > 0 ? xBottom : xTop;
                    break;
                case "z":
                    float xRight = (room.Position.z + room.Size.z / 2) - (currentRoom.Position.z - currentRoom.Size.z / 2);
                    float xLeft = (currentRoom.Position.z + currentRoom.Size.z / 2) - (room.Position.z - room.Size.z / 2);
                    return xRight > 0 ? xRight : xLeft;
                default:
                    return 0;
                    break;
            }

        }

        private void OnDrawGizmos()
        {
            if (Rooms == null)
                return;

            if (ShowBestPointsOnly)
            {
                foreach (var point in bestRooms)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(point.Position, point.Size);
                }
            }
            else
            {
                foreach (var point in Rooms)
                {
                    Gizmos.color = Color.black;
                    Gizmos.color = new Color(point.weight / 20, Gizmos.color.g, Gizmos.color.b, Gizmos.color.a);
                    Gizmos.DrawWireCube(point.Position, point.Size);
                    //Handles.Label(point.Position, point.Position.ToString());
                }
            }

            if (mesh == null)
                return;

            if (!showConections)
            {
                Gizmos.color = Color.green;
                foreach (var edge in mesh.Edges)
                {
                    TriangleNet.Geometry.Vertex v0 = mesh.vertices[edge.P0];
                    TriangleNet.Geometry.Vertex v1 = mesh.vertices[edge.P1];
                    Vector3 p0 = new Vector3((float)v0.x, 0.0f, (float)v0.y);
                    Vector3 p1 = new Vector3((float)v1.x, 0.0f, (float)v1.y);
                    Gizmos.DrawLine(p0, p1);
                }

            }

            if (connections == null)
                return;

            if (showConections)
            {
                foreach (var connection in connections)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(connection.Room1.Position, connection.Room2.Position);
                }
            }

            if (showRoomConections)
            {
                foreach (var room in bestRooms)
                {
                    Gizmos.color = Color.cyan;
                    foreach (var connection in room.connections)
                    {
                        Gizmos.DrawLine(room.Position, connection.Position);
                    }
                }
            }
        }
    }
}