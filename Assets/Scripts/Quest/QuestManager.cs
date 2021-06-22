using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class QuestManager : MonoBehaviour, IQuestManager
{
    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest> OnQuestFailed;

    public string QuestStartedMessage = "Quest Started";
    public string QuestCompletedMessage = "Quest Completed";
    public string QuestFailedMessage = "Quest Failed";

    readonly List<Quest> m_activeQuests = new List<Quest>();
    readonly List<Quest> m_completedQuests = new List<Quest>();
    readonly List<Quest> m_failedQuests = new List<Quest>();

    readonly HashSet<Quest> m_activeQuestsSet = new HashSet<Quest>();
    readonly HashSet<Quest> m_completedQuestsSet = new HashSet<Quest>();
    readonly HashSet<Quest> m_failedQuestsSet = new HashSet<Quest>();

    [Inject] INotificationManager m_notificationManager = default;

    public IList<Quest> ActiveQuests => m_activeQuests.AsReadOnly();
    public IList<Quest> CompletedQuests => m_completedQuests.AsReadOnly();
    public IList<Quest> FailedQuests => m_failedQuests.AsReadOnly();

    public void StartQuest(Quest quest)
    {
        if (QuestStarted(quest)) {
            Debug.LogError($"Attempting to start quest {quest.name} again.");
            return;
        }

        m_activeQuests.Add(quest);
        m_activeQuestsSet.Add(quest);

        OnQuestStarted?.Invoke(quest);

        m_notificationManager.DisplayMessage(string.Format(QuestStartedMessage, quest.Name));
    }

    public void CompleteQuest(Quest quest)
    {
        if (!m_activeQuestsSet.Contains(quest))
            Debug.LogError($"Attempted to complete non-started quest {quest.name}.");
        else {
            m_activeQuests.Remove(quest);
            m_activeQuestsSet.Remove(quest);
            AddCompletedQuest(quest);
        }
    }

    public void FailQuest(Quest quest)
    {
        if (!m_activeQuestsSet.Contains(quest))
            Debug.LogError($"Attempted to complete non-started quest {quest.name}.");
        else {
            m_activeQuests.Remove(quest);
            m_activeQuestsSet.Remove(quest);
            AddFailedQuest(quest);
        }
    }

    public bool QuestStarted(Quest quest)
    {
        return (m_activeQuestsSet.Contains(quest)
             || m_completedQuestsSet.Contains(quest)
             || m_failedQuestsSet.Contains(quest));
    }

    public bool QuestActive(Quest quest)
    {
        return m_activeQuestsSet.Contains(quest);
    }

    public bool QuestCompleted(Quest quest)
    {
        return m_completedQuestsSet.Contains(quest);
    }

    public bool QuestFailed(Quest quest)
    {
        return m_failedQuestsSet.Contains(quest);
    }

    void Update()
    {
        int n = m_activeQuests.Count;
        while (n-- > 0) {
            var quest = m_activeQuests[n];
            switch (quest.GetState(this)) {
                case Quest.State.Unknown:
                    break;

                case Quest.State.Success:
                    m_activeQuests.RemoveAt(n);
                    m_activeQuestsSet.Remove(quest);
                    AddCompletedQuest(quest);
                    break;

                case Quest.State.Failure:
                    m_activeQuests.RemoveAt(n);
                    m_activeQuestsSet.Remove(quest);
                    AddFailedQuest(quest);
                    break;
            }
        }
    }

    void AddCompletedQuest(Quest quest)
    {
        m_completedQuests.Add(quest);
        m_completedQuestsSet.Add(quest);

        OnQuestCompleted?.Invoke(quest);

        m_notificationManager.DisplayMessage(string.Format(QuestCompletedMessage, quest.Name));
    }

    void AddFailedQuest(Quest quest)
    {
        m_failedQuests.Add(quest);
        m_failedQuestsSet.Add(quest);

        OnQuestFailed?.Invoke(quest);

        m_notificationManager.DisplayMessage(string.Format(QuestFailedMessage, quest.Name));
    }
}
