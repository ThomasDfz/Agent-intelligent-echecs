using System;
using System.Collections.Generic;

namespace processAI1
{
    public class Node
    {
        public List<String> PiecesPositions = new List<string>();
        public BoardOpt BO;

        public Node(List<String> piecesPositions, BoardOpt bo)
        {
            PiecesPositions = piecesPositions;
            BO = bo;
        }

    }
}