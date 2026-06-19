using MyMetroidVania.Entity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyMetroidVania.System.UI
{
    /// <summary>
    /// ライフのUIを管理
    /// </summary>
    public class LifeUI : MonoBehaviour
    {
        [SerializeField] private StatusManager _statusManager;
        [SerializeField, Tooltip("ライフの画像")] private Sprite _lifeSprite;
        [SerializeField, Tooltip("ライフの画像（欠損）")] private Sprite _deficitLifeSprite;
        [SerializeField, Tooltip("ライフの画像（左から）")] private Image[] _lifeImageList;
        [SerializeField, Tooltip("ライフ増加時の画像拡大率")] private float _scaleFactor = 1.2f;
        [SerializeField, Tooltip("ライフ拡大にかかる時間")] private float _duration = 0.5f;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            _statusManager.OnLifeCountChanged += UpdateLifeCount;
            _statusManager.OnLifeChanged += UpdateValue;
        }

        /// <summary>
        /// 最大ライフ数のUIを更新
        /// </summary>
        /// <param name="value">最大ライフ数</param>
        private void UpdateLifeCount(int value)
        {
            for (int i = 0; i < _lifeImageList.Length; i++)
            {
                if (i < value)
                {
                    if (!_lifeImageList[i].enabled)
                    {
                        StartCoroutine(ScaleRoutine(_lifeImageList[i].transform));
                    }
                    _lifeImageList[i].enabled = true;
                    _lifeImageList[i].sprite = _lifeSprite;
                }
                else
                {
                    _lifeImageList[i].enabled = false;
                }
            }
        }

        /// <summary>
        /// ライフ数のUIを更新
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

        /// <summary>
        /// ライフを一瞬大きくする
        /// </summary>
        private IEnumerator ScaleRoutine(Transform imgTransform)
        {
            float timer = 0f;
            var originalScale = imgTransform.localScale;

            while (timer < _duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Sin((timer / _duration) * Mathf.PI);
                imgTransform.localScale = Vector3.Lerp(originalScale, originalScale * _scaleFactor, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// イベント購読解除処理
        /// </summary>
        private void OnDestroy()
        {
            _statusManager.OnLifeCountChanged -= UpdateLifeCount;
            _statusManager.OnLifeChanged -= UpdateValue;
        }
    }
}
