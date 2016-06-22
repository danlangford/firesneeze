public class CardPropertyCopy : CardProperty
{
    public CardPower[] Powers
    {
        get
        {
            if (Scenario.Current.Discard.Count > 0)
            {
                return Scenario.Current.Discard[0].GetPowers();
            }
            return null;
        }
    }
}

