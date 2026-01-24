using MyMetroidVania.Data.ScriptableObjects;
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
         * BGM
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
            _bgmSource.clip = bgm.Clip;
            _bgmSource.volume = bgm.Volume;
            _bgmSource.Play();
        }
        public void GetAndPlayBgm(string clipName)
        {
            SoundData bgm = GetBgm(clipName);
            if (bgm != null) PlayBgm(bgm);
        }

        /*
         * SE
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