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
    public static class BlockTool
    {
        /// <summary>
        /// 添加块表记录到图形数据库
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="btrName">块表记录名</param>
        /// <param name="ents">图形对象集合</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddBlockTableRecord(this Database db, string btrName, List<Entity> ents)
        {
            ObjectId btrId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (!bt.Has(btrName))
                {
                    BlockTableRecord btr = new BlockTableRecord();
                    btr.Name = btrName;
                    foreach (var item in ents)
                    {
                        btr.AppendEntity(item);

                    }
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                    bt.DowngradeOpen();
                }
                btrId = bt[btrName];
                tr.Commit();
            }
            return btrId;
        }

        /// <summary>
        /// 向模型空间插入块参照
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="BlockRecordId">块的ObjectId</param>
        /// <param name="position">插入位置</param>
        /// <returns>ObjectId</returns>
        public static ObjectId InsertBlockReference(this Database db, ObjectId BlockRecordId, Point3d position)
        {
            ObjectId blkRefId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (bt.Has(BlockRecordId))
                {
                    BlockReference br = new BlockReference(position, BlockRecordId);
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    blkRefId = btr.AppendEntity(br);
                    tr.AddNewlyCreatedDBObject(br, true);
                }
                tr.Commit();
            }

            return blkRefId;
        }

        public static ObjectId InsertBlockReference(this Database db, ObjectId BlockRecordId, Point3d position, double rotation, Scale3d scale)
        {
            ObjectId blkRefId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (bt.Has(BlockRecordId))
                {
                    BlockReference br = new BlockReference(position, BlockRecordId);
                    br.Rotation = rotation;
                    br.ScaleFactors = scale;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    blkRefId = btr.AppendEntity(br);
                    tr.AddNewlyCreatedDBObject(br, true);
                }
                tr.Commit();
            }

            return blkRefId;
        }
    }
}
