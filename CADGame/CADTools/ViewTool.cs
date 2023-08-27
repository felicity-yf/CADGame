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
    static class ViewTool
    {
        /// <summary>
        /// 给定观察点与边界值，进行视图显示
        /// </summary>
        /// <param name="vp">观察点</param>
        /// <param name="height">长度</param>
        /// <param name="width">宽度</param>
        public static void VPoint(this Editor ed,Database db, Point3d vp, Double height, Double width)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                ViewTable vt = (ViewTable)trans.GetObject(db.ViewTableId, OpenMode.ForWrite);
                ViewTableRecord vtr = new ViewTableRecord();
                if (vt.Has("tempView"))
                {
                    vtr = (ViewTableRecord)trans.GetObject(vt["tempView"], OpenMode.ForWrite);
                }
                else
                {
                    vtr.Name = "tempView";
                    vt.Add(vtr);
                    trans.AddNewlyCreatedDBObject(vtr, true);
                }
                vtr.CenterPoint = new Point2d(vp.X, vp.Y);
                vtr.Height = height;
                vtr.Width = width;
                trans.Commit();
                ed.SetCurrentView(vtr);
            }
        }
    }
}
