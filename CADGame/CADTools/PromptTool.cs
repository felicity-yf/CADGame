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
    public static partial class PromptTool
    {
        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="ed">Editor</param>
        /// <param name="promptString">提示字符串</param>
        /// <returns>点提示结果</returns>
        public static PromptPointResult GetPoint2(this Editor ed, string promptString)
        {
            //声明一个获取点的指示类;
            PromptPointOptions ppo = new PromptPointOptions(promptString);
            ppo.AllowNone = true;  //使回车和空格有效
            return ed.GetPoint(ppo);
        }
        /// <summary>
        /// 获取点或关键字
        /// </summary>
        /// <param name="ed">命令行</param>
        /// <param name="promptString">指示词</param>
        /// <param name="pointBase">基准点</param>
        /// <param name="keyWords">关键字</param>
        /// <returns>点提示结果</returns>
        public static PromptPointResult GetPoint(this Editor ed, string promptString, Point3d pointBase, params string[] keyWords)
        {
            PromptPointOptions ppo = new PromptPointOptions(promptString);
            ppo.AllowNone = true;
            //添加字符，使相应的字符有效
            for (int i = 0; i < keyWords.Length; i++)
            {
                ppo.Keywords.Add(keyWords[i]);
            }
            //取消系统自动的关键字显示
            ppo.AppendKeywordsToMessage = false;
            //设置基准点;
            ppo.BasePoint = pointBase;
            ppo.UseBasePoint = true;
            return ed.GetPoint(ppo);
        }
    }
}
