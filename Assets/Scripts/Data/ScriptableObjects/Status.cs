using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Data/Status")]
public class Status : ScriptableObject
{
    [SerializeField, Tooltip("ライフ"), Min(0)] private int _life;
    public int Life => _life;
    [SerializeField, Tooltip("攻撃力")] private int _attackPower;
    public int AttackPower => _attackPower;
    [SerializeField, Tooltip("無敵時間（秒）")] private float _invincibleSec;
    public float InvincibleSec => _invincibleSec;


    /// <summary>
    /// デフォルトのステータスからランタイム用のステータスのインスタンスを生成する<br/>
    /// TODO：ランタイム用とクラスの分割を検討
    /// </summary>
    public Status CreateCurrentStatus()
    {
        var newInstance = CreateInstance<Status>();
        newInstance._life = _life;
        newInstance._attackPower = _attackPower;
        newInstance._invincibleSec = _invincibleSec;
        return newInstance;
    }

    public void UpdateLife(int value)
    {
        _life += value;
    }
}
