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
    //添加图层的返回状态
    public enum AddLayerStatus
    {
        AddLayerOk,
        IllegalLayerName,
        LayerNameExist
    }
    //添加图层的返回值
    public struct AddLayerResult
    {
        public AddLayerStatus Status;
        public ObjectId Value;
        public string LayerName;
    }
    //改变图层属性的返回状态
    public enum ChangeLayerPropertyStatus
    {
        ChangeOk,
        LayerNotExist,
    }

    public static partial class LayerTool
    {
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns>AddLayerResult</returns>
        public static AddLayerResult AddLayer(this Database db, string layerName)
        {
            //声明AddLayerResult类型用于返回
            AddLayerResult res = new AddLayerResult();
            //判断layerName是否合法
            try
            {
                SymbolUtilityServices.ValidateSymbolName(layerName, false);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                res.Status = AddLayerStatus.IllegalLayerName;
                res.Value = ObjectId.Null;
                return res;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开层表
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //新建层表记录
                LayerTableRecord ltr = new LayerTableRecord();

                if (!lt.Has(layerName))
                {
                    res.Status = AddLayerStatus.AddLayerOk;
                    ltr.Name = layerName;
                    lt.UpgradeOpen(); //提升打开权限
                    res.Value = lt.Add(ltr);
                    res.LayerName = layerName;
                    lt.DowngradeOpen(); //降低打开权限
                    tr.AddNewlyCreatedDBObject(ltr, true);
                    tr.Commit();
                }
                else
                {
                    res.Status = AddLayerStatus.LayerNameExist;
                    res.Value = ObjectId.Null;
                    res.LayerName = layerName;
                }


            }

            return res;
        }
        /// <summary>
        /// 修改图层颜色
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <param name="colorIndex">颜色索引值</param>
        /// <returns>修改状态</returns>
        public static ChangeLayerPropertyStatus ChangeLayerColor(this Database db, string layerName, byte colorIndex)
        {
            ChangeLayerPropertyStatus status;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开层表
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断指定图层是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
                    status = ChangeLayerPropertyStatus.ChangeOk;
                }
                else
                {
                    status = ChangeLayerPropertyStatus.LayerNotExist;
                }
                tr.Commit();
            }
            return status;
        }
        /// <summary>
        /// 锁定与解锁图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns>修改状态</returns>
        public static ChangeLayerPropertyStatus ChangeLayerLockStatus(this Database db, string layerName, bool isLock)
        {
            ChangeLayerPropertyStatus status;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开层表
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断指定图层是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.IsLocked = isLock;
                    status = ChangeLayerPropertyStatus.ChangeOk;
                }
                else
                {
                    status = ChangeLayerPropertyStatus.LayerNotExist;
                }
                tr.Commit();
            }
            return status;
        }
        /// <summary>
        /// 修改图层线宽
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <param name="lineWeight">线宽</param>
        /// <returns>ChangeLayerPropertyStatus</returns>
        public static ChangeLayerPropertyStatus ChangeLayerLineWeight(this Database db, string layerName, LineWeight lineWeight)
        {
            ChangeLayerPropertyStatus status;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开层表
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断指定图层是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.LineWeight = lineWeight;
                    status = ChangeLayerPropertyStatus.ChangeOk;
                }
                else
                {
                    status = ChangeLayerPropertyStatus.LayerNotExist;
                }
                tr.Commit();
            }
            return status;
        }
        /// <summary>
        /// 设置当前图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns>是否设置成功</returns>
        public static bool SetCurrentLayer(this Database db, string layerName)
        {
            bool isSetOk = false;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开层表
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断图层是否存在
                if (lt.Has(layerName))
                {
                    ObjectId layerId = lt[layerName];
                    //判断传入的图层是否为当前图层
                    if (db.Clayer != layerId)
                    {
                        db.Clayer = layerId;
                    }
                    isSetOk = true;
                }
                tr.Commit();
            }
            return isSetOk;
        }
        /// <summary>
        /// 获取所有图层的层表记录列表
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <returns>层表记录列表</returns>
        public static List<LayerTableRecord> GetAllLayers(this Database db)
        {
            List<LayerTableRecord> layerList = new List<LayerTableRecord>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                lt.GenerateUsageData();
                foreach (ObjectId Item in lt)
                {
                    LayerTableRecord ltr = Item.GetObject(OpenMode.ForRead) as LayerTableRecord;
                    layerList.Add(ltr);
                }
            }
            return layerList;
        }
        /// <summary>
        /// 获取所有图层的图层名
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <returns>所有图层名组成的列表</returns>
        public static List<string> GetAllLayersName(this Database db)
        {
            List<string> layerNmaeList = new List<string>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (ObjectId item in lt)
                {
                    LayerTableRecord ltr = item.GetObject(OpenMode.ForRead) as LayerTableRecord;
                    layerNmaeList.Add(ltr.Name);
                }
            }
            return layerNmaeList;
        }

        /// <summary>
        /// 删除图层，空图层，非当前图层会被删除
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns>是否成功</returns>
        public static bool DeleteLayer(this Database db, string layerName)
        {
            bool isDeleteOK = false;
            if (layerName == "0" || layerName == "Defpoints") return isDeleteOK;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                lt.GenerateUsageData();
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = tr.GetObject(lt[layerName], OpenMode.ForWrite) as LayerTableRecord;
                    if (!ltr.IsUsed && db.Clayer != lt[layerName])
                    {
                        ltr.Erase();
                        isDeleteOK = true;
                    }
                }
                else
                {
                    isDeleteOK = true;
                }
                tr.Commit();
            }
            return isDeleteOK;
        }
        /// <summary>
        /// 强制删除图层，无论有无实体。
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <param name="delete"></param>
        /// <returns>是否成功</returns>
        public static bool DeleteLayer(this Database db, string layerName, bool delete)
        {
            bool isDeleteOK = false;
            if (layerName == "0" || layerName == "Defpoints") return isDeleteOK;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                lt.GenerateUsageData();
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = tr.GetObject(lt[layerName], OpenMode.ForWrite) as LayerTableRecord;
                    //判断是否强制删除
                    if (delete)
                    {
                        if (ltr.IsUsed) { ltr.DeleteAllEntityInLayer(db); }
                        db.SetCurrentLayer("0");
                        ltr.Erase();
                        isDeleteOK = true;
                    }
                    else
                    {
                        if (!ltr.IsUsed && db.Clayer != lt[layerName]) { ltr.Erase(); isDeleteOK = true; }
                    }
                }
                else
                {
                    isDeleteOK = true;
                }
                tr.Commit();
            }
            return isDeleteOK;
        }

        /// <summary>
        /// 删除拽定图层的所有实体
        /// </summary>
        /// <param name="ltr">层表记录</param>
        /// <param name="db">图形数据库</param>
        public static void DeleteAllEntityInLayer(this LayerTableRecord ltr, Database db)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] value = new TypedValue[]
            {
                new TypedValue((int)DxfCode.LayerName,ltr.Name)
            };
            SelectionFilter filter = new SelectionFilter(value);
            PromptSelectionResult psr = ed.SelectAll();
            if (psr.Status == PromptStatus.OK)
            {
                ObjectId[] ids = psr.Value.GetObjectIds();
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    //BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    //BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    for (int i = 0; i < ids.Length; i++)
                    {
                        Entity ent = ids[i].GetObject(OpenMode.ForWrite) as Entity;
                        ent.Erase();
                    }
                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// 删除所有未使用的图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        public static void DeleteNotUsedLayer(this Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                lt.GenerateUsageData();
                foreach (ObjectId item in lt)
                {
                    LayerTableRecord ltr = item.GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    if (!ltr.IsUsed)
                    {
                        ltr.Erase();
                    }
                }
                tr.Commit();
            }
        }
    }
}
