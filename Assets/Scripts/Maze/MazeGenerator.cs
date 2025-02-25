using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Maze
{
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] private MazeNode _prefabMazeNode;
        private int _mazeWidth;
        private int _mazeHeight;
        private MazeNode[,] Nodes;

        private IEnumerator Start()
        {
            _mazeHeight = GameManager.instance.MazeSize.y;
            _mazeWidth = GameManager.instance.MazeSize.x;

            Nodes = new MazeNode[_mazeWidth, _mazeHeight];

            int x = 0, z = 0;
            while (x < _mazeWidth)
            {
                while (z < _mazeHeight)
                {
                    Nodes[x, z] = Instantiate(_prefabMazeNode, new Vector3(x, 0, z), Quaternion.identity);
                    z++;
                }
                x++;
                z = 0;
            }

            yield return GenereteMazeNode(null, Nodes[0, 0]);

            GameManager.instance.SetNodesTable(Nodes);
            GameManager.instance.CompleteMazeGenerate();
        }

        private IEnumerator GenereteMazeNode(MazeNode previousNode, MazeNode currentNode)
        {
            currentNode.Visible();
            ClearWall(previousNode, currentNode);

            yield return new WaitForSeconds(0.01f);

            MazeNode nextNode;

            do
            {
                nextNode = GetNextUnivisitedNode(currentNode);

                if (nextNode != null) yield return GenereteMazeNode(currentNode, nextNode);
            } while (nextNode != null);
        }

        private MazeNode GetNextUnivisitedNode(MazeNode currentNode)
        {
            var unvisitedNode = GetUnvisitedNode(currentNode);

            return unvisitedNode.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
        }

        private IEnumerable<MazeNode> GetUnvisitedNode(MazeNode currentNode)
        {
            int x = (int)currentNode.transform.position.x;
            int z = (int)currentNode.transform.position.z;

            if ((x + 1) < _mazeWidth)
            {
                var nodeToRight = Nodes[x + 1, z];
                if (!nodeToRight.isVisible)
                {
                    yield return nodeToRight;
                }
            }

            if ((x - 1) >= 0)
            {
                var nodeToLeft = Nodes[x - 1, z];
                if (!nodeToLeft.isVisible)
                {
                    yield return nodeToLeft;
                }
            }

            if ((z + 1) < _mazeHeight)
            {
                var nodeToFront = Nodes[x, z + 1];
                if (!nodeToFront.isVisible)
                {
                    yield return nodeToFront;
                }
            }

            if ((z - 1) >= 0)
            {
                var nodeToBack = Nodes[x, z - 1];
                if (!nodeToBack.isVisible)
                {
                    yield return nodeToBack;
                }
            }
        }

        private void ClearWall(MazeNode previousNode, MazeNode currentNode)
        {
            if (previousNode == null) return;

            if (previousNode.transform.position.x < currentNode.transform.position.x)
            {
                previousNode.ClearRightWall();
                currentNode.ClearLeftWall();
                return;
            }

            if (previousNode.transform.position.x > currentNode.transform.position.x)
            {
                previousNode.ClearLeftWall();
                currentNode.ClearRightWall();
                return;
            }

            if (previousNode.transform.position.z < currentNode.transform.position.z)
            {
                previousNode.ClearFrontWall();
                currentNode.ClearBackWall();
                return;
            }

            if (previousNode.transform.position.z > currentNode.transform.position.z)
            {
                previousNode.ClearBackWall();
                currentNode.ClearFrontWall();
                return;
            }
        }
    }
}