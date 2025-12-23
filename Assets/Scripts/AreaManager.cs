using UnityEngine;

public class AreaManager : MonoBehaviour
{
    void Start()
    {
        var test = JsonHandler.LoadResourcesJsonFile<WorldStateData>("WorldStateData");
        test.TryGetAllAreaTargetState("Ability", out var testTarget);
        Debug.Log(testTarget.TargetId);
    }
}
