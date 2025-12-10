using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class DialogueDefinition : ScriptableObject
{
    public string dialogueId;
    public string npcName;
    public Sprite npcPortrait;

    public List<DialogueNode> nodes = new List<DialogueNode>();

    public DialogueNode GetNode(string nodeId)
    {
        return nodes.Find(n => n.id == nodeId);
    }
}

[Serializable]
public class DialogueNode
{
    public string id;
    [TextArea] public string text;
    public List<DialogueChoice> choices = new List<DialogueChoice>();
}

[Serializable]
public class DialogueChoice
{
    public string text;
    public string nextNodeId;
    public bool closesDialogue;
}
