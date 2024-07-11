using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WFCSTileInfo : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _AbrDisplay = _grassAbr + _pathAbr + _forestAbr;
    [SerializeField] public string _top;
    [SerializeField] public string _right;
    [SerializeField] public string _down;
    [SerializeField] public string _left;

    [SerializeField, HideInInspector] private bool _showWarning;
    [SerializeField, ReadOnly, ConditionalField(nameof(_showWarning))] private string _warning;

    private const string _grassAbr = "G";
    private const string _pathAbr = "P";
    private const string _forestAbr = "F";
    private List<string> _Abrs = new List<string>() {_grassAbr, _pathAbr, _forestAbr};


    private void OnValidate()
    {
        CheckStrings();
    }

    public bool HasOtherTypes(string types)
    {
        var edges = GetEdges();
        foreach (var edge in edges) {
            foreach (var letter in edge) {
                if (!types.Contains(letter.ToString())) return true;
            }
        }
        return false;
    }

    public List<string> GetEdges() => GetEdges(Quaternion.identity);

    public List<string> GetEdges(Quaternion rot)
    {
        var edgeList = new List<string>() { _top, _right, _down, _left };
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 90, 0)) < 0.1f) rotateEdges(ref edgeList);
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 180, 0)) < 0.1f) rotateEdges(ref edgeList, 1);
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 270, 0)) < 0.1f) rotateEdges(ref edgeList, 2);
        return edgeList;
    }

    public void Rotate(Quaternion rot)
    {
        var turnCount = Utilities.QuaternionToTurnCount(rot);
        RotatePublicEdges(turnCount);
    }

    private void RotatePublicEdges(int numTimes)
    {
        if (numTimes == 0) return;
        var edges = new List<string>() { _top, _right, _down, _left };
        for (int i = 0; i < numTimes; i++) rotateEdges(ref edges);
        _top = edges[0];
        _right = edges[1];
        _down = edges[2];
        _left = edges[3];
    }

    public bool GetValidRotation(List<string> hole, out Quaternion rot)
    {
        var edges = new List<string>() { _top, _right, _down, _left };

        if (FitsInHole(edges, hole)) {
            rot = Quaternion.identity;
            return true;
        }
        rotateEdges(ref edges);
        if (FitsInHole(edges, hole)) {
            rot = Quaternion.Euler(0, 90, 0);
            return true;
        }
        rotateEdges(ref edges);
        if (FitsInHole(edges, hole)) {
            rot = Quaternion.Euler(0, 180, 0);
            return true;
        }
        rotateEdges(ref edges);
        if (FitsInHole(edges, hole)) {
            rot = Quaternion.Euler(0, 270, 0);
            return true;
        }

        rot = Quaternion.identity;
        return false;
    }

    private void rotateEdges(ref List<string> edges, int numTimes = 0)
    {
        edges.Insert(0, edges[3]);
        edges.RemoveAt(4);
        if (numTimes > 0) rotateEdges(ref edges, numTimes - 1);
    }

    private bool FitsInHole(List<string> edges, List<string> hole)
    {
        for (int i = 0; i < hole.Count; i++) {
            if (string.IsNullOrEmpty(hole[i])) continue;
            if (!string.Equals(edges[i], hole[i])) return false;
        }
        return true;
    }

    public string GetEdgeAt(Direction dir)
    {
        if (dir == Direction.UP) return _top;
        if (dir == Direction.RIGHT) return _right;
        if (dir == Direction.DOWN) return _down;
        if (dir == Direction.LEFT) return _left;
        return "";
    }

    public string AbrToLong(string Abr)
    {
        var output = new List<string>();
        foreach (var character in Abr) {
            var letter = character.ToString();
            if (letter == _grassAbr) output.Add("Grass");
            if (letter == _pathAbr) output.Add("Path");
            if (letter == _forestAbr) output.Add("Forest");
        }
        return string.Join(", ", output);
    }

    private void CheckStrings()
    {
        _Abrs = new List<string>() { _grassAbr, _pathAbr, _forestAbr };
        _AbrDisplay = _grassAbr + _pathAbr + _forestAbr;

        _showWarning = false;
        _warning = "";

        CheckString(ref _top, "top");
        CheckString(ref _right, "right");
        CheckString(ref _down, "down");
        CheckString(ref _left, "left");
    }

    private void CheckString(ref string value, string label)
    {
        if (value.Length == 0) return;
        
        value = value.ToUpper();
        foreach (var letter in value) {
            if (!_Abrs.Contains(letter.ToString())) {
                _showWarning = true;
                _warning += label + ", ";
            }
        }
    }

    
}
