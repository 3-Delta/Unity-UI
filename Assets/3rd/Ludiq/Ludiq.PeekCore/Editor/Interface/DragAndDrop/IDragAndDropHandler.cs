using UnityEditor;

namespace Ludiq.PeekCore
{
	public interface IDragAndDropHandler
	{
		DragAndDropVisualMode dragAndDropVisualMode { get; }
		bool AcceptsDragAndDrop();
		void PerformDragAndDrop();
		void UpdateDragAndDrop();
		void DrawDragAndDropPreview();
		void ExitDragAndDrop();
	}
}
