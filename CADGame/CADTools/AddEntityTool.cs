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
    public static class AddEntityTool
    {
        /// <summary>
        /// 将图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形实体</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddEntityToModelSpace(this Database db, Entity ent)
        {
            ObjectId entId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                entId = btr.AppendEntity(ent);
                tr.AddNewlyCreatedDBObject(ent, true);
                tr.Commit();
            }
            return entId;
        }

        /// <summary>
        /// 将多个图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ents">图形实体列表</param>
        /// <returns>添加后对象的ObjectId列表</returns>
        public static List<ObjectId> AddEntityToModelSpace(this Database db, List<Entity> ents)
        {
            List<ObjectId> entsId = new List<ObjectId>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                foreach (Entity ent in ents)
                {
                    entsId.Add(btr.AppendEntity(ent));
                    tr.AddNewlyCreatedDBObject(ent, true);
                }
                tr.Commit();
            }
            return entsId;
        }

        /// <summary>
        /// 添加图形对象到图形数据库
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ents">图形对象数组</param>
        /// <returns>图形对象的ObjectId列表</returns>
        public static List<ObjectId> AddEntity(this Database db, Entity[] ents)
        {
            List<ObjectId> entIds = new List<ObjectId>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                for (int i = 0; i < ents.Length; i++)
                {
                    entIds.Add(btr.AppendEntity(ents[i]));
                    tr.AddNewlyCreatedDBObject(ents[i], true);
                }

                tr.Commit();
            }
            return entIds;
        }

        /// <summary>
        /// 将多个图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ents">不定个数的图形对象</param>
        /// <returns>添加后对象的ObjectId数组</returns>
        public static ObjectId[] AddEntityToModelSpace(this Database db, params Entity[] ents)
        {
            ObjectId[] entsId = new ObjectId[ents.Length];
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                for (int i = 0; i < ents.Length; i++)
                {
                    entsId[i] = btr.AppendEntity(ents[i]);
                    tr.AddNewlyCreatedDBObject(ents[i], true);
                }
                tr.Commit();
            }
            return entsId;

        }

        /// <summary>
        /// 通过两个点添加一条直线到图形文件
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startX">第一个点的x</param>
        /// <param name="startY">第一个点的y</param>
        /// <param name="endX">第二个点的x</param>
        /// <param name="endY">第二个点的y</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddLineToModelSpace(this Database db, double startX, double startY, double endX, double endY)
        {
            ObjectId entId = ObjectId.Null;
            Line line = new Line(new Point3d(startX, startY, 0), new Point3d(endX, endY, 0));
            return AddEntityToModelSpace(db, line);
        }

        /// <summary>
        /// 通过两个点添加一条直线到图形文件
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">第一个点</param>
        /// <param name="endPoint">第二个点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddLineToModelSpace(this Database db, Point3d startPoint, Point3d endPoint)
        {
            ObjectId entId = ObjectId.Null;
            Line line = new Line(startPoint, endPoint);
            return AddEntityToModelSpace(db, line);
        }

        /// <summary>
        /// 通过起点长度以及与X轴正方向的角度创建直线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">起始点</param>
        /// <param name="length">长度</param>
        /// <param name="degree">与X轴正方向的夹角</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddLineToModelSpace(this Database db, Point3d startPoint, double length, double degree)
        {
            double X = startPoint.X + length * Math.Cos(degree.DegreeToRadian());
            double Y = startPoint.Y + length * Math.Sin(degree.DegreeToRadian());
            Point3d endPoint = new Point3d(X, Y, 0);
            return AddLineToModelSpace(db, startPoint, endPoint);
        }

        /// <summary>
        /// 通过圆心半径起始角终止角创建圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <param name="startDegree">起始角</param>
        /// <param name="endDegree">终止角</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddArcToModelSpace(this Database db, Point3d center, double radius, double startDegree, double endDegree)
        {
            return AddEntityToModelSpace(db, new Arc(center, radius, startDegree.DegreeToRadian(), endDegree.DegreeToRadian()));
        }

        /// <summary>
        /// 通过圆弧对象直接创建圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="arc">圆弧对象</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddArcToModelSpace(this Database db, Arc arc)
        {
            return AddEntityToModelSpace(db, arc);
        }

        /// <summary>
        /// 通过三个点创建圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">起始点</param>
        /// <param name="midPoint">中间点</param>
        /// <param name="endPoint">终止点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddArcToModelSpace(this Database db, Point3d startPoint, Point3d midPoint, Point3d endPoint)
        {
            if (BaseTool.IsOnOneLine(startPoint, midPoint, endPoint))
            {
                return ObjectId.Null;
            }
            CircularArc3d cArc = new CircularArc3d(startPoint, midPoint, endPoint); //用于计算的圆弧对象

            Vector3d cs = cArc.Center.GetVectorTo(startPoint);               //起点到圆心的向量
            Vector3d ce = cArc.Center.GetVectorTo(endPoint);                 //终点到圆心的向量
            Vector3d xVector = new Vector3d(1, 0, 0);

            double startAngle = cs.Y > 0 ? xVector.GetAngleTo(cs) : -xVector.GetAngleTo(cs);
            double endAngle = ce.Y > 0 ? xVector.GetAngleTo(ce) : -xVector.GetAngleTo(ce);

            Arc arc = new Arc(cArc.Center, cArc.Radius, startAngle, endAngle);

            return AddArcToModelSpace(db, arc);
        }

        /// <summary>
        /// 通过圆心起点角度创建圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="startPoint">起始点</param>
        /// <param name="degree">角度</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddArcToModelSpace(this Database db, Point3d center, Point3d startPoint, double degree)
        {
            double radius = center.GetDistanceBetweenTwoPoint(startPoint);
            double startAngle = center.GetAngleToXAxis(startPoint);

            Arc arc = new Arc(center, radius, startAngle, startAngle + degree.DegreeToRadian());

            return AddArcToModelSpace(db, arc);
        }

        /// <summary>
        /// 通过圆心和半径创建圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddCircleToModelSpace(this Database db, Point3d center, double radius)
        {
            return AddEntityToModelSpace(db, new Circle(center, new Vector3d(0, 0, 1), radius));
        }

        /// <summary>
        /// 通过两点创建圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddCircleToModelSpace(this Database db, Point3d point1, Point3d point2)
        {
            Point3d center = BaseTool.GetMidPointBetweenTwoPoint(point1, point2);
            double radius = BaseTool.GetDistanceBetweenTwoPoint(center, point1);
            return AddEntityToModelSpace(db, new Circle(center, new Vector3d(0, 0, 1), radius));
        }

        /// <summary>
        /// 通过三点创建圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <param name="point3">第三个点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddCircleToModelSpace(this Database db, Point3d point1, Point3d point2, Point3d point3)
        {
            if (point1.IsOnOneLine(point2, point3))
            {
                return ObjectId.Null;
            }
            CircularArc3d cArc = new CircularArc3d(point1, point2, point3);
            double radius = cArc.Center.GetDistanceBetweenTwoPoint(point1);
            return AddEntityToModelSpace(db, new Circle(cArc.Center, new Vector3d(0, 0, 1), radius));

        }

        /// <summary>
        /// 通过多个点创建多段线，不定参数形式。
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="isClosed">是否闭合</param>
        /// <param name="constantWidth">线宽</param>
        /// <param name="vertices">顶点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddPolyLineToModelSpace(this Database db, bool isClosed, double constantWidth, params Point2d[] vertices)
        {
            Polyline pLine = new Polyline();
            if (vertices.Length < 2)
            {
                return ObjectId.Null;
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                pLine.AddVertexAt(i, vertices[i], 0, 0, 0);
            }
            pLine.Closed = isClosed;
            pLine.ConstantWidth = constantWidth;
            return AddEntityToModelSpace(db, pLine);
        }
        /// <summary>
        /// 通过多个点创建多段线,列表形式
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="isClosed">是否闭合</param>
        /// <param name="constantWidth">线宽</param>
        /// <param name="vertices">点列表</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddPolyLineToModelSpace(this Database db, bool isClosed, double constantWidth, List<Point2d> vertices)
        {
            if (vertices.Count < 2)
            {
                return ObjectId.Null;
            }
            Polyline pLine = new Polyline();
            foreach (Point2d point in vertices)
            {
                pLine.AddVertexAt(vertices.IndexOf(point), point, 0, 0, 0);
            }
            pLine.Closed = isClosed;
            pLine.ConstantWidth = constantWidth;
            return AddEntityToModelSpace(db, pLine);
        }

        /// <summary>
        /// 创建多条多段线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="pLines">多段线对象</param>
        /// <returns>多段线ObjectId列表</returns>
        public static List<ObjectId> AddPolyLineToModelSpace(this Database db, List<Polyline> pLines)
        {
            List<ObjectId> entsId = new List<ObjectId>();
            foreach (Polyline pLine in pLines)
            {
                entsId.Add(AddEntityToModelSpace(db, pLine));
            }
            return entsId;
        }

        /// <summary>
        /// 通过两点创建矩形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="leftPoint">左边的点</param>
        /// <param name="rightPoint">右边的点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddRectToModelSpace(this Database db, Point2d leftPoint, Point2d rightPoint)
        {

            if (leftPoint.Y > rightPoint.Y)
            {
                Point2d leftUpPoint = leftPoint;
                Point2d rightLowPoint = rightPoint;
                Point2d leftLowPoint = new Point2d(leftPoint.X, rightPoint.Y);
                Point2d rightUpPoint = new Point2d(rightPoint.X, leftPoint.Y);
                return AddPolyLineToModelSpace(db, true, 0, leftUpPoint, rightUpPoint, rightLowPoint, leftLowPoint);
            }
            else
            {
                Point2d leftLowPoint = leftPoint;
                Point2d rightUpPoint = rightPoint;
                Point2d leftUpPoint = new Point2d(leftLowPoint.X, rightUpPoint.Y);
                Point2d rightLowPoint = new Point2d(rightUpPoint.X, leftLowPoint.Y);
                return AddPolyLineToModelSpace(db, true, 0, leftUpPoint, rightUpPoint, rightLowPoint, leftLowPoint);
            }

        }

        /// <summary>
        /// 通过圆心，半径，边数，角度创建外接正多边形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <param name="edges">边数</param>
        /// <param name="degree">旋转角</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddPolygonToModelSpace(this Database db, Point2d center, double radius, int edges, double degree)
        {
            if (edges < 3)
            {
                return ObjectId.Null;
            }
            List<Point2d> points = new List<Point2d>();
            double startDegree = 0;
            double stepDegree = 360 / edges;
            for (int i = 0; i < edges; i++)
            {
                double x = radius * Math.Cos((startDegree + stepDegree * i + degree + 90).DegreeToRadian()) + center.X;
                double y = radius * Math.Sin((startDegree + stepDegree * i + degree + 90).DegreeToRadian()) + center.Y;
                points.Add(new Point2d(x, y));

            }
            return AddEntityTool.AddPolyLineToModelSpace(db, true, 0, points);
        }

        /// <summary>
        /// 通过中心,长短轴,起止角,旋转角创建椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">椭圆心</param>
        /// <param name="majorRadius">长轴</param>
        /// <param name="shortRadius">短轴</param>
        /// <param name="startAngel">起始角</param>
        /// <param name="endAngle">终止角</param>
        /// <param name="degree">长轴与X轴正方向的夹角</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d center, double majorRadius, double shortRadius, double startAngle = 0, double endAngle = 360, double degree = 0)
        {
            double ratio = shortRadius / majorRadius;
            Vector3d majorAxis = new Vector3d(majorRadius * Math.Cos(degree.DegreeToRadian()), majorRadius * Math.Sin(degree.DegreeToRadian()), 0);
            Ellipse el = new Ellipse(center, Vector3d.ZAxis, majorAxis, ratio, startAngle.DegreeToRadian(), endAngle.DegreeToRadian());
            return AddEntityToModelSpace(db, el);
        }

        /// <summary>
        /// 通过两点和半轴长,起止角绘制椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="majorPoint1">第一个点</param>
        /// <param name="majorPoint2">第二个点</param>
        /// <param name="shortRadius">半轴长</param>
        /// <param name="startAngle">起始角</param>
        /// <param name="endAngle">终止角</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d majorPoint1, Point3d majorPoint2, double shortRadius, double startAngle = 0, double endAngle = 360)
        {
            Point3d center = majorPoint1.GetMidPointBetweenTwoPoint(majorPoint2);
            //短轴与长轴比
            double ratio = 2 * shortRadius / majorPoint1.GetDistanceBetweenTwoPoint(majorPoint2);
            Vector3d majorAxis = majorPoint2.GetVectorTo(center);
            Ellipse el = new Ellipse(center, Vector3d.ZAxis, majorAxis, ratio, startAngle.DegreeToRadian(), endAngle.DegreeToRadian());
            return AddEntityToModelSpace(db, el);
        }

        /// <summary>
        /// 通过两点创建与两点构成的矩形内切的椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>添加后对象的ObjectId</returns>
        public static ObjectId AddEllipseToModelSpace(this Database db, Point3d point1, Point3d point2)
        {
            Point3d center = point1.GetMidPointBetweenTwoPoint(point2);
            if (Math.Max(Math.Abs(point1.Y - point2.Y), Math.Abs(point1.X - point2.X)) == Math.Abs(point1.Y - point2.Y))
            {
                double ratio = Math.Abs((point1.X - point2.X) / (point1.Y - point2.Y));
                Vector3d majorVector = new Vector3d(0, Math.Abs(point1.Y - point2.Y) / 2, 0);
                Ellipse el = new Ellipse(center, Vector3d.ZAxis, majorVector, ratio, 0, 2 * Math.PI);
                return AddEntityToModelSpace(db, el);
            }
            else
            {
                double ratio = Math.Abs((point1.Y - point2.Y) / (point1.X - point2.X));
                Vector3d majorVector = new Vector3d(Math.Abs(point1.X - point2.X) / 2, 0, 0);
                Ellipse el = new Ellipse(center, Vector3d.ZAxis, majorVector, ratio, 0, 2 * Math.PI);
                return AddEntityToModelSpace(db, el);
            }

        }


    }
}
