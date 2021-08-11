using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public struct ProbeHit
	{
		public GameObject gameObject;

		public Vector3? point;
		
		public float? distance;

		public string label;

		public EditorTexture icon;

		public ProbeHitSelectHandler selectHandler;

		public Action focusHandler;

		public Action lostFocusHandler;

		public Transform transform
		{
			get => gameObject.transform;
			set => gameObject = value.gameObject;
		}

		public RectTransform rectTransform
		{
			get => gameObject.GetComponent<RectTransform>();
			set => gameObject = value.gameObject;
		}

		public GameObject groupGameObject;

		public double groupOrder;

		public ProbeHit(GameObject gameObject)
		{
			this.gameObject = gameObject;
			this.groupGameObject = gameObject;
			groupOrder = 0;
			point = default;
			distance = default;
			label = default;
			icon = default;
			selectHandler = default;
			focusHandler = default;
			lostFocusHandler = default;
		}
		
		public void Select(bool add)
		{
			if (selectHandler != null)
			{
				selectHandler(add);
			}
			else if (gameObject != null)
			{
				if (add)
				{
					Selection.objects = Selection.objects.Append(gameObject).ToArray();
				}
				else
				{
					Selection.activeGameObject = gameObject;
				}
			}
		}

		public void OnFocusEnter()
		{
			if (focusHandler != null)
			{
				focusHandler.Invoke();
			}
			else
			{
				Probe.Highlight(gameObject);
			}
		}

		public void OnFocusLeave()
		{
			if (lostFocusHandler != null)
			{
				lostFocusHandler.Invoke();
			}
			else
			{
				Probe.ClearHighlight();
			}
		}
	}
}