using UnityEngine;

/*
 * Highlightable toggles the authored selection shader flag on renderers
 * under this object. PlayerController owns when target focus changes; this class
 * only applies the local cosmetic state.
 */

public class Highlightable : MonoBehaviour
{
    static readonly int SelectionEnabledId = Shader.PropertyToID("_Selection_Enabled");

    Renderer[] _targetRenderers;

    void Awake()
    {
        // TODO Slice 3.1: cache every child renderer and begin unselected.
    }

    public void SetHighlighted(bool isHighlighted)
    {
        // TODO Slice 3.3: forward the requested state to ApplyHighlight. </> end of Slice 3
    }

    public void SetSelected(bool isSelected) => SetHighlighted(isSelected);

    void ApplyHighlight(bool isHighlighted)
    {
        // TODO Slice 3.2: set _Selection_Enabled on each material that supports it.
    }
}
