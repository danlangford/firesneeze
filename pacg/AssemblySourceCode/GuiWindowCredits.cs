using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

public class GuiWindowCredits : GuiWindow
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map2;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map3;
    [Tooltip("reference to the scroll panel in this scene")]
    public GuiPanelCredits Scroller;
    [Tooltip("maps xml tag name to prefab")]
    public StringToLabelPair[] StringToLabelPairs;
    [Tooltip("maps xml tag name to prefab")]
    public StringToSpritePair[] StringToSpritePairs;

    private GuiImage CreateNewSprite(Dictionary<string, string> attributes)
    {
        string str = attributes["name"];
        GuiImage original = null;
        for (int i = 0; i < this.StringToSpritePairs.Length; i++)
        {
            if (str == this.StringToSpritePairs[i].Tag)
            {
                original = this.StringToSpritePairs[i].Prefab;
                break;
            }
        }
        if (original != null)
        {
            GuiImage image2 = UnityEngine.Object.Instantiate<GuiImage>(original);
            if (image2 != null)
            {
                float x = image2.transform.localScale.x;
                if (attributes.ContainsKey("width"))
                {
                    x = float.Parse(attributes["width"]);
                }
                float y = image2.transform.localScale.y;
                if (attributes.ContainsKey("height"))
                {
                    y = float.Parse(attributes["height"]);
                }
                float num4 = 1f;
                if (attributes.ContainsKey("scale"))
                {
                    num4 = float.Parse(attributes["scale"]);
                }
                image2.transform.localScale = new Vector3(x * num4, y * num4, 1f);
                return image2;
            }
        }
        return null;
    }

    private GuiLabel CreateNewTextLabel(Dictionary<string, string> attributes)
    {
        string str = attributes["name"];
        GuiLabel original = null;
        for (int i = 0; i < this.StringToLabelPairs.Length; i++)
        {
            if (str == this.StringToLabelPairs[i].Tag)
            {
                original = this.StringToLabelPairs[i].Prefab;
                break;
            }
        }
        if (original != null)
        {
            GuiLabel label2 = UnityEngine.Object.Instantiate<GuiLabel>(original);
            if (label2 != null)
            {
                label2.Alignment = this.GetJustifyAttribute(attributes, TextAlignment.Center);
                return label2;
            }
        }
        return null;
    }

    private TextAlignment GetJustifyAttribute(Dictionary<string, string> attributes, TextAlignment defaultValue)
    {
        if (attributes.ContainsKey("justify"))
        {
            int num;
            string key = attributes["justify"];
            if (key == null)
            {
                return defaultValue;
            }
            if (<>f__switch$map3 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(3) {
                    { 
                        "center",
                        0
                    },
                    { 
                        "left",
                        1
                    },
                    { 
                        "right",
                        2
                    }
                };
                <>f__switch$map3 = dictionary;
            }
            if (<>f__switch$map3.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        return TextAlignment.Center;

                    case 1:
                        return TextAlignment.Left;

                    case 2:
                        return TextAlignment.Right;
                }
            }
        }
        return defaultValue;
    }

    private void LoadCreditsFile()
    {
        XmlDocument document = new XmlDocument();
        string xml = Resources.Load("Localized/En/Text/Credits/credits").ToString();
        document.LoadXml(xml);
        Dictionary<string, string> attributes = new Dictionary<string, string>();
        IEnumerator enumerator = document.DocumentElement.ChildNodes.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                GuiLabel label;
                XmlNode current = (XmlNode) enumerator.Current;
                attributes.Add("justify", "center");
                attributes.Add("br_height", "10");
                attributes.Add("name", "text_normal");
                if (current.Attributes != null)
                {
                    IEnumerator enumerator2 = current.Attributes.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            XmlAttribute attribute = (XmlAttribute) enumerator2.Current;
                            if (attributes.ContainsKey(attribute.Name))
                            {
                                attributes[attribute.Name] = attribute.Value;
                            }
                            else
                            {
                                attributes.Add(attribute.Name, attribute.Value);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
                float height = float.Parse(attributes["br_height"]);
                string name = current.Name;
                if (name != null)
                {
                    int num2;
                    if (<>f__switch$map2 == null)
                    {
                        Dictionary<string, int> dictionary2 = new Dictionary<string, int>(4) {
                            { 
                                "image",
                                0
                            },
                            { 
                                "br",
                                1
                            },
                            { 
                                "localized_text",
                                2
                            },
                            { 
                                "text",
                                2
                            }
                        };
                        <>f__switch$map2 = dictionary2;
                    }
                    if (<>f__switch$map2.TryGetValue(name, out num2))
                    {
                        switch (num2)
                        {
                            case 0:
                            {
                                GuiImage image = this.CreateNewSprite(attributes);
                                if (image != null)
                                {
                                    this.Scroller.Add(image, this.GetJustifyAttribute(attributes, TextAlignment.Center));
                                }
                                break;
                            }
                            case 1:
                                this.Scroller.Add(height);
                                break;

                            case 2:
                                goto Label_01E3;
                        }
                    }
                }
                goto Label_0262;
            Label_01E3:
                label = this.CreateNewTextLabel(attributes);
                if (label != null)
                {
                    IEnumerator enumerator3 = current.ChildNodes.GetEnumerator();
                    try
                    {
                        while (enumerator3.MoveNext())
                        {
                            XmlNode node2 = (XmlNode) enumerator3.Current;
                            label.Text = node2.InnerText;
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator3 as IDisposable;
                        if (disposable2 == null)
                        {
                        }
                        disposable2.Dispose();
                    }
                    this.Scroller.Add(label, height);
                }
            Label_0262:
                attributes.Clear();
            }
        }
        finally
        {
            IDisposable disposable3 = enumerator as IDisposable;
            if (disposable3 == null)
            {
            }
            disposable3.Dispose();
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            Game.UI.ShowMainMenu();
        }
    }

    protected override void Start()
    {
        base.Start();
        this.Scroller.Initialize();
        this.LoadCreditsFile();
        this.Scroller.Scroll();
    }

    private void Update()
    {
        if (Device.GetIsBackButtonPushed())
        {
            this.OnCloseButtonPushed();
        }
    }

    public override WindowType Type =>
        WindowType.Credits;

    [Serializable]
    public class StringToLabelPair
    {
        public GuiLabel Prefab;
        public string Tag;
    }

    [Serializable]
    public class StringToSpritePair
    {
        public GuiImage Prefab;
        public string Tag;
    }
}

