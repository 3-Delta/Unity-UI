using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public static class PropertyUtils
    {
        public static PropertyPath Appended(this PropertyPath propPath, string part)
        {
            PropertyPath copy = new PropertyPath(propPath.ToString());
            copy.Push(part);
            return copy;
        }
        public static PropertyPath AppendedIndexer(this PropertyPath propPath, string indexer)
        {
            return new PropertyPath(propPath.ToString() + indexer);
        }

        public static PropertyPath AppendedIndexer(this PropertyPath propPath, string input, RangeInt indexerRange)
        {
            return AppendedIndexer(propPath, input.Substring(indexerRange));
        }


        public static bool VisitMembersAtPath<TContainer>(ref TContainer container, PropertyPath path, Func<ContextMemberInfo, bool> visitor)
        {
            
            if (path == null || path.PartsCount == 0)
            {
                var visitor2 = new VisitMembersVisitor(visitor);
                PropertyContainer.Visit(ref container, visitor2);
                return visitor2.Result;
            }
            else
            {
                var ct = new ChangeTracker();
                var visitor2 = new VisitMembersAtPathVisitor(visitor);
                PropertyContainer.TryVisitAtPath(ref container, path, visitor2, ref ct);
                return visitor2.Result;
            }
        }

        public class VisitMembersVisitor : IPropertyVisitor
        {
            public Func<ContextMemberInfo, bool> Visitor;
            public bool Result = true;
            public VisitMembersVisitor(Func<ContextMemberInfo, bool> visitor)
            {
                Visitor = visitor;
            }
            VisitStatus IPropertyVisitor.VisitProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                if (Result)
                {
                    var info = new ContextMemberInfo(
                        ContextFieldInfo.Make(property.GetName()),
                        ContextTypeInfo.MakeProperty(property)
                    );
                    Result = Visitor(info);
                }
                return VisitStatus.Handled;
            }

            VisitStatus IPropertyVisitor.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                if (Result)
                {
                    var info = new ContextMemberInfo(
                        ContextFieldInfo.Make(property.GetName()),
                        ContextTypeInfo.MakeProperty(property, ref container)
                    );
                    Result = Visitor(info);
                }
                return VisitStatus.Handled;
            }
        }


        public class VisitMembersAtPathVisitor : IPropertyVisitor
        {
            public Func<ContextMemberInfo, bool> Visitor;
            public bool Result = true;
            bool visitedFirst = false;
            public VisitMembersAtPathVisitor(Func<ContextMemberInfo, bool> visitor)
            {
                Visitor = visitor;
            }

            class FirstVisitor : IPropertyVisitor
            {
                public Func<ContextMemberInfo, bool> Visitor;
                public bool Result = true;
                public FirstVisitor(Func<ContextMemberInfo, bool> visitor)
                {
                    Visitor = visitor;
                }
                VisitStatus IPropertyVisitor.VisitProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
                {
                    if (Result)
                    {
                        var info = new ContextMemberInfo(
                            ContextFieldInfo.Make(property.GetName()),
                            ContextTypeInfo.MakeProperty(property)
                        );
                        Result = Visitor(info);
                    }
                    return VisitStatus.Handled;
                }

                public VisitStatus VisitCollectionProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
                    where TProperty2 : ICollectionProperty<TContainer2, TValue2>
                {
                    if (Result)
                    {
                        int count = property.GetCount(ref container);
                        for (int i = 0; i != count && Result; ++i)
                        {
                            var getter = new MemberInfoGetter<TContainer2>();
                            property.GetPropertyAtIndex(ref container, i, ref changeTracker, ref getter);
                            ContextMemberInfo info;
                            if (getter.Found)
                            {
                                info = new ContextMemberInfo(
                                    ContextFieldInfo.MakeOperator("[" + i + "]")
                                    , getter.Result
                                );
                            }
                            else
                            {
                                // Unknown type
                                info = new ContextMemberInfo(
                                    ContextFieldInfo.MakeOperator("[" + i + "]")
                                    , new ContextTypeInfo()
                                );
                            }
                            Result = Visitor(info);
                        }
                    }
                    return VisitStatus.Handled;
                }
                public class MemberInfoGetter<TContainer2> : ICollectionElementPropertyGetter<TContainer2>
                {
                    public ContextTypeInfo Result;
                    public bool Found = false;
                    void ICollectionElementPropertyGetter<TContainer2>.VisitProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer2 container, ref ChangeTracker changeTracker)
                    {
                        if (Found) return;
                        Found = true;
                        Result.IsProperty = true;
                        Result.IsPropertyContainer = property.IsContainer;
                        Result.IsCollection = false;
                        Result.CollectionLenth = 0;
                        Result.ValueType = typeof(TElement);
                    }
                    void ICollectionElementPropertyGetter<TContainer2>.VisitCollectionProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer2 container, ref ChangeTracker changeTracker)
                    {
                        if (Found) return;
                        Found = true;
                        Result.IsProperty = true;
                        Result.IsPropertyContainer = property.IsContainer;
                        Result.IsCollection = true;
                        Result.CollectionLenth = property.GetCount(ref container);
                        Result.ValueType = typeof(TElement);
                    }
                }
            }









            VisitStatus IPropertyVisitor.VisitProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                if (visitedFirst) return VisitStatus.Handled;
                if (!Result) return VisitStatus.Handled;
                visitedFirst = true;

                if (property.IsContainer)
                {
                    var bag = PropertyBagResolver.Resolve<TValue2>();
                    if (bag != null)
                    {
                        var ct = new ChangeTracker();
                        var v = property.GetValue(ref container);
                        var visitor2 = new FirstVisitor(Visitor);
                        bag.Accept(ref v, ref visitor2, ref ct);
                        Result = visitor2.Result;
                    }
                }
                return VisitStatus.Handled;
            }

            VisitStatus IPropertyVisitor.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                if (visitedFirst) return VisitStatus.Handled;
                if (!Result) return VisitStatus.Handled;
                visitedFirst = true;

                var visitor2 = new FirstVisitor(Visitor);
                var vs = visitor2.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(property, ref container, ref changeTracker);
                Result = visitor2.Result;
                return vs;
            }
        }


        public static bool VisitAtPath<TContainer>(ref TContainer container, PropertyPath path, IPropertyVisitor visitor)
        {
            if (path == null || path.PartsCount == 0)
            {
                PropertyContainer.Visit(ref container, visitor);
            }
            else
            {
                var ct = new ChangeTracker();
                PropertyContainer.TryVisitAtPath(ref container, path, new CollectionElementVisitor(visitor), ref ct);
            }
            return true;
        }
        class CollectionElementVisitor : IPropertyVisitor
        {
            IPropertyVisitor Visitor;
            public CollectionElementVisitor(IPropertyVisitor visitor)
            {
                Visitor = visitor;
            }
            VisitStatus IPropertyVisitor.VisitProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                if (property.IsContainer)
                {
                    var bag = PropertyBagResolver.Resolve<TValue2>();
                    if (bag != null)
                    {
                        var ct = new ChangeTracker();
                        var v = property.GetValue(ref container);
                        bag.Accept(ref v, ref Visitor, ref ct);
                    }
                }
                return VisitStatus.Handled;
            }

            VisitStatus IPropertyVisitor.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                return Visitor.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(property, ref container, ref changeTracker);
            }
        }
    }


}