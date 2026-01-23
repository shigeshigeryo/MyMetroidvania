using MyMetroidVania.System;
using UnityEngine;
using MyMetroidVania.Data.ScriptableObjects;

namespace MyMetroidVania.Entity.Item
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField, Tooltip("アイテム取得音源ファイル名")] private string _getSoundName;
        private SoundData _getSoundData;
        private bool isTrigger = false;

        protected virtual void Start()
        {
            _getSoundData = AudioManager.Instance.GetSe(_getSoundName);
        }

        // 取得したときの処理内容
        protected abstract void Apply(Collider2D collision);

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (isTrigger) return;

            isTrigger = true;
            Apply(collision);
            AudioManager.Instance.PlayOneShotSe(_getSoundData);
            Destroy(gameObject);
        }
    }
}