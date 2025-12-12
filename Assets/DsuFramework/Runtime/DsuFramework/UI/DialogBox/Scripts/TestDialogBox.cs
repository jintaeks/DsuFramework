using UnityEngine;
using Dsu.Framework;

public class TestDialogBox : MonoBehaviour
{
    void Start()
    {
        // First Dialog -----------------------------
        DialogBox.Instance
        .SetTitle("Message 1")
        .SetMessage("Hello!")
        .SetButtonColor(DialogButtonColor.Blue)
        .OnClose((DialogButtonType t) => Debug.Log($"Closed 1 button:{t}"))
        .Show();

        // Second Dialog ----------------------------
        DialogBox.Instance
        .SetTitle("Message 2")
        .SetMessage("Hello Again :)")
        .SetButtonColor(DialogButtonColor.Magenta)
        .SetButton1Text("Ok")
        .OnClose((DialogButtonType t) => Debug.Log($"Closed 2 button:{t}"))
        .Show();

        // Third Dialog -----------------------------
        DialogBox.Instance
        .SetTitle("Message 3")
        .SetMessage("No")
        .SetFadeInDuration(1f)
        .SetButtonColor(DialogButtonColor.Red)
        .SetButton1Text("No")
        .SetButton2Text("Yes")
        .OnClose((DialogButtonType t) => Debug.Log($"Closed 3 button:{t}"))
        .Show();
    }
}
