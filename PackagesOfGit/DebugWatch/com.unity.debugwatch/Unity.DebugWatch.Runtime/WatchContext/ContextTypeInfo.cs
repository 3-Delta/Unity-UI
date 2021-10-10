
using System;
using Unity.Properties;

namespace Unity.DebugWatch.WatchContext
{
    public struct ContextTypeInfo
    {
        public bool IsProperty;
        public bool IsPropertyContainer;
        public bool IsCollection;
        public int CollectionLenth;
        public Type ValueType;

        public static ContextTypeInfo Make(Type valueType)
        {
            return new ContextTypeInfo()
            {
                IsProperty = false,
                IsPropertyContainer = false,
                IsCollection = false,
                CollectionLenth = 0,
                ValueType = valueType
            };
        }
        public static ContextTypeInfo MakeProperty(Type valueType, bool isPropertyContainer)
        {
            return new ContextTypeInfo()
            {
                IsProperty = true,
                IsPropertyContainer = isPropertyContainer,
                IsCollection = false,
                CollectionLenth = 0,
                ValueType = valueType
            };
        }
        public static ContextTypeInfo MakePropertyCollection(Type valueType, int length, bool isPropertyContainer)
        {
            return new ContextTypeInfo()
            {
                IsProperty = true,
                IsPropertyContainer = isPropertyContainer,
                IsCollection = true,
                CollectionLenth = length,
                ValueType = valueType
            };
        }
        public static ContextTypeInfo MakeProperty<TContainer, TValue>(IProperty<TContainer, TValue> property)
        {
            return new ContextTypeInfo()
            {
                IsProperty = true,
                IsPropertyContainer = property.IsContainer,
                IsCollection = false,
                CollectionLenth = 0,
                ValueType = typeof(TValue)
            };
        }

        public static ContextTypeInfo MakeProperty<TContainer, TValue>(ICollectionProperty<TContainer, TValue> property, ref TContainer container)
        {
            return new ContextTypeInfo()
            {
                IsProperty = true,
                IsPropertyContainer = property.IsContainer,
                IsCollection = true,
                CollectionLenth = property.GetCount(ref container),
                ValueType = typeof(TValue)
            };
        }
        public static ContextTypeInfo MakeUnknown()
        {
            return new ContextTypeInfo()
            {
                IsProperty = false,
                IsPropertyContainer = false,
                IsCollection = false,
                CollectionLenth = 0,
                ValueType = null
            };
        }
    }


}