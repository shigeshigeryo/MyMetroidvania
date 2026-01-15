using MyMetroidVania.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace MyMetroidVania.System.UI
{
    public class LifeUI : MonoBehaviour
    {
        [SerializeField] private StatusManager _statusManager;
        [SerializeField, Tooltip("ライフの画像")] private Sprite _lifeSprite;
        [SerializeField, Tooltip("ライフの画像（欠損）")] private Sprite _deficitLifeSprite;
        [SerializeField, Tooltip("ライフの画像（左から）")] private Image[] _lifeImageList;

        private void Start()
        {
            _statusManager.OnLifeCountChanged += UpdateLifeCount;
            _statusManager.OnLifeChanged += UpdateValue;
        }

        /// <summary>
        /// ステータスのライフ数の情報でUIを更新
        /// </summary>
        /// <param name="value"></param>
        private void UpdateLifeCount(int value)
        {
            for (int i = 0; i < _lifeImageList.Length; i++)
            {
                if (i < value)
                {
                    _lifeImageList[i].enabled = true;
                }
                else
                {
                    _lifeImageList[i].enabled = false;
                }
            }
        }

        /// <summary>
        /// ステータスのライフの情報でUIを更新
        /// </summary>
        /// <param name="value">残りライフ</param>
        private void UpdateValue(int value)
        {
            for (int i = 0; i < _lifeImageList.Length; i++)
            {
                if (i < value)
                {
                    _lifeImageList[i].sprite = _lifeSprite;
                }
                else
                {
                    _lifeImageList[i].sprite = _deficitLifeSprite;
                }
            }
        }

        private void OnDestroy()
        {
            _statusManager.OnLifeCountChanged -= UpdateLifeCount;
            _statusManager.OnLifeChanged -= UpdateValue;
        }
    }
}