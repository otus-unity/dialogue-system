using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

public class DialogUI : MonoBehaviour
{
    [SerializeField] Transform m_messagesContainer;
    [SerializeField] Image m_background;
    [SerializeField] Image m_leftPortrait;
    [SerializeField] Image m_rightPortrait;
    [SerializeField] string m_endDialogMessage;

    float m_leftPortraitX;
    float m_rightPortraitX;
    float m_backgroundOpacity;

    [Inject] DialogRow.Factory m_dialogRowFactory = default;
    [Inject] IQuestManager m_questManager = default;

    readonly List<DialogRow> m_rows = new List<DialogRow>();

    void Start()
    {
        m_leftPortraitX = m_leftPortrait.transform.localPosition.x;
        m_rightPortraitX = m_rightPortrait.transform.localPosition.x;
        m_backgroundOpacity = m_background.color.a;
    }

    public void DisplayDialogs(Sprite playerPortrait, Sprite opponentPortrait, List<Dialog> dialogs)
    {
        m_leftPortrait.sprite = playerPortrait;
        m_rightPortrait.sprite = opponentPortrait;
        StartCoroutine(DialogCoroutine(dialogs));
    }

    IEnumerator DialogCoroutine(List<Dialog> dialogs)
    {
        Clear();

        var dialogTask = ShowDialogs(dialogs);
        while (!dialogTask.IsCompleted)
            yield return null;

        dialogTask.Wait(); // убеждаемся, что возможные исключения обработаны
    }

    async Task ShowDialogs(List<Dialog> dialogs)
    {
        const float BackgroundAppearTime = 0.2f;
        const float BackgroundDisappearTime = 0.2f;
        const float PortraitsAppearTime = 1.0f;
        const float PortraitsDisappearTime = 1.0f;

        const float PortraitOffset = 300.0f;

        m_background.DOFade(m_backgroundOpacity, BackgroundAppearTime).From(0.0f);
        m_leftPortrait.transform.DOLocalMoveX(m_leftPortraitX, PortraitsAppearTime).From(m_leftPortraitX - PortraitOffset);
        await m_rightPortrait.transform.DOLocalMoveX(m_rightPortraitX, PortraitsAppearTime).From(m_rightPortraitX + PortraitOffset).AsyncWaitForCompletion();

        List<DialogNode> rootNodes = new List<DialogNode>();
        do {
            rootNodes.Clear();
            foreach (var dialog in dialogs) {
                foreach (var node in dialog.RootNodes) {
                    if (node.CanShow(m_questManager))
                        rootNodes.Add(node);
                }
            }
        } while (await ShowOptions(rootNodes, true));

        Clear();

        m_background.DOFade(0.0f, BackgroundDisappearTime);
        m_leftPortrait.transform.DOLocalMoveX(m_leftPortraitX - PortraitOffset, PortraitsDisappearTime);
        await m_rightPortrait.transform.DOLocalMoveX(m_rightPortraitX + PortraitOffset, PortraitsDisappearTime).AsyncWaitForCompletion();
    }

    async Task<bool> ShowOptions(List<DialogNode> dialogs, bool allowEndDialog)
    {
        if (dialogs.Count == 0)
            return false;

        do {
            DialogNode selectedNode = null;
            if (dialogs.Count == 1 && !allowEndDialog) {
                var row = CreateRow(dialogs[0].IsPlayer);
                row.Text.text = dialogs[0].Text;
                row.Text.gameObject.SetActive(true);
                //row.BalloonImage.GetComponent<Image>().color = Color.white;
                selectedNode = dialogs[0];
            } else {
                var row = CreateRow(true);
                row.Text.gameObject.SetActive(false);

                foreach (var dialog in dialogs) {
                    if (dialog.CanShow(m_questManager))
                        row.AddButton(dialog.Text, dialog.Text, dialog);
                }
                if (allowEndDialog)
                    row.AddButton(m_endDialogMessage, m_endDialogMessage, null);

                do {
                    await Task.Yield();
                } while (!row.TryGetSelected(out selectedNode));

                row.Text.text = selectedNode != null ? selectedNode.Text : m_endDialogMessage;
                row.Text.gameObject.SetActive(true);
                row.DestroyButtons();

                if (selectedNode == null)
                    return false;
            }

            selectedNode.SetHasBeenUsed();

            switch (selectedNode.NodeAction) {
                case DialogNode.Action.Default:
                    break;

                case DialogNode.Action.StartQuest:
                    m_questManager.StartQuest(selectedNode.ActionQuest);
                    break;
            }

            allowEndDialog = false;
            dialogs = selectedNode.Next;
        } while (dialogs != null && dialogs.Count > 0);

        return true;
    }

    DialogRow CreateRow(bool isPlayer)
    {
        var row = m_dialogRowFactory.Create();
        row.transform.SetParent(m_messagesContainer, false);

        var scale = row.BalloonImage.localScale;
        scale.x = (isPlayer ? 1.0f : -1.0f);
        row.BalloonImage.localScale = scale;

        //row.BalloonImage.GetComponent<Image>().color = Color.white; // FIXME

        m_rows.Add(row);

        // FIXME: этот хак обесечивает корректное обновление UI при первом отображении диалога
        var vlg = m_messagesContainer.GetComponent<VerticalLayoutGroup>();
        vlg.enabled = false;
        DOVirtual.DelayedCall(0.001f, () => { vlg.enabled = true; }, ignoreTimeScale: true);

        return row;
    }

    void Clear()
    {
        foreach (var row in m_rows)
            row.Dispose();
        m_rows.Clear();
    }
}
