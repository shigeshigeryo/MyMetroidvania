using Newtonsoft.Json;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    void Start()
    {
        var a = Resources.Load<TextAsset>("WorldStateData");
        var b = JsonConvert.DeserializeObject<WorldStateData>(a.text);
        b.TryGetAllAreaTargetState("Ability", out var test);
        Debug.Log(test.TargetId);
    }
}
