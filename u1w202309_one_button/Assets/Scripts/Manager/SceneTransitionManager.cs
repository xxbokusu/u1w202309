using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;

namespace unity1week202309.Manager
{
    public enum Scene
    {
        Initialize, // 初期値
        Title,
        Main,
        Result,
        Option
    }

    /*
     * <summery>
     * シーン遷移を管理するクラス
     * </summery>
     */
    public class SceneTransitionManager: MonoBehaviour {
        private static SceneTransitionManager _instance = null;
        public static SceneTransitionManager Instance {
            get {
                return _instance;
            }
        }
        
        private Scene _currentScene = Scene.Initialize;
        private Scene _nextScene = Scene.Title;
        private bool _isTransition;
        private bool _isInitialized = false;
        
        void Awake() {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            ChangeScene(_nextScene);
        }
        
        public void ChangeScene(Scene targetScene) {
            _nextScene = targetScene;
            LoadSceneAsync(_nextScene, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid LoadSceneAsync(Scene targetScene, CancellationToken token) {
            Debug.Util.LogFormat("SceneTransitionManager::loadSceneAsync()::scene transition {0} > {1}", this._currentScene ,targetScene);
            if (_currentScene != Scene.Initialize && SceneManager.GetSceneByName(_currentScene.ToString()).IsValid())
            {
                await SceneManager.UnloadSceneAsync(_currentScene.ToString());
            }
            var progress =SceneManager.LoadSceneAsync(_nextScene.ToString(), LoadSceneMode.Additive);
            _isTransition = true;

            await UniTask.WaitUntil(() => progress.isDone, cancellationToken: token);
            _isTransition = false;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(this._nextScene.ToString()));
            _currentScene = targetScene;
        }
    }
}