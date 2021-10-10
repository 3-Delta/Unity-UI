using System;
using Unity.Properties;
using UnityEditor.Graphs;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{

    [Serializable]
    public class PropertyWatch<TContainer, TValue> : IWatch<TValue>
    {
        [SerializeField]
        public IWatchContext Context;
        [SerializeField]
        public TContainer Container;
        [SerializeField]
        public PropertyPath PropPath;
        public PropertyWatch(IWatchContext context, TContainer container, PropertyPath propPath) 
        {
            Context = context;
            Container = container;
            PropPath = propPath;
            Unity.Assertions.Assert.IsTrue(PropPath.PartsCount != 0);
        }
        public PropertyWatch() { }
        public virtual string GetContextName()
        {
            return "";
        }
        public virtual string GetName()
        {
            if (Context != null)
            {
                return Context.Scope(PropPath.ToString());
            }
            return PropPath.ToString();
        }
        public virtual Type GetValueType()
        {
            return typeof(TValue);
        }
        public virtual bool TryGet(out TValue value)
        {
            try
            {
                return PropertyContainer.TryGetValueAtPath<TContainer, TValue>(ref Container, PropPath, out value);
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }
        public virtual bool TrySet(TValue value)
        {
            try
            {
                return PropertyContainer.TrySetValueAtPath<TContainer, TValue>(ref Container, PropPath, value);
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }
    }
    public class PropertyWatch2<TContainer, TValue> : IWatch<TValue>
    {
        [SerializeField]
        public IWatchContext Context;
        [SerializeField]
        public TContainer Container;
        [SerializeField]
        public IProperty<TContainer, TValue> Property;
        public PropertyWatch2(IWatchContext context, TContainer container, IProperty<TContainer, TValue> property)
        {
            Context = context;
            Container = container;
            Property = property;
        }
        public PropertyWatch2() { }
        public virtual string GetContextName()
        {
            if (Context != null)
            {
                return Context.Scope("");
            }
            return $"Property Container '{Container.ToString()}'";
        }
        public virtual string GetName()
        {
            return Property.GetName();
        }
        public Type GetValueType()
        {
            return typeof(TValue);
        }
        public bool TryGet(out TValue value)
        {
            value = Property.GetValue(ref Container);
            return true;
        }
        public bool TrySet(TValue value)
        {
            Property.SetValue(ref Container, value);
            return true;
        }
    }

}