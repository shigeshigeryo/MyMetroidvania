using MyMetroidVania.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Player
{
    public class Shuriken : MonoBehaviour, IDamageDealer
    {
        [SerializeField] private CircleCaster _groundChecker = null;
        [SerializeField] private HitBox _hitBox = null;
        [SerializeField] private SpriteRenderer _visual = null;
        [Header("挙動")]
        [SerializeField, Tooltip("移動距離")] private float _moveDistance;
        [SerializeField, Tooltip("移動時間（秒）")] private float _moveTimeSec;
        [SerializeField, Tooltip("毎秒の回転速度")] private float _rotateSpeed;

        private bool _isInit = false;
        private int _atkPower = 1;
        private Vector3 _startPosition;
        private Vector3 _arrivalPosition;
        private float _currentTime = 0f;

        private IObjectPool<Shuriken> _pool;
        public void SetPool(IObjectPool<Shuriken> pool)
        {
            _pool = pool;
        }


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

        public void Initialize(Vector3 position, Quaternion rotation, int atkPower)
        {
            // 現在位置と角度と攻撃力を初期化
            transform.position = position;
            transform.rotation = rotation;
            _atkPower = atkPower;

            // 初期位置、到達位置を設定
            _startPosition = position;
            _arrivalPosition = _startPosition + (transform.right * _moveDistance);

            _currentTime = 0;

            _isInit = true;
        }

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

        // 時間経過、敵、壁にヒットで消滅
        private void Disappear()
        {
            if (!_isInit) return;

            _isInit = false;
            _pool.Release(this);
        }


        private void OnDestroy()
        {
            // イベント購読解除
            _hitBox.OnTriggered -= Disappear;
        }
    }
}