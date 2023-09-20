using UnityEngine;

namespace unity1week202309.Manager
{
    /*
     * <summery>
     * タイトルシーンの進行を管理する
     * </summery>
     */
    class TitleSceneManager: GameSceneManager {
        private enum TitleSceneState
        {
            Waiting,
            Working
        }
        private TitleSceneState _state = TitleSceneState.Waiting;
        public bool IsWorking { get { return _state == TitleSceneState.Working; } }

        void Start() {
            Initialize();
        }
        public override void Initialize() {
            _state = TitleSceneState.Working;
        }

        void Update() {
            WatchSceneState();
        }
        public override void WatchSceneState() {
            switch (_state) {
                case TitleSceneState.Waiting:
                    _state = TitleSceneState.Working;
                    break;
                case TitleSceneState.Working:
                    if (Input.GetMouseButtonDown(0)) {
                        SceneTransitionManager.Instance.ChangeScene(Scene.Main);
                    }
                    break;
            }
        }
    }
}