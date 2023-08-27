using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{
    class Snake : IDraw
    {
        SnakeBody[] bodies;
        int nowIndex; //当前身体数
        E_Direction nowDir=E_Direction.Up; //当前方向

        public Snake(int x,int y)
        {
            bodies = new SnakeBody[200];
            bodies[0] = new SnakeBody(x, y, E_SnakeType.Head);
            nowIndex = 1;
        }

        public void Draw()
        {
            for(int i = 0; i < nowIndex; i++)
            {
                bodies[i].Draw();
            }  
        }

        public void Move()
        {
            SnakeBody last = bodies[nowIndex - 1];
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId lastId = ObjectId.Null;
            for(int i = 0; i < Main.brickRecord.Count; i++)
            {
                if (Main.brickRecord[i].pos == last.pos)
                {
                    lastId = Main.brickRecord[i].id;
                }
            }

            if(lastId != ObjectId.Null)
            {
                EditEntityTool.EraseEntity(db, lastId);
                UpdateTool.UpdateScreenEx(db, lastId);
            }
            

            for(int i = nowIndex - 1; i > 0; i--)
            {
                bodies[i].pos = bodies[i - 1].pos;
            }
            switch (nowDir)
            {
                case E_Direction.Up:
                    bodies[0].pos.y -= 10;
                    break;
                case E_Direction.Down:
                    bodies[0].pos.y += 10;
                    break;
                case E_Direction.Left:
                    bodies[0].pos.y -= 10;
                    break;
                case E_Direction.Right:
                    bodies[0].pos.y += 10;
                    break;
            }

        }

        public void ChangeDir(E_Direction dir)
        {
            if (nowIndex > 1 &&
                (nowDir == E_Direction.Left && dir == E_Direction.Right) ||
                (nowDir == E_Direction.Right && dir == E_Direction.Left) ||
                (nowDir == E_Direction.Down && dir == E_Direction.Up) ||
                (nowDir == E_Direction.Up && dir == E_Direction.Down)) { return; } //当前方向和要改变的方向相反就不改变方向
            nowDir = dir;
        }

    }

    enum E_Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
