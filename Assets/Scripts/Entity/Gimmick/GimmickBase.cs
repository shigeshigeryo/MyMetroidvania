using MyMetroidVania.System;
using UnityEngine;
using MyMetroidVania.Data.ScriptableObjects;
using MyMetroidVania.Data;

namespace MyMetroidVania.Entity.Gimmick
{
    public abstract class GimmickBase : MonoBehaviour
    {
        [SerializeField, Tooltip("ID")] protected string _id;
        public string Id => _id;
        [SerializeField, Tooltip("インタラクトされて流れる音源のファイル名")]
        private string _interactedSoundName;
        private SoundData _interactedSound;
        // クラスなので参照型
        protected TargetStateData _stateData;

        protected virtual void Start()
        {
            _interactedSound = AudioManager.Instance.GetSe(_interactedSoundName.GetHashCode());
        }

        protected void PlayOneShotInteractedSe()
        {
            AudioManager.Instance.PlayOneShotSe(_interactedSound);
        }

        /// <summary>
        /// 主に保存データをロードして状態の初期化を行う
        /// Managerでエリアの情報を取得してから初期化を行う必要があるため、Startで呼び出す。
        /// </summary>
        public abstract void InitializeState();

        public void SetGimmickStateData(TargetStateData stateData)
        {
            _stateData = stateData;
        }
    }
}