using System;
using UnityEngine;

public class CharacterTokenText : MonoBehaviour
{
    [Tooltip("reference to the level label in our hierarchy")]
    public GuiLabel Level;
    [Tooltip("reference to the name label in our hierarchy")]
    public GuiLabel Name;

    public static CharacterTokenText Create(CharacterToken token)
    {
        if (token != null)
        {
            GameObject prefab = Resources.Load<GameObject>("Art/Templates/CharProg_Data");
            if (prefab != null)
            {
                GameObject obj3 = Geometry.CreateChildObject(token.gameObject, prefab, "Text");
                if (obj3 != null)
                {
                    CharacterTokenText component = obj3.GetComponent<CharacterTokenText>();
                    if (component != null)
                    {
                        if (token.Character != null)
                        {
                            if (!string.IsNullOrEmpty(token.Character.NickName))
                            {
                                component.Name.Text = token.Character.NickName;
                            }
                            else
                            {
                                component.Name.Text = token.Character.DisplayName;
                            }
                            if (token.Character.Level > 0)
                            {
                                component.Level.Text = token.Character.Level.ToString();
                                return component;
                            }
                            component.Level.Clear();
                        }
                        return component;
                    }
                }
            }
        }
        return null;
    }

    public void Refresh(CharacterToken token)
    {
        this.Name.Text = token.Character.NickName;
        this.Level.Text = token.Character.Level.ToString();
    }
}

