using System.Threading;
using Cysharp.Threading.Tasks;
using ScriptableObject;
using TMPro;
using unity1week202309.Controller;
using UnityEngine;
using unityroom.Api;

namespace unity1week202309.Manager {
    /*
     * <summery>
     * メインシーンの進行を管理する
     * 時間制限付きなので時間を管理し、残時間に応じて表現が変遷する
     * </summery>
     */
    class MainSceneManager : BaseSceneManager {
        private enum MainSceneState {
            Initialize,
            Playing,
            Result,
        }

        private MainSceneState _currentState = MainSceneState.Initialize;
        public bool IsResult => _currentState == MainSceneState.Result;

        private bool CanTransition() {
            return IsResult;
        }

        GameObject _unityChan;
        private GirlController _girlController;
        [SerializeField] private CameraViewController _cameraViewController;

        [SerializeField] private float playingResource = 50.0f;

        // Result時に表示するPanel
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI scoreText;

        // resourceを時間に見立てて、残量に応じてDirectional Lightの向きと色を変える
        [SerializeField] private Light directionalLight;

        public void WasteResource(float waste) {
            playingResource -= waste;
            if (playingResource < 0.0f) {
                playingResource = 0.0f;
            }
        }

        public bool IsResourceEmpty => playingResource <= 0.0f;

        [SerializeField] private ScoreScriptableObject scoreScriptableObject;

        public void AddScore(float score) {
            scoreScriptableObject.AddScore(score);
        }

        private void Start() {
            Initialize();
            TransitionAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void Initialize() {
            if (_cameraViewController == null) {
                Debug.Util.LogError("MainSceneManager::Start()::_cameraViewController is null");
                return;
            }

            if (scoreScriptableObject == null) {
                Debug.Util.LogError("MainSceneManager::Start()::scoreScriptableObject is null");
                return;
            }

            if (resultPanel == null) {
                Debug.Util.LogError("MainSceneManager::Start()::resultPanel is null");
                return;
            }

            if (scoreText == null) {
                Debug.Util.LogError("MainSceneManager::Start()::scoreText is null");
                return;
            }

            if (directionalLight == null) {
                Debug.Util.LogError("MainSceneManager::Start()::directionalLight is null");
                return;
            }

            resultPanel.SetActive(false);

            SoundManager.Instance.PlayBGM("Picnic-Xy02-2(Slow)");
            var groundCube = ObjectsManager.Instance.GetObject("Prefab/GameStageTerrain");
            // var unityChan = ObjectsManager.Instance.GetObject("Prefab/unitychan_dynamic");
            var unityChan = ObjectsManager.Instance.GetObject("Prefab/RAYNOS-chan");
            if (unityChan == null) {
                Debug.Util.LogError("MainSceneManager::Initialize()::unitychan is null");
                return;
            }

            Instantiate(groundCube, new Vector3(-75.0f, 1.0f, -75.0f), Quaternion.identity, transform);
            // 右向きに生成する
            _unityChan = Instantiate(unityChan, new Vector3(0.0f, 1.5f, 0.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f),
                transform);
            _girlController = _unityChan.GetComponent<GirlController>();
            if (_girlController == null) {
                Debug.Util.LogError("MainSceneManager::Initialize()::_girlController is null");
                return;
            }

            _cameraViewController.SetCharaObject(_unityChan);
        }

        private void Update() {
            if (IsResult) return;
            // resourceを時間に見立てて、残量に応じてDirectional Lightの向きと色を変える
            var rate = playingResource / 50.0f;
            directionalLight.transform.rotation = Quaternion.Euler(90.0f * rate, 0.0f, 0.0f);
            // 残量が経るにつれて夕焼け色(赤)になる
            directionalLight.color = new Color(1.0f, 0.5f + 0.5f * rate, 0.5f + 0.5f * rate);
        }

        private async UniTaskVoid TransitionAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);
            _currentState = MainSceneState.Playing;

            await UniTask.WaitUntil(() => playingResource <= 0.0f, cancellationToken: token);
            _currentState = MainSceneState.Result;
            SoundManager.Instance.PlaySE("Crow-Real_part");
            // Score表示用のTextMeshproを更新
            var score = (int)scoreScriptableObject.Score;
            scoreText.text = score.ToString();
            resultPanel.SetActive(true);
            // C#スクリプトの冒頭に `using unityroom.Api;` を追加してください。
            // ボードNo1にスコア123.45fを送信する。
            UnityroomApiClient.Instance.SendScore(1, score, ScoreboardWriteMode.HighScoreDesc);

            // 3秒待ってスペースキーでリザルトへ
            await UniTask.Delay(3000, cancellationToken: token);
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0), cancellationToken: token);
            SoundManager.Instance.StopBGM("Picnic-Xy02-2(Slow)");
            SceneTransitionManager.Instance.ChangeScene(Scene.Title);
        }
    }
}