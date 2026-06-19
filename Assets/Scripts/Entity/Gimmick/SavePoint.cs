using MyMetroidVania.Entity.Character.Player;
using MyMetroidVania.System;
using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick
{
    /// <summary>
    /// セーブポイントを管理
    /// </summary>
    public class SavePoint : GimmickBase, IInteractable
    {
        [SerializeField, Tooltip("インタラクトされて流れる音源のファイル名")]
        private string _interactedSoundName;
        private SoundData _interactedSound;
        [SerializeField, Tooltip("セーブポイントの中のぐるぐる")] private SpriteRenderer _innerRenderer;
        /// <summary>
        /// セーブポイントの状態
        /// </summary>
        public enum State
        {
            None, // 未アクセス
            Accessed, // アクセス済み（ワープポイントとして活躍できるといいかも）
            AccessedNow // 最近アクセスしたセーブポイント
        }
        private State _currentState = State.None;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            _interactedSound = AudioManager.Instance.GetSe(_interactedSoundName);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void InitializeState()
        {
            switch ((State)_stateData.State)
            {
                case State.None:
                    // アクセスしていない場合はinnerを表示させない
                    _innerRenderer.enabled = false;
                    break;

                case State.Accessed:
                    ChangeState(State.Accessed);
                    break;

                case State.AccessedNow:
                    ChangeState(State.AccessedNow);
                    break;

                default:
                    _currentState = State.None;
                    break;

            }
        }

        /// <summary>
        /// インタラクト時の処理
        /// プレイヤーを回復し、セーブポイントのステートを変更
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void Interact(Player player)
        {
            AudioManager.Instance.PlayOneShotSe(_interactedSound);
            player.Heal();
            
            ChangeState(State.AccessedNow); 
        }

        /// <summary>
        /// エリア初期化時、セーブポイントアクセス時に発火
        /// </summary>
        /// <param name="state">セットするState</param>
        public void ChangeState(State state)
        {
            _currentState = state;
            _stateData.SetState((int)state); // エリアデータの更新

            // アクセスしたらinnerを表示させる
            _innerRenderer.enabled = (state != State.None);

            if (state == State.AccessedNow)
            {
                WorldManager.Instance.SetCurrentSavePoint(this);
            }
        }
    }
}
