#region
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
using System.Threading;
#endregion

namespace CADGame
{
    public class Main
    {
        [CommandMethod("helloCAD")]
        public static void helloCAD()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ed.WriteMessage("helloCAD");
        }

        [CommandMethod("tooltest")]
        public static void tooltest()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId circleId = db.AddCircleToModelSpace(new Point3d(0, 0, 0),200);
            HatchTool.HatchGradient(db, HatchTool.HatchGradientPattern.GR_LINEAR, 100, 100, circleId, 0);
            db.AddLineToModelSpace(new Point3d(0, 0, 0), new Point3d(300, 300, 0));
            db.AddRectToModelSpace(new Point2d(100, 100), new Point2d(300, 500));
        }

        [CommandMethod("Tetris")]
        public static void Tetris()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            double lineLength = 10;
            Point3d center = new Point3d(0, 0, 0);
            double factor = 0.6;

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    center = new Point3d(i * lineLength, j * lineLength, 0);
                    ObjectId innerRecId = db.AddRectToModelSpace(new Point2d(center.X - (factor / 2) * lineLength, center.Y - (factor / 2) * lineLength), new Point2d(center.X + (factor / 2) * lineLength, center.Y + (factor / 2) * lineLength));
                    ObjectId outerRecId = db.AddRectToModelSpace(new Point2d(center.X - lineLength / 2, center.Y - lineLength / 2), new Point2d(center.X + lineLength / 2, center.Y + lineLength / 2));
                    ObjectId line1 = db.AddLineToModelSpace(new Point3d(center.X - lineLength / 2, center.Y - lineLength / 2, 0), new Point3d(center.X - (factor / 2) * lineLength, center.Y - (factor / 2) * lineLength, 0));
                    ObjectId line2 = db.AddLineToModelSpace(new Point3d(center.X - lineLength / 2, center.Y + lineLength / 2, 0), new Point3d(center.X - (factor / 2) * lineLength, center.Y + (factor / 2) * lineLength, 0));
                    ObjectId line3 = db.AddLineToModelSpace(new Point3d(center.X + (factor / 2) * lineLength, center.Y + (factor / 2) * lineLength, 0), new Point3d(center.X + lineLength / 2, center.Y + lineLength / 2, 0));
                    ObjectId line4 = db.AddLineToModelSpace(new Point3d(center.X + (factor / 2) * lineLength, center.Y - (factor / 2) * lineLength, 0), new Point3d(center.X + lineLength / 2, center.Y - lineLength / 2, 0));
                    ObjectId hatch = HatchTool.HatchGradient(db, HatchTool.HatchGradientPattern.GR_LINEAR, 100, 100, innerRecId, 45);
                    UpdateTool.UpdateScreenEx(db, innerRecId);
                    UpdateTool.UpdateScreenEx(db, outerRecId);
                    UpdateTool.UpdateScreenEx(db, line1);
                    UpdateTool.UpdateScreenEx(db, line2);
                    UpdateTool.UpdateScreenEx(db, line3);
                    UpdateTool.UpdateScreenEx(db, line4);
                    UpdateTool.UpdateScreenEx(db, hatch);
                    Thread.Sleep(100);

                }
            }
        }

    }
}
