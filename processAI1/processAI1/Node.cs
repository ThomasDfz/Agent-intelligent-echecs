using System;
using System.Collections.Generic;

namespace processAI1
{
    public class Node
    {
        private List<String> piecesPositions;
        private List<int> piecesValues;
        
        public List<Node> Children(bool Player)
        {
            List<Node> children = new List<Node>();
            /*
                Ajouter tous les coups possibles et leurs noeud résultant
            */
            return children;
        }
 
        public bool IsCheckmate(bool Player)
        {
            /*    */
            return false;
        }
 
        //Score total de la situation actuelle pour l'heuristique
        public int GetTotalScore(bool Player)
        {
            int totalScore = 0;
            PieceValue pv = new PieceValue();
            foreach (var piece in piecesValues)
            {
                if (piece > 0) totalScore += pv.GetPieceValue(piece);
                else totalScore -= pv.GetPieceValue(piece);
            }
            if (!Player) totalScore *= -1;
            return totalScore;
        }
    }
}