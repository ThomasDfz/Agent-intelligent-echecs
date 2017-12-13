using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace processAI1
{
    public struct BoardStruct
    {
        public bool WhiteTrait;
        public ulong WhitePieces;
        public ulong BlackPieces; // free tiles = 0xFFFFFFFFFFFFFFFF ^ (WhitePieces | BlackPieces)
        public Dictionary<ulong, int> TypesMap;
    }
}
