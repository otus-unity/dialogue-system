using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogNode
{
    public enum Action
    {
        Default,
        StartQuest,
    }

    public enum Condition
    {
        None,
        QuestCompleted,
    }

    public int UniqueId;
    public Rect Bounds;
    public string Text;
    public bool IsPlayer;
    public bool AllowReuse;

    public Condition NodeCondition;
    public Quest ConditionQuest;

    public Action NodeAction;
    public Quest ActionQuest;

    [NonSerialized] public List<DialogNode> Next;
    public List<int> NextIds;

    bool hasBeenUsed;

    public bool CanShow(IQuestManager questManager)
    {
        if (hasBeenUsed && !AllowReuse)
            return false;

        if (NodeAction == Action.StartQuest && questManager.QuestStarted(ActionQuest))
            return false;

        if (NodeCondition == Condition.QuestCompleted && !questManager.QuestCompleted(ConditionQuest))
            return false;

        return true;
    }

    public void SetHasBeenUsed()
    {
        hasBeenUsed = true;
    }

    public bool CanConnectTo(DialogNode otherNode)
    {
        if (otherNode == this)
            return false;

        foreach (var node in Next) {
            if (node == otherNode)
                return false;
        }

        return true;
    }
}
