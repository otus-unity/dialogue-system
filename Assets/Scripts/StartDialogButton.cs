using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogButton : MonoBehaviour
{
    public DialogUI DialogUI;
    public Sprite LeftSprite;
    public Sprite RightSprite;
    public List<Dialog> Dialogs;

    public void Click()
    {
        DialogUI.DisplayDialogs(LeftSprite, RightSprite, Dialogs);
    }
}
