using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace unity1week202309.Controller {
    /*
     * <summery>
     * カメラワークを制御するクラス
     * Titleから進行するとキャラとステージを横から写す
     * </summery>
     */
    public class CameraViewController : MonoBehaviour {
        private Camera _camera;
        private Transform _charaTransform;
        private Vector3 _prePosition;
        private bool _isInitialized = false;
        private void Start() {
            _camera = GetComponent<Camera>();
            if (_camera == null) {
                Debug.Util.LogError("CameraViewController::Start()::_camera is null");
                return;
            }
            InitializeAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private async UniTaskVoid InitializeAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => _charaTransform != null, cancellationToken: token);
            
            _isInitialized = true;
            _prePosition = _charaTransform.position;
            Debug.Util.Log("CameraViewController::InitializeAsync()::_prePosition = " + _prePosition);
        }
        
        public void SetCharaTransform(Transform charaTransform) {
            _charaTransform = charaTransform;
        }
        
        private void Update() {
            if (!_isInitialized) return;
            
            // charaとの相対距離を保ちながらカメラを移動させる
            var charaPosition = _charaTransform.position;
            var diff = charaPosition - _prePosition;
            _camera.transform.position += diff;
            _prePosition = charaPosition;
        }
    }
}