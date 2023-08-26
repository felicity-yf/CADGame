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
    public static partial class TextStyleTool
    {
        public static void AddTextStyle(this Database db, string textStyleName)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //打开文字样式表
                TextStyleTable tst = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (!tst.Has(textStyleName))
                {
                    //声明文字样式表记录
                    TextStyleTableRecord tstr = new TextStyleTableRecord();
                    tstr.Name = textStyleName;
                    //把新的文字样式表记录加入文字样式表
                    tst.UpgradeOpen();
                    tst.Add(tstr);
                    tr.AddNewlyCreatedDBObject(tstr, true);
                    tst.DowngradeOpen();
                }
                tr.Commit();
            }
        }
    }
}
