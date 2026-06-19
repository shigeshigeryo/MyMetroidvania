using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Newtonsoft.Json を用いたJsonファイルを操作するクラス
/// com.unity.nuget.newtonsoft-json
/// </summary>
public static class JsonHandler
{
    /// <summary>
    /// ResourcesフォルダからJsonファイルのデータを取得する
    /// </summary>
    /// <typeparam name="T">取得したいデータの型</typeparam>
    /// <param name="path">ファイルのパス</param>
    /// <returns>取得したデータ</returns>
    public static T LoadResourcesJsonFile<T>(string path)
    {
        // Newtonsoft.Json は try-catch推奨
        try
        {
            var text = Resources.Load<TextAsset>(path).text;
            return JsonConvert.DeserializeObject<T>(text);
        }
        catch (Exception e)
        {
            Debug.LogError($"{path}.jsonの取得に失敗しました。");
            Debug.LogError(e.ToString());
            return default;
        }
    }

    /// <summary>
    /// Jsonファイルからデータを取得し、その成否を返す
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">ファイルのパス</param>
    /// <param name="data">取得するデータを返す</param>
    /// <returns>取得の成否</returns>
    public static bool TryLoadJsonFile<T>(string path, out T data)
    {
        // Newtonsoft.Json は try-catch推奨
        try
        {
            if (TryGetJsonText(Application.persistentDataPath + "/" + path + ".json", out var text))
            {
                data = JsonConvert.DeserializeObject<T>(text);
                return true;
            }
            else
            {
                data = default;
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{path}.jsonの取得に失敗しました。");
            Debug.LogError(e.ToString());
            data = default;
            return false;
        }
    }

    /// <summary>
    /// Jsonファイルにデータを書き込む
    /// </summary>
    /// <typeparam name="T">入力したいデータの型</typeparam>
    /// <param name="path">ファイルのパス</param>
    /// <param name="obj">入力するデータ</param>
    public static void WriteJsonFile<T>(string path, T obj)
    {
        string fullPath = Application.persistentDataPath + "/" + path + ".json";

        if (!TryGetJsonText(fullPath, out var _))
        {
            Debug.Log($"{path}が存在しないため、新規作成をします。");

            // ディレクトリから作成する
            string[] array = path.Split('/');
            string tmpPath = Application.persistentDataPath;
            for (int i = 0; i < array.Length - 1; i++)
            {
                tmpPath += "/" + array[i];
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                    Debug.Log($"ディレクトリ '{tmpPath}' を作成しました。");
                }
            }
        }
        string json = JsonConvert.SerializeObject(obj);
        // 書き込み処理
        File.WriteAllText(fullPath, json);
    }

    /// <summary>
    /// 全てのJSONデータファイルを削除する
    /// </summary>
    public static void DeleteAllData()
    {
        try
        {
            Debug.Log("セーブデータを削除します。");
            Directory.Delete(Application.persistentDataPath + "/WorldData", true);
            Debug.Log("セーブデータを削除しました。");
        }
        catch (Exception e)
        {
            Debug.LogError("セーブデータの削除に失敗しました。<br>" + e.ToString());
        }
    }


    /// <summary>
    /// Jsonファイルの内容を取得し、その成否を返す。
    /// </summary>
    /// <param name="path">取得したいファイルのパス</param>
    /// <param name="text">ファイルの内容を格納する 失敗の場合は空文字</param>
    /// <returns>取得の成否</returns>
    private static bool TryGetJsonText(string path, out string text)
    {
        try
        {
            text = File.ReadAllText(path);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{path}の取得に失敗しました。");
            Debug.LogWarning(e.ToString());
            text = "";
            return false;
        }
    }
}
