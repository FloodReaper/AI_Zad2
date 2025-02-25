using UnityEngine;
using System.Linq;
using Game.Maze;
using System.Collections.Generic;
using UnityEditor;

namespace Game.SimplePathFinding
{
    public class SimplePathFinding : MonoBehaviour
    {
        public Material closedMaterial;
        public Material openMaterial;

        public GameObject start;
        public GameObject end;
        public GameObject mark;

        private MazeNode[,] nodes => GameManager.instance.Nodes;
        private List<PathMarker> openMarkers = new List<PathMarker>();
        private List<PathMarker> closedMarkers = new List<PathMarker>();

        private PathMarker goalMarker;
        private PathMarker startMarker;
        private PathMarker lastPosition;

        private bool isDone = false;

        private void Start()
        {
            GameManager.instance.MazeCompleteEvent += PrepareStartAndGoal;
        }

        private void Update()
        {
            if (GameManager.instance.isPreparingComplete && !isDone) Search(lastPosition);
            if (isDone) GetPath();
        }

        private void PrepareStartAndGoal()
        {
            if (!GameManager.instance.isMazeComplete) return;

            Vector3Int startLocation = new Vector3Int(
                (int)Random.Range(
                    0,
                    GameManager.instance.MazeSize.x
                    ),
                1,
                (int)Random.Range(
                    0,
                    GameManager.instance.MazeSize.y
                    ));

            Debug.Log($"Start Random Position: {startLocation.ToString()}");

            Vector3Int goalLocation = new Vector3Int(
                (int)Random.Range(
                    0,
                    GameManager.instance.MazeSize.x
                    ),
                1,
                (int)Random.Range(
                    0,
                    GameManager.instance.MazeSize.y
                    ));

            Debug.Log($"Goal Random Position: {goalLocation.ToString()}");

            startMarker = new PathMarker(new Vector2Int(startLocation.x, startLocation.z), 0, 0, 0,
                                Instantiate(start, startLocation, Quaternion.identity), null);
            goalMarker = new PathMarker(new Vector2Int(goalLocation.x, goalLocation.z), 0, 0, 0,
                                Instantiate(end, goalLocation, Quaternion.identity), null);

            Debug.Log($"Start Set Position: {startMarker.location.ToString()}");
            Debug.Log($"Goal Set Position: {goalMarker.location.ToString()}");

            openMarkers.Add(startMarker);
            lastPosition = startMarker;

            GameManager.instance.CompletePrepate();
        }

        private void Search(PathMarker thisMark)
        {
            Debug.Log($"This Mark: {thisMark.location.ToString()}");

            if (thisMark == null) return;
            if (thisMark.Equals(goalMarker)) { isDone = true; return; }

            foreach(DirectionEnum direction in
                new DirectionEnum[4] {DirectionEnum.Forward, DirectionEnum.Backward, DirectionEnum.Right, DirectionEnum.Left})
            {
                MazeNode node = nodes[thisMark.location.x, thisMark.location.y];
                if (!node.CanGoInCertainDirection(direction)) continue;

                Vector2Int neighbourLocation = thisMark.location;
                switch(direction)
                {
                    case DirectionEnum.Forward:
                        neighbourLocation += Vector2Int.up;
                        break;
                    case DirectionEnum.Backward:
                        neighbourLocation += Vector2Int.down;
                        break;
                    case DirectionEnum.Right:
                        neighbourLocation += Vector2Int.right;
                        break;
                    case DirectionEnum.Left:
                        neighbourLocation += Vector2Int.left;
                        break;
                }

                if (neighbourLocation.x < 0 || neighbourLocation.x >= GameManager.instance.MazeSize.x ||
                    neighbourLocation.y < 0 || neighbourLocation.y >= GameManager.instance.MazeSize.y) continue;
                if (IsClosed(neighbourLocation)) continue;

                float G = Vector2Int.Distance(thisMark.location, neighbourLocation) + thisMark.G;
                float H = Vector2Int.Distance(neighbourLocation, goalMarker.location);
                float F = G + H;

                GameObject _mark = Instantiate(mark, new Vector3(neighbourLocation.x, 1, neighbourLocation.y), Quaternion.identity);
                _mark.GetComponent<MeshRenderer>().material = openMaterial;

                if (!UpdateMark(neighbourLocation, G, H, F, thisMark))
                    openMarkers.Add(
                        new PathMarker(neighbourLocation, G, H, F, _mark, thisMark));
            }

            openMarkers = openMarkers.OrderBy(n => n.F).ThenBy(n => n.H).ToList<PathMarker>();
            PathMarker pm = (PathMarker)openMarkers.ElementAt(0);
            closedMarkers.Add(pm);
            openMarkers.RemoveAt(0);

            pm.marker.GetComponent<MeshRenderer>().material = closedMaterial;

            lastPosition = pm;
        }

        private bool IsClosed(Vector2Int markerLocation)
        {
            foreach(PathMarker mark in closedMarkers)
            {
                if(mark.location.Equals(markerLocation)) return true;
            }

            return false;
        }

        private bool UpdateMark(Vector2Int location, float g, float h, float f, PathMarker prt)
        {
            foreach (PathMarker mark in openMarkers)
            {
                if (mark.location.Equals(location))
                {
                    mark.G = g;
                    mark.H = h;
                    mark.F = f;
                    mark.parent = prt;
                    return true;
                }
            }

            return false;
        }

        private void RemoveMarks()
        {
            List<GameObject> markers = GameObject.FindGameObjectsWithTag("Marks").ToList();
            foreach (GameObject mark in markers)
            {
                Destroy(mark);
            }
        }

        private void GetPath()
        {
            RemoveMarks();
            PathMarker begin = lastPosition;

            Instantiate(end, new Vector3(begin.location.x, 1, begin.location.y), Quaternion.identity);
            begin = begin.parent;

            while (!startMarker.Equals(begin) && begin != null)
            {
                Instantiate(mark, new Vector3(begin.location.x, 1, begin.location.y), Quaternion.identity).GetComponent<MeshRenderer>().material = openMaterial;
                begin = begin.parent;
            }

            Instantiate(start, new Vector3(startMarker.location.x, 1, startMarker.location.y), Quaternion.identity).GetComponent<MeshRenderer>();
        }
    }
}