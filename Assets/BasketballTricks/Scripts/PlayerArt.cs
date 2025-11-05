using UnityEngine;

public class PlayerArt : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinRenderer;
    [SerializeField] private Material _skinMaterial;
    [SerializeField] private Material _hairMaterial;
    [SerializeField] private Transform _head;
    [SerializeField] private Vector3 _localHairPosition = new Vector3(0, -0.0159f, 0.00015f);
    [SerializeField] private Vector3 _localHairRotation = new Vector3(-90, 0, 0);

    public Transform Head => _head;
    private GameObject _currentHair;
    private GameObject _currentFacialHair;
    private GameObject _currentHeadband;

    public void SetPlayerArt(PlayerArtData art)
    {
        var skinMat = new Material(_skinMaterial);
        //skinMat.color = Random.ColorHSV(0.05f, 0.2f, 0.0f, 0.25f, 0.5f, 1.0f);
        skinMat.color = art.SkinColor;

        SwitchMaterial(_skinRenderer, skinMat, 0);

        if (_currentHair != null) Destroy(_currentHair);
        if (_currentFacialHair != null) Destroy(_currentFacialHair);
        if (_currentHeadband != null) Destroy(_currentHeadband);

        if (art.HairPrefab != null || art.FacialHairPrefab != null)
        {
            var hairMat = new Material(_hairMaterial);
            hairMat.color = art.HairColor;

            if (art.HairPrefab != null)
            {
                _currentHair = Instantiate(art.HairPrefab, transform, false);
                _currentHair.transform.SetParent(_head);
                _currentHair.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
                _currentHair.layer = _skinRenderer.gameObject.layer;
                SwitchMaterial(_currentHair.GetComponent<MeshRenderer>(), hairMat, 0);
            }

            if (art.FacialHairPrefab != null)
            {
                _currentFacialHair = Instantiate(art.FacialHairPrefab, transform, false);
                _currentFacialHair.transform.SetParent(_head);
                _currentFacialHair.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
                _currentFacialHair.layer = _skinRenderer.gameObject.layer;
                SwitchMaterial(_currentFacialHair.GetComponent<MeshRenderer>(), hairMat, 0);
            }
        }

        if (art.AccessoryPrefab != null)
        {
            _currentHeadband = Instantiate(art.AccessoryPrefab, transform, false);
            _currentHeadband.transform.SetParent(_head);
            _currentHeadband.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
            _currentHeadband.layer = _skinRenderer.gameObject.layer;
        }
    }

    private void SwitchMaterial(Renderer renderer, Material material, int index = 0)
    {
        var mats = renderer.sharedMaterials;
        mats[index] = material;
        renderer.materials = mats;
    }
}
