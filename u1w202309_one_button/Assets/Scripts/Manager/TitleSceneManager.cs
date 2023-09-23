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
            _state = TitleSceneState.Waiting;
            SoundManager.Instance.PlayBGM("Sparrow-Real_Ambi01-1");
        }

        void Update() {
            WatchSceneState();
        }
        public override void WatchSceneState() {
            switch (_state) {
                // 徐々にLogoをFade outし, 完了するとWorkingへ
                case TitleSceneState.Waiting:
                    _state = TitleSceneState.Working;
                    break;
                case TitleSceneState.Working:
                    // スペースキー入力でMainシーンへ遷移
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        SceneTransitionManager.Instance.ChangeScene(Scene.Main);
                    }
                    break;
            }
        }
    }
}