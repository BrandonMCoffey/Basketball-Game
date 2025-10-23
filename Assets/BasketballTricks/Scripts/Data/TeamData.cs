using UnityEngine;

[CreateAssetMenu(fileName = "TeamData", menuName = "BasketballTricks/TeamData")]
public class TeamData : ScriptableObject
{
    [SerializeField] private string _teamName;
    [SerializeField] private Sprite _teamLogo;
    [SerializeField] private Color _teamPrimaryColor = Color.white;
    [SerializeField] private Color _teamSecondaryColor = Color.black;

    public string TeamName => _teamName;
    public Sprite TeamLogo => _teamLogo;
    public Color TeamPrimaryColor => _teamPrimaryColor;
    public Color TeamSecondaryColor => _teamSecondaryColor;
}
