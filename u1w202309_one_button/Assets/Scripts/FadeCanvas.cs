using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace unity1week202309.Object {
    public class FadeCanvas : MonoBehaviour {
        private static bool _isFadeInstance = false;

        // フェードイン/フェードアウトフラグ
        private bool _isFadeIn = false;
        public bool IsFadeIn => _isFadeIn;
        private bool _isFadeOut = false;
        public bool IsFadeOut => _isFadeOut;
        public bool IsInFade => _isFadeIn || _isFadeOut;

        // フェードを管理するパラメータ
        public float fadeAlpha;
        [SerializeField] private float fadeSpeed = 0.5f;

        // Start is called before the first frame update
        void Start() {
            if (_isFadeInstance) {
                Destroy(this);
            }
            else {
                DontDestroyOnLoad(this);
                _isFadeInstance = true;
            }
        }

        public async UniTaskVoid SceneFadeInAsync(CancellationToken token) {
            _isFadeIn = true;
            _isFadeOut = false;
            while (token.IsCancellationRequested == false && fadeAlpha > 0.0f) {
                fadeAlpha -= fadeSpeed;
                GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, fadeAlpha);
                await UniTask.DelayFrame(10, cancellationToken: token);
            }

            fadeAlpha = 0.0f;
            _isFadeIn = false;
            Debug.Util.Log("FadeCanvas::SceneFadeInAsync()::fadeAlpha = " + fadeAlpha);
            this.gameObject.SetActive(false);
        }

        public async UniTaskVoid SceneFadeOutAsync(CancellationToken token) {
            this.gameObject.SetActive(true);
            _isFadeIn = false;
            _isFadeOut = true;
            while (token.IsCancellationRequested == false && fadeAlpha < 1.0f) {
                fadeAlpha += fadeSpeed;
                GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, fadeAlpha);
                await UniTask.DelayFrame(10, cancellationToken: token);
            }

            fadeAlpha = 1.0f;
            _isFadeOut = false;
            Debug.Util.Log("FadeCanvas::SceneFadeOutAsync()::fadeAlpha = " + fadeAlpha);
        }
    }
}