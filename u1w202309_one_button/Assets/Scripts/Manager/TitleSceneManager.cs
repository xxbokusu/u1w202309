using System.Threading;
using Cysharp.Threading.Tasks;
using ScriptableObject;
using unity1week202309.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace unity1week202309.Manager
{
    /*
     * <summery>
     * タイトルシーンの進行を管理する
     * </summery>
     */
    class TitleSceneManager: BaseSceneManager {
        private enum TitleSceneState
        {
            Waiting,
            Working,
            Transitioning
        }
        private TitleSceneState _state = TitleSceneState.Waiting;
        public bool IsWorking { get { return _state == TitleSceneState.Working; } }
        
        [SerializeField] private ScoreScriptableObject scoreScriptableObject;
        [SerializeField] private PowerChargerController powerChargerController;
        [SerializeField] private bool doTransition = false;

        private void Start() {
            Initialize();
            ChangeSceneAsync(this.GetCancellationTokenOnDestroy()).Forget();
            SetTransitionFlagByKeyDownAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        public override void Initialize() {
            _state = TitleSceneState.Waiting;
            SoundManager.Instance.PlayBGM("Sparrow-Real_Ambi01-1");
            
            if (scoreScriptableObject == null) {
                Debug.Util.LogError("TitleSceneManager::Initialize()::scoreScriptableObject is null");
                return;
            }
            scoreScriptableObject.Score = 0;
            
            if (powerChargerController == null) {
                Debug.Util.LogError("TitleSceneManager::Initialize()::powerChargerController is null");
                return;
            }

            var button = powerChargerController.gameObject.GetComponentInChildren<Button>();
            if (button == null) {
                Debug.Util.LogError("TitleSceneManager::Initialize()::button is null");
                return;
            }

            button.onClick.AddListener(() => {
                doTransition = true;
                Debug.Util.Log("clicked");
            });
        }

        private async UniTaskVoid SetTransitionFlagByKeyDownAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            doTransition = true;
        }

        private async UniTaskVoid ChangeSceneAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);
            _state = TitleSceneState.Working;
            powerChargerController.Activate();

            await UniTask.WaitUntil(() => doTransition, cancellationToken: token);
            _state = TitleSceneState.Transitioning;
            SoundManager.Instance.StopBGM("Sparrow-Real_Ambi01-1");
            SoundManager.Instance.PlaySE("Bush_Warbler_part");
            SceneTransitionManager.Instance.ChangeScene(Scene.Main);
        }
    }
}