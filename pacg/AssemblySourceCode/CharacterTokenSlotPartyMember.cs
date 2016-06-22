using System;

public class CharacterTokenSlotPartyMember : CharacterTokenSlot
{
    public override bool OnDrop(CharacterToken token)
    {
        if (base.Locked)
        {
            return false;
        }
        if (base.Token == token)
        {
            return false;
        }
        if (token == null)
        {
            return false;
        }
        if (base.Token != null)
        {
            return false;
        }
        if (!token.Licensed)
        {
            UI.Window.SendMessage("ShowLicensePopup");
            return false;
        }
        if (token.Slot != null)
        {
            token.Slot.Token = null;
        }
        UI.Sound.Play(base.DropSound);
        token.Slot = this;
        base.Token = token;
        if (string.IsNullOrEmpty(token.Character.NickName))
        {
            token.Character.NickName = Vault.CreateNickname(token.Character);
            token.Character = token.Character;
        }
        return true;
    }

    public override void Refresh()
    {
        if (base.gameObject.activeSelf)
        {
            GuiImage component = base.GetComponent<GuiImage>();
            if (component != null)
            {
                if (base.Token != null)
                {
                    component.FadeOut(0.15f);
                }
                else
                {
                    component.FadeIn(0.15f);
                }
            }
        }
    }
}

