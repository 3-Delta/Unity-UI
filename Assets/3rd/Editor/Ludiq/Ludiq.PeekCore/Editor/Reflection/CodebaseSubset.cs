using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore
{
	public sealed class CodebaseSubset
	{
		public CodebaseSubset(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
		{
			Ensure.That(nameof(types)).IsNotNull(types);
			Ensure.That(nameof(memberFilter)).IsNotNull(memberFilter);

			this.types = types.ToHashSet();
			this.memberFilter = memberFilter;
			this.memberTypeFilter = memberTypeFilter;

			Filter();
		}

		public CodebaseSubset(IEnumerable<Type> typeSet, TypeFilter typeFilter, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
		{
			Ensure.That(nameof(typeSet)).IsNotNull(typeSet);
			Ensure.That(nameof(typeFilter)).IsNotNull(typeFilter);
			Ensure.That(nameof(memberFilter)).IsNotNull(memberFilter);

			this.typeSet = typeSet;
			this.typeFilter = typeFilter;
			this.memberFilter = memberFilter;
			this.memberTypeFilter = memberTypeFilter;

			Filter();
		}

		public IEnumerable<Type> typeSet { get; }
		public TypeFilter typeFilter { get; }
		public MemberFilter memberFilter { get; }
		public TypeFilter memberTypeFilter { get; }
		public HashSet<Type> types { get; private set; }
		public HashSet<Member> members { get; private set; }

		private void Filter()
		{
			types = typeSet.WhereTask("Filtering types...", typeFilter.ValidateType);
			members = types.SelectManyTask("Filtering members...", true, null, FilterMembers);
		}

		public bool ContainsType(Type type)
		{
			return types.Contains(type);
		}

		public bool ContainsMember(Member member)
		{
			return members.Contains(member);
		}

		private IEnumerable<Member> FilterMembers(Type type)
		{
			if (!type.SupportsMembers())
			{
				yield break;
			}

			if (memberFilter.Operators)
			{
				foreach (var member in type.GetOperators()
										   .Where(member => member.DeclaringType == type && memberFilter.validMemberTypes.HasFlag(member.MemberType) && memberFilter.ValidateMember(member, memberTypeFilter))
									       .Select(member => member.ToMember(type)))
				{
					yield return member;
				}
			}

			foreach (var member in type.GetMembers(memberFilter.validBindingFlags)
									   .Where(member => memberFilter.validMemberTypes.HasFlag(member.MemberType) && memberFilter.ValidateMember(member, memberTypeFilter) && !member.IsExtensionMethod())
									   .Select(member => member.ToMember(type)))
			{
				yield return member;
			}

			if (memberFilter.Methods && memberFilter.Extensions)
			{
				foreach (var member in type.GetExtensionMethods()
										   .Where(method => ContainsType(method.DeclaringType) && memberFilter.ValidateMember(method, memberTypeFilter))
										   .Select(member => member.ToMember(type)))
				{
					yield return member;
				}
			}
		}
	}
}