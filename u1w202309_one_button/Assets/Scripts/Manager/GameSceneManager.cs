using UnityEngine;

namespace unity1week202309.Manager
{
    abstract class GameSceneManager : MonoBehaviour {
        public abstract void Initialize();
        public abstract void WatchSceneState();
    }
}