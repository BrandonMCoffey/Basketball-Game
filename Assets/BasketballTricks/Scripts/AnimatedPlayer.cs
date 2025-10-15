using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimatedPlayer : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private SkinnedMeshRenderer _skinRenderer;
    [SerializeField] private Material _skinMaterial;
    [SerializeField] private Gradient _skinGradient;
    [SerializeField] private Transform _head;
    [SerializeField] private Material _hairMaterial;
    [SerializeField] private List<HairData> _hairPrefabs;

    public PlayerData PlayerData => _playerData;

    private void Start()
    {
        var skinMat = new Material(_skinMaterial);
        //skinMat.color = Random.ColorHSV(0.05f, 0.2f, 0.0f, 0.25f, 0.5f, 1.0f);
        skinMat.color = _skinGradient.Evaluate(Random.value);

        SwitchMaterial(_skinRenderer, skinMat, 0);

        if (_hairPrefabs.Count > 0)
        {
            var hairData = _hairPrefabs[Random.Range(0, _hairPrefabs.Count - 1)];
            if (hairData != null)
            {
                var hair = Instantiate(hairData.HairObject, transform, false);
                hair.transform.SetParent(_head);
                var hairMat = new Material(_hairMaterial);
                hairMat.color = hairData.GetHairColor();
                SwitchMaterial(hair.GetComponent<MeshRenderer>(), hairMat, 0);

                var facialHairPrefab = hairData.GetFacialHair();
                if (facialHairPrefab != null)
                {
                    var facialHair = Instantiate(facialHairPrefab, transform, false);
                    facialHair.transform.SetParent(_head);
                    SwitchMaterial(facialHair.GetComponent<MeshRenderer>(), hairMat, 0);
                }

                var headbandPrefab = hairData.GetHeadBand();
                if (headbandPrefab != null)
                {
                    var headband = Instantiate(headbandPrefab, transform, false);
                    headband.transform.SetParent(_head);
                }
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
