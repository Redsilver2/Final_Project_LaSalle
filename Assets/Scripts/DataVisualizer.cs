using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class DataVisualizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dataTextDisplayer;
    [SerializeField] private TextMeshProUGUI indexTextDisplayer;
    private LineRenderer lineRenderer;

    public void SetLineRenderer()
    {
        // Sets And Gets LineRenderer In Object If Null
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled       = false;
        }
    }

    public void UpdateDisplayer(string data)
    {
        // Checks If Data Text Displayer Is Null And Sets Text
        if (dataTextDisplayer != null)
            dataTextDisplayer.text = data;

        // Checks If Data Index Displayer Is Null And Sets Text
        if (indexTextDisplayer != null)
            indexTextDisplayer.text = string.Empty;

        // Sets Data Index Displayer Enabled State To False
        SetDisplayerActiveState(indexTextDisplayer, false);
    }


    public void UpdateDisplayers(uint index, string data)
    {
        // Checks If Data Text Displayer Is Null And Sets Text
        if (dataTextDisplayer != null)
            dataTextDisplayer.text = data;

        // Checks If Data Index Displayer Is Null And Sets Text
        if (indexTextDisplayer != null)
            indexTextDisplayer.text = index.ToString();

        // Sets Data Index Displayer Enabled State To True
        SetDisplayerActiveState(indexTextDisplayer, true);
    }

    public void UpdateLineRendererPositions(DataVisualizer data)
    {
        // Checks If Data Is Null And Sets The Enabled State To False
        if (data == null)
        {
            SetLineRendererEnabledState(false);
            return;
        }


        // Sets The LineRenderer's Enabled State To True
        SetLineRendererEnabledState(true);

        // Checks If LineRenderer Is Not Null
        if (lineRenderer != null)
        {
            // Sets The LineRenderer's Positions
            lineRenderer.SetPositions(new Vector3[]
            {
                    transform.position,
                    data.transform.position + Vector3.forward * 1
            });
        }
    }

    public void ResetLineRenderer()
    {
        // Checks If LineRenderer Is Not Null And Resets The LineRenderer's Values
        if (lineRenderer != null)
        {
            lineRenderer.GetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            lineRenderer.enabled = false;
        }
    }

    public void SetLineRendererEnabledState(bool isEnabled)
    {
        // Checks If Linerenderer Is Not Null And Sets Enable State
        if (lineRenderer != null)
            lineRenderer.enabled = isEnabled;
    }

    private void SetDisplayerActiveState(TextMeshProUGUI displayer, bool isActif)
    {
        // Checks If Displayer Is Not Null And Sets Active State
        if (displayer != null)
            displayer.gameObject.SetActive(isActif);
    }
}
