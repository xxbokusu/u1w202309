using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Working,
            Transitioning
        }
        private TitleSceneState _state = TitleSceneState.Waiting;
        public bool IsWorking { get { return _state == TitleSceneState.Working; } }

        void Start() {
            Initialize();
            ChangeSceneAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        public override void Initialize() {
            _state = TitleSceneState.Waiting;
            SoundManager.Instance.PlayBGM("Sparrow-Real_Ambi01-1");
        }

        private async UniTaskVoid ChangeSceneAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);
            _state = TitleSceneState.Working;

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            _state = TitleSceneState.Transitioning;
            SceneTransitionManager.Instance.ChangeScene(Scene.Main);
        }
    }
}