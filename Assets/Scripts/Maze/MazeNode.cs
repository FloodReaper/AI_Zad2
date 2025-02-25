using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Maze
{
    public class MazeNode : MonoBehaviour
    {
        [SerializeField] private GameObject LeftWall;
        [SerializeField] private GameObject RightWall;
        [SerializeField] private GameObject FrontWall;
        [SerializeField] private GameObject BackWall;
        [SerializeField] private GameObject UniversalBlock;

        public bool isVisible { get; private set; } = false;

        public void Visible()
        {
            isVisible = true;
            UniversalBlock.gameObject.SetActive(!isVisible);
        }

        public void ClearLeftWall() => LeftWall.SetActive(!isVisible);
        public void ClearRightWall() => RightWall.SetActive(!isVisible);
        public void ClearFrontWall() => FrontWall.SetActive(!isVisible);
        public void ClearBackWall() => BackWall.SetActive(!isVisible);

        public bool CanGoInCertainDirection(DirectionEnum direction)
        {
            switch(direction)
            {
                case DirectionEnum.Forward:
                    return !FrontWall.activeInHierarchy;
                case DirectionEnum.Backward:
                    return !BackWall.activeInHierarchy;
                case DirectionEnum.Right:
                    return !RightWall.activeInHierarchy;
                case DirectionEnum.Left:
                    return !LeftWall.activeInHierarchy;
                default:
                    return true;
            }
        }
    }

}