using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ControlDefinition : MonoBehaviour
{
    [SerializeField] private List<string> _testList = new List<string>();

    private void OnValidate()
    {
        for (int i = 0; i < _testList.Count; i++) {
            _testList[i] = _testList[i].Replace(" ", "_");
            _testList[i] = _testList[i].Replace("/", "_");
        }
    }

    [ButtonMethod]
    private void WriteToFile()
    {
        string filePath = "Assets/scripts/nonMonobehavior/ControlEnums.cs"; 

        var uppercaseList = _testList.Select(x => x.ToUpper()).ToList();
        StreamWriter writer = new StreamWriter(filePath);
        var line = "public enum Control {" + string.Join(", ", uppercaseList) + "}";

        writer.WriteLine(line);
        writer.Close();

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log("File written successfully.");

    }
}
