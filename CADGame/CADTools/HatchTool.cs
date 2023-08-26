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
    public static class HatchTool
    {
        public struct HatchPattern
        {
            public static readonly string SOLID = "SOLID";
            public static readonly string ANGLE = "ANGLE";
            public static readonly string ANSI31 = "ANSI31";
            public static readonly string ANSI32 = "ANSI32";
            public static readonly string ANSI33 = "ANSI33";
            public static readonly string ANSI34 = "ANSI34";
            public static readonly string ANSI35 = "ANSI35";
            public static readonly string ANSI36 = "ANSI36";
            public static readonly string ANSI37 = "ANSI37";
            public static readonly string ANSI38 = "ANSI38";
            public static readonly string AR_B816 = "AR-B816";
            public static readonly string AR_B816C = "AR-B816C";
            public static readonly string AR_B88 = "AR-B88";
            public static readonly string AR_BRELM = "AR-BRELM";
            public static readonly string AR_BRSTD = "AR-BSTD";
            public static readonly string AR_CONC = "AR-CONC";
            public static readonly string AR_HBONE = "AR-PARQ1";
            public static readonly string AR_RROOF = "AR-RROOF";
            public static readonly string AR_RSHKE = "AR-RSHKE";
            public static readonly string AR_SAND = "AR-SAND";
            public static readonly string BOX = "BOX";
            public static readonly string BRASS = "BRASS";
            public static readonly string BRICK = "BRICK";
            public static readonly string BRSTONE = "BRTONE";
            public static readonly string CLAY = "CLAY";
            public static readonly string CORK = "CORK";
            public static readonly string CROSS = "CROSS";
            public static readonly string DASH = "DASH";
            public static readonly string DOLMIT = "DOLMIT";
            public static readonly string DOTS = "DOTS";
            public static readonly string EARTH = "EARTH";
            public static readonly string ESCHER = "ESCHER";
            public static readonly string FLEX = "FLEX";
            public static readonly string GOST_GLASS = "GOST_GLASS";
            public static readonly string GOST_GROUND = "GOST_GROUND";
            public static readonly string GOST_WOOD = "GOST_WOOD";
            public static readonly string GRASS = "GRASS";
            public static readonly string GRATE = "GRATE";
            public static readonly string GRAVEL = "GRAVEL";
            public static readonly string HEX = "HEX";
            public static readonly string HONEY = "HONEY";
            public static readonly string HOUND = "HOUND";
            public static readonly string INSUL = "INSUL";
            public static readonly string NET = "NET";
            public static readonly string NET3 = "NET3";
            public static readonly string STARS = "STARS";
        }

        public struct HatchGradientPattern
        {
            public static readonly string GR_LINEAR = "Linear";
            public static readonly string GR_CYLIN = "Cylinder";
            public static readonly string GR_INVCYL = "Invcylinder";
            public static readonly string GR_SPHER = "Spherical";
            public static readonly string GR_HEMISP = "Hemispherical";
            public static readonly string GR_CURVED = "Curved";
            public static readonly string GR_INVSPH = "Invspherical";
            public static readonly string GR_INVHEM = "Invhemispherical";
            public static readonly string GR_INVCUR = "Invcurved";
        }
        /// <summary>
        /// 图案填充
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="patternName">图案名</param>
        /// <param name="entId">边界图形ObjectId</param>
        /// <param name="backgroundColor">背景色</param>
        /// <param name="hatchColorIndex">前景色索引</param>
        /// <param name="patternAngle">填充角度</param>
        /// <param name="patternScale">填充比例</param>
        /// <returns>对象的ObjectId</returns>
        public static ObjectId HatchEntity(this Database db, string patternName, ObjectId entId, Color backgroundColor, int hatchColorIndex = 0, double patternAngle = 45, double patternScale = 5)
        {
            ObjectId hatchId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Hatch hatch = new Hatch();
                hatch.PatternScale = patternScale; //填充比例
                hatch.BackgroundColor = backgroundColor;
                hatch.ColorIndex = hatchColorIndex;
                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName); //填充类型和图案名
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                hatchId = btr.AppendEntity(hatch);
                tr.AddNewlyCreatedDBObject(hatch, true);

                hatch.PatternAngle = patternAngle.DegreeToRadian();//填充角度
                hatch.Associative = true; //设置关联

                ObjectIdCollection obIds = new ObjectIdCollection();
                obIds.Add(entId);
                hatch.AppendLoop(HatchLoopTypes.Outermost, obIds);//设置边界图形和填充方式
                hatch.EvaluateHatch(true); //计算填充并显示


                tr.Commit();
            }
            return hatchId;
        }
        /// <summary>
        /// 图案填充
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="patternName">图案名称</param>
        /// <param name="hatchLoopTypes"></param>
        /// <param name="entsId">图案边界ObjectId列表</param>
        /// <param name="backgroundColor">背景色</param>
        /// <param name="hatchColorIndex">前景色索引</param>
        /// <param name="patternAngle">填充角度</param>
        /// <param name="patternScale">填充比例</param>
        /// <returns>填充对象的ObjectId</returns>
        public static ObjectId HatchEntity(this Database db, string patternName, List<HatchLoopTypes> hatchLoopTypes, List<ObjectId> entsId, Color backgroundColor, int hatchColorIndex = 0, double patternAngle = 45, double patternScale = 5)
        {
            ObjectId hatchId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Hatch hatch = new Hatch();
                hatch.PatternScale = patternScale; //填充比例
                hatch.BackgroundColor = backgroundColor;
                hatch.ColorIndex = hatchColorIndex;
                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName); //填充类型和图案名
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                hatchId = btr.AppendEntity(hatch);
                tr.AddNewlyCreatedDBObject(hatch, true);

                hatch.PatternAngle = patternAngle.DegreeToRadian();//填充角度
                hatch.Associative = true; //设置关联

                ObjectIdCollection obIds = new ObjectIdCollection();

                foreach (ObjectId entId in entsId)
                {
                    obIds.Clear();
                    obIds.Add(entId);
                    hatch.AppendLoop(hatchLoopTypes[entsId.IndexOf(entId)], obIds);//设置边界图形和填充方式
                }
                hatch.EvaluateHatch(true); //计算填充并显示
                tr.Commit();
            }
            return hatchId;
        }

        /// <summary>
        /// 渐变填充
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="gradientPattern">图案名称</param>
        /// <param name="colorIndex1">第一个颜色</param>
        /// <param name="colorIndex2">第二个颜色</param>
        /// <param name="entId">要填充对象的ObjectId</param>
        /// <param name="gradientAngle">填充角度</param>
        /// <returns>填充对象的ObjectId</returns>
        public static ObjectId HatchGradient(this Database db, string gradientPattern, short colorIndex1, short colorIndex2, ObjectId entId, double gradientAngle)
        {
            ObjectId hatchId = ObjectId.Null;
            ObjectIdCollection entsId = new ObjectIdCollection();
            entsId.Add(entId);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Hatch hatch = new Hatch();
                hatch.HatchObjectType = HatchObjectType.GradientObject; //设置渐变填充
                hatch.SetGradient(GradientPatternType.PreDefinedGradient, gradientPattern); //设置填充类型和图案名称
                hatch.GradientAngle = gradientAngle.DegreeToRadian();

                //设置填充色
                Color color1 = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                Color color2 = Color.FromColorIndex(ColorMethod.ByColor, colorIndex2);
                GradientColor gradientColor1 = new GradientColor(color1, 0);
                GradientColor gradientColor2 = new GradientColor(color2, 1);
                hatch.SetGradientColors(new GradientColor[] { gradientColor1, gradientColor2 });

                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                hatchId = btr.AppendEntity(hatch);
                tr.AddNewlyCreatedDBObject(hatch, true);
                //添加关联
                hatch.Associative = true;
                hatch.AppendLoop(HatchLoopTypes.Outermost, entsId);
                //计算并显示
                hatch.EvaluateHatch(true);

                tr.Commit();

            }
            return hatchId;

        }



    }
}
