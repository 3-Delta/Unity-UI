using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class SelectableSetter {
    public static void Enable(this Selectable selectable, bool interactable, bool toGray) {
        selectable.interactable = interactable;
        if (!selectable.TryGetComponent<GraphicGrayer>(out var grayer)) {
            grayer = selectable.gameObject.AddComponent<GraphicGrayer>();
        }

        grayer.Gray(toGray);
    }
}
