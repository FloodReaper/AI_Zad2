using Game.Extensions;
using Game.Maze;
using Game.SimplePathFinding;
using System;
using UnityEngine;

namespace Game
{
    public class GameManager : SingletonInstance<GameManager>
    {
        public Vector2Int MazeSize = new Vector2Int(20, 20);

        public Action MazeCompleteEvent;
        public Action PrepareCompleteEvent;

        private bool _isMazeComplete = false;
        private bool _isPreparingComplete = false;

        public bool isMazeComplete
        {
            get => _isMazeComplete;
            private set
            {
                _isMazeComplete = value;
                MazeCompleteEvent?.Invoke();
            }
        }
        public bool isPreparingComplete
        {
            get => _isPreparingComplete;
            private set
            {
                _isPreparingComplete = value;
                PrepareCompleteEvent?.Invoke();
            }
        }
        public MazeNode[,] Nodes {  get; private set; }

        public void SetNodesTable(MazeNode[,] nodes) => Nodes = nodes;
        public void CompleteMazeGenerate() => isMazeComplete = true;
        public void CompletePrepate() => isPreparingComplete = true;
    }
}