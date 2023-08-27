using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{
     public class BrickRecord
    {
        public Position pos;
        public ObjectId id;
        public BrickRecord(Position pos,ObjectId id)
        {
            this.pos = pos;
            this.id = id;
        }
    }
}
