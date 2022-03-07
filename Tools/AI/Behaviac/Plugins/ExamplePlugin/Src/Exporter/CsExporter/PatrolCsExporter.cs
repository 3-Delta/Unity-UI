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
    public class PatrolCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            Patrol t = node as Patrol;
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
            Patrol t = node as Patrol;

            stream.WriteLine("{0}\t\tprotected override float GetR(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            string retStr = RightValueCsExporter.GenerateCode(node, t.r, stream, indent + "\t\t\t", string.Empty, string.Empty, "r");
            if (!t.r.IsPublic && (t.r.IsMethod || t.r.Var != null && t.r.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetSpeedX(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.speedX, stream, indent + "\t\t\t", string.Empty, string.Empty, "speedX");
            if (!t.speedX.IsPublic && (t.speedX.IsMethod || t.speedX.Var != null && t.speedX.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetSpeedY(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.speedY, stream, indent + "\t\t\t", string.Empty, string.Empty, "speedY");
            if (!t.speedY.IsPublic && (t.speedY.IsMethod || t.speedY.Var != null && t.speedY.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetSpeedZ(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.speedZ, stream, indent + "\t\t\t", string.Empty, string.Empty, "speedZ");
            if (!t.speedZ.IsPublic && (t.speedZ.IsMethod || t.speedZ.Var != null && t.speedZ.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetAccelerateX(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.accelerateX, stream, indent + "\t\t\t", string.Empty, string.Empty, "accelerateX");
            if (!t.accelerateX.IsPublic && (t.accelerateX.IsMethod || t.accelerateX.Var != null && t.accelerateX.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetAccelerateY(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.accelerateY, stream, indent + "\t\t\t", string.Empty, string.Empty, "accelerateY");
            if (!t.accelerateY.IsPublic && (t.accelerateY.IsMethod || t.accelerateY.Var != null && t.accelerateY.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);

            stream.WriteLine("{0}\t\tprotected override float GetAccelerateZ(Agent pAgent)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            retStr = RightValueCsExporter.GenerateCode(node, t.accelerateZ, stream, indent + "\t\t\t", string.Empty, string.Empty, "accelerateZ");
            if (!t.accelerateZ.IsPublic && (t.accelerateZ.IsMethod || t.accelerateZ.Var != null && t.accelerateZ.Var.IsProperty))
            {
                retStr = string.Format("Convert.ToDouble({0})", retStr);
            }
            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
