using UnityEngine;
using System;
using MyMetroidVania.Entity.Character.Player;

namespace MyMetroidVania.Entity.Gimmick
{
    public class Door : GimmickBase, IInteractable
    {
        [Serializable]
        public enum State
        {
            Close, // 閉鎖
            Open, // 開放
            Lock // 施錠
        }

        [SerializeField, Tooltip("見た目")] private SpriteRenderer _visual;
        [SerializeField, Tooltip("コライダー")] private Collider2D _collider;

        /// <summary>
        /// ドアの状態を初期化
        /// </summary>
        public override void InitializeState()
        {
            switch ((State)_stateData.State)
            {
                case State.Open:
                    Open();
                    break;

                case State.Lock:
                    // 鍵付き特有の状態
                    break;

                // 初期状態のため処理は不要
                case State.Close:
                default:
                    break;
            }
        }

        public void Interact(Player _)
        {
            PlayOneShotInteractedSe();
            Open();
        }

        /// <summary>
        /// 開放状態
        /// </summary>
        private void Open()
        {
            _visual.enabled = false;
            _collider.enabled = false;
            _stateData.SetState((int)State.Open);
        }
    }
}