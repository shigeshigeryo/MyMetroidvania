using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    [SerializeField, Tooltip("ID")] protected string _id;
    public string Id => _id;

    /// <summary>
    /// 主に保存データをロードして状態の初期化を行う
    /// Managerでエリアの情報を取得してから初期化を行う必要があるため、Startで呼び出す。
    /// </summary>
    protected abstract void InitializeState();
}
