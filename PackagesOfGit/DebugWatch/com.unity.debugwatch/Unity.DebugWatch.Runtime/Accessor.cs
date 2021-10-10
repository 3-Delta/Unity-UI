using System;
using UnityEngine;

namespace Unity.DebugWatch
{

    public interface IAccessor
    {
        Type GetValueType();
    }
    public interface IAccessor<TValue> : IAccessor
    {
        bool TryGet(out TValue value);
        bool TrySet(TValue value);
    }
    public interface IAccessorFactory
    {
        IAccessor Create(IWatch w, IAccessor a);
    }

    [Serializable]
    public class DefaultStringAccessor<TValue> : IAccessor<string>
    {
        [SerializeField]
        public IAccessor<TValue> Accessor;
        public DefaultStringAccessor(IAccessor<TValue> accessor)
        {
            Accessor = accessor;
        }
        public bool TryGet(out string value)
        {
            if (Accessor.TryGet(out var v))
            {
                value = v.ToString();
                return true;
            }
            value = default;
            return false;
        }
        public bool TrySet(string value)
        {
            return false;
        }
        public Type GetValueType()
        {
            return typeof(string);
        }

        public class Factory : IAccessorFactory
        {
            public IAccessor Create(IWatch w, IAccessor a)
            {
                if (a is IAccessor<TValue> av)
                {
                    return new DefaultStringAccessor<TValue>(av);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}