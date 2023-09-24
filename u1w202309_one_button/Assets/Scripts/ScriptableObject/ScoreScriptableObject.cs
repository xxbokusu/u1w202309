using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObject {
    /*
     * <summery>
     * プレイ時のスコアを管理するScriptableObject
     * 最終的にはrankingに送信する
     * </summery>
     */
    [CreateAssetMenu(fileName = "Score", menuName = "ScriptableObject/Score")]
    public class ScoreScriptableObject : UnityEngine.ScriptableObject {
        [SerializeField] private int score = 0;
        public void AddScore(int addScore) {
            score += addScore;
        }
        public int Score {
            get { return score; }
            set { score = value; }
        }
    }
}