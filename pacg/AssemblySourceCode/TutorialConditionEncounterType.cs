using System;

public class TutorialConditionEncounterType : TutorialCondition
{
    public CardType Type = CardType.Monster;

    public override bool Evaluate() => 
        ((Turn.Card != null) && (Turn.Card.Type == this.Type));
}

