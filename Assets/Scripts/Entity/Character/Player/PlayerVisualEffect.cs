using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Entity.Effect;
using MyMetroidVania.System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace MyMetroidVania.Entity.Character.Player
{
    public class PlayerVisualEffect : MonoBehaviour
    {
        [SerializeField] private StatusManager _statusManager = null;
        [SerializeField] private AudioSource _audioSource = null;
        [SerializeField, Tooltip("プレイヤーのビジュアル")] private SpriteRenderer _renderer = null;

        [Header("エフェクト")]
        [SerializeField, Tooltip("走るエフェクト")] private RunEffect _runEffectPrefab = null;
        private RunEffect _runEffect = null;
        [SerializeField, Tooltip("ジャンプエフェクト")] private PoolingEffect _jumpEffectPrefab = null;
        [SerializeField, Tooltip("着地エフェクト")] private PoolingEffect _landEffectPrefab = null;
        private IObjectPool<PoolingEffect> _jumpEffectPool;
        private IObjectPool<PoolingEffect> _landEffectPool;


        [Header("サウンド")]
        [SerializeField, Tooltip("ジャンプ音源ファイル名")] private string _jumpSoundName = "SE_PlayerJump";
        private SoundData _jumpSound = null;
        [SerializeField, Tooltip("フック音源ファイル名")] private string _hookSoundName = "SE_PlayerHook";
        private SoundData _hookSound = null;
        [SerializeField, Tooltip("被弾時音源ファイル名")] private string _takeDamageSoundName = "SE_PlayerTakeDamage";
        private SoundData _takeDamageSound = null;
        [SerializeField, Tooltip("死亡時音源ファイル名")] private string _deadSoundName = "SE_PlayerDead";
        private SoundData _deadSound = null;

        public void Initialize()
        {
            InitializeEffects();
            InitializeSounds();
        }


        /// <summary>
        /// エフェクト（プール）の初期化
        /// </summary>
        private void InitializeEffects()
        {
            // ジャンプエフェクトプール
            _jumpEffectPool = new ObjectPool<PoolingEffect>(
                createFunc: () =>
                {
                    PoolingEffect effect = Instantiate(_jumpEffectPrefab);
                    effect.SetPool(_jumpEffectPool);
                    return effect;
                },
                actionOnGet: (effect) =>
                {
                    effect.gameObject.SetActive(true);
                },
                actionOnRelease: (effect) =>
                {
                    effect.gameObject.SetActive(false);
                },
                actionOnDestroy: (effect) =>
                {
                    Destroy(effect.gameObject);
                },
                defaultCapacity: 3, // 準備数（仮）
                maxSize: 5 // 最大数（仮）
            );

            // 着地エフェクトプール
            _landEffectPool = new ObjectPool<PoolingEffect>(
                createFunc: () =>
                {
                    PoolingEffect effect = Instantiate(_landEffectPrefab);
                    effect.SetPool(_landEffectPool);
                    return effect;
                },
                actionOnGet: (effect) =>
                {
                    effect.gameObject.SetActive(true);
                },
                actionOnRelease: (effect) =>
                {
                    effect.gameObject.SetActive(false);
                },
                actionOnDestroy: (effect) =>
                {
                    Destroy(effect.gameObject);
                },
                defaultCapacity: 3, // 準備数（仮）
                maxSize: 5 // 最大数（仮）
            );
        }

        /// <summary>
        /// サウンドの初期化
        /// </summary>
        private void InitializeSounds()
        {
            // サウンドの取得
            _jumpSound = AudioManager.Instance.GetSe(_jumpSoundName);
            _hookSound = AudioManager.Instance.GetSe(_hookSoundName);
            _takeDamageSound = AudioManager.Instance.GetSe(_takeDamageSoundName);
            _deadSound = AudioManager.Instance.GetSe(_deadSoundName);

            // サウンドの購読
            _statusManager.OnDamageTaken += () => _audioSource.PlayOneShot(_takeDamageSound.Clip, _takeDamageSound.Volume);
            _statusManager.OnDead += () => _audioSource.PlayOneShot(_deadSound.Clip, _deadSound.Volume);

        }


        /// <summary>
        /// 入力方向によってプレイヤーの体の向きを変える
        /// </summary>
        /// <param name="dirX">X軸の入力値</param>
        public void SetFlip(float dirX)
        {
            // 入力がない場合はFlipの更新を行わない
            if (dirX > 0.01f)
            {
                _renderer.flipX = false;
            }
            else if(dirX < -0.01f)
            {
                _renderer.flipX = true;
            }
        }

        /// <summary>
        /// ジャンプのエフェクト、SEを再生
        /// </summary>
        public void PlayJumpEffect()
        {
            var effect = _jumpEffectPool.Get();
            effect.transform.position = transform.position; // プレイヤーの足元

            _audioSource.PlayOneShot(_jumpSound.Clip, _jumpSound.Volume);
        }

        /// <summary>
        /// フックのSEを再生
        /// </summary>
        public void PlayHookEffect()
        {
            _audioSource.PlayOneShot(_hookSound.Clip, _hookSound.Volume);
        }

        /// <summary>
        /// 着地のエフェクトを再生
        /// </summary>
        public void PlayLandEffect()
        {
            var effect = _landEffectPool.Get();
            effect.transform.position = transform.position; // プレイヤーの足元
        }

        /// <summary>
        /// 走るエフェクトを再生
        /// </summary>
        /// <param name="dirX">入力方向 右向きに走っている場合 true</param>
        public IEnumerator PlayRunEffect(float dirX)
        {
            if (_runEffect == null)
            {
                // ランエフェクトがない場合は生成
                _runEffect = Instantiate(_runEffectPrefab);
            }
            _runEffect.transform.position = transform.position; // 足元にエフェクトを生成
            _runEffect.PlayAnimation(dirX > 0); // 入力方向を引数で渡す

            // エフェクトのインターバル 0.5s
            yield return new WaitForSeconds(0.5f);
        }
    }
}