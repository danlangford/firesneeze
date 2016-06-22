using System;

public class CharacterTokenSlotPartyOrder : CharacterTokenSlot
{
    public override bool OnDrop(CharacterToken token)
    {
        if (base.Locked)
        {
            return false;
        }
        if (token == null)
        {
            return false;
        }
        if (base.Token == token)
        {
            return false;
        }
        Party.Swap(Party.Characters[base.Slot], Party.Characters[token.Slot.Slot]);
        if (base.Token != null)
        {
            base.Token.OnGuiDrop(token.Slot);
        }
        if (token.Slot != null)
        {
            token.Slot.Token = base.Token;
        }
        base.Token = token;
        UI.Sound.Play(base.DropSound);
        return true;
    }
}

