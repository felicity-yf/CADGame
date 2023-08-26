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
    public class LineJig : EntityJig
    {
        private Point3d jStartPoint; //直线的起点
        private Point3d jEndPoint;  //直线的终点
        private string jPromptString;  //提示信息
        private string[] jKeywords;   //交互关键字

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="jPromptStr">提示信息</param>
        /// <param name="keywords">交互关键字</param>
        public LineJig(Point3d startPoint, string jPromptStr, string[] keywords) : base(new Line())
        {
            jStartPoint = startPoint;
            ((Line)Entity).StartPoint = jStartPoint;
            jPromptString = jPromptStr;
            jKeywords = keywords;

        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            //声明提示信息类
            JigPromptPointOptions jppo = new JigPromptPointOptions(jPromptString);
            //添加关键字
            for (int i = 0; i < jKeywords.Length; i++)
            {
                jppo.Keywords.Add(jKeywords[i]);
            }
            char space = (char)32;
            jppo.Keywords.Add(space.ToString());
            //设置获取信息类型
            jppo.UserInputControls = UserInputControls.Accept3dCoordinates;
            //取消系统自动添加关键字提示信息
            jppo.AppendKeywordsToMessage = false;
            //jppo.Cursor = CursorType.RubberBand;
            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            jEndPoint = ppr.Value;
            return SamplerStatus.NoChange;

        }

        protected override bool Update()
        {
            ((Line)Entity).EndPoint = jEndPoint;
            return true;
        }
        /// <summary>
        /// 返回图形对象
        /// </summary>
        /// <returns>Entity</returns>
        public Entity GetEntity()
        {
            return Entity;
        }
    }
}
