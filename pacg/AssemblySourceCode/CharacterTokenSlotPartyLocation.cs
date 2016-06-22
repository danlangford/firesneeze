using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTokenSlotPartyLocation : CharacterTokenSlot
{
    private static readonly Vector3 dropSize = new Vector3(1f, 1f, 1f);
    private static readonly int slotSortingOrder = 0x13;
    private static Stack<string> SwitchesDone = new Stack<string>(Constants.MAX_PARTY_MEMBERS);

    private int CountCharactersAtLocation(string locID)
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Location == locID)
            {
                num++;
            }
        }
        return num;
    }

    private Vector3 GetCharacterIconPosition(int index, int max)
    {
        float num = 1f;
        float characterIconSpacing = this.GetCharacterIconSpacing(Party.Characters.Count);
        float num3 = (characterIconSpacing * (max - 1)) / 2f;
        float num4 = (180f - num3) + ((index - 1) * characterIconSpacing);
        float x = num * Mathf.Sin(num4 * 0.01745329f);
        return new Vector3(x, num * Mathf.Cos(num4 * 0.01745329f), 0f);
    }

    private float GetCharacterIconSpacing(int partySize)
    {
        if (partySize <= 4)
        {
            return 75f;
        }
        if (partySize == 5)
        {
            return 65f;
        }
        return 60f;
    }

    private Character GetNextCharacter(string ID)
    {
        SwitchesDone.Push(base.Token.ID);
        if (SwitchesDone.Count < Party.Characters.Count)
        {
            int index = Party.IndexOf(base.Token.ID);
            if (index >= 0)
            {
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    index = Party.GetNextPartyMemberIndex(index);
                    if (!SwitchesDone.Contains(Party.Characters[index].ID))
                    {
                        return Party.Characters[index];
                    }
                }
            }
        }
        return null;
    }

    public override bool OnDrop(CharacterToken token)
    {
        if (base.Locked)
        {
            return false;
        }
        UI.Sound.Play(base.DropSound);
        LeanTween.move(token.gameObject, base.transform.position, 0.2f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.scale(token.gameObject, dropSize, 0.2f).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action<object>(this.SetCharacterLocation)).setOnCompleteParam(token);
        if ((token.Slot != null) && !(token.Slot is CharacterTokenSlotPartyLocation))
        {
            token.Slot.Locked = true;
        }
        token.Home = null;
        token.Slot = null;
        base.Token = token;
        return true;
    }

    public override void Refresh()
    {
        int num = 0;
        ScenarioMapIcon component = base.GetComponent<ScenarioMapIcon>();
        int max = this.CountCharactersAtLocation(component.ID);
        CharacterToken[] tokenArray = UnityEngine.Object.FindObjectsOfType<CharacterToken>();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Location == component.ID)
            {
                for (int j = 0; j < tokenArray.Length; j++)
                {
                    if (tokenArray[j].ID == Party.Characters[i].ID)
                    {
                        Vector3 to = component.transform.position + this.GetCharacterIconPosition(++num, max);
                        LeanTween.moveLocal(tokenArray[j].gameObject, to, 0.2f).setEase(LeanTweenType.easeOutQuad);
                        LeanTween.scale(tokenArray[j].gameObject, dropSize, 0.2f).setEase(LeanTweenType.easeOutQuad);
                        tokenArray[j].SortingOrder = slotSortingOrder;
                    }
                }
            }
        }
    }

    private void RefreshMapIcons()
    {
        CharacterTokenSlotPartyLocation[] locationArray = UnityEngine.Object.FindObjectsOfType<CharacterTokenSlotPartyLocation>();
        if (locationArray != null)
        {
            for (int i = 0; i < locationArray.Length; i++)
            {
                locationArray[i].Refresh();
            }
        }
    }

    private void SetCharacterLocation(object token)
    {
        ScenarioMapIcon component = base.GetComponent<ScenarioMapIcon>();
        if (component != null)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].ID == base.Token.ID)
                {
                    Party.Characters[i].Location = component.ID;
                    break;
                }
            }
            CharacterToken token2 = token as CharacterToken;
            token2.SortingOrder = slotSortingOrder;
            this.RefreshMapIcons();
            if (token2.Slot != null)
            {
                token2.Slot.Locked = false;
            }
            GuiWindowScenario window = UI.Window as GuiWindowScenario;
            if (window != null)
            {
                window.LocationPanel.Show(component.ID);
            }
            UI.Window.SendMessage("OnTokenDropComplete");
        }
    }
}

