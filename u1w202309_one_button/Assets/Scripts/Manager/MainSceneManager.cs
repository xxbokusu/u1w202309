using System;
using UnityEngine;

namespace unity1week202309.Manager {
    /*
     * <summery>
     * メインシーンの進行を管理する
     * 時間制限付きなので時間を管理し、残時間に応じて表現が変遷する
     * </summery>
     */
    class MainSceneManager : GameSceneManager {
        GameObject _unityChan = null;
        void Start() {
            Initialize();
        }

        public override void Initialize() {
            var groundCube = ObjectsManager.Instance.GetObject("Prefab/GroundCube");
            var unityChan = ObjectsManager.Instance.GetObject("Prefab/unitychan_dynamic");
            if (unityChan == null) {
                Debug.Util.LogError("MainSceneManager::Initialize()::unitychan is null");
                return;
            }
            
            Instantiate(groundCube, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            _unityChan = Instantiate(unityChan, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        }

        void Update() {
            WatchSceneState();
            // unitychanの位置を表示する
            Debug.Util.LogFormat("unitychan position: {0}", _unityChan.transform.position);
        }

        public override void WatchSceneState() {
            if (Input.GetMouseButtonDown(0)) {
                SceneTransitionManager.Instance.ChangeScene(Scene.Result);
            }
        }
    }
}