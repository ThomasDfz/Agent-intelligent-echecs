﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;


namespace processAI2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /* test */
                ulong voidTile = 0;


                /* end test */

                bool stop = false;
                int[] tabVal = new int[64];
                String value;
                String[] coord = new String[] { "", "", "" };
                String[] tabCoord = new string[] { "a8","b8","c8","d8","e8","f8","g8","h8",
                                                   "a7","b7","c7","d7","e7","f7","g7","h7",
                                                   "a6","b6","c6","d6","e6","f6","g6","h6",
                                                   "a5","b5","c5","d5","e5","f5","g5","h5",
                                                   "a4","b4","c4","d4","e4","f4","g4","h4",
                                                   "a3","b3","c3","d3","e3","f3","g3","h3",
                                                   "a2","b2","c2","d2","e2","f2","g2","h2",
                                                   "a1","b1","c1","d1","e1","f1","g1","h1" };
                
                while (!stop)
                {
                    using (var mmf = MemoryMappedFile.OpenExisting("plateau"))
                    {
                        using (var mmf2 = MemoryMappedFile.OpenExisting("repAI2"))
                        {
                            Mutex mutexStartAI2 = Mutex.OpenExisting("mutexStartAI2");
                            Mutex mutexAI2 = Mutex.OpenExisting("mutexAI2");
                            mutexAI2.WaitOne();
                            mutexStartAI2.WaitOne();

                            using (var accessor = mmf.CreateViewAccessor())
                            {
                                ushort Size = accessor.ReadUInt16(0);
                                byte[] Buffer = new byte[Size];
                                accessor.ReadArray(0 + 2, Buffer, 0, Buffer.Length);

                                value = ASCIIEncoding.ASCII.GetString(Buffer);
                                if (value == "stop") stop = true;
                                else
                                {
                                    String[] substrings = value.Split(',');
                                    for (int i = 0; i < substrings.Length; i++)
                                    {
                                        tabVal[i] = Convert.ToInt32(substrings[i]);
                                    }
                                }
                            }
                            if (!stop)
                            {
                                /******************************************************************************************************/
                                /***************************************** ECRIRE LE CODE DE L'IA *************************************/
                                /******************************************************************************************************/

                                
                                Stopwatch sw = Stopwatch.StartNew();
                                
                                /* get free tiles and pieces */
                                List<String> mesPieces = new List<String>();
                                List<int> myPiecesT = new List<int>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] < 0)
                                    {
                                        mesPieces.Add(tabCoord[i]);
                                        myPiecesT.Add(tabVal[i]);
                                    }
                                }
                                List<String> advPieces = new List<String>();
                                List<int> advPiecesT = new List<int>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] > 0)
                                    {
                                        advPieces.Add(tabCoord[i]);
                                        advPiecesT.Add(tabVal[i]);
                                    }
                                }
                                List<String> voidTiles = new List<String>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] == 0) voidTiles.Add(tabCoord[i]);
                                }
                                BoardOpt BO = new BoardOpt(advPieces.ToArray(), mesPieces.ToArray(), voidTiles.ToArray(),
                                    advPiecesT.ToArray(), myPiecesT.ToArray(), false);

                                /* verify if the last play has been done */
                                if (voidTile == (0xFFFFFFFFFFFFFFFF ^ (BO.board.WhitePieces | BO.board.BlackPieces)))
                                {
                                    Console.WriteLine("last play not done");
                                }
                                else
                                    voidTile = (0xFFFFFFFFFFFFFFFF ^ (BO.board.WhitePieces | BO.board.BlackPieces));
                                ulong[] positions = { };
                                int[] values = { };
                                
                                /*Is our king in danger ?*/
                               /* if (isCheck(mesPieces, myPiecesT, advPieces, advPiecesT))
                                {
                                    
                                }*/

                                /*Select best move according to minimax algorithm*/
                                Node actualChessboardBeliefs = new Node(mesPieces, BO);
                                MiniMax miniMax = new MiniMax();
                                int depth = 3; 
                                Tuple<String, String> intentions = miniMax.ComputeIntentions(actualChessboardBeliefs, depth, 0);
                                coord[0] = intentions.Item1;
                                coord[1] = intentions.Item2;
                                
                                /*int maxEarns = Int32.MinValue + 1;
                                foreach (var piece in mesPieces)
                                {
                                    BO.GetPossiblePositions(BO.ConvertPositionStringToLong(piece), out positions, out values);
                                    int index = 0;
                                    foreach (var movement in positions)
                                    {
                                        /*if (Math.Abs(values[index]) > maxEarns)
                                        {
                                            coord[0] = piece;
                                            coord[1] = BO.ConvertPositionLongToString(movement);
                                            maxEarns = Math.Abs(values[index]);
                                        }*/
                                        /*BoardStruct bs = BO.GetNewBoard(BO.ConvertPositionStringToLong(piece), movement);
                                        BoardOpt BO2 = new BoardOpt(bs);
                                        index++;
                                    }
                                }*/
                                
                                
                                /* take a random legal play */
 
                                /*Random rnd = new Random();
                                while(positions.Length <= 0)
                                {
                                    coord[0] = mesPieces[rnd.Next(mesPieces.Count)];
                                    BO.GetPossiblePositions(BO.ConvertPositionStringToLong(coord[0]), out positions, out values);

                                    Console.WriteLine("Moving from " + coord[0] + " to :");
                                    if (positions.Length > 0)
                                        foreach (var p in positions)
                                            Console.WriteLine(BO.ConvertPositionLongToString(p));
                                    else
                                        Console.WriteLine("this piece cannot move");
                                }
                                
                                coord[1] = BO.ConvertPositionLongToString(positions[rnd.Next(positions.Length)]);
                                
                                */
                                // create a new board with the last play
                                //BoardStruct bo = BO.GetNewBoard(BO.ConvertPositionStringToLong(coord[0]), BO.ConvertPositionStringToLong(coord[1]));
                                // create a new board object
                                //BoardOpt BO2 = new BoardOpt(bo);
                                //Console.WriteLine(BO.ToString());
                                //Console.WriteLine(BO2.ToString());
                                coord[2] = "D"; // always take a Queen if a pion cross the board
                                Console.WriteLine("TIME " + sw.ElapsedMilliseconds);

                                /*List<String> mesPieces = new List<String>();
                                List<int> myPiecesT = new List<int>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] < 0)
                                    {
                                        mesPieces.Add(tabCoord[i]);
                                        myPiecesT.Add(tabVal[i]);
                                    }
                                }
                                List<String> advPieces = new List<String>();
                                List<int> advPiecesT = new List<int>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] > 0)
                                    {
                                        advPieces.Add(tabCoord[i]);
                                        advPiecesT.Add(tabVal[i]);
                                    }
                                }
                                List<String> voidTiles = new List<String>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] == 0) voidTiles.Add(tabCoord[i]);
                                }
                                BoardOpt BO = new BoardOpt(advPieces.ToArray(), mesPieces.ToArray(), voidTiles.ToArray(),
                                    advPiecesT.ToArray(), myPiecesT.ToArray(), false);

                                if(voidTile == (0xFFFFFFFFFFFFFFFF ^ (BO.board.WhitePieces | BO.board.BlackPieces)))
                                {
                                    Console.WriteLine("last play not done");
                                }
                                else
                                    voidTile = (0xFFFFFFFFFFFFFFFF ^ (BO.board.WhitePieces | BO.board.BlackPieces));

                                ulong[] positions = { };
                                int[] values = { };


                                Random rnd = new Random();

                                while (positions.Length <= 0)
                                {
                                    coord[0] = mesPieces[rnd.Next(mesPieces.Count)];
                                    //coord[0] = "b1";
                                    BO.GetPossiblePositions(BO.ConvertPositionStringToLong(coord[0]), out positions, out values);

                                    Console.WriteLine("Moving from " + coord[0] + " to :");
                                    if (positions.Length > 0)
                                        foreach (var p in positions)
                                            Console.WriteLine(BO.ConvertPositionLongToString(p));
                                    else
                                        Console.WriteLine("this piece cannot move");
                                }
                                //coord[1] = "b3";
                                coord[1] = BO.ConvertPositionLongToString(positions[rnd.Next(positions.Length)]);
                                coord[2] = "D";*/


                                /********************************************************************************************************/
                                /********************************************************************************************************/
                                /********************************************************************************************************/

                                using (var accessor = mmf2.CreateViewAccessor())
                                {
                                    value = coord[0];
                                    for (int i = 1; i < coord.Length; i++)
                                    {
                                        value += "," + coord[i];
                                    }
                                    byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(value);
                                    accessor.Write(0, (ushort)Buffer.Length);
                                    accessor.WriteArray(0 + 2, Buffer, 0, Buffer.Length);
                                }
                            }
                            mutexAI2.ReleaseMutex();
                            mutexStartAI2.ReleaseMutex();
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first.");
                Console.ReadLine();
            }
        }
    }
}
