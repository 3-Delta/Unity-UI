using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using OdinSerializationData = Ludiq.OdinSerializer.SerializationData;

namespace Ludiq.PeekCore
{
	public abstract class LudiqEditorWindow : EditorWindow, ISerializationCallbackReceiver, ILudiqRootObject, IHasCustomMenu
	{
		#region Serialization
		
		[FormerlySerializedAs("_data")]
		[SerializeField, DoNotSerialize] // Serialize with Unity, but not with Full Serializer.
		protected FullSerializationData _fullData;
		
		[SerializeField, DoNotSerialize] // Serialize with Unity, but not with Odin Serializer.
		protected OdinSerializationData _odinData;

		[DoNotSerialize]
		protected bool _deserializationFailed;

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			Serialization.OnBeforeSerializeImplementation(this, ref _fullData, ref _odinData, _deserializationFailed);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Serialization.OnAfterDeserializeImplementation(this, _fullData, _odinData, ref _deserializationFailed);
		}

		void ILudiqRootObject.OnBeforeSerialize()
		{
			OnBeforeSerialize();
		}

		void ILudiqRootObject.OnAfterSerialize() { }

		void ILudiqRootObject.OnBeforeDeserialize() { }

		void ILudiqRootObject.OnAfterDeserialize()
		{
			OnAfterDeserialize();
		}

		protected virtual void OnBeforeSerialize() { }

		protected virtual void OnAfterSerialize() { }

		protected virtual void OnBeforeDeserialize() { }

		protected virtual void OnAfterDeserialize() { }
		
		public virtual void ShowData()
		{
			Serialization.ShowData(this.ToSafeString(), _fullData, _odinData);
		}

		#endregion
		
		protected virtual Event e => Event.current;

		protected Vector2 scroll;

		protected virtual void OnEnable()
		{
			// Manual handlers have to be used over magic methods because
			// magic methods don't get triggered when the window is out of focus
			EditorApplicationUtility.onSelectionChange += _OnSelectionChange;
			EditorApplicationUtility.onProjectChange += _OnProjectChange;
			EditorApplicationUtility.onHierarchyChange += _OnHierarchyChange;
			EditorApplicationUtility.onModeChange += _OnModeChange;
			EditorApplicationUtility.onEnterPlayMode += _OnEnterPlayMode;
			EditorApplicationUtility.onExitPlayMode += _OnExitPlayMode;
			EditorApplicationUtility.onEnterEditMode += _OnEnterEditMode;
			EditorApplicationUtility.onExitEditMode += _OnExitEditMode;
			EditorApplicationUtility.onUndoRedo += _OnUndoRedo;
			EditorApplicationUtility.onPrefabChange += _OnPrefabChange;
			EditorApplication.update += _Update;
		}

		protected virtual void OnDisable()
		{
			EditorApplicationUtility.onSelectionChange -= _OnSelectionChange;
			EditorApplicationUtility.onProjectChange -= _OnProjectChange;
			EditorApplicationUtility.onHierarchyChange -= _OnHierarchyChange;
			EditorApplicationUtility.onModeChange -= _OnModeChange;
			EditorApplicationUtility.onEnterPlayMode -= _OnEnterPlayMode;
			EditorApplicationUtility.onExitPlayMode -= _OnExitPlayMode;
			EditorApplicationUtility.onEnterEditMode -= _OnEnterEditMode;
			EditorApplicationUtility.onExitEditMode -= _OnExitEditMode;
			EditorApplicationUtility.onUndoRedo -= _OnUndoRedo;
			EditorApplicationUtility.onPrefabChange -= _OnPrefabChange;
			EditorApplication.update -= _Update;
		}

		protected virtual void _OnSelectionChange()
		{

		}

		protected virtual void _OnProjectChange()
		{

		}

		protected virtual void _OnHierarchyChange()
		{

		}

		protected virtual void _OnModeChange()
		{

		}

		protected virtual void _OnEnterPlayMode()
		{
		}

		protected virtual void _OnExitPlayMode()
		{

		}

		protected virtual void _OnEnterEditMode()
		{

		}

		protected virtual void _OnExitEditMode()
		{

		}

		protected virtual void _OnUndoRedo()
		{

		}

		protected virtual void _OnPrefabChange(GameObject instance)
		{

		}

		protected virtual void Update()
		{
			// Position isn't reliable in GUI calls due to layouting, so cache it here
			reliablePosition = position;
		}

		protected virtual void _Update() { }

		protected virtual void OnGUI()
		{

		}

		public Rect reliablePosition { get; private set; }

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Show Data..."), false, ShowData);
		}

		public override string ToString()
		{
			return this.ToSafeString();
		}
	}
}