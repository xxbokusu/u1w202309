using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace unity1week202309.Manager {
    /*
     * <summery>
     * リザルトシーンの進行を管理する
     * unityroomのランキングAPIを活用して送信する
     * https://help.unityroom.com/how-to-implement-scoreboard
     * </summery>
     */
    class ResultSceneManager : GameSceneManager {
        private enum ResultSceneState {
            Initialize,
            Working,
            Transitioning,
        }
        private ResultSceneState _state = ResultSceneState.Initialize;
        void Start() {
            Initialize();
            ChangeSceneAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void Initialize() {
        }
        
        private async UniTaskVoid ChangeSceneAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);
            _state = ResultSceneState.Working;

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            _state = ResultSceneState.Transitioning;
            SceneTransitionManager.Instance.ChangeScene(Scene.Title);
        }

    }
}