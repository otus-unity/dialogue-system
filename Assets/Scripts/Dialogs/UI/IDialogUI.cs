using System.Collections.Generic;
using UnityEngine;

public interface IDialogUI
{
    void DisplayDialogs(Sprite playerPortrait1, Sprite opponentPortrait, List<Dialog> dialogs);
}
