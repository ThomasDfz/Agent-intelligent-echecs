using System;
using System.Collections.Generic;
using System.Linq;

namespace processAI2
{
    public class MiniMax
    {
        public Tuple<String, String> ComputeIntentions(Node node, int depth, int earns)
        {
            ulong[] positions = { };
            int[] values = { };
            int maxEarns = Int32.MinValue + 1;
            Tuple<String, String> bestMove = new Tuple<string, string>("", "");
            Random rnd = new Random();
            
            while(positions.Length <= 0)
            {
                bestMove = new Tuple<String, String>(node.PiecesPositions[rnd.Next(node.PiecesPositions.Count)], "");
                node.BO.GetPossiblePositions(node.BO.ConvertPositionStringToLong(bestMove.Item1), out positions, out values);
            }
            int randomMove = rnd.Next(positions.Length);
            maxEarns = values[randomMove];
            bestMove = new Tuple<string, string>(bestMove.Item1, node.BO.ConvertPositionLongToString(positions[randomMove]));

            List<int> earnsList = new List<int>();
            List<Tuple<String, String>> moveList = new List<Tuple<String, String>>();
            foreach (var piece in node.PiecesPositions)
            {
                node.BO.GetPossiblePositions(node.BO.ConvertPositionStringToLong(piece), out positions, out values);
                int index = 0;
                Console.WriteLine("Ma pièce en : " + piece + " peut aller en : ");
                foreach (var movement in positions)
                {
                    Console.WriteLine(node.BO.ConvertPositionLongToString(movement));
                    if (depth == 1)
                    {
                        if (Math.Abs(values[index]) > maxEarns)
                        {
                            bestMove = new Tuple<string, string>(piece, node.BO.ConvertPositionLongToString(movement));
                            maxEarns = Math.Abs(values[index]);
                        }
                    }
                    else
                    {
                        earnsList.Add(Math.Abs(values[index]));
                        moveList.Add(new Tuple<string, string>(piece, node.BO.ConvertPositionLongToString(movement)));
                        BoardStruct newBoardStruct = node.BO.GetNewBoard(node.BO.ConvertPositionStringToLong(piece), movement);
                        earnsList[earnsList.Count - 1] = earnsList[earnsList.Count - 1] - RecursiveMinMax(newBoardStruct, depth - 1, false);
                        index++;                        
                    }
                }
            }     
            if(depth == 1) return bestMove;
            int maxIndex = earnsList.IndexOf(earnsList.Max());
            if (earnsList.Max() == 0) maxIndex = rnd.Next(earnsList.Count);
            return moveList[maxIndex];
        }

        public int RecursiveMinMax(BoardStruct boardStruct, int depth, bool player)
        {
            BoardOpt newBoardOptions = new BoardOpt(boardStruct);
            int maxAdvEarns = Int32.MinValue + 1;
            List<String> advPieces = new List<string>();
            if (boardStruct.WhiteTrait)
            {
                //White ones
                foreach (var type in boardStruct.TypesMap)
                {
                    if (type.Value > 0)
                    {
                        advPieces.Add(newBoardOptions.ConvertPositionLongToString(type.Key));
                    }
                }
            }
            else
            {
                //Black ones
                foreach (var type in boardStruct.TypesMap)
                {
                    if (type.Value < 0)
                    {
                        advPieces.Add(newBoardOptions.ConvertPositionLongToString(type.Key));
                    }
                }   
            }
            List<int> earnsList = new List<int>();
            foreach (var advPiece in advPieces)
            {
                ulong[] advPositions = { };
                int[] advValues = { };
                newBoardOptions.GetPossiblePositions(newBoardOptions.ConvertPositionStringToLong(advPiece), out advPositions, out advValues);
                int advIndex = 0;
                foreach (var advMovement in advPositions)
                {
                    if (depth == 1 && Math.Abs(advValues[advIndex]) > maxAdvEarns)
                    {
                        maxAdvEarns = Math.Abs(advValues[advIndex]);
                    }
                    else
                    {
                        earnsList.Add(Math.Abs(advValues[advIndex]));
                        if (depth != 1 && advValues[advIndex] != 0)
                        {
                            BoardStruct newBoardStruct = newBoardOptions.GetNewBoard(newBoardOptions.ConvertPositionStringToLong(advPiece), advMovement);
                            if (player)
                            {
                                earnsList[earnsList.Count - 1] = earnsList[earnsList.Count - 1] + RecursiveMinMax(newBoardStruct, depth - 1, !player);
                            }
                            if (!player)
                            {
                                earnsList[earnsList.Count - 1] = earnsList[earnsList.Count - 1] - RecursiveMinMax(newBoardStruct, depth - 1, !player);
                            }
                        }
                        advIndex++;
                    }
                    
                }
            }
            if (depth == 1)
            {
                return maxAdvEarns;
            }
            return earnsList.Max();
        }
        
        //test
        /*public int Iterate2(Node node, int depth, int alpha, int beta, bool Player)
        {
            if (depth == 0 || node.IsCheckmate(Player))
            {
                return node.GetTotalScore(Player);
            }
 
            if (Player == MaxPlayer)
            {
                foreach (Node child in node.Children(Player))
                {
                    alpha = Math.Max(alpha, Iterate2(child, depth - 1, alpha, beta, !Player));
                    if (beta < alpha)
                    {
                        break;
                    }
 
                }

                return alpha;
            }
            else
            {
                foreach (Node child in node.Children(Player))
                {
                    beta = Math.Min(beta, Iterate2(child, depth - 1, alpha, beta, !Player));
 
                    if (beta < alpha)
                    {
                        break;
                    }
                }
                return beta;
            }
        }*/
    }
}