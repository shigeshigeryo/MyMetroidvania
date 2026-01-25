using MyMetroidVania.Entity.Character.Player;
using MyMetroidVania.System;
using UnityEngine;

namespace MyMetroidVania.Entity.Gimmick
{
    public class SavePoint : GimmickBase, IInteractable
    {
        [SerializeField, Tooltip("セーブポイントの中のぐるぐる")] private SpriteRenderer _innerRenderer;

        public enum State
        {
            None, // 未アクセス
            Accessed, // アクセス済み（ワープポイントとして活躍できるといいかも）
            AccessedNow // 最近アクセスしたセーブポイント
        }
        private State _currentState = State.None;

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

        public void Interact(Player player)
        {
            Debug.Log($"インタラクト:{_id}");
            PlayOneShotInteractedSe();
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