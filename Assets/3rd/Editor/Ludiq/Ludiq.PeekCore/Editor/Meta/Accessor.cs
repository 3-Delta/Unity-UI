using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Ludiq.OdinSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public abstract class Accessor : IAttributeProvider, IList, IDictionary, IObservableDisposable
	{
		protected Accessor(object subpath, Accessor parent)
		{
			Ensure.That(nameof(subpath)).IsNotNull(subpath);

			if (isRoot)
			{
				Ensure.That(nameof(parent)).IsNull(parent);
			}
			else
			{
				Ensure.That(nameof(parent)).IsNotNull(parent);
			}

			this.subpath = subpath;
			this.parent = parent;

			children = new Children();
		}

		public GUIContent label { get; protected set; } = new GUIContent();

		protected virtual bool isRoot => false;

		public virtual bool isEditable => true;

		public void RecordUndo()
		{
			RecordUndo($"Modify {label?.text ?? "accessor value"}");
		}

		public void RecordUndo(string name)
		{
			UndoUtility.RecordObject(serializedObject, name);
		}



		#region Hierarchy

		void IDisposable.Dispose()
		{
			Unlink();
		}

		bool IObservableDisposable.IsDisposed => !isLinked;

		public class Children : KeyedCollection<object, Accessor>
		{
			protected override object GetKeyForItem(Accessor item)
			{
				return item.subpath;
			}
		}

		public Children children { get; }

		public Accessor parent { get; }

		public bool isLinked => isRoot || parent != null;

		public void Unlink()
		{
			if (isRoot)
			{
				throw new InvalidOperationException("Cannot unlink root accessor.");
			}

			if (!isLinked)
			{
				return;
			}

			var toString = ToString();

			UnlinkChildren();

			parent?.children.Remove(subpath);

			if (isMatchedPrefabInstance)
			{
				prefabDefinition.Unlink();
			}

			if (LudiqCore.Configuration.developerMode && LudiqCore.Configuration.trackAccessorState)
			{
				Debug.LogWarning($"Unlinked accessor node:\n{toString}");
			}
		}

		public void UnlinkChildren()
		{
			while (children.Any())
			{
				children.First().Unlink();
			}
		}

		public void EnsureLinked()
		{
			if (!isLinked)
			{
				throw new InvalidOperationException($"Accessor node has been unlinked: '{this}'.");
			}
		}

		public RootAccessor root
		{
			get
			{
				var level = this;

				while (level.parent != null)
				{
					level = level.parent;
				}

				return level as RootAccessor;
			}
		}

		public virtual UnityObject serializedObject => root?.serializedObject;

		public Accessor Ancestor(Func<Accessor, bool> predicate, bool includeSelf = false)
		{
			var level = includeSelf ? this : parent;

			while (level != null)
			{
				if (predicate(level))
				{
					return level;
				}

				level = level.parent;
			}

			return null;
		}

		public T Ancestor<T>(bool includeSelf = false) where T : Accessor
		{
			return (T)Ancestor(accessor => accessor is T, includeSelf);
		}

		public IEnumerable<Accessor> Descendants(Func<Accessor, bool> predicate)
		{
			var result = children.Where(predicate);

			foreach (var child in children)
			{
				result = result.Concat(child.Descendants(predicate));
			}

			return result;
		}

		public IEnumerable<T> Descendants<T>() where T : Accessor
		{
			return Descendants(accessor => accessor is T).Cast<T>();
		}

		#endregion



		#region Path

		public string path { get; private set; }

		protected object subpath { get; private set; }

		private void CachePath()
		{
			path = Subpath();

			if (parent != null)
			{
				path = parent.path + "." + path;
			}

			odinPath = OdinPath(parent?.odinPath);
		}

		protected virtual string Subpath()
		{
			return subpath.ToString();
		}

		public override string ToString()
		{
			return path ?? "(?)." + Subpath();
		}

		#endregion



		#region Defined type

		public Type definedType
		{
			get
			{
				EnsureLinked();

				return _definedType;
			}
			protected set
			{
				_definedType = value;

				instantiator = value.Instantiator();
			}
		}

		protected Type _definedType;

		#endregion



		#region Instantiation

		public Func<object> instantiator { get; set; }

		public bool instantiate { get; set; }

		private bool canInstantiate => instantiate && instantiator != null;

		private object instantiatedValue
		{
			get
			{
				var rawValue = this.rawValue;

				if (rawValue == null && canInstantiate)
				{
					var fallbackValue = instantiator();

					if (fallbackValue == null)
					{
						throw new InvalidOperationException("Accessor instantiator returns null. Aborting to prevent stack overflow.");
					}

					this.rawValue = rawValue = fallbackValue;
				}

				return rawValue;
			}
			set
			{
				if (value == null && canInstantiate)
				{
					value = instantiator();

					if (value == null)
					{
						throw new InvalidOperationException("Accessor instantiator returns null. Aborting to prevent stack overflow.");
					}
				}

				rawValue = value;
			}
		}

		#endregion



		#region Value

		private bool obtainedValue;

		private object lastObservedValue;

		protected abstract object rawValue { get; set; }

		protected bool ValueEquals(object a, object b)
		{
			if (!Equals(a, b))
			{
				return false;
			}

			return true;
		}

		public object value
		{
			get
			{
				EnsureLinked();

				try
				{
					var cachedInstantiatedValue = instantiatedValue;

					if (!obtainedValue)
					{
						lastObservedValue = cachedInstantiatedValue;
						obtainedValue = true;
					}
					else if (!ValueEquals(cachedInstantiatedValue, lastObservedValue))
					{
						var previousValue = lastObservedValue;
						lastObservedValue = cachedInstantiatedValue;
						OnValueChange(previousValue);
					}

					return cachedInstantiatedValue;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("Failed to get accessor value for:\n" + this, ex);
				}
			}
			set
			{
				EnsureLinked();

				try
				{
					var previousValue = instantiatedValue;

					lastObservedValue = instantiatedValue = value;

					if (!ValueEquals(value, previousValue))
					{
						OnValueChange(previousValue);
					}
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("Failed to set accessor value for:\n" + this, ex);
				}
			}
		}

		protected virtual void OnValueChange(object previousValue)
		{
			// Force a value type change check by calling the getter.
			var forceWatchValueType = valueType;

			if (PluginContainer.initialized && LudiqCore.Configuration.developerMode && LudiqCore.Configuration.trackAccessorState)
			{
				Debug.LogFormat
				(
					"Value changed on accessor node: {0}\n{1} => {2}",
					this,
					previousValue != null ? previousValue.ToString() : "(null)",
					value != null ? value.ToString() : "(null)"
				);
			}

			foreach (var child in children)
			{
				child.OnParentValueChange(previousValue);
			}

			_valueChanged?.Invoke(previousValue);
		}

		protected virtual void OnParentValueChange(object previousValue) { }

		private event Action<object> _valueChanged;

		public event Action<object> valueChanged
		{
			add
			{
				lastObservedValue = instantiatedValue;
				obtainedValue = true;
				value(this.value);
				_valueChanged += value;
			}
			remove => _valueChanged -= value;
		}

		#endregion



		#region Value Type

		private bool obtainedValueType;

		private Type rawValueType
		{
			get
			{
				var cachedInstantiatedValue = instantiatedValue;

				return cachedInstantiatedValue != null ? cachedInstantiatedValue.GetType() : definedType;
			}
		}

		private Type lastObservedValueType;

		public Type valueType
		{
			get
			{
				EnsureLinked();

				var cachedRawValueType = rawValueType;

				if (!obtainedValueType)
				{
					lastObservedValueType = cachedRawValueType;
					obtainedValueType = true;
				}
				else if (cachedRawValueType != lastObservedValueType)
				{
					var previousValueType = lastObservedValueType;
					lastObservedValueType = cachedRawValueType;
					OnValueTypeChange(previousValueType);
				}

				return cachedRawValueType;
			}
		}

		private void AnalyzeCollection()
		{
			isEnumerable = typeof(IEnumerable).IsAssignableFrom(valueType);

			if (isEnumerable)
			{
				enumerableType = valueType;
				enumerableElementType = TypeUtility.GetEnumerableElementType(enumerableType, true);
			}
			else
			{
				enumerableType = null;
				enumerableElementType = null;
			}

			isList = typeof(IList).IsAssignableFrom(valueType);

			if (isList)
			{
				listType = valueType;
				listElementType = TypeUtility.GetListElementType(listType, true);
			}
			else
			{
				listType = null;
				listElementType = null;
			}

			isDictionary = typeof(IDictionary).IsAssignableFrom(valueType);
			isOrderedDictionary = typeof(IOrderedDictionary).IsAssignableFrom(valueType);

			if (isDictionary)
			{
				dictionaryType = valueType;
				dictionaryKeyType = TypeUtility.GetDictionaryKeyType(dictionaryType, true);
				dictionaryValueType = TypeUtility.GetDictionaryValueType(dictionaryType, true);
			}
			else
			{
				dictionaryType = null;
				dictionaryKeyType = null;
				dictionaryValueType = null;
			}
		}

		protected virtual void OnValueTypeChange(Type previousType)
		{
			if (PluginContainer.initialized && LudiqCore.Configuration.developerMode && LudiqCore.Configuration.trackAccessorState)
			{
				Debug.LogFormat
				(
					"Value type changed on accessor node: {0}\n{1} => {2}",
					this,
					previousType != null ? previousType.CSharpName(false) : "(null)",
					valueType != null ? valueType.CSharpName(false) : "(null)"
				);
			}

			AnalyzeCollection();

			foreach (var child in children)
			{
				child.OnParentValueTypeChange(previousType);
			}

			if (_valueTypeChanged != null)
			{
				_valueTypeChanged(previousType);
			}
		}

		protected virtual void OnParentValueTypeChange(Type previousType) { }

		private event Action<Type> _valueTypeChanged;

		public event Action<Type> valueTypeChanged
		{
			add
			{
				lastObservedValueType = rawValueType;
				obtainedValueType = true;
				value(valueType);
				_valueTypeChanged += value;
			}
			remove => _valueTypeChanged -= value;
		}

		#endregion



		#region Digging

		// Using TSubpath to avoid boxing and alloc of object
		protected TAccessor Dig<TSubpath, TAccessor>(TSubpath subpath, Func<Accessor, TAccessor> constructor, bool createInPrefab, Accessor prefabInstance = null) where TAccessor : Accessor
		{
			if (subpath == null)
			{
				throw new ArgumentNullException(nameof(subpath));
			}

			if (constructor == null)
			{
				throw new ArgumentNullException(nameof(constructor));
			}

			Accessor child;

			if (children.TryGetValue(subpath, out child))
			{
				if (child is TAccessor accessorChild)
				{
					return accessorChild;
				}
				else
				{
					throw new InvalidOperationException($"Accessor mismatch: expected '{typeof(TAccessor).Name}', found '{child.GetType().Name}'.");
				}
			}
			else
			{
				try
				{
					var containsBefore = children.Contains(subpath);

					child = constructor(this);

					if (!containsBefore && children.Contains(child.subpath))
					{
						throw new InvalidOperationException($"Children didn't contain '{subpath} / {child.subpath}' subpath before the constructor but now does.");
					}

					child.CachePath();
					child.AnalyzeCollection();

					if (isMatchedPrefabInstance)
					{
						if (createInPrefab)
						{
							child.prefabDefinition = prefabDefinition.Dig(subpath, constructor, false, child);
						}
						else if (prefabDefinition.children.Contains(subpath))
						{
							child.prefabDefinition = prefabDefinition.children[subpath];
						}
						else
						{
							child.isUnmatchedPrefabInstance = true;
						}
					}
					else if (isUnmatchedPrefabInstance)
					{
						child.isUnmatchedPrefabInstance = true;
					}

					children.Add(child);

					if (PluginContainer.initialized && LudiqCore.Configuration.developerMode && LudiqCore.Configuration.trackAccessorState)
					{
						Debug.LogWarningFormat
						(
							"Created {0} node{1}:\n{2}",
							child.GetType().CSharpName(false),
							child.isPrefabInstance ? " (prefab instance)" : prefabInstance != null ? " (prefab definition)" : "",
							child
						);
					}

					return (TAccessor)child;
				}
				catch
				{
					// If digging fails and we're creating a prefab definition mirror,
					// we will simply notify the instance accessor that it has no hierarchy equivalent.
					if (prefabInstance != null)
					{
						prefabInstance.isUnmatchedPrefabInstance = true;
						return null;
					}
					else
					{
						throw;
					}
				}
			}
		}

		#endregion



		#region Prefabs

		public Accessor prefabDefinition { get; protected set; }

		public bool isPrefabInstance => isMatchedPrefabInstance || isUnmatchedPrefabInstance;

		public bool isMatchedPrefabInstance => prefabDefinition != null;

		public bool isUnmatchedPrefabInstance { get; private set; }

		public bool isRevertibleToPrefab => isMatchedPrefabInstance;

		public void RevertToPrefab()
		{
			if (!isRevertibleToPrefab)
			{
				throw new InvalidOperationException($"Cannot revert {this} to prefab.");
			}

			RecordUndo($"Revert {label?.text ?? "accessor value"} to prefab");

			value = prefabDefinition.value.CloneViaSerializationPolicy();
		}

		internal string odinPath { get; private set; }

		protected virtual string OdinPath(string parentPath)
		{
			return null;
		}

		public virtual bool supportsPrefabModifications => serializedObject is ISupportsPrefabSerialization && odinPath != null;

		public bool hasPrefabModifications
		{
			get
			{
				if (!supportsPrefabModifications)
				{
					return false;
				}

				foreach (var modification in root.prefabModifications)
				{
					if (modification.Path == odinPath)
					{
						return true;
					}
				}

				return false;
			}
		}

		public void GetPrefabModificationsNoAlloc(List<PrefabModification> modifications)
		{
			Ensure.That(nameof(modifications)).IsNotNull(modifications);

			modifications.Clear();

			if (!supportsPrefabModifications)
			{
				return;
			}

			foreach (var modification in root.prefabModifications)
			{
				if (modification.Path == odinPath)
				{
					modifications.Add(modification);
				}
			}
		}

		public void SetPrefabModifications(List<PrefabModification> modifications)
		{
			Ensure.That(nameof(modifications)).IsNotNull(modifications);

			if (!supportsPrefabModifications)
			{
				return;
			}

			var allModifications = root.prefabModifications;

			allModifications.RemoveAll(m => m.Path == odinPath);

			foreach (var modification in modifications)
			{
				if (modification.Path != odinPath)
				{
					Debug.LogWarning($"Trying to assign a modification with a different path than the accessor's:\n{modification.Path} != {odinPath}");
				}

				allModifications.Add(modification);
			}

			root.ApplyPrefabModifications();
		}

		#endregion



		#region Attributes

		public abstract Attribute[] GetCustomAttributes(bool inherit = true);

		#endregion



		#region Indexers

		public MemberAccessor this[string name] => Member(name);

		public IndexAccessor this[int index] => Index(index);

		#endregion



		#region Enumerables

		public bool isEnumerable { get; private set; }

		public Type enumerableType { get; private set; }

		public Type enumerableElementType { get; private set; }

		private IEnumerable enumerable
		{
			get
			{
				if (!isEnumerable)
				{
					throw new InvalidOperationException();
				}

				return (IEnumerable)value;
			}
		}

		#endregion



		#region Collections

		public IEnumerator GetEnumerator()
		{
			if (isDictionary)
			{
				return dictionary.GetEnumerator();
			}

			if (isList)
			{
				return list.GetEnumerator();
			}
			else if (isEnumerable)
			{
				return enumerable.GetEnumerator();
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		public int Count
		{
			get
			{
				if (isDictionary)
				{
					return dictionary.Count;
				}

				if (isList)
				{
					return list.Count;
				}
				else if (isEnumerable)
				{
					return enumerable.Cast<object>().Count();
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		public bool IsFixedSize
		{
			get
			{
				if (isDictionary)
				{
					return dictionary.IsFixedSize;
				}
				else if (isList)
				{
					return !listType.IsArray && list.IsFixedSize;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		public bool IsReadOnly
		{
			get
			{
				if (isDictionary)
				{
					return dictionary.IsReadOnly;
				}
				else if (isList)
				{
					return list.IsReadOnly;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		public bool IsSynchronized
		{
			get
			{
				if (isDictionary)
				{
					return dictionary.IsSynchronized;
				}
				else if (isList)
				{
					return list.IsSynchronized;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		public object SyncRoot
		{
			get
			{
				if (isDictionary)
				{
					return dictionary.SyncRoot;
				}
				else if (isList)
				{
					return list.SyncRoot;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		public bool Contains(object value)
		{
			if (isDictionary)
			{
				return dictionary.Contains(value);
			}
			else if (isList)
			{
				return list.Contains(value);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		public void Remove(object value)
		{
			if (isDictionary)
			{
				dictionary.Remove(value);
				UnlinkDictionaryChildren();
			}
			else if (isList)
			{
				var list = GetResizableList();
				list.Remove(value);
				ApplyResizableList();
				UnlinkListChildren();
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		public void Clear()
		{
			if (isDictionary)
			{
				dictionary.Clear();
				UnlinkDictionaryChildren();
			}
			else if (isList)
			{
				var list = GetResizableList();
				list.Clear();
				ApplyResizableList();
				UnlinkListChildren();
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		#endregion



		#region Lists

		public bool isList { get; private set; }

		public Type listType { get; private set; }

		public Type listElementType { get; private set; }

		private IList list
		{
			get
			{
				if (!isList)
				{
					throw new InvalidOperationException();
				}

				return (IList)value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				if (!isList)
				{
					throw new InvalidOperationException();
				}

				return Index(index).value;
			}
			set
			{
				if (!isList)
				{
					throw new InvalidOperationException();
				}

				Index(index).value = value;
			}
		}

		private void UnlinkListChildren()
		{
			var listChildren = children.OfType<IndexAccessor>().ToArray();

			foreach (var listChild in listChildren)
			{
				listChild.Unlink();
			}
		}

		private IList resizableList;

		protected IList GetResizableList()
		{
			if (listType.IsArray)
			{
				if (resizableList == null)
				{
					resizableList = new List<object>();
				}
				else
				{
					resizableList.Clear();
				}

				foreach (var item in list)
				{
					resizableList.Add(item);
				}

				return resizableList;
			}
			else
			{
				return list;
			}
		}

		protected void ApplyResizableList()
		{
			if (listType.IsArray)
			{
				var array = Array.CreateInstance(listElementType, resizableList.Count);
				resizableList.CopyTo(array, 0);
				value = array;
			}
		}

		public int Add(object value)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			var list = GetResizableList();
			var newIndex = list.Add(value);
			ApplyResizableList();
			return newIndex;
		}

		public void Insert(int index, object value)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			var list = GetResizableList();
			list.Insert(index, value);
			ApplyResizableList();
		}

		public int IndexOf(object value)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			return list.IndexOf(value);
		}

		public void RemoveAt(int index)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			var list = GetResizableList();
			list.RemoveAt(index);
			ApplyResizableList();
			UnlinkListChildren();
		}

		public void CopyTo(Array array, int index)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			list.CopyTo(array, index);
		}

		public void Move(int sourceIndex, int destinationIndex)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			if (destinationIndex > sourceIndex)
			{
				destinationIndex--;
			}

			var list = GetResizableList();
			var item = list[sourceIndex];
			list.RemoveAt(sourceIndex);
			list.Insert(destinationIndex, item);
			ApplyResizableList();

			UnlinkListChildren();
		}

		public void Duplicate(int index)
		{
			if (!isList)
			{
				throw new InvalidOperationException();
			}

			object newItem;

			var list = GetResizableList();

			if (Index(index).valueType.IsValueType)
			{
				newItem = list[index];
			}
			else if (typeof(ICloneable).IsAssignableFrom(Index(index).valueType))
			{
				newItem = ((ICloneable)list[index]).Clone();
			}
			else
			{
				newItem = list[index].CloneViaSerializationPolicy();
			}

			list.Insert(index + 1, newItem);

			ApplyResizableList();
		}

		#endregion



		#region Dictionaries

		public bool isDictionary { get; private set; }

		public bool isOrderedDictionary { get; private set; }

		public Type dictionaryType { get; private set; }

		public Type dictionaryKeyType { get; private set; }

		public Type dictionaryValueType { get; private set; }

		private IDictionary dictionary
		{
			get
			{
				if (!isDictionary)
				{
					throw new InvalidOperationException();
				}

				return (IDictionary)value;
			}
		}

		public ICollection Keys
		{
			get
			{
				if (!isDictionary)
				{
					throw new InvalidOperationException();
				}

				return dictionary.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				if (!isDictionary)
				{
					throw new InvalidOperationException();
				}

				return dictionary.Values;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (!isDictionary)
				{
					throw new InvalidOperationException();
				}

				return Indexer(key).value;
			}
			set
			{
				if (!isDictionary)
				{
					throw new InvalidOperationException();
				}

				Indexer(key).value = value;
			}
		}

		public void Add(object key, object value)
		{
			if (!isDictionary)
			{
				throw new InvalidOperationException();
			}

			dictionary.Add(key, value);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			if (!isDictionary)
			{
				throw new InvalidOperationException();
			}

			return dictionary.GetEnumerator();
		}

		public Accessor KeyAccessor(int index)
		{
			if (!isDictionary)
			{
				throw new InvalidOperationException();
			}

			return DictionaryKeyAt(index);
		}

		public Accessor ValueAccessor(int index)
		{
			if (!isDictionary)
			{
				throw new InvalidOperationException();
			}

			return DictionaryValueAt(index);
		}

		private void UnlinkDictionaryChildren()
		{
			var dictionaryChildren = children.Where
			(
				c =>
					c is IndexerAccessor ||
					c is DictionaryIndexAccessor
			).ToArray();

			foreach (var dicitonaryChild in dictionaryChildren)
			{
				dicitonaryChild.Unlink();
			}
		}

		#endregion



		#region Digs

		private abstract class NoAllocDelegate<T>
		{
			protected T @delegate;
		}

		private abstract class NoAllocDig<T> : NoAllocDelegate<Func<Accessor, T>> where T : Accessor { }

		private class DigMember : NoAllocDig<MemberAccessor>
		{
			public DigMember()
			{
				@delegate = parent => new MemberAccessor(name, bindingFlags, parent);
			}

			private string name;

			private BindingFlags bindingFlags;

			public Func<Accessor, MemberAccessor> Get(string name, BindingFlags bindingFlags)
			{
				this.name = name;
				this.bindingFlags = bindingFlags;
				return @delegate;
			}
		}

		private class DigIndex : NoAllocDig<IndexAccessor>
		{
			public DigIndex()
			{
				@delegate = parent => new IndexAccessor(index, parent);
			}

			private int index;

			public Func<Accessor, IndexAccessor> Get(int index)
			{
				this.index = index;
				return @delegate;
			}
		}

		private class DigIndexer : NoAllocDig<IndexerAccessor>
		{
			public DigIndexer()
			{
				@delegate = parent => new IndexerAccessor(indexer, parent);
			}

			private object indexer;

			public Func<Accessor, IndexerAccessor> Get(object indexer)
			{
				this.indexer = indexer;
				return @delegate;
			}
		}

		private class DigCast : NoAllocDig<CastAccessor>
		{
			public DigCast()
			{
				@delegate = parent => new CastAccessor(type, parent);
			}

			private Type type;

			public Func<Accessor, CastAccessor> Get(Type type)
			{
				this.type = type;
				return @delegate;
			}
		}

		private class DigStaticObject : NoAllocDig<LambdaAccessor>
		{
			public DigStaticObject()
			{
				@delegate = parent => new LambdaAccessor(@object, definedType, parent);
			}

			private object @object;

			private Type definedType;

			public Func<Accessor, LambdaAccessor> Get(object @object, Type definedType)
			{
				this.@object = @object;
				this.definedType = definedType;
				return @delegate;
			}
		}

		private static readonly DigMember digMember = new DigMember();

		private static readonly DigIndex digIndex = new DigIndex();

		private static readonly DigIndexer digIndexer = new DigIndexer();

		private static readonly DigCast digCast = new DigCast();

		private static readonly DigStaticObject digStaticObject = new DigStaticObject();
		
		public static RootAccessor Root()
		{
			var root = new RootAccessor();
			root.CachePath();
			return root;
		}

		public static RootAccessor Root(object value)
		{
			Ensure.That(nameof(serializedObject)).IsNotNull(value);
			var root = new RootAccessor(value);
			root.CachePath();
			return root;
		}

		public static RootAccessor Root(object value, Type definedType)
		{
			var root = new RootAccessor(value, definedType);
			root.CachePath();
			return root;
		}

		public LambdaAccessor Lambda(object subpath, object @object, Type definedType)
		{
			return Dig(subpath, parent => new LambdaAccessor(subpath, @object, definedType, parent), false);
		}

		public LambdaAccessor Lambda(object subpath, object @object)
		{
			return Lambda(subpath, @object, typeof(object));
		}

		public LambdaAccessor Lambda(object @object, Type definedType)
		{
			return Dig(@object, digStaticObject.Get(@object, definedType), false);
		}

		public LambdaAccessor Lambda(object @object)
		{
			Ensure.That(nameof(@object)).IsNotNull(@object);

			return Lambda(@object, @object.GetType());
		}

		public MemberAccessor Member(string name, BindingFlags bindingFlags = MemberAccessor.DefaultBindingFlags)
		{
			return Dig(name, digMember.Get(name, bindingFlags), true);
		}

		public IndexAccessor Index(int index)
		{
			return Dig(index, digIndex.Get(index), true);
		}

		public IndexerAccessor Indexer(object indexer)
		{
			return Dig(indexer, digIndexer.Get(indexer), true);
		}

		public CastAccessor Cast(Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			return Dig(type, digCast.Get(type), true);
		}

		public CastAccessor Cast<T>()
		{
			return Cast(typeof(T));
		}

		public DictionaryKeyAtIndexAccessor DictionaryKeyAt(int index)
		{
			return Dig("__keyAt." + index, parent => new DictionaryKeyAtIndexAccessor(index, parent), false);
		}

		public DictionaryValueAtIndexAccessor DictionaryValueAt(int index)
		{
			return Dig("__valueAt." + index, parent => new DictionaryValueAtIndexAccessor(index, parent), false);
		}

		public ProxyAccessor Proxy(object subpath, Accessor binding)
		{
			return Dig(subpath, parent => new ProxyAccessor(subpath, binding, parent), false);
		}

		public EditorPrefAccessor EditorPref(PluginConfiguration configuration, MemberInfo member)
		{
			return Dig(member, parent => new EditorPrefAccessor(configuration, member, parent), false);
		}

		public ProjectSettingAccessor ProjectSetting(PluginConfiguration configuration, MemberInfo member)
		{
			return Dig(member, parent => new ProjectSettingAccessor(configuration, member, parent), false);
		}

		public Accessor AutoDig(string path)
		{
			var accessor = this;

			foreach (var pathPart in path.Split('.'))
			{
				string fieldName;
				int index;

				if (SerializedPropertyUtility.IsPropertyIndexer(pathPart, out fieldName, out index))
				{
					accessor = accessor.Member(fieldName);
					accessor = accessor.Index(index);
				}
				else
				{
					accessor = accessor.Member(fieldName);
				}
			}

			return accessor;
		}

		public static Accessor FromProperty(SerializedProperty property)
		{
			return Root(property.serializedObject.targetObject).AutoDig(property.FixedPropertyPath());
		}

		#endregion
	}
}