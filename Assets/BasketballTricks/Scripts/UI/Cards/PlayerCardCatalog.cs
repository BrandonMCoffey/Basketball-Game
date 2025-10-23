public class PlayerCardCatalog : PlayerCard
{
    private int _index;

    protected override void OnClickCard()
    {
        FlipCard();
    }

    public void SetIndex(int index)
    {
        _index = index;
    }
}
