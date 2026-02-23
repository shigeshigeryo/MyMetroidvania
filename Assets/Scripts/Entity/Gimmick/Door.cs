using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Entity.Character.Player;
using MyMetroidVania.System;
using System;
using UnityEngine;

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
        private State _currentState;

        [SerializeField, Tooltip("Close時のInteract音源")]
        private string _closedSoundName;
        private SoundData _closedSound;
        [SerializeField, Tooltip("Lock時のInteract音源")]
        private string _lockedSoundName;
        private SoundData _lockedSound;
        [SerializeField, Tooltip("見た目")] private SpriteRenderer _visual;
        [SerializeField, Tooltip("コライダー")] private Collider2D _collider;

        private void Start()
        {
            _closedSound = AudioManager.Instance.GetSe(_closedSoundName);
            _lockedSound = AudioManager.Instance.GetSe(_lockedSoundName);
        }

        /// <summary>
        /// ドアの状態を初期化
        /// </summary>
        public override void InitializeState()
        {
            _currentState = (State)_stateData.State;
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
            switch (_currentState)
            {
                case State.Close:
                    AudioManager.Instance.PlayOneShotSe(_closedSound);
                    Open();
                    break;
                case State.Lock:
                    AudioManager.Instance.PlayOneShotSe(_lockedSound);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 開放状態
        /// </summary>
        private void Open()
        {
            _visual.enabled = false;
            _collider.enabled = false;
            _currentState = State.Open;
            _stateData.SetState((int)_currentState);
        }
    }
}