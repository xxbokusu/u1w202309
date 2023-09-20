using System;
using UnityEngine;

namespace unity1week202309.Manager
{
    /*
     * <summery>
     * メインシーンの進行を管理する
     * 時間制限付きなので時間を管理し、残時間に応じて表現が変遷する
     * </summery>
     */
    class MainSceneManager: GameSceneManager {
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
                SceneTransitionManager.Instance.ChangeScene(Scene.Result);
            }
        }
    }
}