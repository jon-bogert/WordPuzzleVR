using UnityEngine;

namespace WordPuzzle
{
    //public struct Vector2i
    //{
    //    public int x, y;
    //    public Vector2i(int X, int Y)
    //    {
    //        x = X;
    //        y = Y;
    //    }
    //}

    public class Letter
    {
        public char val;
        public Vector2Int pos;
    }

    public class Word
    {
        public Letter[] letters = null;
        public bool isHorizontal = true;

        public Word(string content, Vector2Int start, bool horiz)
        {
            int length = content.Length;
            letters = new Letter[length];
            isHorizontal = horiz;
            for (int i = 0; i < length; ++i)
            {
                letters[i] = new Letter();
                letters[i].val = content[i];
                letters[i].pos = start;
                if (horiz)
                    letters[i].pos.x += i;
                else
                    letters[i].pos.y += i;
            }
        }
        public Word()
        {
            letters = null;
            isHorizontal = true;
        }

        public override string ToString()
        {
            string result = "";
            foreach (Letter letter in letters)
            {
                result += letter.val;
            }
            return result;
        }
    }
}
