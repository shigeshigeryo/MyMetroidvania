using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

// Newtonsoft.Json を用いたJsonファイルを操作するクラス
// com.unity.nuget.newtonsoft-json
public static class JsonHandler
{
    // ResourcesフォルダからJsonファイルのデータを取得する
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

    // Jsonファイルからデータを取得する
    public static T LoadJsonFile<T>(string path)
    {
        // Newtonsoft.Json は try-catch推奨
        try
        {
            if (TryGetJsonText(Application.persistentDataPath + "/" + path + ".json", out var text))
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            else
            {
                return default;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{path}.jsonの取得に失敗しました。");
            Debug.LogError(e.ToString());
            return default;
        }
    }

    // Jsonファイルにデータを書き込む
    public static void WriteJsonFile<T>(string path, T obj)
    {
        string fullPath = Application.persistentDataPath + "/" + path + ".json";

        if (!TryGetJsonText(fullPath, out var _))
        {
            Debug.Log($"{path}が存在しないため、新規作成をします。");
        }
        string json = JsonConvert.SerializeObject(obj);
        // 書き込み処理
        File.WriteAllText(fullPath, json);
    }


    /// <summary>
    /// Jsonファイルの内容を取得し、その成否を返す。
    /// </summary>
    /// <param name="path">取得したいファイルのパス</param>
    /// <param name="text">ファイルの内容を格納する 失敗の場合は空文字</param>
    /// <returns></returns>
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
