using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimatedPlayer : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private SkinnedMeshRenderer _skinRenderer;
    [SerializeField] private Material _skinMaterial;
    [SerializeField] private Material _hairMaterial;
    [SerializeField] private List<GameObject> _hairPrefabs;
    [SerializeField] private List<GameObject> _facialHairPrefabs;
    [SerializeField, Range(0f, 1f)] private float _facialHairChance = 0.5f;
    [SerializeField] private Gradient _skinGradient;
    [SerializeField] private Gradient _hairGradient;
    [SerializeField] private Transform _head;

    public PlayerData PlayerData => _playerData;

    private void Start()
    {
        var skinMat = new Material(_skinMaterial);
        //skinMat.color = Random.ColorHSV(0.05f, 0.2f, 0.0f, 0.25f, 0.5f, 1.0f);
        skinMat.color = _skinGradient.Evaluate(Random.value);

        var hairMat = new Material(_hairMaterial);
        //hairMat.color = Random.ColorHSV(0.0f, 0.15f, 0.3f, 0.5f, 0.05f, 0.7f);
        hairMat.color = _hairGradient.Evaluate(Random.value);

        SwitchMaterial(_skinRenderer, skinMat, 0);

        if (_hairPrefabs.Count > 0)
        {
            var facialHairPrefab = _hairPrefabs[Random.Range(0, _hairPrefabs.Count - 1)];
            if (facialHairPrefab != null)
            {
                var facialHair = Instantiate(facialHairPrefab, _head);
                SwitchMaterial(facialHair.GetComponent<MeshRenderer>(), hairMat, 0);
            }
        }
        if (Random.value <= _facialHairChance && _facialHairPrefabs.Count > 0)
        {
            var hairPrefab = _facialHairPrefabs[Random.Range(0, _facialHairPrefabs.Count - 1)];
            if (hairPrefab != null)
            {
                var hair = Instantiate(hairPrefab, _head);
                SwitchMaterial(hair.GetComponent<MeshRenderer>(), hairMat, 0);
            }
        }
    }

    private void SwitchMaterial(Renderer renderer, Material material, int index = 0)
    {
        var mats = renderer.sharedMaterials;
        mats[index] = material;
        renderer.materials = mats;
    }
}
