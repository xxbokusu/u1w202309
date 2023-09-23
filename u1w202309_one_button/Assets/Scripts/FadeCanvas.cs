using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace unity1week202309.Object {
    public class FadeCanvas : MonoBehaviour
    {
        private static bool _isFadeInstance = false;
        // フェードイン/フェードアウトフラグ
        private bool _isFadeIn = false;
        public bool IsFadeIn => this._isFadeIn;
        private bool _isFadeOut = false;
        public bool IsFadeOut => this._isFadeOut;

        // フェードを管理するパラメータ
        public float fadeAlpha = 0.0f;
        public float fadeSpeed = 0.2f;

        // Start is called before the first frame update
        void Start()
        {
            if (_isFadeInstance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
                _isFadeInstance = true;
            }
        }
        
        public async UniTaskVoid SceneFadeInAsync(CancellationToken token) {
            this._isFadeIn = true;
            this._isFadeOut = false;
            while (token.IsCancellationRequested == false && this.fadeAlpha > 0.0f) {
                this.fadeAlpha -= this.fadeSpeed;
                this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, this.fadeAlpha);
                await UniTask.DelayFrame(10, cancellationToken: token);
            }
            this.fadeAlpha = 0.0f;
            this._isFadeIn = false;
            Debug.Util.Log("FadeCanvas::SceneFadeInAsync()::fadeAlpha = " + this.fadeAlpha);
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