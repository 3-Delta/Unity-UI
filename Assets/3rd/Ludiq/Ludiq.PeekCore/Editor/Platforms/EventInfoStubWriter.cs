using System;
using System.Collections.Generic;
using System.Reflection;
using Ludiq.PeekCore.CodeDom;
using Ludiq.PeekCore;

[assembly: RegisterAotStubWriter(typeof(EventInfo), typeof(EventInfoStubWriter))]

namespace Ludiq.PeekCore
{
	public sealed class EventInfoStubWriter : MemberInfoStubWriter<EventInfo>
	{
		public EventInfoStubWriter(EventInfo eventInfo) : base(eventInfo) { }

		protected override bool supportsOptimization => false;
		
		public override IEnumerable<CodeStatement> GetStubStatements()
		{
			/* 
			 * Required output:
			 * 1. Create a target variable
			 * 2. Add a hook to a dummy event to prevent stripping
			 * 2. Remove the hook to the dummy event to prevent stripping
			*/

			var targetType = Code.TypeRef(manipulator.targetType, true);
			var handlerType = Code.TypeRef(manipulator.type, true);

			CodeExpression target;

			if (manipulator.requiresTarget)
			{
				// 1. Player target = default(Player);
				yield return Code.VarDecl(targetType, "target", targetType.DefaultValue());

				target = Code.VarRef("target");
			}
			else
			{
				target = targetType.Expression();
			}

			// target.onKill
			var eventReference = target.Field(manipulator.name);
			
			if (manipulator.isPubliclyHookable)
			{
				// 2. target.onKill += default(Action);
				yield return eventReference.AddAssign(handlerType.DefaultValue()).Statement();

				// 3. target.onKill -= default(Action);
				yield return eventReference.SubtractAssign(handlerType.DefaultValue()).Statement();
			}
		}
	}
}