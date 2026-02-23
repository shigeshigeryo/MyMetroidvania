using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Entity.Character.Player;
using MyMetroidVania.System;
using System;
using System.Collections;
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
        [SerializeField, Tooltip("UnLock時のInteract音源")]
        private string _unlockedSoundName;
        private SoundData _unlockedSound;
        [SerializeField, Tooltip("見た目")] private SpriteRenderer _visual;
        [SerializeField, Tooltip("コライダー")] private Collider2D _collider;
        [SerializeReference, Tooltip("開錠に必要なキー<br>開錠が必要なければnull")]
        private GameObject _unlockKeyObj = null;
        private IUnlockKey _unlockKey = null;

        private Coroutine _lockedRoutine = null;
        private void OnEnable()
        {
            if (_unlockKey == null && _unlockKeyObj != null)
            {
                if (_unlockKeyObj.TryGetComponent<IUnlockKey>(out var key))
                {
                    _unlockKey = key;
                }
                else
                {
                    Debug.LogError($"IUnlockKeyインタフェースを実装したオブジェクトを指定してください。<br>{_unlockKeyObj}", gameObject);
                    return;
                }
            }

            if (_unlockKey != null)
            {
                _unlockKey.OnUnlocked += UnLock;
            }
        }

        private void Start()
        {
            _closedSound = AudioManager.Instance.GetSe(_closedSoundName);
            _lockedSound = AudioManager.Instance.GetSe(_lockedSoundName);
            _unlockedSound = AudioManager.Instance.GetSe(_unlockedSoundName);
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
                    if (_lockedRoutine == null)
                    {
                        _lockedRoutine = StartCoroutine(ReactInLocked());
                    }
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

        /// <summary>
        /// ロック反応
        /// </summary>
        private IEnumerator ReactInLocked()
        {
            var tmpScale = _visual.transform.localScale;
            _visual.transform.localScale *= 1.1f;
            yield return new WaitForSeconds(0.1f);
            _visual.transform.localScale = tmpScale;
            _lockedRoutine = null;
        }

        /// <summary>
        /// アンロック処理
        /// </summary>
        public void UnLock()
        {
            if (_currentState != State.Lock) return;

            AudioManager.Instance.PlayOneShotSe(_unlockedSound);
            _currentState = State.Close;
        }

        private void OnDisable()
        {
            _lockedRoutine = null;
            if (_unlockKey != null)
            {
                _unlockKey.OnUnlocked -= UnLock;
            }
        }
    }
}