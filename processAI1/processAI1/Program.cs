using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace processAI1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /* tests */
                string coor = "d4";

                string[] mesPieces = { coor };
                int[] myPiecesT = { 5 };

                string[] advPieces = { "e2", "g2", "f4" };
                int[] advPiecesT = { 1, 5, 1 };

                List<string> voidTiles = new List<string>();


                char[] tab = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

                for (int i = 1; i <= 8; i++)
                    foreach (char c in tab)
                        voidTiles.Add(c + i.ToString());

                BoardOpt BO = new BoardOpt(mesPieces, advPieces, voidTiles.ToArray(),
                                    myPiecesT, advPiecesT);

                ulong[] positions = { };
                int[] values = { };

                Random rnd = new Random();
                BO.GetPossiblePositions(BO.ConvertPositionStringToLong(coor), out positions, out values);

                int it = 0;
                Console.WriteLine("Moving from " + coor + " to :");
                if (positions.Length > 0)
                    foreach (var p in positions)
                        Console.WriteLine(BO.ConvertPositionLongToString(p) + " : " + values[it++]);
                else
                    Console.WriteLine("this piece cannot move");

                /* end test*/

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
                        using (var mmf2 = MemoryMappedFile.OpenExisting("repAI1"))
                        {
                            Mutex mutexStartAI1 = Mutex.OpenExisting("mutexStartAI1");
                            Mutex mutexAI1 = Mutex.OpenExisting("mutexAI1");
                            mutexAI1.WaitOne();

                            mutexStartAI1.WaitOne();

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
                                /*
                                List<String> mesPieces = new List<String>();
                                List<int> myPiecesT = new List<int>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] > 0)
                                    {
                                        mesPieces.Add(tabCoord[i]);
                                        myPiecesT.Add(tabVal[i]);
                                    }
                                }
                                List<String> advPieces = new List<String>();
                                List<int> advPiecesT = new List<int>();
                                for (int i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] < 0)
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
                                BoardOpt BO = new BoardOpt(mesPieces.ToArray(), advPieces.ToArray(), voidTiles.ToArray(),
                                    myPiecesT.ToArray(), advPiecesT.ToArray());

                                ulong[] positions = { };
                                float[] values = { };



                                Random rnd = new Random();

                                //while(positions.Length <= 0)
                                {
                                    //coord[0] = mesPieces[rnd.Next(mesPieces.Count)];
                                    coord[0] = "b1";
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
                                coord[2] = "P";

                                */
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
                            mutexAI1.ReleaseMutex();
                            mutexStartAI1.ReleaseMutex();
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
