using UnityEngine;

public class PlayerArt : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinRenderer;
    [SerializeField] private Material _skinMaterial;
    [SerializeField] private Material _hairMaterial;
    [SerializeField] private Transform _head;
    [SerializeField] private Vector3 _localHairPosition = new Vector3(0, -0.0159f, 0.00015f);
    [SerializeField] private Vector3 _localHairRotation = new Vector3(-90, 0, 0);

    public void SetPlayerArt(PlayerArtData art)
    {
        var skinMat = new Material(_skinMaterial);
        //skinMat.color = Random.ColorHSV(0.05f, 0.2f, 0.0f, 0.25f, 0.5f, 1.0f);
        skinMat.color = art.SkinColor;

        SwitchMaterial(_skinRenderer, skinMat, 0);

        if (art.HairPrefab != null || art.FacialHairPrefab != null)
        {
            var hairMat = new Material(_hairMaterial);
            hairMat.color = art.HairColor;

            if (art.HairPrefab != null)
            {
                var hair = Instantiate(art.HairPrefab, transform, false);
                hair.transform.SetParent(_head);
                hair.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
                hair.layer = _skinRenderer.gameObject.layer;
                SwitchMaterial(hair.GetComponent<MeshRenderer>(), hairMat, 0);
            }

            if (art.FacialHairPrefab != null)
            {
                var facialHair = Instantiate(art.FacialHairPrefab, transform, false);
                facialHair.transform.SetParent(_head);
                facialHair.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
                facialHair.layer = _skinRenderer.gameObject.layer;
                SwitchMaterial(facialHair.GetComponent<MeshRenderer>(), hairMat, 0);
            }
        }

        if (art.AccessoryPrefab != null)
        {
            var headband = Instantiate(art.AccessoryPrefab, transform, false);
            headband.transform.SetParent(_head);
            headband.transform.SetLocalPositionAndRotation(_localHairPosition, Quaternion.Euler(_localHairRotation));
            headband.layer = _skinRenderer.gameObject.layer;
        }
    }

    private void SwitchMaterial(Renderer renderer, Material material, int index = 0)
    {
        var mats = renderer.sharedMaterials;
        mats[index] = material;
        renderer.materials = mats;
    }
}
