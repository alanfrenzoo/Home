using UnityEngine;
using UnityEngine.UI;

public class Demo_UI_SelectedPart : MonoBehaviour
{
    #region Public Fields

    public string TextFormat = "Current Part : {0}";

    #endregion

    #region Private Fields

    private Text Text;

    #endregion

    #region Private Methods

    private void Start ()
    {
        Text = GetComponent<Text>();
    }

    private void Update ()
    {
        if (DesktopBuilderBehaviour.Instance == null)
            return;

        if (DesktopBuilderBehaviour.Instance.SelectedPrefab == null)
            return;

        Text.text = string.Format(TextFormat, DesktopBuilderBehaviour.Instance.SelectedPrefab.Name);
    }

    #endregion
}