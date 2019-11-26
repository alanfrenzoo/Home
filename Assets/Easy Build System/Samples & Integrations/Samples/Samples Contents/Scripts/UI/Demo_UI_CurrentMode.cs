using UnityEngine;
using UnityEngine.UI;

public class Demo_UI_CurrentMode : MonoBehaviour
{
    #region Public Fields

    public string TextFormat = "Current Mode : {0}";

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

        Text.text = string.Format(TextFormat, DesktopBuilderBehaviour.Instance.CurrentMode.ToString());
    }

    #endregion
}