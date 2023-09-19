using UnityEngine;

namespace unity1week202309.Manager
{
    /*
     * <summery>
     * リザルトシーンの進行を管理する
     * unityroomのランキングAPIを活用して送信する
     * https://help.unityroom.com/how-to-implement-scoreboard
     * </summery>
     */
    class ResultSceneManager: GameSceneManager
    {
        void Start() {
            Initialize();
        }
        public override void Initialize() {
            
        }

        void Update() {
            WatchSceneState();
        }
        public override void WatchSceneState() {
            if (Input.GetMouseButtonDown(0)) {
                SceneTransitionManager.Instance.ChangeScene(Scene.Title);
            }
        }
    }
}