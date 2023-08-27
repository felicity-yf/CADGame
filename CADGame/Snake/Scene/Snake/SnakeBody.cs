using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{
    class SnakeBody:GameObject
    {
        E_SnakeType nowType;

        public SnakeBody(int x,int y, E_SnakeType type)
        {
            pos.x = x;
            pos.y = y;
            nowType = type;
        }

        public override void Draw()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId bodyId = nowType == E_SnakeType.Head ? SnakeHeadBrick.BlockId : SnakeBodyBrick.BlockId; //根据E_SnakeType给出方块
            ObjectId brickId = db.InsertBlockReference(bodyId, new Point3d(pos.x, pos.y, 0));
            UpdateTool.UpdateScreenEx(db, brickId);
            BrickRecord brk = new BrickRecord(pos, brickId);
            Main.brickRecord.Add(brk);
        }
    }
    enum E_SnakeType
    {
        Head,
        Body
    }
}
