using MyMetroidVania.Utility;
using UnityEngine;

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

        private int _playerAtkPower = 1;
        private Vector3 _startPosition;
        private Vector3 _arrivalPosition;
        private float _currentTime = 0f;

        private void Start()
        {
            if (_hitBox == null)
            {
                Destroy(gameObject);
                return;
            }

            // 初期位置、到達位置を設定
            _startPosition = transform.position;
            _arrivalPosition = _startPosition + (transform.right * _moveDistance);

            // イベント購読
            _hitBox.OnTriggered += Disappear;
        }

        private void Update()
        {
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
            return _playerAtkPower;
        }

        public void SetAttackPower(int attackPower)
        {
            _playerAtkPower = attackPower;
        }

        // 時間経過、敵、壁にヒットで消滅
        private void Disappear()
        {
            Destroy(gameObject);
        }


        private void OnDisable()
        {
            // イベント購読解除
            _hitBox.OnTriggered -= Disappear;
        }
    }
}