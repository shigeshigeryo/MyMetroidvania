using MyMetroidVania.Data.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyMetroidVania.System
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("BGM")]
        [SerializeField]
        private AudioSource _bgmSource;
        [SerializeField]
        private SoundDatabase _bgmDatabase;
        private readonly Dictionary<int, SoundData> _bgmList = new();
        private SoundData _currentBgm = null;
        private SoundData _backupBgm = null;
        private bool _isFading = false;
        private const float FADE_AMOUNT = 0.1f;

        [Header("SE")]
        [SerializeField]
        private AudioSource _seSource;
        [SerializeField]
        private SoundDatabase _seDatabase;
        private readonly Dictionary<int, SoundData> _seList = new();

        private void Awake()
        {
            // シングルトン化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // データベースからサウンドを取得
            foreach (var bgm in _bgmDatabase.SoundDataList)
            {
                _bgmList[bgm.Clip.name.GetHashCode()] = bgm;
            }
            if (_bgmList.Count == 0) Debug.LogError("データベース内にBGMが存在しません。");
            foreach (var se in _seDatabase.SoundDataList)
            {
                _seList[se.Clip.name.GetHashCode()] = se;
            }
            if (_seList.Count == 0) Debug.LogError("データベース内にSEが存在しません。");
        }

        /*
         * ------------------------------
         * BGM
         * ------------------------------
         */
        /// <summary>
        /// 名前をハッシュ化し、ゲットする。
        /// </summary>
        public SoundData GetBgm(string clipName)
        {
            // データベースから名前検索
            SoundData bgm = _bgmList[clipName.GetHashCode()];

            if (bgm == null)
            {
                Debug.LogWarning($"BGM：{name} が見つかりませんでした。");
            }
            return bgm;
        }
        public void PlayBgm(SoundData bgm)
        {
            Action playHandler = () =>
            {
                _currentBgm = bgm;
                _bgmSource.clip = bgm.Clip;
                _bgmSource.loop = bgm.IsLoop;
                _bgmSource.Play();
                FadeInBGM(bgm.Volume);
            };

            if (_currentBgm != null)
            {
                FadeOutBGM(playHandler);
            }
            else
            {
                playHandler.Invoke();
            }
        }

        /// <summary>
        /// bgmの割り込み再生
        /// </summary>
        /// <param name="bgm">再生するBGM</param>
        public void InterruptPlayBgm(SoundData bgm)
        {
            _backupBgm = _currentBgm;
            PlayBgm(bgm);
        }

        /// <summary>
        /// バックアップしていたBGMの再生に戻す
        /// </summary>
        public void ReturnBackupBgm()
        {
            if (_backupBgm == null) return;

            PlayBgm(_backupBgm);
            _backupBgm = null;
        }

        public void GetAndPlayBgm(string clipName)
        {
            SoundData bgm = GetBgm(clipName);
            if (bgm != null) PlayBgm(bgm);
        }

        /// <summary>
        /// BGMをフェードインルーチンを開始
        /// </summary>
        /// <param name="maxVolume">フェードイン音量上限</param>
        /// <param name="callback">コールバック関数</param>

        public void FadeInBGM(float maxVolume, Action callback = null)
        {
            _bgmSource.volume = 0;
            StartCoroutine(OnFadeInBGM(maxVolume));
        }

        /// <summary>
        /// BGMをフェードインさせる
        /// </summary>
        /// <param name="maxVolume"></param>
        /// <param name="callback">コールバック関数</param>
        private IEnumerator OnFadeInBGM(float maxVolume, Action callback = null)
        {
            while (_isFading)
            {
                yield return null;
            }
            _isFading = true;

            //フェードイン
            while (_bgmSource.volume < maxVolume)
            {
                _bgmSource.volume += FADE_AMOUNT * Time.deltaTime;
                yield return null;
            }

            _bgmSource.volume = maxVolume;
            _isFading = false;

            callback?.Invoke();
        }

        /// <summary>
        /// BGMをフェードアウトルーチンの開始
        /// </summary>
        /// <param name="callback">コールバック関数</param>
        public void FadeOutBGM(Action callback = null)
        {
            StartCoroutine(OnFadeOutBGM(callback));
        }

        /// <summary>
        /// BGMをフェードアウトさせる
        /// </summary>
        /// <param name="callback">コールバック関数</param>
        private IEnumerator OnFadeOutBGM(Action callback = null)
        {
            while (_isFading)
            {
                yield return null;
            }
            _isFading = true;

            //フェードアウト
            while (_bgmSource.volume > 0)
            {
                _bgmSource.volume -= FADE_AMOUNT * Time.deltaTime;
                yield return null;
            }

            _bgmSource.volume = 0;
            _isFading = false;

            callback?.Invoke();
        }

        /*
         * ------------------------------
         * Jingle
         * ------------------------------
         */
        /// <summary>
        /// ジングル再生
        /// </summary>
        /// <param name="jingle">再生するジングル</param>
        /// <param name="callback">コールバック関数</param>
        public void PlayJingle(SoundData jingle, Action callback = null)
        {
            _bgmSource.clip = jingle.Clip;
            _bgmSource.volume = jingle.Volume;
            _bgmSource.loop = false;
            _bgmSource.Play();

            if(callback != null)
            {
                StartCoroutine(MonitorPlaying(callback));
            }
        }

        /// <summary>
        /// ジングル再生の監視
        /// </summary>
        /// <param name="callback">コールバック関数</param>
        private IEnumerator MonitorPlaying(Action callback)
        {
            while (_bgmSource.isPlaying)
            {
                yield return null;
            }

            _bgmSource.volume = 0f; // フェードインがスムーズに行われるようにボリュームを下げておく
            callback.Invoke();
        }

        /*
         * ------------------------------
         * SE
         * ------------------------------
         */
        /// <summary>
        /// 名前をハッシュ化し、ゲットする。
        /// </summary>
        public SoundData GetSe(string clipName)
        {
            // データベースから名前検索（ハッシュ化してintで検索）
            SoundData se = _seList[clipName.GetHashCode()];

            if (se == null)
            {
                Debug.LogWarning($"SE：{name} が見つかりませんでした。");
            }
            return se;
        }

        public void PlaySe(SoundData se)
        {
            _seSource.clip = se.Clip;
            _seSource.volume = se.Volume;
            _seSource.Play();
        }
        public void PlayOneShotSe(SoundData se)
        {
            _seSource.PlayOneShot(se.Clip, se.Volume);
        }
    }
}