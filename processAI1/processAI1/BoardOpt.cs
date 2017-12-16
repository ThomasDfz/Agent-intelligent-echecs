using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace processAI1
{
    public class BoardOpt
    {
        // pieces positions are stored on 64 bits (8 x 8) corresponding 
        // to cases of the board game
        public BoardStruct board; // todo : set private after tests

        private int typePieceToValue(int type)
        { // return value of a piece, if returned vlaue > 8.8F : chec
            int sign = (type < 0) ? -1 : 1;
            type = (type < 0) ? -type : type;

            switch (type)
            {
                case 1: // pion
                    return 100 * sign;
                case 10: // pion passant
                    return 100 * sign;
                case 21: // tour
                    return 510 * sign;
                case 22: // tour
                    return 510 * sign;
                case 31: // cavalier
                    return 320 * sign;
                case 32: // cavalier
                    return 320 * sign;
                case 4: // fou
                    return 330 * sign;
                case 5: // dame
                    return 880 * sign;
                case 6: // roi
                    return 1100 * sign;

                default:
                    return 0;
            }
        }
        private ulong[] playsToPosition(ulong actualPos, int[] p)
        {
            List<ulong> plays = new List<ulong>();
            foreach (int pl in p)
            {
                if (pl > 0)
                    plays.Add(actualPos << pl);
                else
                    plays.Add(actualPos >> -pl);
            }

            return plays.ToArray();
        }
        private ulong newPosition(ulong start, int col, int line)
        {
            ulong newPos = 0;
            if (col != 0)
            {
                int rs = getColumn(start) + col;
                if (rs <= 0 || rs > 8)
                    return 0; // out of board
            }
            if (line != 0)
            {
                int rs = getLine(start) + line;
                if (rs <= 0 || rs > 8)
                    return 0; // out of board
            }

            if (line >= 0)
                newPos = start << line;
            else
                newPos = start >> -line;

            if (col >= 0)
                newPos = newPos << col * 8;
            else
                newPos = newPos >> -col * 8;

            return newPos;
        }
        /* possible solutions methods */
        private void GetPossiblePositionPion(ulong position, out ulong[] sol, out int[] val)
        {  // ?? implements change piece ??
            List<ulong> solutions = new List<ulong>();
            List<int> values = new List<int>();

            List<ulong> newPositions = new List<ulong>();

            if (board.WhiteTrait)
            {
                ulong pos = newPosition(position, 0, 1);
                if ((pos & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0)
                {
                    solutions.Add(pos);
                    values.Add(0);
                }

                int[] playsCol = { -1, 1 };
                int[] playsLine = { 1, 1 };
                for (int i = 0; i < playsCol.Length; i++)
                {
                    newPositions.Add(newPosition(position, playsCol[i], playsLine[i]));
                    if ((newPositions[i] & board.BlackPieces) != 0)
                    {
                        solutions.Add(newPositions[i]);
                        values.Add(typePieceToValue(board.TypesMap[newPositions[i]]));
                    }
                }
            }
            else
            {
                ulong pos = newPosition(position, 0, -1);
                if ((pos & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0)
                {
                    solutions.Add(pos);
                    values.Add(0);
                }

                int[] playsCol = { -1, 1 };
                int[] playsLine = { -1, -1 };
                for (int i = 0; i < playsCol.Length; i++)
                {
                    newPositions.Add(newPosition(position, playsCol[i], playsLine[i]));
                    if ((newPositions[i] & board.WhitePieces) != 0)
                    {
                        solutions.Add(newPositions[i]);
                        values.Add(typePieceToValue(board.TypesMap[newPositions[i]]));
                    }
                }
            }

            sol = solutions.ToArray();
            val = values.ToArray();
        }

        private void GetPossiblePositionCavalier(ulong position, out ulong[] sol, out int[] val)
        {
            List<ulong> solutions = new List<ulong>();
            List<int> values = new List<int>();

            int[] playsCol = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] playsLine = { -1, 1, -2, 2, -2, 2, -1, 1 };

            for (int i = 0; i < playsCol.Length; i++)
            {
                ulong pos = newPosition(position, playsCol[i], playsLine[i]);

                if ((pos & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0)
                {
                    solutions.Add(pos);
                    values.Add(0);
                    continue;
                }
                if ((board.WhiteTrait && (pos & board.BlackPieces) != 0) || (!board.WhiteTrait && (pos & board.WhitePieces) != 0))
                {
                    solutions.Add(pos);
                    values.Add(typePieceToValue(board.TypesMap[pos]));
                }
            }

            sol = solutions.ToArray();
            val = values.ToArray();
        }

        private void GetPossiblePositionKing(ulong position, out ulong[] sol, out int[] val)
        {  // ?? implement roques ?? 
            List<ulong> solutions = new List<ulong>();
            List<int> values = new List<int>();

            int[] playsCol = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] playsLine = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < playsCol.Length; i++)
            {
                ulong pos = newPosition(position, playsCol[i], playsLine[i]);
                if ((pos & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0)
                {
                    solutions.Add(pos);
                    values.Add(0);
                    continue;
                }
                if ((board.WhiteTrait && (pos & board.BlackPieces) != 0) || (!board.WhiteTrait && (pos & board.WhitePieces) != 0))
                {
                    solutions.Add(pos);
                    values.Add(typePieceToValue(board.TypesMap[pos]));
                }
            }

            sol = solutions.ToArray();
            val = values.ToArray();
        }
        private void GetPossiblePositionTour(ulong position, out ulong[] sol, out int[] val)
        {
            List<ulong> solutions = new List<ulong>();
            List<int> values = new List<int>();

            List<ulong> newPositions = new List<ulong>();

            for (int dir = 0; dir < 4; dir++)
            {
                int left = 0;
                int up = 0;
                // gaves the direction of the search
                if (dir == 0)
                    up = 1;
                if (dir == 1)
                    up = -1;
                if (dir == 2)
                    left = -1;
                if (dir == 3)
                    left = 1;

                for (int i = 1; i <= 7; i++)
                {
                    ulong np = newPosition(position, i * left, i * up);
                    if (np != 0) // still in board
                    {
                        if ((np & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0) // in a free tile
                        {
                            solutions.Add(np);
                            values.Add(0);
                        }
                        else
                        {
                            if ((board.WhiteTrait && (np & board.BlackPieces) != 0) || (!board.WhiteTrait && (np & board.WhitePieces) != 0)) // on an adverse tile
                            {
                                solutions.Add(np);
                                values.Add(typePieceToValue(board.TypesMap[np]));
                            }
                            break; // a piece block the way
                        }
                    }
                    else
                        break; // out of board
                }
            }
            sol = solutions.ToArray();
            val = values.ToArray();
        }
        private void GetPossiblePositionFou(ulong position, out ulong[] sol, out int[] val)
        {
            List<ulong> solutions = new List<ulong>();
            List<int> values = new List<int>();

            List<ulong> newPositions = new List<ulong>();

            for (int dir = 0; dir < 4; dir++)
            {
                int left = 1;
                int up = 1;
                // gaves the direction of the search

                if (dir % 2 == 1)
                    up = -1;
                if (dir == 2)
                    left = -1;
                if (dir == 3)
                    left = -1;

                for (int i = 1; i <= 7; i++)
                {
                    ulong np = newPosition(position, i * left, i * up);
                    if (np != 0) // still in board
                    {
                        if ((np & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0) // in a free tile
                        {
                            solutions.Add(np);
                            values.Add(0);
                        }
                        else
                        {
                            if ((board.WhiteTrait && (np & board.BlackPieces) != 0) || (!board.WhiteTrait && (np & board.WhitePieces) != 0)) // on an adverse tile
                            {
                                solutions.Add(np);
                                values.Add(typePieceToValue(board.TypesMap[np]));
                            }
                            break; // a piece blocks the way
                        }
                    }
                    else
                        break; // out of board
                }
            }

            sol = solutions.ToArray();
            val = values.ToArray();
        }
        private void GetPossiblePositionQueen(ulong position, out ulong[] sol, out int[] val)
        {
            List<ulong> solutions = new List<ulong>();
            List<int> values = new List<int>();
            List<ulong> newPositions = new List<ulong>();

            for (int dir = 0; dir < 8; dir++)
            {
                int left = 1;
                int up = 1;
                // gaves the direction of the search

                if (dir < 4)
                { // move likes a fou
                    if (dir % 2 == 1)
                        up = -1;
                    if (dir == 2)
                        left = -1;
                    if (dir == 3)
                        left = -1;
                }
                else
                { // move likes a tour
                    left = 0;
                    up = 0;
                    // gaves the direction of the search
                    if (dir == 4)
                        up = 1;
                    if (dir == 5)
                        up = -1;
                    if (dir == 6)
                        left = -1;
                    if (dir == 7)
                        left = 1;
                }

                for (int i = 1; i <= 7; i++)
                {
                    ulong np = newPosition(position, i * left, i * up);
                    if (np != 0) // still in board
                    {
                        if ((np & (0xFFFFFFFFFFFFFFFF ^ (board.WhitePieces | board.BlackPieces))) != 0) // in a free tile
                        {
                            solutions.Add(np);
                            values.Add(0);
                        }
                        else
                        {
                            if ((board.WhiteTrait && (np & board.BlackPieces) != 0) || (!board.WhiteTrait && (np & board.WhitePieces) != 0)) // on an adverse tile
                            {
                                solutions.Add(np);
                                values.Add(typePieceToValue(board.TypesMap[np]));
                            }
                            break; // a piece block the way
                        }
                    }
                    else
                        break; // out of board
                }
            }
            sol = solutions.ToArray();
            val = values.ToArray();
        }
        public void GetPossiblePositions(ulong posPiece, out ulong[] solutions, out int[] values)
        {
            int typePiece = board.TypesMap[posPiece];

            board.WhiteTrait = (typePiece > 0);
            typePiece = (typePiece < 0) ? -typePiece : typePiece;

            switch (typePiece)
            {
                case 1: // pion 
                    GetPossiblePositionPion(posPiece, out solutions, out values);
                    break;
                case 4: // fou
                    GetPossiblePositionFou(posPiece, out solutions, out values);
                    break;
                case 5: // queen
                    GetPossiblePositionQueen(posPiece, out solutions, out values);
                    break;
                case 6: // king
                    GetPossiblePositionKing(posPiece, out solutions, out values);
                    break;
                case 10: // pion passant not discriminated
                    GetPossiblePositionPion(posPiece, out solutions, out values);
                    break;
                case 21: // tour
                    GetPossiblePositionTour(posPiece, out solutions, out values);
                    break;
                case 22: // tour
                    GetPossiblePositionTour(posPiece, out solutions, out values);
                    break;
                case 31: // cavalier
                    GetPossiblePositionCavalier(posPiece, out solutions, out values);
                    break;
                case 32: // cavalier
                    GetPossiblePositionCavalier(posPiece, out solutions, out values);
                    break;

                default:
                    solutions = null;
                    values = null;
                    break;
            }
            //Console.WriteLine("  ######  ");
        }

        public BoardStruct GetNewBoard(ulong start, ulong arrive)
        {
            BoardStruct bo = this.board;
            if (board.WhiteTrait)
            {
                bo.BlackPieces = bo.BlackPieces ^ arrive;
                bo.WhitePieces = (bo.WhitePieces | arrive) ^ start;
            }
            else
            {
                bo.WhitePieces = bo.WhitePieces ^ arrive;
                bo.BlackPieces = (bo.BlackPieces | arrive) ^ start;
            }
            bo.WhiteTrait = !bo.WhiteTrait;
            return bo;
        }

        /* constructors */
        public BoardOpt(BoardStruct bo)
        {
            this.board = bo;
            bo.WhitePieces = 0;
            bo.BlackPieces = 0;
        }
        public BoardOpt(string[] whitePieces, string[] blackPieces, string[] voidTiles, int[] whitePiecesT, int[] blackPiecesT, bool wp = true)
        {
            board.WhiteTrait = wp;

            board.TypesMap = new Dictionary<ulong, int>();
            foreach (string s in voidTiles)
                board.TypesMap[ConvertPositionStringToLong(s)] = 0;

            int n = 0;
            foreach (string s in whitePieces)
            {
                ulong pos = ConvertPositionStringToLong(s);
                this.board.WhitePieces = this.board.WhitePieces | pos;
                board.TypesMap[pos] = whitePiecesT[n];
                n++;
            }
            n = 0;
            foreach (string s in blackPieces)
            {
                ulong pos = ConvertPositionStringToLong(s);
                this.board.BlackPieces = this.board.BlackPieces | pos;
                board.TypesMap[pos] = blackPiecesT[n];
                n++;
            }
            if (board.TypesMap.Count != 64)
            {
                Console.WriteLine("didn't receive all the data from board, only " + board.TypesMap.Count + "/64 elements got.");
            }
        }

        /* convert functions*/
        private bool sameColumn(ulong p1, ulong p2)
        {
            if (getColumn(p1) == getColumn(p2))
                return true;
            return false;
        }
        private int getColumn(ulong p)
        {
            if (p == 0)
                Console.WriteLine("null position");
            int i = 1;
            while (p >> (i * 8) != 0 && (p >> (i * 8)) < p)
            {
                i++;
            }
            return i;
        }
        private int getLine(ulong p)
        {
            if (p == 0)
                Console.WriteLine("null position");

            int i = 1;
            p = p % 255;
            while (p >> i != 0 && (p >> i) < p)
                i++;

            return i;
        }
        public ulong ConvertPositionStringToLong(string s)
        {
            ulong piece = 1;
            short line = (short)s[0];
            line -= (short)'a';
            short row = (short)s[1];
            row -= (short)'1'; ;

            piece = piece << 8 * line;
            piece = piece << row;

            return piece;
        }
        public string ConvertPositionLongToString(ulong l)
        {
            if (l == 0)
                return "a1";

            string piece = "";

            short i = 0;
            while ((l >> i) != 1)
            {
                i++;
            }
            piece += (char)('a' + i / 8);
            piece += i % 8 + 1;

            return piece;
        }
        private ulong[] getPiecesInBoard(ulong b)
        { // careful, may have bugs
            List<ulong> listP = new List<ulong>();
            ulong p = 1;
            while(p <= b && (p << 1) >= p)
            {
                p = p << 1;
                if ((b & p) != 0)
                    listP.Add(p);
            }
            return listP.ToArray();
        }
        public String ToString()
        {
            String str = "Trait : ";
            str += board.WhiteTrait? "white" : "black";
            str += "\n White Pieces : \n";
            foreach (ulong pos in getPiecesInBoard(board.WhitePieces))
                str += $" {board.TypesMap[pos]} in {ConvertPositionLongToString(pos)} -";
            str += "\n Black pieces : \n";
            foreach (ulong pos in getPiecesInBoard(board.BlackPieces))
                str += $" {board.TypesMap[pos]} in {ConvertPositionLongToString(pos)} -";

            return str;
        }

    }
}
