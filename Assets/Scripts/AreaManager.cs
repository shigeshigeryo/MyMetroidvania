using UnityEngine;

public class AreaManager : MonoBehaviour
{
    void Start()
    {
        var test = JsonHandler.LoadResourcesJsonFile<WorldStateData>("WorldData/WorldStateData");
        test.TryGetAllAreaTargetState("Ability", out var testTarget);
        Debug.Log(testTarget.TargetId);

        test.TryGetAreaDataPath("Area_001", out var path);
        var areaTest = JsonHandler.LoadResourcesJsonFile<AreaStateData>(path);
        areaTest.TryGetTargetState("Door_001", out var doorData);
        Debug.Log(doorData.TargetId);
    }
}
