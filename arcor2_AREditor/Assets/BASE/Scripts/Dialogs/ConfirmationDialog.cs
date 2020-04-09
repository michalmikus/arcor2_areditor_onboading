using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmationDialog : Dialog
{
    [SerializeField]
    private TMPro.TMP_Text OKButtonLabelNormal, OKButtonLabelHighlighted, CancelButtonLabelNormal, CancelButtonLabelHighlighted;

    public void SetConfirmLabel(string name) {
        OKButtonLabelNormal.text = name;
        OKButtonLabelHighlighted.text = name;
    }

    public void SetCancelLabel(string name) {
        CancelButtonLabelNormal.text = name;
        CancelButtonLabelHighlighted.text = name;
    }

    public void AddConfirmCallback(UnityAction callback) {
        WindowManager.onConfirm.AddListener(callback);
    }

    public void AddCancelCallback(UnityAction callback) {
        WindowManager.onCancel.AddListener(callback);
    }

    public void SetDescription(string description) {
        WindowManager.windowDescription.text = description;
    }

    public void SetTitle(string title) {
        WindowManager.windowTitle.text = title;
    }

    public void Open(string title, string description, UnityAction confirmationCallback, UnityAction cancelCallback, string confirmLabel = "Confirm", string cancelLabel = "Cancel") {
        SetTitle(title);
        SetDescription(description);
        AddConfirmCallback(confirmationCallback);
        AddCancelCallback(cancelCallback);
        SetConfirmLabel(confirmLabel);
        SetCancelLabel(cancelLabel);
        WindowManager.OpenWindow();
    }

    public void Close() {
        WindowManager.CloseWindow();
    }
}