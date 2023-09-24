using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace unity1week202309.Controller {
    public class PowerChargerController : MonoBehaviour {
        private RectTransform _heartTransform;
        private RectTransform _thunderTransform;
        
        // chargeされたパワー. 振動の度合いに影響する
        [SerializeField] private float _power = 0.0f;
        public float Power => _power;
        
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
            ChargeByInputAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private void Update() {
            // 時間に応じてパワーは減衰する
            _power -= Time.deltaTime;
            if (_power < 0.0f) {
                _power = 0.0f;
            }
        }
        
        private async UniTaskVoid ChargeByInputAsync(CancellationToken token) {
            while (token.IsCancellationRequested == false) {
                await UniTask.WaitUntil(() => Input.anyKeyDown, cancellationToken: token);
            
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
}