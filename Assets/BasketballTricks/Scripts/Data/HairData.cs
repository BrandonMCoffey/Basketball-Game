using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HairData : ScriptableObject
{
    [SerializeField] private GameObject _hair;
    [SerializeField] private Gradient _hairColorGraident;
    [SerializeField, Range(0f, 1f)] private float _facialHairChance = 0.5f;
    [SerializeField] private List<GameObject> _facialHair;
    [SerializeField, Range(0f, 1f)] private float _headBandChance = 0.5f;
    [SerializeField] private GameObject _headBand;

    public GameObject HairObject => _hair;

    public Color GetHairColor()
    {
        return _hairColorGraident.Evaluate(Random.value);
    }

    public GameObject GetFacialHair()
    {
        if (_facialHair.Count > 0 && Random.value < _facialHairChance)
        {
            return _facialHair[Random.Range(0, _facialHair.Count - 1)];
        }
        return null;
    }

    public GameObject GetHeadBand()
    {
        return Random.value < _headBandChance ? _headBand : null;
    }
}
