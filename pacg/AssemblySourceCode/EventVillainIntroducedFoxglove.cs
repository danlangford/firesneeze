using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventVillainIntroducedFoxglove : Event
{
    private void DeleteCardFromGame(string id)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].Hand.Remove(Party.Characters[i].Hand[id]);
            Party.Characters[i].Deck.Remove(Party.Characters[i].Deck[id]);
            Party.Characters[i].Discard.Remove(Party.Characters[i].Discard[id]);
            Party.Characters[i].Bury.Remove(Party.Characters[i].Bury[id]);
        }
        Location.Current.Deck.Remove(Location.Current.Deck[id]);
        for (int j = 0; j < Scenario.Current.Locations.Length; j++)
        {
            if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[j].LocationName))
            {
                string locationName = Scenario.Current.Locations[j].LocationName;
                if (Location.Current.ID != locationName)
                {
                    Location.Remove(locationName, id);
                }
            }
        }
        Campaign.Box.Pull(id);
        Campaign.GalleryCards.Remove(id);
    }

    private Character GetCardOwner(string id)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Hand[id] != null)
            {
                return Party.Characters[i];
            }
            if (Party.Characters[i].Deck[id] != null)
            {
                return Party.Characters[i];
            }
            if (Party.Characters[i].Discard[id] != null)
            {
                return Party.Characters[i];
            }
            if (Party.Characters[i].Bury[id] != null)
            {
                return Party.Characters[i];
            }
        }
        return null;
    }

    public override void OnVillainIntroduced(Card card)
    {
        if (((card.ID == Scenario.Current.Villain) && (Turn.CheckBoard.Get<int>("FoxgloveEncountered") == 0)) && (Scenario.Current.NumVillainEncounters == 1))
        {
            Turn.CheckBoard.Set<int>("FoxgloveEncountered", 1);
            Game.Instance.StartCoroutine(this.VisualSequenceFoxglove());
        }
    }

    [DebuggerHidden]
    private IEnumerator VisualSequenceFoxglove() => 
        new <VisualSequenceFoxglove>c__Iterator29 { <>f__this = this };

    public override EventType Type =>
        EventType.OnVillainIntroduced;

    [CompilerGenerated]
    private sealed class <VisualSequenceFoxglove>c__Iterator29 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventVillainIntroducedFoxglove <>f__this;
        internal Card <foxglove>__2;
        internal Card <gift>__3;
        internal Character <owner>__1;
        internal GuiWindowLocation <window>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    Turn.State = GameStateType.Null;
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    goto Label_0365;

                case 1:
                    this.<owner>__1 = this.<>f__this.GetCardOwner("AL11_AldernFoxglove");
                    if (this.<owner>__1 == null)
                    {
                        goto Label_0322;
                    }
                    this.<foxglove>__2 = this.<owner>__1.Hand["AL11_AldernFoxglove"];
                    if (this.<foxglove>__2 == null)
                    {
                        this.<foxglove>__2 = CardTable.Create("AL11_AldernFoxglove");
                        if (this.<foxglove>__2 != null)
                        {
                            this.<foxglove>__2.transform.position = this.<window>__0.layoutLocation.transform.position;
                            this.<foxglove>__2.transform.localScale = this.<window>__0.layoutLocation.Scale;
                            this.<foxglove>__2.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
                            this.<foxglove>__2.Show(true);
                        }
                        break;
                    }
                    Turn.Number = Party.IndexOf(this.<owner>__1.ID);
                    this.<window>__0.Refresh();
                    Turn.Character.Hand.Remove(this.<foxglove>__2);
                    this.<foxglove>__2.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
                    this.<foxglove>__2.Show(true);
                    this.<foxglove>__2.MoveCard(this.<window>__0.layoutLocation.transform.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.<foxglove>__2.gameObject, this.<window>__0.layoutLocation.Scale, 0.5f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 2;
                    goto Label_0365;

                case 2:
                    break;

                case 3:
                    UnityEngine.Object.Destroy(this.<foxglove>__2.gameObject);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.2f));
                    this.$PC = 4;
                    goto Label_0365;

                case 4:
                    goto Label_028A;

                case 5:
                    goto Label_0322;

                default:
                    goto Label_0363;
            }
            if (this.<foxglove>__2 != null)
            {
                VisualEffect.ApplyToCard(VisualEffectType.CardBanishToBox, this.<foxglove>__2, 3f);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.32f));
                this.$PC = 3;
                goto Label_0365;
            }
        Label_028A:
            this.<gift>__3 = Campaign.Box.Draw("LT12_StalkerMask");
            if (this.<gift>__3 == null)
            {
                this.<gift>__3 = CardTable.Create("LT12_StalkerMask");
            }
            if (this.<gift>__3 != null)
            {
                Card[] cards = new Card[] { this.<gift>__3 };
                this.<window>__0.DrawCardsFromBox(cards, this.<owner>__1.Hand, Party.IndexOf(this.<owner>__1.ID));
                this.$current = new WaitForSeconds(3f);
                this.$PC = 5;
                goto Label_0365;
            }
        Label_0322:
            this.<>f__this.DeleteCardFromGame("AL11_AldernFoxglove");
            if (Turn.Number != Turn.Current)
            {
                Turn.Number = Turn.Current;
                this.<window>__0.Refresh();
            }
            Turn.State = GameStateType.Villain;
            this.$PC = -1;
        Label_0363:
            return false;
        Label_0365:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

