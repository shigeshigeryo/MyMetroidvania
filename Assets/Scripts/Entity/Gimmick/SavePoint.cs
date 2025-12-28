using UnityEngine;

public class SavePoint : GimmickBase, IInteractable
{
    public enum State
    {
        None, // 未チェック
        Accessed, // アクセス済み（ワープポイントとして活躍できるといいかも）
        AccessedNow // 最近アクセスしたセーブポイント（GameManagerで持っているので必要ないかも？）
    }
    private State _currentState = State.None;

    public override void InitializeState()
    {
        switch ((State)_stateData.State)
        {
            case State.Accessed:
                ChangeState(State.Accessed);
                break;

            case State.AccessedNow:
                ChangeState(State.AccessedNow);
                break;

            case State.None:
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
        //最近アクセスしたセーブポイントがこのセーブポイントでない場合に更新
        if (_currentState != State.AccessedNow)
        {
            ChangeState(State.AccessedNow);
        }
    }

    public void ChangeState(State state)
    {
        _currentState = state;
        _stateData.SetState((int)state);
        if(state == State.AccessedNow)
        {
            WorldManager.Instance.SetCurrentSavePoint(this);
        }
    }
}
