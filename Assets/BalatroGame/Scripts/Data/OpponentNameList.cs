using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Opponent Name List", menuName = "Balatro/Opponent Name List")]
public class OpponentNameList : ScriptableObject
{
    public List<string> Names;
}