using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class PluginConfigurationItemAccessor : Accessor
	{
		public enum Mode
		{
			Field,
			Property
		}

		protected PluginConfigurationItemAccessor(PluginConfiguration configuration, MemberInfo member, Accessor parent) : base(member, parent)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			if (!(member is FieldInfo || member is PropertyInfo))
			{
				throw new ArgumentException($"Configuration item '{member.Name}' is not a field or property.", nameof(member));
			}

			if (!member.DeclaringType.IsInstanceOfType(configuration))
			{
				throw new ArgumentException($"Configuration item '{member.Name}' is not declared in '{configuration.GetType().CSharpName()}'.", nameof(member));
			}

			this.configuration = configuration;
			this.member = member;

			attribute = member.GetAttribute<PluginConfigurationItemAttribute>();
				
			if (attribute == null)
			{
				throw new ArgumentException($"Configuration item '{member.Name}' is missing the [{nameof(PluginConfigurationItemAttribute)}].", nameof(member));
			}

			key = attribute.key ?? member.Name;
				
			if (member is FieldInfo)
			{
				mode = Mode.Field;
				fieldInfo = (FieldInfo)member;
				definedType = fieldInfo.FieldType;
				defaultValue = value.CloneViaSerializationPolicy();
			}
			else // if (memberInfo is PropertyInfo)
			{
				mode = Mode.Property;
				propertyInfo = (PropertyInfo)member;
				definedType = propertyInfo.PropertyType;
				defaultValue = value.CloneViaSerializationPolicy();
			}
					
			label = new GUIContent(member.HumanName(), member.Summary());

			XmlDocumentation.onLoaded += () =>
			{
				label.tooltip = member.Summary();
			};
					
			if (exists)
			{
				Load();
			}
			else
			{
				value = defaultValue;
			}
		}

		public MemberInfo member { get; }
		private FieldInfo fieldInfo { get; }
		private PropertyInfo propertyInfo { get; }
		private Mode mode { get; }

		private PluginConfigurationItemAttribute attribute { get; }

		public string key { get; }
		public object defaultValue { get; set; }
		public PluginConfiguration configuration { get; }

		public bool visible => AttributeUtility.CheckCondition(configuration, attribute.visibleCondition, attribute.visible);
		public bool editable => AttributeUtility.CheckCondition(configuration, attribute.editableCondition, attribute.editable);
		public bool resettable => AttributeUtility.CheckCondition(configuration, attribute.resettableCondition, attribute.resettable);

		public override bool isEditable => editable;

		public abstract bool exists { get; }

		protected override string Subpath()
		{
			return member.Name;
		}

		protected override object rawValue
		{
			// Small optimization:
			// To reduce initialization time, we'll use standard
			// reflection for the value if the plugin container isn't
			// initialized yet, which makes sure we don't do an 
			// expensive caching or code emission in the first frame.

			get
			{
				switch (mode)
				{
					case Mode.Field:
						if (PluginContainer.initialized)
						{
							return fieldInfo.GetValueOptimized(configuration);
						}
						else
						{
							return fieldInfo.GetValue(configuration);
						}

					case Mode.Property:
						if (PluginContainer.initialized)
						{
							return propertyInfo.GetValueOptimized(configuration);
						}
						else
						{
							return propertyInfo.GetValue(configuration, null);
						}

					default:
						throw new UnexpectedEnumValueException<Mode>(mode);
				}
			}
			set
			{
				switch (mode)
				{
					case Mode.Field:
						if (PluginContainer.initialized)
						{
							fieldInfo.SetValueOptimized(configuration, value);
						}
						else
						{
							fieldInfo.SetValue(configuration, value);
						}

						break;

					case Mode.Property:
						if (PluginContainer.initialized)
						{
							propertyInfo.SetValueOptimized(configuration, value);
						}
						else
						{
							propertyInfo.SetValue(configuration, value, null);
						}

						break;

					default:
						throw new UnexpectedEnumValueException<Mode>(mode);
				}
			}
		}

		public abstract void Load();

		public abstract void Save();

		public void Reset(bool force = false)
		{
			if (!resettable && !force)
			{
				return;
			}

			value = defaultValue.CloneViaSerializationPolicy();
		}

		public override Attribute[] GetCustomAttributes(bool inherit = true)
		{
			return Attribute.GetCustomAttributes(member, inherit);
		}
	}
}