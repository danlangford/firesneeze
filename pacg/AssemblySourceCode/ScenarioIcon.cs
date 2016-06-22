using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ScenarioIcon : MonoBehaviour
{
    [Tooltip("sound played when this icon is touched")]
    public AudioClip ClickSound;
    [Tooltip("the world-space height of this icon")]
    public float Height = 2f;
    [Tooltip("reference to the image in our hierarchy")]
    public GuiImage Image;
    [Tooltip("reference to the title in our hierarchy")]
    public GuiLabel TitleLabel;

    private string ConvertColorToString(Color c) => 
        $"#{((int) (c.r * 255f)):X2}{((int) (c.g * 255f)):X2}{((int) (c.b * 255f)):X2}{((int) (c.a * 255f)):X2}";

    public static ScenarioIcon Create(ScenarioTableEntry entry)
    {
        string path = "Blueprints/Gui/List_Scenario";
        GameObject original = Resources.Load<GameObject>(path);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                ScenarioIcon component = obj3.GetComponent<ScenarioIcon>();
                if (component != null)
                {
                    component.TitleLabel.Text = entry.Name;
                }
                return component;
            }
        }
        return null;
    }

    public string GetDescription(Color headingColor)
    {
        ScenarioTableEntry entry = ScenarioTable.Get(this.ID);
        if (entry != null)
        {
            string str = "<b><color=" + this.ConvertColorToString(headingColor) + ">";
            string str2 = ": </color></b> ";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(entry.Description);
            builder.AppendLine();
            builder.Append(str + UI.Text(0x15b).ToUpper() + str2);
            builder.AppendLine(entry.Powers);
            builder.AppendLine();
            builder.Append(str + UI.Text(0x169).ToUpper() + str2);
            builder.AppendLine(entry.Henchmen);
            builder.AppendLine();
            builder.Append(str + UI.Text(0x19d).ToUpper() + str2);
            builder.AppendLine(entry.Villain);
            builder.AppendLine();
            builder.Append(str + UI.Text(0x180).ToUpper() + str2);
            builder.AppendLine(entry.Reward);
            return builder.ToString();
        }
        return null;
    }

    public GameObject LoadArt(Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>("Art/Scenarios/" + this.ID);
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                obj3.transform.parent = parent.transform;
                obj3.transform.localScale = Vector3.one;
                obj3.transform.localPosition = Vector3.zero;
                return obj3;
            }
        }
        return null;
    }

    public void LoadImage(string id)
    {
        string path = "Blueprints/Icons/Scenarios/" + id;
        SpriteRenderer renderer = Game.Cache.Get<SpriteRenderer>(path);
        if (renderer != null)
        {
            this.Image.Image = renderer.sprite;
        }
        else
        {
            this.Image.Image = null;
        }
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public void Tap()
    {
        UI.Sound.Play(this.ClickSound);
    }

    public string ID { get; set; }
}

