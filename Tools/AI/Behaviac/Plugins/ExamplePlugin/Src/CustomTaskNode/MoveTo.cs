using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using ExamplePlugin.Properties;
using Behaviac.Design.Nodes;

namespace ExamplePlugin.Nodes
{
    // 关于自定义Behaviac的task节点的文档
    // http://www.behaviac.com/tutorial9_extendnodes/
    [NodeDesc("Actions:Kaclok", "AI_MoveTo")]
    public class MoveTo : Behaviac.Design.Nodes.Node
    {
        public MoveTo() : base("AI_移动到", "移动到") { }
        public override string ExportClass { get { return "MoveTo"; } }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);
            MoveTo t = newnode as MoveTo;
            if (t != null)
            {
                t.positionX = positionX;
                t.positionY = positionY;
                t.positionZ = positionZ;
                t.speed = speed;
                t.accelerate = accelerate;
            }
        }
        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            base.CheckForErrors(rootBehavior, result);
        }

        // 属性区域
        // 位置
        private RightValueDef _positionX = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("positionX", "positionX", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef positionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        private RightValueDef _positionY = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("positionY", "positionY", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef positionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        private RightValueDef _positionZ = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("positionZ", "positionZ", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 5, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef positionZ
        {
            get { return _positionZ; }
            set { _positionZ = value; }
        }
        // 速度
        private RightValueDef _speed = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("speed", "speed", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 7, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        // 加速度
        private RightValueDef _accelerate = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("accelerate", "accelerate", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 13, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef accelerate
        {
            get { return _accelerate; }
            set { _accelerate = value; }
        }
    }
}