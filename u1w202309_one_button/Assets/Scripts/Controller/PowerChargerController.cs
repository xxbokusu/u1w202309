using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace unity1week202309.Controller {
    public class PowerChargerController : MonoBehaviour {
        private RectTransform _heartTransform;
        private RectTransform _thunderTransform;
        
        // chargeされたパワー. 振動の度合いに影響する
        [SerializeField] private float _power = 0.0f;
        public float Power => _power;
        private bool isActivated = false;
        public void Activate() {
            _heartTransform.gameObject.SetActive(true);
            isActivated = true;
        }
        public void Deactivate() {
            _heartTransform.gameObject.SetActive(false);
            isActivated = false;
        }
        
        private void Start() {
            _heartTransform = this.transform.GetChild(0).transform as RectTransform;
            _thunderTransform = this.transform.GetChild(1).transform as RectTransform;
            if (_heartTransform == null) {
                Debug.Util.LogError("PowerChargerController::Start()::_rectTransform is null");
                return;
            }
            if (_thunderTransform == null) {
                Debug.Util.LogError("PowerChargerController::Start()::_thunderTransform is null");
                return;
            }
            _thunderTransform.gameObject.SetActive(false);
            _heartTransform.gameObject.SetActive(false);
            var button = _heartTransform.gameObject.GetComponent<Button>();
            if (button == null) {
                Debug.Util.LogError("PowerChargerController::Start()::button is null");
                return;
            }
            button.onClick.AddListener(() => ShakeOnAction(this.GetCancellationTokenOnDestroy()));

            ChargeByInputAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private void Update() {
            // 時間に応じてパワーは減衰する
            _power -= Time.deltaTime * 2;
            if (_power < 0.0f) {
                _power = 0.0f;
            }
        }
        
        private async UniTaskVoid ChargeByInputAsync(CancellationToken token) {
            while (token.IsCancellationRequested == false) {
                await UniTask.WaitUntil(() => isActivated, cancellationToken: token);
                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);

                var task = ShakeOnAction(token);
                await UniTask.WaitUntil(() => task.Status.IsCompleted(), cancellationToken: token);
            }
        }

        private async UniTask ShakeOnAction(CancellationToken token) {
            // アタッチした画像を振動させる by Dotween
            _thunderTransform.gameObject.SetActive(true);
            _power += 2.0f;
            this.transform.DOShakePosition(_power, strength: _power, vibrato: 10, randomness: 90, snapping: false, fadeOut: true);
            // Power charge span
            await UniTask.Delay(500, cancellationToken: token);
            _thunderTransform.gameObject.SetActive(false);
        }
    }
}