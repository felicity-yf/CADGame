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
    public class MoveJig : DrawJig
    {
        private List<Entity> jEntList;
        private Point3d jPointBase;
        private Point3d jPointPre;
        Matrix3d jMt = Matrix3d.Displacement(new Vector3d(0, 0, 0));
        public MoveJig(List<Entity> entList, Point3d pointBase)
        {
            jEntList = entList;
            jPointBase = pointBase;
            jPointPre = pointBase;
        }
        /// <summary>
        /// 重绘图形
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            for (int i = 0; i < jEntList.Count; i++)
            {
                draw.Geometry.Draw(jEntList[i]);
            }
            return true;
        }

        /// <summary>
        /// 获取鼠标在屏幕的运动，需要更新图形对象的属性
        /// </summary>
        /// <param name="prompts"></param>
        /// <returns></returns>
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //声明一个指示类
            JigPromptPointOptions jppo = new JigPromptPointOptions("\n 指定第二个点或<使用第一个点作为位移>:");
            jppo.Cursor = CursorType.RubberBand;
            jppo.BasePoint = jPointBase;
            jppo.UseBasePoint = true;
            jppo.Keywords.Add(" ");
            jppo.AppendKeywordsToMessage = false;

            jppo.UserInputControls = UserInputControls.Accept3dCoordinates;
            PromptPointResult ppr = prompts.AcquirePoint(jppo);
            Point3d curPoint = ppr.Value;

            //对图形对象进行矩阵变换
            if (curPoint != jPointPre)
            {
                Vector3d vector = jPointPre.GetVectorTo(curPoint);
                jMt = Matrix3d.Displacement(vector);
                for (int i = 0; i < jEntList.Count; i++)
                {
                    jEntList[i].TransformBy(jMt);
                }
            }

            jPointPre = curPoint;

            return SamplerStatus.NoChange;
        }

        public List<Entity> GetEntityList()
        {
            return jEntList;
        }
    }
}
