using System.Threading;
using Cysharp.Threading.Tasks;
using ScriptableObject;
using UnityEngine;
using UnityEngine.Assertions;
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
        
        [SerializeField] private ScoreScriptableObject scoreScriptableObject;

        private void Start() {
            Initialize();
            ChangeSceneAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        public override void Initialize() {
            _state = TitleSceneState.Waiting;
            SoundManager.Instance.PlayBGM("Sparrow-Real_Ambi01-1");
            
            if (scoreScriptableObject == null) {
                Debug.Util.LogError("TitleSceneManager::Initialize()::scoreScriptableObject is null");
                return;
            }
            scoreScriptableObject.Score = 0;
        }

        private async UniTaskVoid ChangeSceneAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);
            _state = TitleSceneState.Working;

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            _state = TitleSceneState.Transitioning;
            SoundManager.Instance.StopBGM("Sparrow-Real_Ambi01-1");
            SoundManager.Instance.PlaySE("Bush_Warbler_part");
            SceneTransitionManager.Instance.ChangeScene(Scene.Main);
        }
    }
}