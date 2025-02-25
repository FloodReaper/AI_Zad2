using UnityEngine;

namespace Game.Extensions
{
    public class SingletonInstance<T> : MonoBehaviour where T : Component
    {
        public static T instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this as T;
            else
                Destroy(this.gameObject);
        }
    }
}