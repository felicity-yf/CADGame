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
    public static class FoodBrick
    {
        private static string blockName = "FoodBrick";
        private static List<Entity> blockEnts = new List<Entity>(); //用于保存Brick中的各图元
        private static ObjectId blockId = ObjectId.Null;
        private static double lineLength = 10;
        private static double factor = 0.6;
        private static short colorIndex = 10;

        public static string BlockName { get => blockName; set => blockName = value; }
        public static List<Entity> BlockEnts
        {
            get
            {
                Polyline outLine = new Polyline();
                Polyline inLine = new Polyline();
                List<Line> edgeLine = new List<Line>();
                double setp = lineLength / 100;
                for (double i = -0.5 * factor * lineLength; i < 0.5 * factor * lineLength; i += setp)
                {
                    Line line = new Line(new Point3d(i, 0.5 * factor * lineLength, 0), new Point3d(i, -0.5 * factor * lineLength, 0));
                    line.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex);
                    blockEnts.Add(line);
                }
                for (double i = -0.5 * factor * lineLength; i < 0.5 * factor * lineLength; i += setp)
                {
                    Line line = new Line(new Point3d(0.5 * factor * lineLength, i, 0), new Point3d(-0.5 * factor * lineLength, i, 0));
                    line.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex);
                    blockEnts.Add(line);
                }

                outLine.AddVertexAt(0, new Point2d(-0.5 * lineLength, -0.5 * lineLength), 0, 0, 0);
                outLine.AddVertexAt(1, new Point2d(-0.5 * lineLength, 0.5 * lineLength), 0, 0, 0);
                outLine.AddVertexAt(2, new Point2d(0.5 * lineLength, 0.5 * lineLength), 0, 0, 0);
                outLine.AddVertexAt(3, new Point2d(0.5 * lineLength, -0.5 * lineLength), 0, 0, 0);
                outLine.Closed = true;

                inLine.AddVertexAt(0, new Point2d(-0.5 * factor * lineLength, -0.5 * factor * lineLength), 0, 0, 0);
                inLine.AddVertexAt(1, new Point2d(-0.5 * factor * lineLength, 0.5 * factor * lineLength), 0, 0, 0);
                inLine.AddVertexAt(2, new Point2d(0.5 * factor * lineLength, 0.5 * factor * lineLength), 0, 0, 0);
                inLine.AddVertexAt(3, new Point2d(0.5 * factor * lineLength, -0.5 * factor * lineLength), 0, 0, 0);
                inLine.Closed = true;

                edgeLine.Add(new Line(new Point3d(-0.5 * lineLength, -0.5 * lineLength, 0), new Point3d(-0.5 * factor * lineLength, -0.5 * factor * lineLength, 0)));
                edgeLine.Add(new Line(new Point3d(-0.5 * lineLength, 0.5 * lineLength, 0), new Point3d(-0.5 * factor * lineLength, 0.5 * factor * lineLength, 0)));
                edgeLine.Add(new Line(new Point3d(0.5 * lineLength, 0.5 * lineLength, 0), new Point3d(0.5 * factor * lineLength, 0.5 * factor * lineLength, 0)));
                edgeLine.Add(new Line(new Point3d(0.5 * lineLength, -0.5 * lineLength, 0), new Point3d(0.5 * factor * lineLength, -0.5 * factor * lineLength, 0)));

                blockEnts.Add(outLine);
                blockEnts.Add(inLine);
                for (int i = 0; i < edgeLine.Count; i++)
                {
                    blockEnts.Add(edgeLine[i]);
                }
                return FoodBrick.blockEnts;
            }
            set { }
        }

        public static ObjectId BlockId { get => blockId; set => blockId = value; }
    }
}
