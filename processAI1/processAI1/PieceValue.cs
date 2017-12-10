using System;

namespace processAI1
{
    public class PieceValue
    {
        public int GetPieceValue(int piece)
        {
            piece = Math.Abs(piece);
            int value = 0;
            switch (piece)
            {
                case 1:
                case 10:
                    value = 100;
                    break;
                case 21:
                case 22:
                    value = 510;
                    break;
                case 31:
                case 30:
                    value = 320;
                    break;
                case 4:
                    value = 333;
                    break;
                case 5:
                    value = 880;
                    break;
                case 6:
                    value = 9000;
                    break;
            }
            return value;
        }
    }
}