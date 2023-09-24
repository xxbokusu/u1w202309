using System.Threading;
using Cysharp.Threading.Tasks;
using ScriptableObject;
using UnityEngine;
using UnityEngine.Assertions;

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
        
        [SerializeField] private ScoreScriptableObject scoreScriptableObject;
        private void Start() {
            Initialize();
            ChangeSceneAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void Initialize() {
            if (scoreScriptableObject == null) {
                Debug.Util.LogError("ResultSceneManager::Start()::scoreScriptableObject is null");
                return;
            }
        }
        
        private async UniTaskVoid ChangeSceneAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            SceneTransitionManager.Instance.ChangeScene(Scene.Title);
        }

    }
}