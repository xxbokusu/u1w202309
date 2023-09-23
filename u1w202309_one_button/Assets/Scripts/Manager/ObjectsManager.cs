using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace unity1week202309.Manager {
    public class ObjectsManager : MonoBehaviour {
        public static ObjectsManager Instance { get; private set; } = null;
        private enum ObjectState {
            None,
            Loading,
            Ready,
        }
        private ObjectState _currentState = ObjectState.None;
        public bool IsReady => _currentState == ObjectState.Ready;

        private Dictionary<string, GameObject> _objectsDictionary = new() {
            { "Prefab/unitychan_dynamic", null },
            { "Prefab/GroundCube", null}
        };
        
        private void Start() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
    
            LoadObjectsAsync(this.GetCancellationTokenOnDestroy()).Forget();
            _currentState = ObjectState.Loading;
        }
        
        private async UniTaskVoid LoadObjectsAsync(CancellationToken token) {
            var loadReqDictionary = new Dictionary<string, ResourceRequest>();
            foreach (var key in _objectsDictionary.Keys) {
                var req = Resources.LoadAsync<GameObject>(key);
                loadReqDictionary.Add(key, req);
            }
            await UniTask.WaitUntil(() => loadReqDictionary.Values.All(req => req.isDone), cancellationToken:token);
            foreach (var key in loadReqDictionary.Keys) {
                _objectsDictionary[key] = loadReqDictionary[key].asset as GameObject;
            }
            _currentState = ObjectState.Ready;
        }
        
        public GameObject GetObject(string key) {
            if (IsReady) return _objectsDictionary.TryGetValue(key, value: out var o) ? o : null;
            Debug.Util.LogError("ObjectsManager::GetObject()::ObjectsManager is not ready");
            return null;
        }
    }
}