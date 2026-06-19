using MyMetroidVania.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Player
{
    /// <summary>
    /// 手裏剣の管理
    /// </summary>
    public class Shuriken : MonoBehaviour, IDamageDealer
    {
        [SerializeField] private CircleCaster _groundChecker = null;
        [SerializeField] private HitBox _hitBox = null;
        [SerializeField] private SpriteRenderer _visual = null;
        [Header("挙動")]
        [SerializeField, Tooltip("速さ")] private float _initialSpeed = 10f;
        private float _speed;
        [SerializeField, Tooltip("移動時間（秒）")] private float _moveTimeSec;
        [SerializeField, Tooltip("毎秒の回転速度")] private float _rotateSpeed;

        private bool _isInit = false;
        private int _atkPower = 1;
        private Vector3 _startPosition;
        private Vector3 _arrivalPosition;
        private float _currentTime = 0f;

        private IObjectPool<Shuriken> _pool;

        /// <summary>
        /// プールを設定する
        /// </summary>
        /// <param name="pool">設定するプール</param>
        public void SetPool(IObjectPool<Shuriken> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            if (_hitBox == null)
            {
                Destroy(gameObject);
                return;
            }

            // イベント購読
            _hitBox.OnTriggered += Disappear;
        }

        /// <summary>
        /// メインの初期化処理
        /// 出現位置や手裏剣のステータスを反映
        /// </summary>
        /// <param name="position">出現ポイント</param>
        /// <param name="rotation">手裏剣の初期回転</param>
        /// <param name="speed">とうてきスピード</param>
        /// <param name="atkPower">攻撃力</param>
        public void Initialize(Vector3 position, Quaternion rotation, float speed, int atkPower)
        {
            // 現在位置と角度と攻撃力を初期化
            transform.position = position;
            transform.rotation = rotation;
            _atkPower = atkPower;

            // 初期位置、到達位置を設定
            _startPosition = position;
            // プレイヤーのX速度をみてプレイヤーとの相対速度を一定にする
            _speed = _initialSpeed + speed;
            _arrivalPosition = _startPosition + (transform.right * _speed * _moveTimeSec);

            _currentTime = 0;

            _isInit = true;
        }

        /// <summary>
        /// 毎フレーム処理
        /// 移動や消滅の処理
        /// </summary>
        private void Update()
        {
            if (!_isInit) return;

            // 移動時間を過ぎるか、接地判定があった場合に手裏剣を消す
            if (_currentTime > _moveTimeSec || _groundChecker.IsCasted)
            {
                Disappear();
            }

            // 移動
            _currentTime += Time.deltaTime;
            var t = _currentTime / _moveTimeSec;
            transform.position = Vector3.Lerp(_startPosition, _arrivalPosition, t); // 到達地点まで移動

            // 回転
            _visual.transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 攻撃力を取得する
        /// </summary>
        /// <returns>攻撃力の値</returns>
        public int GetAttackPower()
        {
            return _atkPower;
        }

        /// <summary>
        /// 消滅処理
        /// </summary>
        private void Disappear()
        {
            if (!_isInit) return;

            _isInit = false;
            _pool.Release(this);
        }

        /// <summary>
        /// イベント購読解除処理
        /// </summary>
        private void OnDestroy()
        {
            // イベント購読解除
            _hitBox.OnTriggered -= Disappear;
        }
    }
}
