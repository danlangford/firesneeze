using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tutorial
{
    private static BlackBoard blackBoard = new BlackBoard("_TUT_BB");
    private static readonly string ID = "_TUTORIAL";

    public static void Clear()
    {
        if (Script != null)
        {
            Script.Clear();
        }
    }

    public static void Hide()
    {
        if (Game.UI.TutorialPopupOverlay != null)
        {
            Game.UI.TutorialPopupOverlay.Show(false);
        }
        if (Game.UI.TutorialPopup != null)
        {
            Game.UI.TutorialPopup.Show(false);
        }
    }

    public static void Initialize(string script)
    {
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Tutorial/" + script);
        if (prefab != null)
        {
            GameObject target = Game.Instance.Create(prefab);
            if (target != null)
            {
                target.name = script;
                target.transform.parent = null;
                UnityEngine.Object.DontDestroyOnLoad(target);
                Script = target.GetComponent<TutorialScript>();
            }
        }
    }

    public static bool IsMessageDisplayed(int id)
    {
        if (Script != null)
        {
            for (int i = 0; i < Script.Messages.Length; i++)
            {
                if ((Script.Messages[i].id == id) && Script.Messages[i].Displayed)
                {
                    return true;
                }
            }
            for (int j = 0; j < Script.Tips.Length; j++)
            {
                if ((Script.Tips[j].id == id) && Script.Tips[j].Displayed)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void Message(int id, float x, float y)
    {
        Game.UI.TutorialPopup.Show(id);
        Vector3 position = new Vector3(x, y, 0f);
        Vector3 vector2 = UI.Camera.ViewportToWorldPoint(position);
        Game.UI.TutorialPopup.transform.position = new Vector3(vector2.x, vector2.y, Game.UI.TutorialPopup.transform.position.z);
    }

    public static void Message(string msg, float x, float y)
    {
        Game.UI.TutorialPopup.Show(msg);
        Vector3 position = new Vector3(x, y, 0f);
        Vector3 vector2 = UI.Camera.ViewportToWorldPoint(position);
        Game.UI.TutorialPopup.transform.position = new Vector3(vector2.x, vector2.y, Game.UI.TutorialPopup.transform.position.z);
    }

    public static void Notify(TutorialEventType trigger)
    {
        if (Script != null)
        {
            Script.Notify(trigger);
        }
    }

    public static void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                if (stream.ReadBool() && (Script != null))
                {
                    Script.Step = stream.ReadInt();
                    int num = stream.ReadInt();
                    for (int i = 0; i < num; i++)
                    {
                        Script.Messages[i].Displayed = stream.ReadBool();
                    }
                    num = stream.ReadInt();
                    for (int j = 0; j < num; j++)
                    {
                        Script.Tips[j].Displayed = stream.ReadBool();
                    }
                }
            }
        }
        blackBoard.OnLoadData();
        Notify(TutorialEventType.TutorialMessageClosed);
    }

    public static void OnSaveData()
    {
        ByteStream stream = new ByteStream();
        if (stream != null)
        {
            stream.WriteInt(1);
            stream.WriteBool(Script != null);
            if (Script != null)
            {
                stream.WriteInt(Script.Step);
                stream.WriteInt(Script.Messages.Length);
                for (int i = 0; i < Script.Messages.Length; i++)
                {
                    stream.WriteBool(Script.Messages[i].Displayed);
                }
                stream.WriteInt(Script.Tips.Length);
                for (int j = 0; j < Script.Tips.Length; j++)
                {
                    stream.WriteBool(Script.Tips[j].Displayed);
                }
            }
            Game.SetObjectData(ID, stream.ToArray());
            blackBoard.OnSaveData();
        }
    }

    public static void Run(int slot)
    {
        if (Script != null)
        {
            Script.Run(slot);
        }
    }

    [DebuggerHidden]
    public static IEnumerator WaitForOverlay(string art) => 
        new <WaitForOverlay>c__IteratorA8 { 
            art = art,
            <$>art = art
        };

    [DebuggerHidden]
    public static IEnumerator WaitForOverlay(int id, float x, float y) => 
        new <WaitForOverlay>c__IteratorA7 { 
            id = id,
            x = x,
            y = y,
            <$>id = id,
            <$>x = x,
            <$>y = y
        };

    public static BlackBoard BlackBoard =>
        blackBoard;

    public static bool Running
    {
        get
        {
            if (Scenario.Current == null)
            {
                return false;
            }
            if (UI.Window.Type == WindowType.Adventure)
            {
                return false;
            }
            if (UI.Window.Type == WindowType.CreateParty)
            {
                return false;
            }
            return Scenario.Current.ID.StartsWith("SC1T");
        }
    }

    public static TutorialScript Script
    {
        [CompilerGenerated]
        get => 
            <Script>k__BackingField;
        [CompilerGenerated]
        private set
        {
            <Script>k__BackingField = value;
        }
    }

    public static bool Visible
    {
        get
        {
            if (!Running)
            {
                return false;
            }
            return (((Game.UI.TutorialPopup != null) && Game.UI.TutorialPopup.Visible) || ((Game.UI.TutorialPopupOverlay != null) && Game.UI.TutorialPopupOverlay.Visible));
        }
    }

    [CompilerGenerated]
    private sealed class <WaitForOverlay>c__IteratorA7 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>id;
        internal float <$>x;
        internal float <$>y;
        internal GameObject <go>__2;
        internal string <path>__0;
        internal GameObject <prefab>__1;
        internal Vector3 <vp>__3;
        internal Vector3 <wp>__4;
        internal int id;
        internal float x;
        internal float y;

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
                    Tutorial.Hide();
                    this.<path>__0 = "Blueprints/Gui/Tutorial_Popup_Message";
                    this.<prefab>__1 = Resources.Load<GameObject>(this.<path>__0);
                    if (this.<prefab>__1 != null)
                    {
                        this.<go>__2 = Game.Instance.Create(this.<prefab>__1);
                        if (this.<go>__2 != null)
                        {
                            if (Game.UI.TutorialPopupOverlay != null)
                            {
                                UnityEngine.Object.Destroy(Game.UI.TutorialPopupOverlay.gameObject);
                            }
                            Game.UI.TutorialPopupOverlay = this.<go>__2.GetComponent<GuiPanelTutorial>();
                        }
                    }
                    if (Game.UI.TutorialPopupOverlay == null)
                    {
                        goto Label_01B0;
                    }
                    Game.UI.TutorialPopupOverlay.Popup.Display(this.id);
                    this.<vp>__3 = new Vector3(this.x, this.y, 0f);
                    this.<wp>__4 = UI.Camera.ViewportToWorldPoint(this.<vp>__3);
                    Game.UI.TutorialPopupOverlay.Popup.transform.position = new Vector3(this.<wp>__4.x, this.<wp>__4.y, Game.UI.TutorialPopupOverlay.Popup.transform.position.z);
                    break;

                case 1:
                    break;
                    this.$PC = -1;
                    goto Label_01B0;

                default:
                    goto Label_01B0;
            }
            if (Game.UI.TutorialPopupOverlay.Popup.Visible)
            {
                this.$current = null;
                this.$PC = 1;
                return true;
            }
        Label_01B0:
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
    private sealed class <WaitForOverlay>c__IteratorA8 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>art;
        internal GameObject <go>__2;
        internal string <path>__0;
        internal GameObject <prefab>__1;
        internal string art;

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
                    Tutorial.Hide();
                    this.<path>__0 = "Blueprints/Gui/" + this.art;
                    this.<prefab>__1 = Resources.Load<GameObject>(this.<path>__0);
                    if (this.<prefab>__1 != null)
                    {
                        this.<go>__2 = Game.Instance.Create(this.<prefab>__1);
                        if (this.<go>__2 != null)
                        {
                            if (Game.UI.TutorialPopupOverlay != null)
                            {
                                UnityEngine.Object.Destroy(Game.UI.TutorialPopupOverlay.gameObject);
                            }
                            Game.UI.TutorialPopupOverlay = this.<go>__2.GetComponent<GuiPanelTutorial>();
                        }
                    }
                    if (Game.UI.TutorialPopupOverlay == null)
                    {
                        goto Label_011A;
                    }
                    break;

                case 1:
                    break;
                    this.$PC = -1;
                    goto Label_011A;

                default:
                    goto Label_011A;
            }
            if (Game.UI.TutorialPopupOverlay.Popup.Visible)
            {
                this.$current = null;
                this.$PC = 1;
                return true;
            }
        Label_011A:
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

