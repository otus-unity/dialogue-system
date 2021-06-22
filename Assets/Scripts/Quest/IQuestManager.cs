using System;
using System.Collections.Generic;

public interface IQuestManager
{
    // 1 классификация:
    //  - никогда не стартовал (NotStarted)
    //  - стартовал (Started) и возможно был завершен

    // 2 классификация:
    //  - не активен
    //  - активен сейчас (Active)
    //  - завершен (Completed)
    //  - провален (Failed)

    event Action<Quest> OnQuestStarted;
    event Action<Quest> OnQuestCompleted;
    event Action<Quest> OnQuestFailed;

    IList<Quest> ActiveQuests { get; }
    IList<Quest> CompletedQuests { get; }
    IList<Quest> FailedQuests { get; }

    void StartQuest(Quest quest);
    void CompleteQuest(Quest quest);
    void FailQuest(Quest quest);

    bool QuestStarted(Quest quest);
    bool QuestActive(Quest quest);
    bool QuestCompleted(Quest quest);
    bool QuestFailed(Quest quest);
}
