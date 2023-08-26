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
    public static class MyBlockTableRecord
    {
        private static string block1Name = "Block1";
        private static List<Entity> block1Ents = new List<Entity>();
        private static ObjectId block1Id = ObjectId.Null;

        public static string Block1Name { get => block1Name; set => block1Name = value; }
        public static List<Entity> Block1Ents
        {
            get
            {
                Circle circle = new Circle(Point3d.Origin, Vector3d.ZAxis, 10);
                Line line1 = new Line(new Point3d(-10, 0, 0), new Point3d(10, 0, 0));
                Line line2 = new Line(new Point3d(0, 10, 0), new Point3d(0, -10, 0));
                block1Ents.Add(circle);
                block1Ents.Add(line1);
                block1Ents.Add(line2);
                return MyBlockTableRecord.block1Ents;
            }
            set { }
        }
        public static ObjectId Block1Id { get => block1Id; set => block1Id = value; }
    }
}
