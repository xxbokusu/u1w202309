using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace unity1week202309.Object {
    public class FadeCanvas : MonoBehaviour
    {
        public static bool isFadeInstance = false;
        // フェードイン/フェードアウトフラグ
        private bool isFadeIn = false;
        public bool IsFadeIn { get { return this.isFadeIn; } }
        private bool isFadeOut = false;
        public bool IsFadeOut { get { return this.isFadeOut; } }

        // フェードを管理するパラメータ
        public float fadeAlpha = 0.0f;
        public float fadeSpeed = 0.2f;

        // Start is called before the first frame update
        void Start()
        {
            if (isFadeInstance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
                isFadeInstance = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public async UniTaskVoid SceneFadeInAsync(CancellationToken token) {
            this.isFadeIn = true;
            this.isFadeOut = false;
            while (token.IsCancellationRequested == false && this.fadeAlpha > 0.0f) {
                this.fadeAlpha -= this.fadeSpeed;
                this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, this.fadeAlpha);
                await UniTask.DelayFrame(10, cancellationToken: token);
            }
            this.fadeAlpha = 0.0f;
            this.isFadeIn = false;
            Debug.Util.Log("FadeCanvas::SceneFadeInAsync()::fadeAlpha = " + this.fadeAlpha);
        }

        public async UniTaskVoid SceneFadeOutAsync(CancellationToken token) {
            isFadeIn = false;
            isFadeOut = true;
            while (token.IsCancellationRequested == false && fadeAlpha < 1.0f) {
                fadeAlpha += fadeSpeed;
                GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, fadeAlpha);
                await UniTask.DelayFrame(10, cancellationToken: token);
            }
            fadeAlpha = 1.0f;
            isFadeOut = false;
            Debug.Util.Log("FadeCanvas::SceneFadeOutAsync()::fadeAlpha = " + fadeAlpha);
        }
    }
}