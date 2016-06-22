using System;
using System.Collections;
using UnityEngine;

public class GuiPanelVaultCharacters : GuiPanel
{
    [Tooltip("all avatars are under this transform in the scene")]
    public Transform AvatarRoot;
    [Tooltip("reference to the default toggle button in our hierarchy")]
    public GuiButton ButtonDefault;
    [Tooltip("reference to the vault toggle button in our hierarchy")]
    public GuiButton ButtonVault;
    [Tooltip("all default characters are ordered under this transform")]
    public Transform DefaultRoot;
    private TokenMode Mode;
    [Tooltip("references to the party drop slots contained in the scene (ordered)")]
    public CharacterTokenSlot[] PartySlots;
    [Tooltip("reference to the current token highlight in our hierarchy")]
    public GameObject TokenHilight;
    [Tooltip("references to the token slots containined in the scene (ordered)")]
    public CharacterTokenSlot[] TokenSlots;
    [Tooltip("all vault characters are instantiated under this transform")]
    public Transform VaultRoot;

    private void EmptyTokenSlots()
    {
        for (int i = 0; i < this.TokenSlots.Length; i++)
        {
            this.TokenSlots[i].Token = null;
            this.TokenSlots[i].Owner = null;
        }
    }

    private GameObject FindAvatar(string id)
    {
        for (int i = 0; i < this.AvatarRoot.childCount; i++)
        {
            Transform child = this.AvatarRoot.GetChild(i);
            if (child.name == id)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public CharacterToken FindToken(Character character)
    {
        for (int i = 0; i < this.VaultRoot.childCount; i++)
        {
            CharacterToken component = this.VaultRoot.GetChild(i).GetComponent<CharacterToken>();
            if (component.Character.NickName == character.NickName)
            {
                return component;
            }
        }
        for (int j = 0; j < this.DefaultRoot.childCount; j++)
        {
            CharacterToken token2 = this.DefaultRoot.GetChild(j).GetComponent<CharacterToken>();
            if (token2.ID == character.ID)
            {
                token2.Character = character;
                return token2;
            }
        }
        return null;
    }

    private bool GetTokenLocked(CharacterToken token)
    {
        if (this.IsTokenAlterateSelected(token))
        {
            return true;
        }
        if ((this.Mode == TokenMode.Vault) && (token.Character != null))
        {
            if (Vault.IsLocked(token.Character.NickName))
            {
                return true;
            }
            if (Campaign.Deaths.Contains(token.Character.NickName))
            {
                return true;
            }
        }
        return false;
    }

    public void HighlightToken(CharacterToken token)
    {
        if (token != null)
        {
            this.TokenHilight.transform.parent = token.transform;
            this.TokenHilight.transform.localPosition = Vector3.zero;
            this.TokenHilight.transform.localScale = Vector3.one;
            this.TokenHilight.SetActive(true);
        }
        else
        {
            this.TokenHilight.transform.parent = base.transform;
            this.TokenHilight.SetActive(false);
        }
    }

    public override void Initialize()
    {
        this.Mode = TokenMode.Vault;
        IEnumerator enumerator = Vault.List(Game.GameMode).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                string current = (string) enumerator.Current;
                Character character = Party.FindByNickName(current);
                if (character == null)
                {
                    character = Vault.Get(current);
                }
                if (character != null)
                {
                    CharacterToken token = CharacterToken.Create(character.ID);
                    if (token != null)
                    {
                        character.NickName = current;
                        token.transform.parent = this.VaultRoot;
                        token.Avatar = this.FindAvatar(character.ID);
                        token.Avatar.SetActive(false);
                        token.Default = false;
                        token.Character = character;
                        token.Locked = this.GetTokenLocked(token);
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        this.Mode = TokenMode.Default;
        for (int i = 0; i < this.DefaultRoot.childCount; i++)
        {
            CharacterToken component = this.DefaultRoot.GetChild(i).GetComponent<CharacterToken>();
            if (component != null)
            {
                component.Avatar = this.FindAvatar(component.ID);
                component.Avatar.SetActive(false);
                component.Default = true;
                component.Character = CharacterTable.Create(component.ID);
                component.Character.NickName = Vault.CreateNickname(component.Character);
                component.Locked = this.GetTokenLocked(component);
            }
        }
        if (this.PartyContainsVaultCharacter())
        {
            this.Mode = TokenMode.Vault;
        }
        else
        {
            this.Mode = TokenMode.Default;
        }
    }

    private bool IsTokenAlterateSelected(CharacterToken token)
    {
        if ((token != null) && (this.PartySlots != null))
        {
            for (int i = 0; i < this.PartySlots.Length; i++)
            {
                if (((this.PartySlots[i].Token != null) && (this.PartySlots[i].Token != token)) && (this.PartySlots[i].Token.ID == token.ID))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsTokenSelected(CharacterToken token)
    {
        if (token == null)
        {
            return false;
        }
        if (token.Slot == null)
        {
            return false;
        }
        return (token.Slot is CharacterTokenSlotPartyMember);
    }

    private void OnToggleDefaultCharacters()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.Mode = TokenMode.Default;
            UI.Window.Refresh();
        }
    }

    private void OnToggleVaultCharacters()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.Mode = TokenMode.Vault;
            UI.Window.Refresh();
        }
    }

    private bool PartyContainsVaultCharacter()
    {
        if (Party.Characters.Count > 0)
        {
            for (int i = 0; i < this.VaultRoot.childCount; i++)
            {
                CharacterToken component = this.VaultRoot.GetChild(i).GetComponent<CharacterToken>();
                if (component != null)
                {
                    for (int j = 0; j < Party.Characters.Count; j++)
                    {
                        if (Party.Characters[j].NickName == component.Character.NickName)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public override void Refresh()
    {
        if (this.Mode == TokenMode.Default)
        {
            this.EmptyTokenSlots();
            this.ShowVaultTokens(false);
            this.ShowDefaultTokens(true);
            this.ButtonVault.Glow(false);
            this.ButtonDefault.Glow(true);
        }
        if (this.Mode == TokenMode.Vault)
        {
            this.EmptyTokenSlots();
            this.ShowDefaultTokens(false);
            this.ShowVaultTokens(true);
            this.ButtonVault.Glow(true);
            this.ButtonDefault.Glow(false);
        }
        GuiWindowCreateParty window = UI.Window as GuiWindowCreateParty;
        if (window != null)
        {
            window.TokenLayout.Layout();
        }
        this.ButtonVault.Disable(Vault.CountByMode(Game.GameMode) <= 0);
    }

    private void ShowDefaultTokens(bool isVisible)
    {
        for (int i = 0; i < this.DefaultRoot.childCount; i++)
        {
            if (i < this.TokenSlots.Length)
            {
                CharacterToken component = this.DefaultRoot.GetChild(i).GetComponent<CharacterToken>();
                this.ShowToken(component, this.TokenSlots[i], isVisible);
            }
        }
    }

    private void ShowToken(CharacterToken token, CharacterTokenSlot slot, bool isVisible)
    {
        if (token != null)
        {
            if (isVisible)
            {
                slot.Owner = token;
                token.Home = slot;
                if (!this.IsTokenSelected(token))
                {
                    slot.Token = token;
                    token.Slot = slot;
                    token.transform.position = slot.transform.position;
                }
                token.Locked = this.GetTokenLocked(token);
                token.Show(isVisible);
            }
            else
            {
                token.Home = null;
                token.Show(this.IsTokenSelected(token));
                if (token.Shadow != null)
                {
                    token.Shadow.SetActive(false);
                }
            }
        }
    }

    private void ShowVaultTokens(bool isVisible)
    {
        for (int i = 0; i < this.VaultRoot.childCount; i++)
        {
            if (i < this.TokenSlots.Length)
            {
                CharacterToken component = this.VaultRoot.GetChild(i).GetComponent<CharacterToken>();
                this.ShowToken(component, this.TokenSlots[i], isVisible);
            }
        }
    }

    public void Toggle()
    {
        if (this.Mode == TokenMode.Default)
        {
            this.Mode = TokenMode.Vault;
        }
        else
        {
            this.Mode = TokenMode.Default;
        }
        this.Refresh();
    }

    private enum TokenMode
    {
        Default,
        Vault
    }
}

