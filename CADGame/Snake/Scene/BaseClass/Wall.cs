using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{
    class Wall : GameObject
    {
        public Wall(int x,int y)
        {
            pos.x = x;
            pos.y = y;
        }

        public override void Draw()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId brickId = db.InsertBlockReference(Brick.BlockId, new Point3d(pos.x, pos.y, 0));
            UpdateTool.UpdateScreenEx(db, brickId);
            BrickRecord brk = new BrickRecord(pos,brickId);
            Main.brickRecord.Add(brk);
        }
    }
}
