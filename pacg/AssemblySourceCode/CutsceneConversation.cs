using OEIFormats.FlowCharts;
using OEIFormats.FlowCharts.Conversations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneConversation : MonoBehaviour
{
    private int CurrentNodeID;
    private ConversationData Data;
    [Tooltip("name of the xml file containing the conversation (no path and no extension)")]
    public string File;
    private StringTable Strings;

    public bool Continue()
    {
        List<FlowChartLink> links = this.GetNode(this.CurrentNodeID).Links;
        bool flag = false;
        for (int i = 0; i < links.Count; i++)
        {
            FlowChartNode node = this.GetNode(links[i].ToNodeID);
            if (node != null)
            {
                Guid speakerGuid = this.GetNodeSpoken(node.NodeID).GetSpeakerGuid();
                for (int j = 0; j < this.Data.CharacterMappings.Count; j++)
                {
                    if (this.Data.CharacterMappings[j].Guid == speakerGuid)
                    {
                        flag = true;
                        CutsceneActor actor = this.GetActor(this.Data.CharacterMappings[j].InstanceTag);
                        if (!this.IsActorValid(actor) || !this.IsConditionValid(actor, node))
                        {
                            break;
                        }
                        this.RunNodeScripts(actor, node);
                        this.CurrentNodeID = node.NodeID;
                        return true;
                    }
                }
                if (!flag && this.IsConditionValid(null, node))
                {
                    this.RunNodeScripts(null, node);
                    this.CurrentNodeID = node.NodeID;
                    return true;
                }
            }
        }
        return false;
    }

    private bool EvaluateConditional(CutsceneActor actor, ConditionalCall call)
    {
        if (((call.Data == null) || (call.Data.FullName == null)) || (call.Data.Parameters == null))
        {
            return false;
        }
        bool flag = Conditionals.Invoke(actor, call);
        if (call.Not)
        {
            return !flag;
        }
        return flag;
    }

    private CutsceneActor GetActor(string tag)
    {
        GameObject obj2 = GameObject.Find("Actors/" + tag);
        if (obj2 != null)
        {
            return obj2.GetComponent<CutsceneActor>();
        }
        return null;
    }

    private FlowChartNode GetNode(int nodeID)
    {
        if (this.Data != null)
        {
            return this.Data.GetNodeByID(nodeID);
        }
        return null;
    }

    private FlowChartNode GetNodeSpoken(int nodeID)
    {
        FlowChartNode node = this.GetNode(nodeID);
        if (node.NodeType == FlowChartNodeType.Bank)
        {
            BankNode node2 = node as BankNode;
            for (int i = 0; i < node2.ChildNodeIDs.Count; i++)
            {
                FlowChartNode node3 = this.GetNode(node2.ChildNodeIDs[i]);
                Guid speakerGuid = node3.GetSpeakerGuid();
                for (int j = 0; j < this.Data.CharacterMappings.Count; j++)
                {
                    if (this.Data.CharacterMappings[j].Guid == speakerGuid)
                    {
                        CutsceneActor actor = this.GetActor(this.Data.CharacterMappings[j].InstanceTag);
                        if (!this.IsActorValid(actor))
                        {
                            break;
                        }
                        return node3;
                    }
                }
            }
        }
        return node;
    }

    public int GetNumberOfReplies()
    {
        FlowChartNode node = this.GetNode(this.CurrentNodeID);
        if (node != null)
        {
            return node.Links.Count;
        }
        return 0;
    }

    public string[] GetSpeakerList()
    {
        if (this.Data == null)
        {
            return null;
        }
        string[] strArray = new string[this.Data.CharacterMappings.Count];
        for (int i = 0; i < this.Data.CharacterMappings.Count; i++)
        {
            strArray[i] = this.Data.CharacterMappings[i].InstanceTag;
        }
        return strArray;
    }

    public string GetSpeakerTag()
    {
        if (this.Data != null)
        {
            FlowChartNode nodeSpoken = this.GetNodeSpoken(this.CurrentNodeID);
            if (nodeSpoken != null)
            {
                Guid speakerGuid = nodeSpoken.GetSpeakerGuid();
                for (int i = 0; i < this.Data.CharacterMappings.Count; i++)
                {
                    if (this.Data.CharacterMappings[i].Guid == speakerGuid)
                    {
                        return this.Data.CharacterMappings[i].InstanceTag;
                    }
                }
            }
        }
        return string.Empty;
    }

    public string GetSpeakerText()
    {
        if ((this.Data != null) && (this.Strings != null))
        {
            FlowChartNode nodeSpoken = this.GetNodeSpoken(this.CurrentNodeID);
            if (nodeSpoken != null)
            {
                return this.Strings.Get(nodeSpoken.NodeID);
            }
        }
        return string.Empty;
    }

    private bool IsActorValid(CutsceneActor actor)
    {
        if (actor != null)
        {
            if (!actor.PlayerCharacter)
            {
                return true;
            }
            if (actor.IsPartyMember())
            {
                return true;
            }
        }
        return false;
    }

    private bool IsConditionValid(CutsceneActor actor, FlowChartNode node)
    {
        if (node.Conditionals == null)
        {
            return true;
        }
        if (node.Conditionals.Components == null)
        {
            return true;
        }
        if (node.Conditionals.Components.Count <= 0)
        {
            return true;
        }
        bool flag = true;
        LogicalOperator and = LogicalOperator.And;
        for (int i = 0; i < node.Conditionals.Components.Count; i++)
        {
            ConditionalCall call = node.Conditionals.Components[i] as ConditionalCall;
            if (call != null)
            {
                switch (and)
                {
                    case LogicalOperator.And:
                        flag = flag && this.EvaluateConditional(actor, call);
                        break;

                    case LogicalOperator.Or:
                        flag = flag || this.EvaluateConditional(actor, call);
                        break;
                }
                and = call.Operator;
            }
        }
        return flag;
    }

    public void Load()
    {
        this.Data = this.LoadConversation("Conversations", this.File);
        this.Strings = StringTable.Load("Localized/En/Text/Conversations", this.File);
    }

    private ConversationData LoadConversation(string folder, string filename)
    {
        ConversationData data = null;
        TextAsset asset = Resources.Load(folder + "/" + filename, typeof(TextAsset)) as TextAsset;
        if (asset != null)
        {
            data = ConversationData.LoadFromTextAsset(asset.text);
        }
        return data;
    }

    private void RunNodeScripts(CutsceneActor actor, FlowChartNode node)
    {
        for (int i = 0; i < node.OnEnterScripts.Count; i++)
        {
            Scripts.Invoke(actor, node.OnEnterScripts[i]);
        }
        for (int j = 0; j < node.OnExitScripts.Count; j++)
        {
            Scripts.Invoke(actor, node.OnExitScripts[j]);
        }
        for (int k = 0; k < node.OnUpdateScripts.Count; k++)
        {
            Scripts.Invoke(actor, node.OnUpdateScripts[k]);
        }
    }

    public void Unload()
    {
        if (this.Strings != null)
        {
            this.Strings.Clear();
        }
        this.Strings = null;
        this.Data = null;
    }
}

