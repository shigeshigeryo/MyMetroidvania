using MyMetroidVania.System;
using UnityEngine;
using MyMetroidVania.Data.ScriptableObjects;

namespace MyMetroidVania.Entity.Gimmick.Item
{
    public abstract class ItemBase : GimmickBase
    {
        public enum State 
        {
            Normal, // 通常アイテム（再配置あり）
            Unique, // ユニークアイテム（1度のみ取得可能）
            PickedUpUnique // ユニークアイテム取得済み
        }
        protected State _currentState;

        [SerializeField, Tooltip("アイテム取得音源ファイル名")] private string _getSoundName;
        private SoundData _getSoundData;
        private bool isTrigger = false;

        protected virtual void Start()
        {
            _getSoundData = AudioManager.Instance.GetSe(_getSoundName);
        }

        /// <summary>
        /// アイテムの初期化
        /// </summary>
        public override void InitializeState()
        {
            _currentState = (State)_stateData.State;
            if(_currentState == State.PickedUpUnique)
            {
                gameObject.SetActive(false);
            }
        }

        // 取得したときの処理内容
        protected abstract void Apply(Collider2D collision);

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (isTrigger) return;

            isTrigger = true;
            Apply(collision);
            AudioManager.Instance.PlayOneShotSe(_getSoundData);
            gameObject.SetActive(false);
        }
    }
}