using UnityEngine;

public class PlayerArt : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinRenderer;
    [SerializeField] private Material _playerMaterial;
    [SerializeField] private Material _shirtMaterial;
    [SerializeField] private Transform _head;
    [SerializeField] private Vector3 _localHairPosition = new Vector3(0, -0.0159f, 0.00015f);
    [SerializeField] private Vector3 _localHairRotation = new Vector3(-90, 0, 0);

    public Transform Head => _head;
    [SerializeField] private GameObject _currentHair;
    [SerializeField] private GameObject _currentFacialHair;
    [SerializeField] private GameObject _currentHeadband;

    public void SetPlayerArt(PlayerArtData art)
    {
        var playerMat = new Material(_playerMaterial);
        //skinMat.color = Random.ColorHSV(0.05f, 0.2f, 0.0f, 0.25f, 0.5f, 1.0f);
        playerMat.SetColor("_SkinColor", art.SkinColor);
        playerMat.SetColor("_HairColor", art.HairColor);

        SwitchMaterial(_skinRenderer, playerMat, 0);

        if (art.JerseyTexture != null)
        {
            var shirtMat = new Material(_shirtMaterial);
            shirtMat.color = Color.white;
            if (shirtMat.HasTexture("_BaseMap")) shirtMat.SetTexture("_BaseMap", art.JerseyTexture);
            else if (shirtMat.HasTexture("_MainTex")) shirtMat.SetTexture("_MainTex", art.JerseyTexture);
            else Debug.LogWarning("Shirt material has no known texture property to set!");
            SwitchMaterial(_skinRenderer, shirtMat, 1);
        }

#if UNITY_EDITOR // Special handling to destroy objects from OnValidate calls
        if (_currentHair != null || _currentFacialHair != null || _currentHeadband != null)
        {
            var objs = new GameObject[] { _currentHair,  _currentFacialHair,  _currentHeadband };
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                    foreach (var obj in objs)
                        if (obj != null)
                            UnityEditor.Undo.DestroyObjectImmediate(obj);
            };
            _currentHair = null;
            _currentFacialHair = null;
            _currentHeadband = null;
        }
#endif
        if (_currentHair != null) Destroy(_currentHair);
        if (_currentFacialHair != null) Destroy(_currentFacialHair);
        if (_currentHeadband != null) Destroy(_currentHeadband);

        if (art.HairPrefab != null)
        {
            _currentHair = Instantiate(art.HairPrefab, transform, false);
            _currentHair.transform.SetParent(_head);
            _currentHair.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
            _currentHair.layer = _skinRenderer.gameObject.layer;
            SwitchMaterial(_currentHair.GetComponent<MeshRenderer>(), playerMat, 0);
        }

        if (art.FacialHairPrefab != null)
        {
            _currentFacialHair = Instantiate(art.FacialHairPrefab, transform, false);
            _currentFacialHair.transform.SetParent(_head);
            _currentFacialHair.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
            _currentFacialHair.layer = _skinRenderer.gameObject.layer;
            SwitchMaterial(_currentFacialHair.GetComponent<MeshRenderer>(), playerMat, 0);
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
