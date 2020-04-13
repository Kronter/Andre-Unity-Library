using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Andre.AI.EQS
{
    public enum EQSshape
    {
        Grid,
        Circle,
        Donut
    }

    public class EnviromentQuerySystem : MonoBehaviour
    {
        public EQSshape Shape;
        [Range(0, 1)]
        public float bestPointRangeMin;
        [Range(0, 1)]
        public float bestPointRangeMax;
        public float eqsPointWeight;
        public float nodeCheckRadius = 0;
        public float minimumDistanceTest = 0;
        public float pathfindingCheckRadius = 0;
        public LayerMask Layer;
        public Transform target;
        public Transform anchor;
        public LayerMask traceTestLayer;
        public string[] tagsToTest;
        public string tagToTest;
        [Range(-1, 1)]
        public float wantedVert;
        [Range(-1, 1)]
        public float wantedHoz;
        public bool hide = false;
        public bool dotTestAddPoints = true;
        public bool beginEQS = false;
        public bool bestNodeRange = true;

        [Space(10)]
        [Header("Grid")]
        [Range(4, 100)]
        public int GridDensity;
        public float GridSpacing;

        [Space(10)]
        [Header("Circle")]
        [Range(4, 100)]
        public int CircleNodeDensity = 10;
        public float CircleRadius = 2;

        [Space(10)]
        [Header("Donut")]
        [Range(4, 100)]
        public int DonutNodeDensity = 10;
        public float DonutRadius = 2;
        public int DonutNumberOfRings = 4;
        public float DonutOuterSpacing = 1;
        public float DonutInnerSpacing = 1;
        public float DonutSpiralOffSet = 0;

        [Space(10)]
        [Header("Tests")]
        public bool heightAdjustment = true;
        public bool pathfindingTest = true;
        public bool overlapTest = true;
        public bool pathExistsTest = true;
        public bool traceTest = true;
        public bool distanceTest = true;
        public bool dotTest = true;
        public bool tagTest = true;

        [Space(10)]
        [Header("Debug")]
        public bool SeeDebug = false;
        public Gradient debugNodeColours;
        public Color nullDebugNodeColour;
        public bool SeeDebugPointText = false;
        public Color DebugPointTextColour;
        public bool SeeDebugDotText = false;
        public Color DebugDotTextColour;
        public bool SeeDebugTrace = false;
        public Color DebugTracePositive;
        public Color DebugTraceNegative;
        public bool SeeDebugPathfinding = false;
        public Color DebugPathfindingColour;
        public bool SeeDebugChosenPos = false;
        public Color DebugPathfindingChosenPosColour;


        List<EQSNode> eqsNodes;
        float m_Spacing;
        Vector3 ParentPos;
        bool m_Debug;
        Vector3 CurrentPos;
        bool initialized = false;
        float LargestPoint = 0;
        float SmallestPoint = 10000000000000000000;


        EQSshape CurrentShape;
        int CurrentGridDensity;
        float CurrentCircleNodeDensity;
        float CurrentDonutNodeDensity;
        int CurrentDonutNumberOfRings;

        // Start is called before the first frame update
        void Start()
        {
            CurrentShape = Shape;
            InitEQS();
        }

        void InitShape()
        {
            switch (Shape)
            {
                case EQSshape.Grid:
                    InitGrid();
                    break;
                case EQSshape.Circle:
                    InitCircle();
                    break;
                case EQSshape.Donut:
                    InitDonut();
                    break;
                default:
                    break;
            }
        }

        void DrawShape()
        {
            switch (Shape)
            {
                case EQSshape.Grid:
                    DrawGrid();
                    break;
                case EQSshape.Circle:
                    DrawCircle();
                    break;
                case EQSshape.Donut:
                    DrawDonut();
                    break;
                default:
                    break;
            }
        }

        void InitGrid()
        {
            CurrentGridDensity = GridDensity;
            ParentPos = anchor.position;
            ParentPos.z += ((GridDensity / 2) * m_Spacing) - (m_Spacing / 2);
            ParentPos.x += ((GridDensity / 2) * m_Spacing) - (m_Spacing / 2);
            CurrentPos = ParentPos;
            for (int y = 0; y < GridDensity; y++)
            {
                for (int x = 0; x < GridDensity; x++)
                {
                    EQSNode temp = new EQSNode();
                    temp.Position = CurrentPos;
                    eqsNodes.Add(temp);
                }
            }
        }

        void InitCircle()
        {
            CurrentCircleNodeDensity = CircleNodeDensity;
            ParentPos = anchor.position;
            for (int y = 0; y < CircleNodeDensity; y++)
            {
                EQSNode temp = new EQSNode();
                float degrees = 360f / CircleNodeDensity;// total nodes
                degrees *= (float)y; // num nodes
                degrees += 90;
                CurrentPos.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
                CurrentPos.x *= CircleRadius; // radius
                CurrentPos.z = Mathf.Sin(Mathf.Deg2Rad * degrees);
                CurrentPos.z *= CircleRadius; // radius
                CurrentPos.z += ParentPos.z;
                CurrentPos.x += ParentPos.x;

                temp.Position = CurrentPos;
                eqsNodes.Add(temp);
            }
        }

        void InitDonut()
        {
            CurrentDonutNodeDensity = DonutNodeDensity;
            CurrentDonutNumberOfRings = DonutNumberOfRings;
            ParentPos = anchor.position;
            for (int i = 0; i < DonutNumberOfRings; i++)
            {
                for (int y = 0; y < DonutNodeDensity; y++)
                {
                    EQSNode temp = new EQSNode();
                    float degrees = 360f / DonutNodeDensity;// total nodes
                    degrees *= (float)y; // num nodes
                    degrees += 90;

                    if (i > 0)
                    {
                        CurrentPos.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
                        CurrentPos.x *= DonutRadius + DonutInnerSpacing + i; // radius
                        CurrentPos.z = Mathf.Sin(Mathf.Deg2Rad * degrees);
                        CurrentPos.z *= DonutRadius + DonutInnerSpacing + i; // radius
                    }
                    else
                    {
                        CurrentPos.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
                        CurrentPos.x *= DonutRadius; // radius
                        CurrentPos.z = Mathf.Sin(Mathf.Deg2Rad * degrees);
                        CurrentPos.z *= DonutRadius; // radius
                    }
                    CurrentPos.z += ParentPos.z;
                    CurrentPos.x += ParentPos.x;

                    temp.Position = CurrentPos;
                    eqsNodes.Add(temp);
                }
            }
        }

        void DrawGrid()
        {
            ParentPos = anchor.position;
            ParentPos.z += ((GridDensity / 2) * m_Spacing) - (m_Spacing / 2);
            ParentPos.x += ((GridDensity / 2) * m_Spacing) - (m_Spacing / 2);
            Vector3 pos = ParentPos;
            CurrentPos = pos;

            for (int y = 0; y < GridDensity; y++)
            {
                for (int x = 0; x < GridDensity; x++)
                {
                    if (eqsNodes[y * (int)GridDensity + x] == null)
                        continue;
                    if (heightAdjustment)
                        UpdateHeight(CurrentPos, out CurrentPos);

                    eqsNodes[y * (int)GridDensity + x].Position = CurrentPos;
                    CurrentPos.x -= m_Spacing;
                    eqsNodes[y * (int)GridDensity + x].Point = 0;
                    eqsNodes[y * (int)GridDensity + x].Enabled = true;
                    eqsNodes[y * (int)GridDensity + x].DotVert = 0;
                    eqsNodes[y * (int)GridDensity + x].DotHoz = 0;
                }
                CurrentPos.x = ParentPos.x;
                CurrentPos.z -= m_Spacing;
            }
        }

        void DrawCircle()
        {
            ParentPos = anchor.position;
            Vector3 pos = ParentPos;
            CurrentPos = pos;
            int y = 0;
            foreach (var node in eqsNodes)
            {
                if (heightAdjustment)
                    UpdateHeight(CurrentPos, out CurrentPos);
                float degrees = 360f / CircleNodeDensity;// total nodes
                degrees *= (float)y; // num nodes
                degrees += 90;
                CurrentPos.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
                CurrentPos.x *= CircleRadius; // radius
                CurrentPos.z = Mathf.Sin(Mathf.Deg2Rad * degrees);
                CurrentPos.z *= CircleRadius; // radius
                CurrentPos.z += pos.z;
                CurrentPos.x += pos.x;
                node.Point = 0;
                node.Enabled = true;
                node.DotVert = 0;
                node.DotHoz = 0;
                node.Position = CurrentPos;
                y++;
            }
        }

        void DrawDonut()
        {
            ParentPos = anchor.position;
            Vector3 pos = ParentPos;
            CurrentPos = pos;
            float z = 0;
            float d = 0;
            for (int i = 0; i < DonutNumberOfRings; i++)
            {
                for (int y = 0; y < DonutNodeDensity; y++)
                {
                    if (heightAdjustment)
                        UpdateHeight(CurrentPos, out CurrentPos);
                    float degrees = 360f / DonutNodeDensity;// total nodes
                    degrees *= (float)y; // num nodes
                    degrees += 90 + d;
                    if (i > 0)
                    {
                        CurrentPos.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
                        CurrentPos.x *= DonutRadius + DonutInnerSpacing + z; // radius
                        CurrentPos.z = Mathf.Sin(Mathf.Deg2Rad * degrees);
                        CurrentPos.z *= DonutRadius + DonutInnerSpacing + z; // radius
                    }
                    else
                    {
                        CurrentPos.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
                        CurrentPos.x *= DonutRadius; // radius
                        CurrentPos.z = Mathf.Sin(Mathf.Deg2Rad * degrees);
                        CurrentPos.z *= DonutRadius; // radius
                    }
                    CurrentPos.z += pos.z;
                    CurrentPos.x += pos.x;
                    eqsNodes[i * (int)DonutNodeDensity + y].Point = 0;
                    eqsNodes[i * (int)DonutNodeDensity + y].Enabled = true;
                    eqsNodes[i * (int)DonutNodeDensity + y].DotVert = 0;
                    eqsNodes[i * (int)DonutNodeDensity + y].DotHoz = 0;
                    eqsNodes[i * (int)DonutNodeDensity + y].Position = CurrentPos;
                }
                z += DonutOuterSpacing;
                d += DonutSpiralOffSet;
            }
        }

        public void InitEQS()
        {
            CurrentShape = Shape;
            m_Spacing = (nodeCheckRadius * 2) * GridSpacing;
            eqsNodes = new List<EQSNode>();

            InitShape();

            initialized = true;
            Vector3 pos = Vector3.zero;
            UpdateEQS();
            beginEQS = true;
        }

        void ShapeChangeCheck()
        {
            if (CurrentShape != Shape)
            {
                beginEQS = false;
                InitEQS();
            }

            switch (Shape)
            {
                case EQSshape.Grid:
                    if (CurrentGridDensity != GridDensity)
                    {
                        beginEQS = false;
                        InitEQS();
                    }
                    break;
                case EQSshape.Circle:
                    if (CurrentCircleNodeDensity != CircleNodeDensity)
                    {
                        beginEQS = false;
                        InitEQS();
                    }
                    break;
                case EQSshape.Donut:
                    if (CurrentDonutNodeDensity != DonutNodeDensity || CurrentDonutNumberOfRings != DonutNumberOfRings)
                    {
                        beginEQS = false;
                        InitEQS();
                    }
                    break;
                default:
                    break;
            }
        }

        float counter = 0;
        Vector3 bestnode = Vector3.zero;
        void Update()
        {
            ShapeChangeCheck();
            if (!beginEQS)
                return;
            UpdateEQS();
            counter += Time.deltaTime;
            if (counter > 3)
            {
                counter = 0;
                if (bestNodeRange)
                    bestnode = GetBestPositionInRange(bestPointRangeMin, bestPointRangeMax);
                else
                    bestnode = GetBestPosition();
            }
        }

        void UpdateEQS()
        {
            m_Debug = SeeDebug;
            LargestPoint = 0;
            SmallestPoint = 10000000000000000000;

            DrawShape();

            if (pathfindingTest)
                PathfindingTest();
            if (overlapTest)
                OverlapTest();
            if (pathExistsTest)
                PathExistsTest();
            if (traceTest)
                TraceTest(hide);
            if (distanceTest)
                DistanceTest();
            if (dotTest)
            {
                if (dotTestAddPoints)
                    DotTestAddPoints(wantedHoz, wantedVert);
                else
                    DotTest();
            }
        }

        public List<EQSNode> GetEQSInfromation()
        {
            return eqsNodes;
        }

        public Vector3 GetBestPosition()
        {
            List<EQSNode> bestNodes = new List<EQSNode>();
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;
                if (node.Point == LargestPoint)
                    bestNodes.Add(node);
            }

            if (bestNodes.Count == 1)
                return bestNodes[0].Position;
            else if (bestNodes.Count > 1)
                return GetRandomBestPosition(bestNodes);
            else
                return Vector3.zero;
        }

        public Vector3 GetBestPositionInRange(float min, float max)
        {
            List<EQSNode> bestNodes = new List<EQSNode>();
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;
                if (node.Point >= min && node.Point <= max)
                    bestNodes.Add(node);
            }

            if (bestNodes.Count == 1)
                return bestNodes[0].Position;
            else if (bestNodes.Count > 1)
                return GetRandomBestPosition(bestNodes);
            else
                return Vector3.zero;
        }

        public Vector3 GetRandomBestPosition(List<EQSNode> bestNodes)
        {
            int rand = Random.Range(0, bestNodes.Count);
            return bestNodes[rand].Position;
        }

        float GetMaxDist()
        {
            float maxDist = 0;
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;

                float dist = Mathf.Sqrt((target.position - node.Position).sqrMagnitude);
                if (dist > maxDist)
                {
                    maxDist = dist;
                }
            }
            return maxDist;
        }

        float GetMinDist()
        {
            float minDist = 10000000000000000000;
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;

                float dist = Mathf.Sqrt((target.position - node.Position).sqrMagnitude);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
            return minDist;
        }

        void DotTestAddPoints(float wantedHoz, float wantedVert)
        {
            DotTest();

            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;

                if (wantedVert < 0)
                {
                    node.Point += node.DotVert.WeightedNormalizeToRange(-1, 1, 0, 1, 1);
                    node.Point.WeightedNormalizeToRange (0, 2, 0, 1, 1);
                }
                else if (wantedVert > 0)
                {
                    node.Point += node.DotVert.WeightedNormalizeToRange(-1, 1, 0, 1, -1);
                    node.Point.WeightedNormalizeToRange(0, 2, 0, 1, 1);
                }

                if (wantedHoz < 0)
                {
                    node.Point += node.DotHoz.WeightedNormalizeToRange( - 1, 1, 0, 1, 1);
                    node.Point.WeightedNormalizeToRange( 0, 2, 0, 1, 1);
                }
                else if (wantedHoz > 0)
                {
                    node.Point += node.DotHoz.WeightedNormalizeToRange( - 1, 1, 0, 1, -1);
                    node.Point.WeightedNormalizeToRange( 0, 2, 0, 1, 1);
                }
            }
        }

        void DotTest()
        {
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;

                Vector3 objDir;
                Vector3 objectiveDir;
                Vector3 objectiveRight;

                objDir = (target.position - node.Position).normalized;
                objDir.y = 0;
                objectiveDir = target.forward;
                objectiveDir.y = 0;
                node.DotVert = Vector3.Dot(objectiveDir, objDir);
                objectiveRight = target.right;
                objectiveRight.y = 0;
                node.DotHoz = Vector3.Dot(objectiveRight, objDir);
            }
        }

        void TraceTest(bool hide)
        {
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;
                RaycastHit hit;
                var heading = target.position - node.Position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                if (!Physics.Raycast(node.Position, direction, out hit, Mathf.Infinity, traceTestLayer))
                {
                    node.Enabled = hide ? true : false;
                    continue;
                }
                node.Enabled = hide ? hit.collider.transform != target : hit.collider.transform == target;

                if (!SeeDebugTrace)
                    continue;
                if (node.Enabled)
                    Debug.DrawRay(node.Position, direction * hit.distance, DebugTracePositive);
                else
                    Debug.DrawRay(node.Position, direction * hit.distance, DebugTraceNegative);
            }
        }

        void PathfindingTest()
        {
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;
                NavMeshHit hit;
                node.Enabled = NavMesh.SamplePosition(node.Position, out hit, pathfindingCheckRadius, NavMesh.AllAreas);
            }
        }

        void PathExistsTest()
        {
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;
                NavMeshPath path = new NavMeshPath();
                node.Enabled = NavMesh.CalculatePath(ParentPos, node.Position, NavMesh.AllAreas, path);
            }
        }

        void DistanceTest()
        {
            float maxDist = GetMaxDist();
            float minDist = GetMinDist();
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;

                float dist = Mathf.Sqrt((target.position - node.Position).sqrMagnitude);

                if (dist <= minimumDistanceTest)
                {
                    node.Enabled = false;
                    continue;
                }

                node.Point = dist.WeightedNormalizeToRange(minDist, maxDist, 0, 1, eqsPointWeight);

                if (LargestPoint < node.Point)
                {
                    LargestPoint = node.Point;
                }
                if (SmallestPoint > node.Point)
                {
                    SmallestPoint = node.Point;
                }
            }
        }

        void OverlapTest()
        {
            foreach (var node in eqsNodes)
            {
                if (!node.Enabled)
                    continue;

                if (node == null)
                    continue;
                RaycastHit hit;
                Vector3 Pos = node.Position;
                Pos.y += 1;
                node.Enabled = !Physics.CheckSphere(node.Position, nodeCheckRadius);
            }
        }

        void UpdateHeight(Vector3 CurrentPos, out Vector3 PosOut)
        {
            RaycastHit hit;
            Vector3 TempPos = CurrentPos;
            TempPos.y = 100000;
            if (Physics.Raycast(TempPos,
                              Vector3.down,
                              out hit,
                              Mathf.Infinity,
                              Layer))
            {

                TempPos.y = hit.point.y + ((nodeCheckRadius / 2) * 2.3f);
            }
            else
            {
                TempPos.y = 0;
            }
            PosOut = TempPos;
        }

        #region Debug
        void OnDrawGizmosSelected()
        {
            if (!m_Debug || !initialized)
                return;

            foreach (var node in eqsNodes)
            {
                if (SeeDebugPathfinding)
                {
                    Gizmos.color = DebugPathfindingColour;
                    Gizmos.DrawWireSphere(node.Position, pathfindingCheckRadius);
                }

                float p = (node.Point / LargestPoint);
                if (node.Enabled)
                {
                    Gizmos.color = debugNodeColours.Evaluate(p);
                    if (SeeDebugPointText)
                    {
                        Vector3 pos = node.Position;
                        pos.y += nodeCheckRadius * 2;
                        DrawGizmoString(node.Point.ToString(), pos, DebugPointTextColour);
                    }
                    if (SeeDebugDotText)
                    {
                        Vector3 pos = node.Position;
                        DrawGizmoString(node.DotVert.ToString(), pos, DebugDotTextColour);
                        pos = node.Position;
                        pos.y -= (nodeCheckRadius * 2);
                        DrawGizmoString(node.DotHoz.ToString(), pos, DebugDotTextColour);
                    }
                }
                else
                {
                    Gizmos.color = nullDebugNodeColour;
                }
                Gizmos.DrawWireSphere(node.Position, nodeCheckRadius);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(bestnode, nodeCheckRadius * 2);
            }
            // m_Debug = false;
        }

        void DrawGizmoString(string text, Vector3 worldPos, Color? colour = null)
        {
            UnityEditor.Handles.BeginGUI();

            var restoreColor = GUI.color;

            if (colour.HasValue) GUI.color = colour.Value;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

            if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
            {
                GUI.color = restoreColor;
                UnityEditor.Handles.EndGUI();
                return;
            }

            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
        }
        #endregion
    }

    public class EQSNode
    {
        public Vector3 Position;
        public bool Enabled = true;
        public float Point = Mathf.Infinity;
        public float DotVert = 0;
        public float DotHoz = 0;
    }
}