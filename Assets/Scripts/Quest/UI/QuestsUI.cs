using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class QuestsUI : MonoBehaviour
{
    public enum Group
    {
        Active,
        Completed,
        Failed,
    }

    [Inject] IQuestManager m_questManager = default;
    [SerializeField] Transform m_content;

    Group m_selectedGroup;
    readonly List<QuestRow> m_rows = new List<QuestRow>();
    [Inject] QuestRow.Factory m_rowFactory = default;

    void OnEnable()
    {
        m_questManager.OnQuestStarted += OnQuestChanged;
        m_questManager.OnQuestCompleted += OnQuestChanged;
        m_questManager.OnQuestFailed += OnQuestChanged;
        Refresh();
    }

    void OnDisable()
    {
        m_questManager.OnQuestStarted -= OnQuestChanged;
        m_questManager.OnQuestCompleted -= OnQuestChanged;
        m_questManager.OnQuestFailed -= OnQuestChanged;
        DestroyRows();
    }

    void OnQuestChanged(Quest quest)
    {
        Refresh();
    }

    void Refresh()
    {
        DestroyRows();
        FillItems();
    }

    public void SelectActive()
    {
        m_selectedGroup = Group.Active;
        FillItems();
    }

    public void SelectComplete()
    {
        m_selectedGroup = Group.Completed;
        FillItems();
    }

    public void SelectFailed()
    {
        m_selectedGroup = Group.Failed;
        FillItems();
    }

    void FillItems()
    {
        DestroyRows();

        IList<Quest> list = null;
        switch (m_selectedGroup) {
            case Group.Active: list = m_questManager.ActiveQuests; break;
            case Group.Completed: list = m_questManager.CompletedQuests; break;
            case Group.Failed: list = m_questManager.FailedQuests; break;
        }

        foreach (var quest in list) {
            var row = m_rowFactory.Create(quest);
            row.transform.SetParent(m_content, false);
            m_rows.Add(row);
        }
    }

    void DestroyRows()
    {
        foreach (var row in m_rows)
            row.Dispose();
        m_rows.Clear();
    }
}
