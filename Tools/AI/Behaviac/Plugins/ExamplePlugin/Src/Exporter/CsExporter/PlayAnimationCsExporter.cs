using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using PluginBehaviac.DataExporters;
using PluginBehaviac.NodeExporters;
using ExamplePlugin.Nodes;

namespace ExamplePlugin.NodeExporters
{
    public class PlayAnimationCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            PlayAnimation t = node as PlayAnimation;
            return (t != null);
        }
        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);
        }
        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);
        }
        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);
        }
    }
}
