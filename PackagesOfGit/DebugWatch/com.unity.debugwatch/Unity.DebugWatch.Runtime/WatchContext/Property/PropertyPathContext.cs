
using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{


    public class PropertyPathContext<TContainer, TPropertyType> : WatchContext
    {
        public TContainer Container;
        public PropertyPath PropPath;
        public IWatchContext RootPathContext;
        // propPath 
        public PropertyPathContext(IWatchContext parent, IWatchContext rootPathContext, string relativePath, TContainer container, PropertyPath propPath, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(TPropertyType)))
            )
        {
            Container = container;
            PropPath = propPath;
            RootPathContext = rootPathContext;
        }

        public override bool TryParse(string input, ref RangeInt cursor, out IWatchContext ctx)
        {
            var c = cursor;
            if (ParserUtils.TryExtractPropertyPathPart(input, ref c, out var partRange, out var identifierRange))
            {
                var visitor = new TryParseVisitor()
                {
                    Parent = this,
                    Root = RootPathContext,
                    Container = Container,
                    PropertyIdentifier = input.Substring(identifierRange),
                    PropertyPart = input.Substring(partRange),
                    RootPath = PropPath == null ? new PropertyPath() : PropPath
                };
                if (identifierRange.length == 0)
                {
                    if (PropPath == null || PropPath.PartsCount == 0)
                    {
                        ctx = default;
                        return false;
                    }
                    visitor.PropertyIdentifier = PropPath[PropPath.PartsCount - 1].Name;
                }
                PropertyUtils.VisitAtPath(ref Container, PropPath, visitor);
                if (visitor.Result != null)
                {
                    cursor = c;
                    ctx = visitor.Result;
                    return true;
                }
            }
            ctx = default;
            return false;
        }

        class TryParseVisitor : IPropertyVisitor
        {
            public IWatchContext Parent;
            public IWatchContext Root;
            public TContainer Container;
            public string PropertyIdentifier;
            public string PropertyPart;
            public PropertyPath RootPath;
            public IWatchContext Result;
            VisitStatus IPropertyVisitor.VisitProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                var propName = property.GetName();
                if (propName != PropertyIdentifier) return VisitStatus.Handled;
                if (Result != null) return VisitStatus.Handled;
                Result = new PropertyPathContext<TContainer, TValue2>(Parent, Root, ParserUtils.MakePathRelative(PropertyPart), Container, RootPath.Appended(PropertyPart), ContextFieldInfo.Make(PropertyPart));
                return VisitStatus.Handled;
            }

            VisitStatus IPropertyVisitor.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
            {
                var propName = property.GetName();
                if (propName != PropertyIdentifier) return VisitStatus.Handled;
                if (Result != null) return VisitStatus.Handled;
                Result = new PropertyPathContext<TContainer, TValue2>(Parent, Root, ParserUtils.MakePathRelative(PropertyPart), Container, RootPath.AppendedIndexer(PropertyPart), ContextFieldInfo.MakeOperator(PropertyPart));
                return VisitStatus.Handled;
            }
        }

        public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            return PropertyUtils.VisitMembersAtPath(ref Container, PropPath, visitor);
            
        }

        public override bool TryCreateWatch(out IWatch watch)
        {
            watch = new PropertyWatch<TContainer, TPropertyType>(RootPathContext, Container, PropPath);
            return true;
        }

        //public override bool TryCreateWatch(string path, RangeInt pathRange, out IWatch watch)
        //{
        //    watch = null;
        //    return false;
        //}

    }




    //public class PropertyBagContext<TContainer> : WatchContext
    //{
    //    public TContainer Container;
    //    public IPropertyBag<TContainer> Bag;
    //    // propPath 
    //    public PropertyBagContext(IWatchContext parent, string relativePath, TContainer container, ContextFieldInfo fieldInfo)
    //        : base(parent
    //              , relativePath
    //              , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(TContainer)))
    //        )
    //    {
    //        Container = container;
    //        Bag = PropertyBagResolver.Resolve<TContainer>();
    //    }
    //
    //    public override bool TryParse(string input, ref RangeInt cursor, out IWatchContext ctx)
    //    {
    //        var c = cursor;
    //        if (ParserUtils.TryExtractPropertyPathPart(input, ref c, out var partRange, out var identifierRange))
    //        {
    //            var visitor = new TryParseVisitor()
    //            {
    //                Parent = this,
    //                Root = this,
    //                Container = Container,
    //                PropertyIdentifier = input.Substring(identifierRange),
    //                PropertyPart = input.Substring(partRange),
    //                RootPath = PropPath == null ? new PropertyPath() : PropPath
    //            };
    //            if (identifierRange.length == 0)
    //            {
    //                if (PropPath == null || PropPath.PartsCount == 0)
    //                {
    //                    ctx = default;
    //                    return false;
    //                }
    //                visitor.PropertyIdentifier = PropPath[PropPath.PartsCount - 1].Name;
    //            }
    //            PropertyUtils.VisitAtPath(ref Container, PropPath, visitor);
    //            if (visitor.Result != null)
    //            {
    //                cursor = c;
    //                ctx = visitor.Result;
    //                return true;
    //            }
    //        }
    //        ctx = default;
    //        return false;
    //    }
    //
    //    class TryParseVisitor : IPropertyVisitor
    //    {
    //        public IWatchContext Parent;
    //        public IWatchContext Root;
    //        public TContainer Container;
    //        public string PropertyIdentifier;
    //        public string PropertyPart;
    //        public PropertyPath RootPath;
    //        public IWatchContext Result;
    //        VisitStatus IPropertyVisitor.VisitProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
    //        {
    //            var propName = property.GetName();
    //            if (propName != PropertyIdentifier) return VisitStatus.Handled;
    //            if (Result != null) return VisitStatus.Handled;
    //            Result = new PropertyPathContext<TContainer, TValue2>(Parent, Root, ParserUtils.MakePathRelative(PropertyPart), ref Container, RootPath.Appended(PropertyPart), ContextFieldInfo.Make(PropertyPart));
    //            return VisitStatus.Handled;
    //        }
    //
    //        VisitStatus IPropertyVisitor.VisitCollectionProperty<TProperty2, TContainer2, TValue2>(TProperty2 property, ref TContainer2 container, ref ChangeTracker changeTracker)
    //        {
    //            var propName = property.GetName();
    //            if (propName != PropertyIdentifier) return VisitStatus.Handled;
    //            if (Result != null) return VisitStatus.Handled;
    //            Result = new PropertyPathContext<TContainer, TValue2>(Parent, Root, ParserUtils.MakePathRelative(PropertyPart), ref Container, RootPath.AppendedIndexer(PropertyPart), ContextFieldInfo.MakeOperator(PropertyPart));
    //            return VisitStatus.Handled;
    //        }
    //    }
    //
    //    public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
    //    {
    //        var visitor2 = new PropertyUtils.VisitMembersVisitor(visitor);
    //        var ct = new ChangeTracker();
    //        Bag.Accept(ref Container, ref visitor2, ref ct);
    //        //return PropertyUtils.VisitMembersAtPath(ref Container, PropPath, visitor);
    //        return visitor2.Result;
    //    }
    //
    //    public override bool TryCreateWatch(out IWatch watch)
    //    {
    //        watch = default;
    //        return false;
    //        //watch = new PropertyWatch<TContainer, TPropertyType>(RootPathContext, Container, PropPath);
    //        //return true;
    //    }
    //
    //    //public override bool TryCreateWatch(string path, RangeInt pathRange, out IWatch watch)
    //    //{
    //    //    watch = null;
    //    //    return false;
    //    //}
    //
    //}
}