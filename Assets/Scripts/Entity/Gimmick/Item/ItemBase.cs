using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick.Item
{
    /// <summary>
    /// アイテムの基底クラス
    /// </summary>
    public abstract class ItemBase : GimmickBase
    {
        /// <summary>
        /// アイテムの状態
        /// </summary>
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

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected virtual void Start()
        {
            _getSoundData = AudioManager.Instance.GetSe(_getSoundName);
        }

        /// <summary>
        /// アイテムの状態の初期化
        /// </summary>
        public override void InitializeState()
        {
            _currentState = (State)_stateData.State;
            if(_currentState == State.PickedUpUnique)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 取得したときの処理内容
        /// </summary>
        /// <param name="collision">このアイテムを取得した対象</param>
        protected abstract void Apply(Collider2D collision);

        /// <summary>
        /// トリガー処理
        /// SEの再生や非表示処理
        /// </summary>
        /// <param name="collision">このアイテムを取得した対象</param>
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
