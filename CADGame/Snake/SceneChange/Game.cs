using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{ 
    class Game
    {
        static ISceneUpdate nowScene = new GameScene();
        public Game() { }

        public void Start()
        {
            while (true)
            {
                if (nowScene != null)
                    nowScene.Update();
            }
        }

        //public static void ChangeScene(E_SceneType type)
        //{
        //    for(int i = 0; i < Main.brickRecord.Count; i++)
        //    {

        //    }
        //    switch (type)
        //    {
        //        case E_SceneType.Begin:
        //            nowScene = new BeginScene();
        //            break;
        //        case E_SceneType.Game:
        //            nowScene = new GameScene();
        //            break;
        //        case E_SceneType.End:
        //            nowScene = new EndScene();
        //            break;
        //    }
        //}

    }

    enum E_SceneType
    {
        Begin,
        Game,
        End
    }

}
