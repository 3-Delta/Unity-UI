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
    [NodeDesc("Actions:Kaclok", "AI_Patrol")]
    public class Patrol : Behaviac.Design.Nodes.Node
    {
        public Patrol() : base("AI_巡逻", "巡逻") { }
        public override string ExportClass { get { return "Patrol"; } }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);
            Patrol t = newnode as Patrol;
            if (t != null)
            {
                t.r = r;
                t.speedX = speedX;
                t.speedY = speedY;
                t.speedZ = speedZ;
                t.accelerateX = accelerateX;
                t.accelerateY = accelerateY;
                t.accelerateZ = accelerateZ;
            }
        }
        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            base.CheckForErrors(rootBehavior, result);
        }

        // 属性区域
        // 半径R
        private RightValueDef _r = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("r", "r", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef r
        {
            get { return _r; }
            set { _r = value; }
        }
        // 速度
        private RightValueDef _speedX = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("speedX", "speedX", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef speedX
        {
            get { return _speedX; }
            set { _speedX = value; }
        }
        private RightValueDef _speedY = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("speedY", "speedY", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 5, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef speedY
        {
            get { return _speedY; }
            set { _speedY = value; }
        }
        private RightValueDef _speedZ = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("speedZ", "speedZ", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 7, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef speedZ
        {
            get { return _speedZ; }
            set { _speedZ = value; }
        }
        // 加速度
        private RightValueDef _accelerateX = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("accelerateX", "accelerateX", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 9, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef accelerateX
        {
            get { return _accelerateX; }
            set { _accelerateX = value; }
        }
        private RightValueDef _accelerateY = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("accelerateY", "accelerateY", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 11, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef accelerateY
        {
            get { return _accelerateY; }
            set { _accelerateY = value; }
        }
        private RightValueDef _accelerateZ = new RightValueDef(new VariableDef(0f));
        [DesignerRightValueEnum("accelerateZ", "accelerateZ", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 13, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef accelerateZ
        {
            get { return _accelerateZ; }
            set { _accelerateZ = value; }
        }
    }
}