using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{
    class Map : IDraw
    {
        public Wall[] walls;
        public Map()
        {
            walls = new Wall[80+(20-1)*2];
            int index = 0;
            for(int i = -200; i < 200; i += 10)
            {
                walls[index] = new Wall(i, 100); //上面的墙
                index++;
                walls[index] = new Wall(i, -100); //下面的墙
                index++;
            }
            for(int i = -90; i < 100; i += 10)
            {
                walls[index] = new Wall(-200, i); //左
                index++;
                walls[index] = new Wall(190, i); //右
                index++;
            }


        }

        public void Draw()
        {
            for(int i = 0; i < walls.Length; i++)
            {
                walls[i].Draw();
            }
        }
    }
}
