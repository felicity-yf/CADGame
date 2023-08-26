using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// acdbmgd.dll 包含对ObjectDBXtmAPI的封装。用于在图形文件中对对象进行操作
using Autodesk.AutoCAD.DatabaseServices;//(Database,DBPoint,Line,Spline)
using Autodesk.AutoCAD.Runtime; //(CommandMethodAttribute,RXObject,CommandFlag)
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Geometry; //(Point3d,Line3d,Curve3d)
// using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;


using Autodesk.AutoCAD.ApplicationServices; // (Application,Document)
using Autodesk.AutoCAD.EditorInput;// (Editor,PromptXOptions,PromptXResult)
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Windows.ToolPalette;
using Autodesk.AutoCAD.Internal.Windows;
using Autodesk.AutoCAD.Internal.Forms;

namespace CADGame
{
    public class CircleJig : EntityJig
    {

        private double jRadius;
        public CircleJig(Point3d center) : base(new Circle())
        {
            ((Circle)Entity).Center = center;
        }

        /// <summary>
        /// 当鼠标在屏幕上移动时被调用,用于改变图形的属性
        /// </summary>
        /// <param name="prompts"></param>
        /// <returns></returns>
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            //声明jig提示信息
            JigPromptPointOptions jppo = new JigPromptPointOptions("\n请指定圆上的一个点:");
            char space = (char)32;
            jppo.Keywords.Add("U");
            jppo.Keywords.Add(space.ToString());
            jppo.UserInputControls = UserInputControls.Accept3dCoordinates;
            jppo.Cursor = CursorType.RubberBand;  //显示圆心到当前点的虚线            
            jppo.BasePoint = ((Circle)Entity).Center;
            jppo.UseBasePoint = true;
            //获取拖拽时鼠标的位置状态
            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            jRadius = ppr.Value.GetDistanceBetweenTwoPoint(((Circle)Entity).Center);
            return SamplerStatus.NoChange;
        }

        /// <summary>
        /// 用于改新图形对象，不用事务处理
        /// </summary>
        /// <returns></returns>
        protected override bool Update()
        {
            //动态更新圆的半径
            if (jRadius > 0)
            {
                ((Circle)Entity).Radius = jRadius;
                return true;
            }
            return false;
        }

        public Entity GetEntity()
        {
            return Entity;
        }
    }
}
