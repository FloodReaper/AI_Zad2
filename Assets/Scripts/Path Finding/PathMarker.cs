using UnityEngine;

namespace Game.SimplePathFinding
{
    public class PathMarker
    {
        public Vector2Int location;
        public float G;
        public float H;
        public float F;

        public GameObject marker;
        public PathMarker parent;

        public PathMarker(Vector2Int l, float g, float h, float f, GameObject marker, PathMarker parent)
        {
            location = l;
            G = g; //Diatance from start
            H = h; //Distance to goal
            F = f; //Sum Distance (Cost)
            this.marker = marker;
            this.parent = parent;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType())) 
                return false;
            else 
                return location.Equals(((PathMarker)obj).location);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}