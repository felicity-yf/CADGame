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
    public static partial class EditEntityTool
    {
        /// <summary>
        /// 更改图形颜色
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">图形ObjectId</param>
        /// <param name="colorIndex">颜色值索引</param>
        public static void ChangeEntityColor(this Database db, ObjectId entId, short colorIndex)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity;
                ent.ColorIndex = colorIndex;
                tr.Commit();
            }
        }
        /// <summary>
        /// 改变图形颜色
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">图形的ObjectId</param>
        /// <param name="colorIndex">颜色值索引</param>
        public static void ChangeEntityColor(this Database db, Entity entId, short colorIndex)
        {
            if (entId.IsNewObject)
            {
                entId.ColorIndex = colorIndex;
            }
            else
            {
                ChangeEntityColor(db, entId, colorIndex);
            }

        }

        /// <summary>
        /// 移动图形
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entId">图形的ObjectId</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static void MoveEntity(this Database db, ObjectId entId, Point3d sourcePoint, Point3d targetPoint)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable; //打开块表
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord; //打开块表记录
                                                                                                                            //Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity; //打开图形
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint); //计算变换矩阵
                Matrix3d mt = Matrix3d.Displacement(vector);
                ent.TransformBy(mt);
                tr.Commit();
            }
        }

        /// <summary>
        /// 移动图形
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ent">图形Entity</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static void MoveEntity(this Database db, Entity ent, Point3d sourcePoint, Point3d targetPoint)
        {
            // 判断图形的对象是不是新图
            if (ent.IsNewObject)
            {
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                Matrix3d mt = Matrix3d.Displacement(vector);
                ent.TransformBy(mt);
            }
            else
            {
                MoveEntity(db, ent.ObjectId, sourcePoint, targetPoint);
            }
        }
        /// <summary>
        /// 复制图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">要复制的图形的ObjectId</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        /// <returns>复制后的Entity</returns>

        public static Entity CopyEntity(this Database db, ObjectId entId, Point3d sourcePoint, Point3d targetPoint)
        {
            //声明图形对象
            Entity entCopy;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable; //打开块表
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord; //打开块表记录
                                                                                                                            //Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity; //打开图形
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint); //计算变换矩阵
                Matrix3d mt = Matrix3d.Displacement(vector);
                entCopy = ent.GetTransformedCopy(mt);
                btr.AppendEntity(entCopy);
                tr.AddNewlyCreatedDBObject(entCopy, true);
                tr.Commit();
            }
            return entCopy;
        }

        /// <summary>
        /// 复制图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">要复制的Eneity</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        /// <returns>复制后的Entity</returns>
        public static Entity CopyEntity(this Database db, Entity ent, Point3d sourcePoint, Point3d targetPoint)
        {
            Entity entCopy;
            if (ent.IsNewObject)
            {
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                Matrix3d mt = Matrix3d.Displacement(vector);
                entCopy = ent.GetTransformedCopy(mt);
                db.AddEntityToModelSpace(entCopy);

            }
            else
            {
                entCopy = CopyEntity(db, ent.ObjectId, sourcePoint, targetPoint);
            }
            return entCopy;
        }

        /// <summary>
        /// 旋转图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">要旋转的图形的ObjectId</param>
        /// <param name="center">旋转中心</param>
        /// <param name="degree">旋转角度</param>
        public static void RotateEntity(this Database db, ObjectId entId, Point3d center, double degree)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable; //打开块表
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord; //打开块表记录
                                                                                                                            //Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity; //打开图形

                Matrix3d mt = Matrix3d.Rotation(degree.DegreeToRadian(), Vector3d.ZAxis, center);
                ent.TransformBy(mt);
                tr.Commit();
            }
        }

        /// <summary>
        /// 旋转图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">要旋转的图形的Entity</param>
        /// <param name="center">旋转中心</param>
        /// <param name="degree">旋转角度</param>
        public static void RotateEntity(this Database db, Entity ent, Point3d center, double degree)
        {

            if (ent.IsNewObject)
            {

                Matrix3d mt = Matrix3d.Rotation(degree.DegreeToRadian(), Vector3d.ZAxis, center);
                ent.TransformBy(mt);

            }
            else
            {
                RotateEntity(db, ent.ObjectId, center, degree);
            }
        }

        /// <summary>
        /// 镜像图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">要镜像的图形ObjectId</param>
        /// <param name="point1">镜像参考点一</param>
        /// <param name="point2">镜像参考点一</param>
        /// <param name="isEraseSource">是否删除原图</param>
        /// <returns>镜像对象的Entity</returns>
        public static Entity MirrorEntity(this Database db, ObjectId entId, Point3d point1, Point3d point2, bool isEraseSource = false)
        {
            //声明图形对象用于返回
            Entity entCopy;
            //计算镜像的变换矩阵
            Matrix3d mt = Matrix3d.Mirroring(new Line3d(point1, point2));
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
                entCopy = ent.GetTransformedCopy(mt);
                db.AddEntityToModelSpace(entCopy);
                //是否删除原图
                if (isEraseSource)
                {
                    ent.Erase();
                }
                tr.Commit();
            }
            return entCopy;
        }

        /// <summary>
        /// 镜像图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">要镜像的图形</param>
        /// <param name="point1">镜像参考点一</param>
        /// <param name="point2">镜像参考点一</param>
        /// <param name="isEraseSource">是否删除原图</param>
        /// <returns>镜像对象的Entity</returns>
        public static Entity MirrorEntity(this Database db, Entity ent, Point3d point1, Point3d point2, bool isEraseSource = false)
        {
            Entity entCopy;
            if (ent.IsNewObject)
            {
                Matrix3d mt = Matrix3d.Mirroring(new Line3d(point1, point2));
                entCopy = ent.GetTransformedCopy(mt);
                db.AddEntityToModelSpace(entCopy);
            }
            else
            {
                entCopy = MirrorEntity(db, ent.ObjectId, point1, point2, isEraseSource);
            }
            return entCopy;
        }

        /// <summary>
        /// 缩放图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">要缩放的图形ObjectId</param>
        /// <param name="basePoint">参考点</param>
        /// <param name="factor">缩放比例</param>
        public static void ScaleEntity(this Database db, ObjectId entId, Point3d basePoint, double factor)
        {
            Matrix3d mt = Matrix3d.Scaling(factor, basePoint);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开要缩放的图形对象
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity;
                ent.TransformBy(mt);
                tr.Commit();
            }
        }

        /// <summary>
        /// 缩放图形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">要缩放的图形Entity</param>
        /// <param name="basePoint">参考点</param>
        /// <param name="factor">缩放比例</param>
        public static void ScaleEntity(this Database db, Entity ent, Point3d basePoint, double factor)
        {
            if (ent.IsNewObject)
            {
                Matrix3d mt = Matrix3d.Scaling(factor, basePoint);
                ent.TransformBy(mt);
            }
            else
            {
                ScaleEntity(db, ent.ObjectId, basePoint, factor);
            }
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">所要删除对象的ObjectId</param>
        public static void EraseEntity(this Database db, ObjectId entId)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity;
                ent.Erase();
                tr.Commit();
            }
        }

        /// <summary>
        /// 矩形阵列
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">要阵列的图形的ObjectId</param>
        /// <param name="row">行数</param>
        /// <param name="col">列数</param>
        /// <param name="rowGap">行间距</param>
        /// <param name="colGap">列间距</param>
        /// <param name="degree">旋转角</param>
        /// <returns>所阵列出的图形的Entity列表</returns>
        public static List<Entity> ArrayRectEntity(this Database db, ObjectId entId, int row, int col, double rowGap, double colGap)
        {
            List<Entity> entCopy = new List<Entity>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Matrix3d mtDisplacement = Matrix3d.Displacement(new Vector3d(j * colGap, i * rowGap, 0));
                        Entity entA = ent.GetTransformedCopy(mtDisplacement);    //获取变换后的对象

                        btr.AppendEntity(entA);
                        tr.AddNewlyCreatedDBObject(entA, true);
                        entCopy.Add(entA);
                    }
                }
                ent.Erase(); //删除原始的图形
                tr.Commit();
            }
            return entCopy;
        }

        /// <summary>
        /// 矩形阵列
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">要阵列的图形的Entity</param>
        /// <param name="row">行数</param>
        /// <param name="col">列数</param>
        /// <param name="rowGap">行间距</param>
        /// <param name="colGap">列间距</param>
        /// <param name="degree">旋转角</param>
        /// <returns>所阵列出的图形的Entity列表</returns>
        public static List<Entity> ArrayRectEntity(this Database db, Entity ent, int row, int col, double rowGap, double colGap)
        {
            List<Entity> entCopy = new List<Entity>();
            if (ent.IsNewObject)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col; j++)
                        {
                            Matrix3d mtDisplacement = Matrix3d.Displacement(new Vector3d(j * colGap, i * rowGap, 0));
                            Entity entA = ent.GetTransformedCopy(mtDisplacement);    //获取变换后的对象

                            btr.AppendEntity(entA);
                            tr.AddNewlyCreatedDBObject(entA, true);
                            entCopy.Add(entA);
                        }
                    }
                    tr.Commit();
                }

                return entCopy;
            }
            else
            {
                return ArrayRectEntity(db, ent.ObjectId, row, col, rowGap, colGap);
            }
        }
        /// <summary>
        /// 环形阵列
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="entId">要阵列对象的ObjectId</param>
        /// <param name="count">要阵列的个数</param>
        /// <param name="degree">阵列覆盖的角度</param>
        /// <param name="center">阵列中心点</param>
        /// <returns>阵列表对象的列表</returns>
        public static List<Entity> ArrayRingEntity(this Database db, ObjectId entId, int count, double degree, Point3d center)
        {
            List<Entity> entCopy = new List<Entity>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Entity ent = entId.GetObject(OpenMode.ForWrite) as Entity;
                //限定阵列角度
                degree = degree > 360 ? 360 : degree;
                degree = degree < -360 ? -360 : degree;
                int divAngCount = count - 1;
                if (degree == 360 || degree == -360)
                {
                    divAngCount = count;
                }

                for (int i = 0; i < count; i++)
                {
                    Matrix3d mt = Matrix3d.Rotation((i * degree / divAngCount).DegreeToRadian(), Vector3d.ZAxis, center);
                    Entity entA = ent.GetTransformedCopy(mt);
                    btr.AppendEntity(entA);
                    tr.AddNewlyCreatedDBObject(entA, true);
                    entCopy.Add(entA);
                }
                ent.Erase();
                tr.Commit();
            }
            return entCopy;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entId"></param>
        /// <returns></returns>
        public static Entity GetEntity(this Database db, ObjectId entId)
        {
            Entity ent;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                ent = entId.GetObject(OpenMode.ForRead) as Entity;
            }
            return ent;
        }
        /// <summary>
        /// 获取集合中的图形对象
        /// </summary>
        /// <param name="ids">ObjectId数组</param>
        /// <returns>图形对象列表</returns>
        public static List<Entity> GetEntity(this Database db, ObjectId[] ids)
        {
            List<Entity> entList = new List<Entity>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    Entity ent = ids[i].GetObject(OpenMode.ForRead) as Entity;
                    entList.Add(ent);
                }
            }
            return entList;

        }
        /// <summary>
        /// 删除图形对象列表
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ents">列表</param>
        public static void DeleteEntitys(this Database db,Entity[] ents)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < ents.Length; i++)
                {
                    Entity ent = ents[i].ObjectId.GetObject(OpenMode.ForWrite) as Entity;
                    ent.Erase();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// 改变图形对象列表中所有对象的颜色
        /// </summary>
        /// <param name="entList">图形对象列表</param>
        /// <param name="colorIndex">颜色值索引</parma>
        public static void ChangeColorEntity(List<Entity> entList, byte colorIndex)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < entList.Count; i++)
                {
                    Entity ent = entList[i].ObjectId.GetObject(OpenMode.ForWrite) as Entity;
                    ent.ColorIndex = colorIndex;
                }
                tr.Commit();
            }
        }


    }
}
