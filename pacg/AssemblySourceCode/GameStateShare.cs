using System;
using UnityEngine;

public class GameStateShare : GameState
{
    private static string activePowerID;
    private static bool isPowerMode;

    public static void AddDecoration(Card card, string power)
    {
        for (int i = 0; i < Turn.Character.Powers.Count; i++)
        {
            if (Turn.Character.Powers[i].ID == power)
            {
                GameObject obj2 = card.Decorations.Add("Blueprints/Gui/Vfx_Card_Sprite", CardSideType.Front, null, 2f);
                if (obj2 != null)
                {
                    obj2.SetActive(true);
                    SpriteRenderer component = obj2.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    if (component != null)
                    {
                        component.sprite = Turn.Character.Powers[i].Icon;
                    }
                }
                break;
            }
        }
    }

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            isPowerMode = false;
            window.ShowCancelButton(false);
            window.ShowProceedButton(false);
            window.sharePanel.Show(true);
            ShowHelperText(0x5e);
        }
    }

    public override void Exit(GameStateType nextState)
    {
    }

    private static int GetNextSharePriority(Deck hand)
    {
        int a = 0;
        for (int i = 0; i < hand.Count; i++)
        {
            a = Mathf.Max(a, hand[i].SharedPriority);
        }
        return ++a;
    }

    public static Card GetTopPriorityCard(Deck deck)
    {
        if ((deck != null) && (deck.Count <= 0))
        {
            return null;
        }
        Card card = null;
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].Shared && ((card == null) || (card.SharedPriority > deck[i].SharedPriority)))
            {
                card = deck[i];
            }
        }
        return card;
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (action != ActionType.Share)
        {
            return false;
        }
        return (isPowerMode || card.Shareable);
    }

    public static bool IsCardShareable(Card card) => 
        (isPowerMode || card.Shareable);

    public static bool IsCardSharedAsCard(Card card)
    {
        if (!card.Shared)
        {
            return false;
        }
        if (!string.IsNullOrEmpty(card.SharedPower))
        {
            return false;
        }
        return true;
    }

    public static bool IsCardSharedAsFuel(Card card)
    {
        if (!card.Shared)
        {
            return false;
        }
        if (string.IsNullOrEmpty(card.SharedPower))
        {
            return false;
        }
        return true;
    }

    public override void Proceed()
    {
        if (Turn.Phase == TurnPhaseType.End)
        {
            Turn.Next();
            Turn.State = GameStateType.Switch;
        }
        else
        {
            Turn.State = Turn.PopReturnState();
        }
    }

    public static void SetPowerMode(bool b, string id)
    {
        isPowerMode = b;
        activePowerID = id;
        if (isPowerMode)
        {
            ShowHelperText(0x5f);
        }
        else
        {
            ShowHelperText(0x5e);
        }
    }

    public static void Share(Card card, bool isShared)
    {
        if (isShared)
        {
            card.Shared = true;
            card.SharedPriority = GetNextSharePriority(card.Deck);
            if (isPowerMode)
            {
                card.SharedPower = activePowerID;
                AddDecoration(card, activePowerID);
            }
            else
            {
                card.SharedPower = null;
            }
        }
        else
        {
            card.Shared = false;
            card.SharedPriority = 0;
            card.SharedPower = null;
            card.Decorations.Clear();
        }
        SetPowerMode(false, null);
    }

    private static void ShowHelperText(int id)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            string helperText = StringTableManager.GetHelperText(id);
            window.messagePanel.Show(helperText);
        }
    }

    public override GameStateType Type =>
        GameStateType.Share;
}

