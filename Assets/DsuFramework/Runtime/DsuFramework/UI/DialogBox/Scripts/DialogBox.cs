using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Dsu.Framework
{
    public enum DialogButtonColor
    {
        Black,
        Purple,
        Magenta,
        Blue,
        Green,
        Yellow,
        Orange,
        Red
    }

    public enum DialogButtonType
    {
        Button1 = 1,
        Button2 = 2
    }

    public class Dialog
    {
        public string Title = "Title";
        public string Message = "Message goes here.";
        public string Button1Text = "Close";
        public string Button2Text = "Okay";
        public float FadeInDuration = 0.3f;
        public DialogButtonColor ButtonColor = DialogButtonColor.Black;
        public UnityAction<DialogButtonType> OnClose = null;
        public bool enableButton2 = false;
    }

    public class DialogBox : MonoBehaviour
    {
        [SerializeField] GameObject canvas;
        [SerializeField] Text titleUIText;
        [SerializeField] Text messageUIText;
        [SerializeField] Button uiButton1;
        [SerializeField] Button uiButton2;

        Image uiButton1Image;
        Text uiButton1Text;
        Image uiButton2Image;
        Text uiButton2Text;
        CanvasGroup canvasGroup;

        [Space(20f)]
        [Header("Close button colors")]
        [SerializeField] Color[] buttonColors;

        Queue<Dialog> dialogsQueue = new Queue<Dialog>();
        Dialog dialog = new Dialog();
        Dialog tempDialog;

        [HideInInspector] public bool IsActive = false;

        //Singleton pattern
        public static DialogBox Instance;

        void Awake()
        {
            Instance = this;

            uiButton1Image = uiButton1.GetComponent<Image>();
            uiButton1Text = uiButton1.GetComponentInChildren<Text>();
            uiButton2Image = uiButton2.GetComponent<Image>();
            uiButton2Text = uiButton2.GetComponentInChildren<Text>();
            canvasGroup = canvas.GetComponent<CanvasGroup>();

            //Add close event listener
            uiButton1.onClick.RemoveAllListeners();
            uiButton1.onClick.AddListener(() => OnButtonClicked(DialogButtonType.Button1));
            uiButton2.onClick.RemoveAllListeners();
            uiButton2.onClick.AddListener(() => OnButtonClicked(DialogButtonType.Button2));
        }

        public DialogBox SetTitle(string title)
        {
            dialog.Title = title;
            return Instance;
        }

        public DialogBox SetMessage(string message)
        {
            dialog.Message = message;
            return Instance;
        }

        public DialogBox SetButton1Text(string text)
        {
            dialog.Button1Text = text;
            return Instance;
        }

        public DialogBox SetButton2Text(string text)
        {
            dialog.enableButton2 = true;
            dialog.Button2Text = text;
            return Instance;
        }

        public DialogBox SetButtonColor(DialogButtonColor color)
        {
            dialog.ButtonColor = color;
            return Instance;
        }

        public DialogBox SetFadeInDuration(float duration)
        {
            dialog.FadeInDuration = duration;
            return Instance;
        }

		
		public DialogBox OnClose ( UnityAction<DialogButtonType> action ) {
			dialog.OnClose = action;
			return Instance;
		}

        //-------------------------------------
        public void Show()
        {
            dialogsQueue.Enqueue(dialog);
            //Reset Dialog
            dialog = new Dialog();

            if (!IsActive)
                ShowNextDialog();
        }

        void ShowNextDialog()
        {
            tempDialog = dialogsQueue.Dequeue();

            titleUIText.text = tempDialog.Title;
            messageUIText.text = tempDialog.Message;
            uiButton1Text.text = tempDialog.Button1Text.ToUpper();
            uiButton1Image.color = buttonColors[(int)tempDialog.ButtonColor];
            uiButton2Text.text = tempDialog.Button2Text.ToUpper();
            //uiButton2Image.color = buttonColors[(int)tempDialog.ButtonColor];

            uiButton2.gameObject.SetActive(tempDialog.enableButton2);

            canvas.SetActive(true);
            IsActive = true;
            StartCoroutine(FadeIn(tempDialog.FadeInDuration));
        }

        // Hide dialog
        public void Hide()
        {
            canvas.SetActive(false);
            IsActive = false;

            StopAllCoroutines();

            if (dialogsQueue.Count != 0)
                ShowNextDialog();
        }

        void OnButtonClicked(DialogButtonType buttonType)
        {
            if (tempDialog.OnClose != null)
                tempDialog.OnClose.Invoke(buttonType);

            Hide();
        }

        //-------------------------------------
        IEnumerator FadeIn(float duration)
        {
            float startTime = Time.time;
            float alpha = 0f;

            while (alpha < 1f) {
                alpha = Mathf.Lerp(0f, 1f, (Time.time - startTime) / duration);
                canvasGroup.alpha = alpha;

                yield return null;
            }
        }
    }
}
