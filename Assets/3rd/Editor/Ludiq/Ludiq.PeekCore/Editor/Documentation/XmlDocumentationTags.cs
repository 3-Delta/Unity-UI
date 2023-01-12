using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Ludiq.PeekCore
{
	public class XmlDocumentationTags
	{
		public XmlDocumentationTags()
		{
			parameterTypes = new Dictionary<string, Type>();
			parameters = new Dictionary<string, string>();
			typeParameters = new Dictionary<string, string>();
		}

		public XmlDocumentationTags(string summary) : this()
		{
			this.summary = summary;
		}

		public XmlDocumentationTags(XElement xml) : this()
		{
			foreach (var childNode in xml.Elements())
			{
				if (childNode.Name.LocalName == "summary")
				{
					summary = ProcessText(GetElementText(childNode));
				}
				else if (childNode.Name.LocalName == "returns")
				{
					returns = ProcessText(GetElementText(childNode));
				}
				else if (childNode.Name.LocalName == "remarks")
				{
					remarks = ProcessText(GetElementText(childNode));
				}
				else if (childNode.Name.LocalName == "param")
				{
					var paramText = ProcessText(GetElementText(childNode));

					var nameAttribute = childNode.Attribute("name");

					if (paramText != null && nameAttribute != null)
					{
						if (parameters.ContainsKey(nameAttribute.Value))
						{
							parameters[nameAttribute.Value] = paramText;
						}
						else
						{
							parameters.Add(nameAttribute.Value, paramText);
						}
					}
				}
				else if (childNode.Name.LocalName == "typeparam")
				{
					var typeParamText = ProcessText(GetElementText(childNode));

					var nameAttribute = childNode.Attribute("name");

					if (typeParamText != null && nameAttribute != null)
					{
						if (typeParameters.ContainsKey(nameAttribute.Value))
						{
							typeParameters[nameAttribute.Value] = typeParamText;
						}
						else
						{
							typeParameters.Add(nameAttribute.Value, typeParamText);
						}
					}
				}
				else if (childNode.Name.LocalName == "inheritdoc")
				{
					inherit = true;
				}
			}
		}

		private bool methodBaseCompleted = true;

		[Serialize]
		public string summary;

		[Serialize]
		public string returns;

		[Serialize]
		public string remarks;
		
		[Serialize]
		public bool inherit;

		[Serialize]
		public Dictionary<string, string> parameters ;

		[Serialize]
		public Dictionary<string, string> typeParameters;

		[DoNotSerialize]
		public Dictionary<string, Type> parameterTypes;
		
		[DoNotSerialize]
		public Type returnType;

		public void CompleteWithMethodBase(MethodBase methodBase)
		{
			if (methodBaseCompleted)
			{
				return;
			}

			var parameterInfos = methodBase.GetParameters();

			foreach (var parameterInfo in parameterInfos)
			{
				parameterTypes.Add(parameterInfo.Name, parameterInfo.ParameterType);
			}

			// Remove parameter summaries if no matching parameter is found.
			// (Happens frequently in Unity methods)
			foreach (var parameter in parameters.ToArray())
			{
				if (parameterInfos.All(p => p.Name != parameter.Key))
				{
					parameters.Remove(parameter.Key);
				}
			}

			if (methodBase is MethodInfo methodInfo)
			{
				returnType = methodInfo.ReturnType;
			}

			methodBaseCompleted = true;
		}

		public string ParameterSummary(ParameterInfo parameter)
		{
			if (parameters.ContainsKey(parameter.Name))
			{
				return parameters[parameter.Name];
			}

			return null;
		}

		private static string ProcessText(string xmlText)
		{
			xmlText = string.Join(" ", xmlText.Trim().Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries));

			if (xmlText == string.Empty)
			{
				return null;
			}

			return xmlText;
		}

		private static Regex genericArityRegex = new Regex(@"(`[0-9]+)", RegexOptions.Compiled);

		private static string SimplifyCref(string cref)
		{
			// This link has the real details on the `<see cref="...">` format:
			// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/processing-the-xml-file
			//
			// We cut some corners to parse these easier, and just get a nice-enough name out of them.
			if (cref.Contains(":"))
			{
				cref = cref.Split(new[] { ':' }, 2)[1];

				if (cref.Contains(".#ctor"))
				{
					cref = cref.Replace(".#ctor", "");
				}

				cref = genericArityRegex.Replace(cref, "");
			}

			return cref;
		}

		private static string GetElementText(XElement element)
		{
			var sb = new StringBuilder();
			AppendElementText(sb, element);
			return sb.ToString();
		}

		private static void AppendElementText(StringBuilder sb, XElement parentElement)
		{
			foreach (var childNode in parentElement.Nodes())
			{
				if (childNode is XElement childElement)
				{
					if (childElement.Name.LocalName == "see")
					{
						foreach (var attribute in childElement.Attributes())
						{
							if (attribute.Name.LocalName == "cref")
							{
								sb.Append(SimplifyCref(attribute.Value));
							}
						}
					}
					else
					{
						AppendElementText(sb, childElement);
					}
				}
				else if (childNode is XText childText)
				{
					sb.Append(childText.Value);
				}
			}
		}
	}
}