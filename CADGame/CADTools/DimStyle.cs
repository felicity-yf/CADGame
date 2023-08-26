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
    public static partial class DimStyleTool
    {
        /// <summary>
        /// 新建注释样式
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="dimStyleName">注释样式名</param>
        /// <returns>注释样式的ObjecId</returns>
        public static ObjectId AddDimStyle(this Database db, string dimStyleName)
        {
            ObjectId dimStyleId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开注释样式表
                DimStyleTable dst = tr.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                if (!dst.Has(dimStyleName))
                {
                    DimStyleTableRecord dstr = new DimStyleTableRecord();
                    dstr.Name = dimStyleName;
                    dst.UpgradeOpen();
                    dimStyleId = dst.Add(dstr);
                    tr.AddNewlyCreatedDBObject(dstr, true);
                    dst.DowngradeOpen();
                }
                tr.Commit();
            }
            return dimStyleId;
        }
    }
}
