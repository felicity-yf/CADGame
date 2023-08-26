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
    public static class BaseTool
    {
        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="degree">角度值</param>
        /// <returns>弧度</returns>
        public static double DegreeToRadian(this double degree)
        {
            return degree * Math.PI / 180;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="randian">弧度值</param>
        /// <returns>角度</returns>
        public static double RadianToDegree(this double randian)
        {
            return randian * 180 / Math.PI;
        }

        /// <summary>
        /// 判断三点是否在一条直线上
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <param name="point3">第三个点</param>
        /// <returns>是返加真，否返回假</returns>
        public static bool IsOnOneLine(this Point3d point1, Point3d point2, Point3d point3)
        {
            Vector3d v21 = point2.GetVectorTo(point1);
            Vector3d v23 = point2.GetVectorTo(point3);
            if (v21.GetAngleTo(v23) == 0 || v21.GetAngleTo(v23) == Math.PI)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 获取两点直线与X的夹角
        /// </summary>
        /// <param name="startPoint">起始点</param>
        /// <param name="endPoint">终止点</param>
        /// <returns>弧度夹角值</returns>
        public static double GetAngleToXAxis(this Point3d startPoint, Point3d endPoint)
        {
            Vector3d XVector = new Vector3d(1, 0, 0);
            Vector3d SToEVector = startPoint.GetVectorTo(endPoint);
            return SToEVector.Y > 0 ? XVector.GetAngleTo(SToEVector) : -XVector.GetAngleTo(SToEVector);
        }

        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>两点之间的距离</returns>
        public static double GetDistanceBetweenTwoPoint(this Point3d point1, Point3d point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.Z - point2.Z, 2));
        }

        /// <summary>
        /// 获取两点之间的中点
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>中点</returns>
        public static Point3d GetMidPointBetweenTwoPoint(this Point3d point1, Point3d point2)
        {
            return new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }

        /// <summary>
        /// 获取两点直线的斜率
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>两点直线的斜率</returns>
        public static double GetSlopeBetweenTwoPoint(this Point3d point1, Point3d point2)
        {
            if ((point1.X - point2.X) == 0)
            {
                return double.NaN;
            }
            return (point1.Y - point2.Y) / (point1.X - point2.X);
        }

        /// <summary>
        /// 获取三点的外心
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <param name="point3">第三个点</param>
        /// <returns>外心</returns>
        public static Point3d GetCircumcenter(this Point3d point1, Point3d point2, Point3d point3)
        {
            Point3d midPiont21 = BaseTool.GetMidPointBetweenTwoPoint(point1, point2);
            Point3d midPiont23 = BaseTool.GetMidPointBetweenTwoPoint(point3, point2);

            double slopeP21 = BaseTool.GetSlopeBetweenTwoPoint(point1, point2);
            double slopeP23 = BaseTool.GetSlopeBetweenTwoPoint(point3, point2);
            if (slopeP21 == slopeP23)
            {
                return Point3d.Origin;
            }
            double centerX = (slopeP21 * slopeP23 * (midPiont23.Y - midPiont21.Y) - slopeP23 * midPiont21.X + slopeP21 * midPiont23.X) / (slopeP21 - slopeP23);
            double centerY = (-1 / slopeP21) * (centerX - midPiont21.X) + midPiont21.Y;
            return new Point3d(centerX, centerY, 0);
        }


    }
}
