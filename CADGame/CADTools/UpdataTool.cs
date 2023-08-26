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
    public class UpdateTool
    {
        public static void UpdateScreenEx(Entity ent = null)
        {
            ent?.Draw();//图元刷新
            ent?.RecordGraphicsModified(true);//图块刷新
            if (ent is Dimension dim)//标注刷新
                dim.RecomputeDimensionBlock(true);
            Application.UpdateScreen();//和ed.UpdateScreen();//底层实现差不多
                                       //acad2014及以上要加,立即处理队列上面的消息
            System.Windows.Forms.Application.DoEvents();
        }
        public static void UpdateScreenEx(Database db, ObjectId entId)
        {
            Entity ent = EditEntityTool.GetEntity(db, entId) as Entity;
            UpdateScreenEx(ent);
        }
    }
}
