using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlockSetAsideCards : Block
{
    [Tooltip("the amount of cards to remove")]
    public DiceType Amount;
    [Tooltip("the black magga monster")]
    public Animator BlackMagga;
    private Animator BlackMaggaInstance;
    private int[] cardsToRemove;
    private int cardsToRemoveIndex;
    private Effect destination;
    [Tooltip("the bonus to removing cards")]
    public int DiceBonus;
    private Card fakeCardInstance;
    private readonly Vector2 MONSTER_OFFSET_LEFT = new Vector2(1f, 1f);
    private readonly Vector2 MONSTER_OFFSET_RIGHT = new Vector2(-1f, 1f);
    private const float MONSTER_SPEED = 6f;
    [Tooltip("the position to remove cards from")]
    public DeckPositionType RemoveFrom;
    [Tooltip("which random locations to choose;")]
    public LocationSelector Selector;

    public void BlackMaggaChomp()
    {
        if (this.cardsToRemoveIndex > 0)
        {
            UI.Sound.Play(SoundEffectType.MaggaAdditionalBites);
        }
        Game.Instance.StartCoroutine(this.ChompAnimation());
    }

    private Vector3 BlackMaggaDestination()
    {
        Vector3 vector = (Vector3) this.MONSTER_OFFSET_RIGHT;
        if (this.BlackMaggaInstance.transform.rotation != Quaternion.identity)
        {
            vector = (Vector3) this.MONSTER_OFFSET_LEFT;
        }
        return (this.LocationDestination() + vector);
    }

    private void BlackMaggaFace(Vector3 destination)
    {
        if ((destination.x - this.BlackMaggaInstance.transform.position.x) < 0f)
        {
            this.BlackMaggaInstance.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else
        {
            this.BlackMaggaInstance.transform.rotation = Quaternion.identity;
        }
    }

    [DebuggerHidden]
    private IEnumerator ChompAnimation() => 
        new <ChompAnimation>c__Iterator10 { <>f__this = this };

    private Vector3[] FakeCardPath(GameObject card)
    {
        Vector3 end = card.transform.position + ((Vector3) (Vector3.up * 3f));
        Vector3 zero = Vector3.zero;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            zero = window.effectsPanel.transform.position;
        }
        Vector3[] first = Geometry.GetCurve(card.transform.position, end, 0f);
        Vector3[] second = Geometry.GetCurve(end, zero, 0f);
        return first.Concat<Vector3>(second).ToArray<Vector3>();
    }

    private int[] GetCardsToRemove()
    {
        int[] numArray = new int[Mathf.Min(Mathf.Max(Rules.RollDice(this.Amount) + this.DiceBonus, 0), Location.Current.Deck.Count)];
        switch (this.RemoveFrom)
        {
            case DeckPositionType.Top:
                for (int i = 0; i < numArray.Length; i++)
                {
                    numArray[i] = i;
                }
                return numArray;

            case DeckPositionType.Bottom:
                if (numArray.Length > 0)
                {
                    numArray[0] = Location.Current.Deck.Count - 1;
                    for (int j = 1; j < numArray.Length; j++)
                    {
                        numArray[j] = numArray[j - 1] - 1;
                    }
                }
                return numArray;

            case DeckPositionType.Shuffle:
                return numArray;
        }
        return numArray;
    }

    private void InstantiateMagga(string loc)
    {
        if (this.BlackMaggaInstance == null)
        {
            this.BlackMaggaInstance = UnityEngine.Object.Instantiate<Animator>(this.BlackMagga);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                this.BlackMaggaInstance.transform.parent = window.mapPanel.transform.FindChild("Map/Art");
                this.BlackMaggaInstance.transform.position = Vector3.zero;
            }
        }
    }

    public override void Invoke()
    {
        string str = this.Selector.Random();
        if (!string.IsNullOrEmpty(str))
        {
            Location.Load(str);
            this.RefreshCardList();
            this.cardsToRemove = this.GetCardsToRemove();
            this.destination = Scenario.Current.GetEffect(EffectType.Nameable);
            if (this.destination == null)
            {
                this.destination = new EffectNameable(Scenario.Current.ID, Effect.DurationPermament, 0x59);
                Scenario.Current.ApplyEffect(this.destination);
            }
            this.InstantiateMagga(str);
            if (this.BlackMaggaInstance != null)
            {
                this.BlackMaggaInstance.GetComponent<BlackMaggaController>().Block = this;
            }
            Game.Instance.StartCoroutine(this.PlayCoroutine());
        }
    }

    private bool IsFacingRight() => 
        (this.BlackMaggaInstance.transform.rotation == Quaternion.identity);

    private Vector3 LocationDestination()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            return window.mapPanel.GetMapIcon(Location.Current.ID).transform.position;
        }
        return Vector3.zero;
    }

    [DebuggerHidden]
    private IEnumerator PlayCoroutine() => 
        new <PlayCoroutine>c__IteratorF { <>f__this = this };

    private void RefreshCardList()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.Show(Location.Current.ID);
        }
    }

    private void RemoveCard()
    {
        Card card = Location.Current.Deck[this.cardsToRemove[this.cardsToRemoveIndex]];
        this.destination.sources.Add(card.ID);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && (window.effectsPanel.ScenarioButton != null))
        {
            window.effectsPanel.ScenarioButton.Refresh();
        }
        CardType type = card.Type;
        Campaign.Box.Push(card, true);
        Location.Current.Deck.Remove(card);
        card.Destroy();
        for (int i = 1; i < Constants.NUM_CARD_TYPES; i++)
        {
            if ((i != type) && (Scenario.Current.GetCardCount(Location.Current.ID, (CardType) i) > 0))
            {
                Scenario.Current.AddCardCount(Location.Current.ID, (CardType) i, -1);
                Scenario.Current.AddCardCount(Location.Current.ID, CardType.None, 1);
            }
        }
        this.RefreshCardList();
        this.cardsToRemoveIndex++;
    }

    private void Stop()
    {
        this.fakeCard.Destroy();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.Pause(false);
            Location.Load(Turn.Character.Location);
            window.Refresh();
            if (Scenario.Current.IsScenarioOver())
            {
                Turn.State = GameStateType.End;
            }
            else
            {
                window.ShowMap(false);
                Turn.State = GameStateType.StartTurn;
                this.BlackMaggaInstance.gameObject.SetActive(false);
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator StopBlackMagga() => 
        new <StopBlackMagga>c__Iterator11 { <>f__this = this };

    private Card fakeCard
    {
        get
        {
            if (this.fakeCardInstance == null)
            {
                this.fakeCardInstance = CardTable.Create("HE1B_Bandit");
                this.fakeCardInstance.Show(CardSideType.Back);
                this.fakeCardInstance.transform.localScale = Vector3.zero;
                this.fakeCardInstance.SortingOrder = 200;
            }
            return this.fakeCardInstance;
        }
    }

    public override bool Stateless =>
        false;

    [CompilerGenerated]
    private sealed class <ChompAnimation>c__Iterator10 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BlockSetAsideCards <>f__this;

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
                    this.<>f__this.fakeCard.transform.localScale = Vector3.zero;
                    this.<>f__this.fakeCard.transform.position = this.<>f__this.BlackMaggaInstance.transform.position;
                    if (!this.<>f__this.IsFacingRight())
                    {
                        this.<>f__this.fakeCard.transform.position -= new Vector3(this.<>f__this.MONSTER_OFFSET_LEFT.x, this.<>f__this.MONSTER_OFFSET_LEFT.y, 0f);
                        break;
                    }
                    this.<>f__this.fakeCard.transform.position -= new Vector3(this.<>f__this.MONSTER_OFFSET_RIGHT.x, this.<>f__this.MONSTER_OFFSET_RIGHT.y, 0f);
                    break;

                case 1:
                    LeanTween.scale(this.<>f__this.fakeCard.gameObject, Vector3.zero, 0.5f);
                    this.<>f__this.RemoveCard();
                    Game.Instance.StartCoroutine(this.<>f__this.StopBlackMagga());
                    this.$PC = -1;
                    goto Label_01F8;

                default:
                    goto Label_01F8;
            }
            LeanTween.scale(this.<>f__this.fakeCard.gameObject, new Vector3(0.15f, 0.15f), 0.2f);
            this.<>f__this.fakeCard.MoveCard(this.<>f__this.FakeCardPath(this.<>f__this.fakeCard.gameObject), 0.9f);
            this.$current = new WaitForSeconds(0.4f);
            this.$PC = 1;
            return true;
        Label_01F8:
            return false;
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

    [CompilerGenerated]
    private sealed class <PlayCoroutine>c__IteratorF : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BlockSetAsideCards <>f__this;
        internal float <time>__1;
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
                    if (this.<window>__0 == null)
                    {
                        this.<>f__this.Stop();
                        goto Label_0282;
                    }
                    Turn.State = GameStateType.Null;
                    this.<>f__this.BlackMaggaInstance.gameObject.SetActive(true);
                    this.<window>__0.ShowMap(true);
                    this.<window>__0.mapPanel.Center(Location.Current.ID);
                    this.<window>__0.mapPanel.Mode = MapModeType.None;
                    this.<window>__0.mapPanel.Pause(true);
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 1;
                    goto Label_028B;

                case 1:
                {
                    if (!(this.<>f__this.BlackMaggaInstance.transform.position != this.<>f__this.BlackMaggaDestination()))
                    {
                        break;
                    }
                    UI.Sound.Play(SoundEffectType.MaggaStartSwim);
                    this.<>f__this.BlackMaggaInstance.SetBool("Move", true);
                    this.<>f__this.BlackMaggaFace(this.<>f__this.LocationDestination());
                    Vector3 vector = this.<window>__0.mapPanel.GetMapIcon(Location.Current.ID).transform.position - this.<>f__this.BlackMaggaInstance.transform.position;
                    this.<time>__1 = vector.magnitude / 6f;
                    LeanTween.move(this.<>f__this.BlackMaggaInstance.gameObject, this.<>f__this.BlackMaggaDestination(), this.<time>__1);
                    this.$current = new WaitForSeconds(this.<time>__1);
                    this.$PC = 2;
                    goto Label_028B;
                }
                case 2:
                    break;

                case 3:
                    this.<>f__this.Stop();
                    goto Label_0282;

                default:
                    goto Label_0289;
            }
            this.<>f__this.BlackMaggaInstance.SetBool("Move", false);
            this.<>f__this.BlackMaggaInstance.SetInteger("EatNum", this.<>f__this.cardsToRemove.Length);
            this.<>f__this.cardsToRemoveIndex = 0;
            this.<>f__this.BlackMaggaInstance.SetTrigger("Eat");
            if (this.<>f__this.cardsToRemove.Length != 0)
            {
                UI.Sound.Play(SoundEffectType.MaggaFirstBite);
            }
            else
            {
                UI.Sound.Play(SoundEffectType.MaggaConfused);
                this.$current = new WaitForSeconds(5f);
                this.$PC = 3;
                goto Label_028B;
            }
        Label_0282:
            this.$PC = -1;
        Label_0289:
            return false;
        Label_028B:
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

    [CompilerGenerated]
    private sealed class <StopBlackMagga>c__Iterator11 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BlockSetAsideCards <>f__this;

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
                    if (((this.<>f__this.cardsToRemoveIndex >= 0) && (this.<>f__this.cardsToRemoveIndex < this.<>f__this.cardsToRemove.Length)) && ((this.<>f__this.cardsToRemove[this.<>f__this.cardsToRemoveIndex] < Location.Current.Deck.Count) && (this.<>f__this.cardsToRemove[this.<>f__this.cardsToRemoveIndex] >= 0)))
                    {
                        break;
                    }
                    this.$current = new WaitForSeconds(2.5f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Stop();
                    break;

                default:
                    goto Label_00C5;
            }
            this.$PC = -1;
        Label_00C5:
            return false;
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

