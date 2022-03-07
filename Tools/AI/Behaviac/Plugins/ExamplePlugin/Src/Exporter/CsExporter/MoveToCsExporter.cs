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
    public class MoveToCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            MoveTo t = node as MoveTo;
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
            MoveTo t = node as MoveTo;

            stream.WriteLine("{0}\t\tprotected override float GetPositionX(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            string retStr = RightValueCsExporter.GenerateCode(node, t.positionX, stream, indent + "\t\t\t", string.Empty, string.Empty, "positionX");
            if (!t.positionX.IsPublic && (t.positionX.IsMethod || t.positionX.Var != null && t.positionX.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetPositionY(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.positionY, stream, indent + "\t\t\t", string.Empty, string.Empty, "positionY");
            if (!t.positionY.IsPublic && (t.positionY.IsMethod || t.positionY.Var != null && t.positionY.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetPositionZ(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.positionZ, stream, indent + "\t\t\t", string.Empty, string.Empty, "positionZ");
            if (!t.positionZ.IsPublic && (t.positionZ.IsMethod || t.positionZ.Var != null && t.positionZ.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetSpeed(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.speed, stream, indent + "\t\t\t", string.Empty, string.Empty, "speed");
            if (!t.speed.IsPublic && (t.speed.IsMethod || t.speed.Var != null && t.speed.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetAccelerate(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.accelerate, stream, indent + "\t\t\t", string.Empty, string.Empty, "accelerate");
            if (!t.accelerate.IsPublic && (t.accelerate.IsMethod || t.accelerate.Var != null && t.accelerate.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
