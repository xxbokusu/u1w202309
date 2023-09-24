using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace unity1week202309.Controller {
    public class PowerChargerController : MonoBehaviour {
        private RectTransform _heartTransform;
        private RectTransform _thunderTransform;
        
        void Start() {
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
        
        private async UniTaskVoid ChargeByInputAsync(CancellationToken token) {
            while (token.IsCancellationRequested == false) {
                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            
                // アタッチした画像を振動させる by Dotween
                _thunderTransform.gameObject.SetActive(true);
                this.transform.DOShakePosition(0.5f, 10.0f, 90, 90, false, true);
                // 振動が止まるのを待って消す
                await UniTask.WaitUntil(() => !DOTween.IsTweening(this.transform), cancellationToken: token);
                _thunderTransform.gameObject.SetActive(false);
            }
        }
    }
}