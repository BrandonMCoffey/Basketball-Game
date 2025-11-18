using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomPlayerArtData", menuName = "BasketballTricks/RandomPlayerArtData", order = 1)]
public class RandomPlayerArtData : ScriptableObject
{
    [SerializeField] private Gradient _skinGradient;
    [SerializeField] private GameObject _hair;
    [SerializeField] private Gradient _hairColorGraident;
    [SerializeField, Range(0f, 1f)] private float _facialHairChance = 0.5f;
    [SerializeField] private List<GameObject> _facialHair;
    [SerializeField, Range(0f, 1f)] private float _headBandChance = 0.5f;
    [SerializeField] private GameObject _headBand;

    public GameObject HairObject => _hair;

    public Color GetSkinColor() => _skinGradient.Evaluate(Random.value);
    public Color GetHairColor() => _hairColorGraident.Evaluate(Random.value);
    public GameObject GetFacialHair() => _facialHair.Count > 0 && Random.value < _facialHairChance ? _facialHair[Random.Range(0, _facialHair.Count - 1)] : null;
    public GameObject GetHeadBand() => Random.value < _headBandChance ? _headBand : null;

    public PlayerArtData GetData()
    {
        var data = new PlayerArtData
        {
            SkinColor = GetSkinColor(),
            HairColor = GetHairColor(),
            JerseyTexture = null,
            HairPrefab = HairObject,
            FacialHairPrefab = GetFacialHair(),
            AccessoryPrefab = GetHeadBand()
        };
        return data;
    }
}