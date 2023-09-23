using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using unity1week202309.Controller;
using UnityEngine;

namespace unity1week202309.Manager {
    /*
     * <summery>
     * メインシーンの進行を管理する
     * 時間制限付きなので時間を管理し、残時間に応じて表現が変遷する
     * </summery>
     */
    class MainSceneManager : GameSceneManager {
        GameObject _unityChan;
        private GirlController _girlController;
        [SerializeField]Camera _camera;

        void Start() {
            if (_camera == null) {
                Debug.Util.LogError("MainSceneManager::Start()::_camera is null");
                return;
            }
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
            // 右向きに生成する
            _unityChan = Instantiate(unityChan, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
            _girlController = _unityChan.GetComponent<GirlController>();
            if (_girlController == null) {
                Debug.Util.LogError("MainSceneManager::Initialize()::_girlController is null");
                return;
            }
        }

        async void Update() {
            WatchSceneState();
            // unitychanの位置を表示する
            Debug.Util.LogFormat("unitychan position: {0}", _unityChan.transform.position);
            // unitychanを右に移動させ、カメラも追従する
            if (!_girlController.IsWalking) return;

            var moveVector = new Vector3(1, 0, 0);
            if (_girlController.IsRunning) moveVector *= 2;

            _unityChan.transform.DOMove(moveVector, 1).SetRelative(true);
            _camera.transform.DOMove(moveVector, 1).SetRelative(true);
            await UniTask.Delay(1000);
        }

        public override void WatchSceneState() {
            if (Input.GetMouseButtonDown(0)) {
                SceneTransitionManager.Instance.ChangeScene(Scene.Result);
            }
        }
    }
}