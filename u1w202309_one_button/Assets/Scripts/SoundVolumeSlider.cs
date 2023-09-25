using UnityEngine;
using UnityEngine.UI;
using ScriptableObject;

namespace unity1week202309 {
    public class SoundVolumeSlider : MonoBehaviour {
        private Slider _slider;
        [SerializeField] private ConfigScriptableObject configScriptableObject;

        private void Start() {
            _slider = GetComponent<Slider>();
            if (_slider == null) {
                Debug.Util.LogError("SoundVolumeSlider::Start()::slider is null");
                return;
            }
            if (configScriptableObject == null) {
                Debug.Util.LogError("SoundVolumeSlider::Start()::configScriptableObject is null");
                return;
            }
            _slider.value = configScriptableObject.GetBGMVolumeRate();
        }
    }
}