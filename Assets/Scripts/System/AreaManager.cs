using MyMetroidVania.Data;
using MyMetroidVania.Entity.Character.Enemy;
using MyMetroidVania.Entity.Gimmick;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyMetroidVania.System
{
    /// <summary>
    /// エリアの状態を管理
    /// </summary>
    public class AreaManager : MonoBehaviour
    {
        public static Dictionary<string, AreaManager> AreaManagerList = new Dictionary<string, AreaManager>();
        private List<EnemyBase> _enemyList = new List<EnemyBase>();

        [SerializeField, Tooltip("エリアID")] private string _areaId;
        /// <summary>
        /// エリアID
        /// </summary>
        public string AreaId => _areaId;
        // エリアの初期の状態データ
        private AreaStateData _areaStateData;
        /// <summary>
        /// エリアの状態
        /// </summary>
        public AreaStateData AreaStateData => _areaStateData;
        private string _areaStateDataPath;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            AreaManagerList.Add(_areaId, this);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 初期化処理
        /// 発火は初めてActiveにされた1度だけ
        /// </summary>
        private void Start()
        {
            // 特定エリアの情報を取得
            if (WorldManager.Instance.WorldStateData.TryGetAreaDataPath(_areaId, out _areaStateDataPath))
            {
                // ファイルが存在しなかった場合に初期値のファイルをロードし、
                // その値で新規にJSONファイルを作成する
                if (!JsonHandler.TryLoadJsonFile(_areaStateDataPath, out _areaStateData))
                {
                    _areaStateData = JsonHandler.LoadResourcesJsonFile<AreaStateData>(_areaStateDataPath);
                    SaveAreaStateData();
                }
            }

            InitializeAllGimmicks();
        }

        /// <summary>
        /// エリア移動時の初期化
        /// </summary>
        public void InitializeAreaState()
        {
            InitializeAllEnemies();
        }

        /// <summary>
        /// 初回起動時、セーブポイントアクセス時、リスポーン時の初期化
        /// </summary>
        public void InitializeAreaStateRespawn()
        {
            RespawnAllEnemies();
        }

        /// <summary>
        /// エリア内の全てのギミックを初期化
        /// </summary>
        private void InitializeAllGimmicks()
        {
            GimmickBase[] gimmicks = GetComponentsInChildren<GimmickBase>();
            foreach (var gimmick in gimmicks)
            {
                if (_areaStateData.TryGetTargetState(gimmick.Id, out var stateData))
                {
                    gimmick.SetGimmickStateData(stateData);
                }
                else
                {
                    Debug.LogError($"ID:{gimmick.Id} を取得できませんでした。デフォルトの初期値で処理します。");
                    gimmick.SetGimmickStateData(new(gimmick.Id, 0));
                }
                gimmick.InitializeState();
            }
        }

        /// <summary>
        /// エリア内の敵の初期化処理
        /// </summary>
        private void InitializeAllEnemies()
        {
            bool isFirst = _enemyList.Count == 0;
            if (isFirst)
            {
                // lazyLoad
                _enemyList = GetComponentsInChildren<EnemyBase>().ToList<EnemyBase>();
            }

            foreach (var enemy in _enemyList)
            {
                if (isFirst)
                {
                    enemy.InitializeOnce(); // 初回時のみ実行する初期化
                    enemy.OnDestroyed += RemoveEnemy;
                }
                enemy.Initialize();
            }
        }

        /// <summary>
        /// エリア内の敵の初期化処理
        /// </summary>
        private void RespawnAllEnemies()
        {
            foreach (var enemy in _enemyList)
            {
                enemy.Respawn();
            }
        }

        /// <summary>
        /// エネミーリストから対象のエネミーを削除
        /// </summary>
        /// <param name="target">対象のエネミー</param>
        private void RemoveEnemy(EnemyBase target)
        {
            target.OnDestroyed -= RemoveEnemy;
            _enemyList.Remove(target);
        }

        /// <summary>
        /// 現在のエリアの状態を保存する
        /// 各オブジェクトステートは、そのオブジェクトに紐づいているスクリプトで更新済み
        /// </summary>
        private void SaveAreaStateData()
        {
#if UNITY_EDITOR
            if (WorldManager.Instance.IsDebug) return;
#endif
            JsonHandler.WriteJsonFile(_areaStateDataPath, _areaStateData);
        }

        /// <summary>
        /// clean処理
        /// </summary>
        private void OnDestroy()
        {
            AreaManagerList.Remove(_areaId);

            // 更新の必要の有無をチェック
            // エリアのパスが存在しない場合は一度も訪れられていない
            if (_areaStateDataPath != default)
            {
                SaveAreaStateData();
            }
        }
    }
}
