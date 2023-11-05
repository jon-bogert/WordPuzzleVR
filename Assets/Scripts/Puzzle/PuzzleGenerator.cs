using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace WordPuzzle
{
    public class PuzzleGenerator
    {
        class char_bool { public char val; public bool isUsed = false; public char_bool(char c) { val = c; } }
        enum Dir { Horiz, Vert };
        public string filePath = "words.txt";
        public Word[] Generate(int characterCount, out Vector2Int dimensions, bool threeAllowed = true)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Could not find file");
                dimensions = new Vector2Int();
                return null;
            }

            //Get All Words
            string[] words = File.ReadAllLines(filePath);

            System.Random rand = new System.Random();
            bool isCorrectLength = false;
            string baseWord = "";

            // Find a word with specified length
            while (!isCorrectLength)
            {
                int nextIndex = rand.Next(words.Length);
                baseWord = words[nextIndex];
                isCorrectLength = (baseWord.Length == characterCount);
            }

            // Setup character count Dictionary for lookup
            Dictionary<char, int> charInfo = new Dictionary<char, int>();

            foreach (char c in baseWord)
            {
                if (charInfo.ContainsKey(c))
                {
                    charInfo[c]++;
                    continue;
                }
                charInfo[c] = 1;
            }

            //Look through all the rest of the words
            List<string> validWords = new List<string> { baseWord };

            foreach (string w in words)
            {
                if (w == baseWord)
                    continue;
                if (!threeAllowed && w.Length <= 3)
                    continue;

                bool success = WordComp(new Dictionary<char, int>(charInfo), w);
                if (success)
                {
                    validWords.Add(w); 
                }
            }

            Console.WriteLine("Base Word: " + baseWord + "\n");
            for (int i = 1; i < validWords.Count; ++i)
            {
                Console.WriteLine(validWords[i]);
            }

            //Sort words by length
            validWords.RemoveAt(0);
            validWords = validWords.OrderByDescending(str => str.Length).ToList();

            //Setup Grid
            int iterCount = 25;
            int gridSize = 25;
            Dir startDir = (Dir)rand.Next(0, 2);
            Grid grid = new Grid();
            grid.SetSize(gridSize, gridSize);
            List<Word> finalWords = new List<Word>();

            //Add base word to center
            int iterX = (startDir == Dir.Horiz) ?
                (gridSize / 2) - baseWord.Length / 2 :
                gridSize / 2;
            int iterY = (startDir == Dir.Horiz) ?
                gridSize / 2 :
                (gridSize / 2) - baseWord.Length / 2;

            finalWords.Add(new Word(baseWord, new Vector2Int(iterX, iterY), (startDir) == Dir.Horiz));

            if (startDir == Dir.Horiz)
            {
                for (int i = 0; i < baseWord.Length; ++i)
                {
                    Grid.Cell c = grid.GetCell(iterX++, iterY);
                    c.val = baseWord[i];
                    c.connectHoriz = true;
                }
            }
            else
            {
                for (int i = 0; i < baseWord.Length; ++i)
                {
                    Grid.Cell c = grid.GetCell(iterX, iterY++);
                    c.val = baseWord[i];
                    c.connectVert = true;
                }
            }

            // Begin Slotting other words
            while (iterCount-- > 0 && validWords.Count > 0)
            {
                for (int i = 0; i < validWords.Count;)
                {
                    bool wasSuccess = false;
                    // store current word with bool flag
                    char_bool[] word = new char_bool[validWords[i].Length];
                    for (int c = 0; c < validWords[i].Length; ++c)
                    {
                        word[c] = new char_bool(validWords[i][c]);
                    }

                    //check each character
                    bool allVisited = false;
                    while (!allVisited)
                    {
                        //get a new character from word
                        bool isUsed = true;
                        char letter = '\0';
                        int index = 0;
                        while (isUsed)
                        {
                            index = rand.Next(word.Length);
                            isUsed = word[index].isUsed;
                            letter = word[index].val;
                        }
                        word[index].isUsed = true;

                        wasSuccess = CheckPlacement(gridSize, grid, word, letter, index, finalWords);

                        //check if all characters visited
                        allVisited = true;
                        if (!wasSuccess)
                        {
                            for (int c = 0; c < word.Length; ++c)
                            {
                                if (!word[c].isUsed)
                                    allVisited = false;
                            }
                        }
                    }

                    if (!wasSuccess)
                        ++i;
                    else
                        validWords.RemoveAt(i);
                }
            }

            //Grid Generated
            TrimCoords(finalWords, out int dimX, out int dimY);

            //Grid finalGrid = new Grid();
            //finalGrid.SetSize(dimX, dimY);
            //foreach (Word word in finalWords)
            //{
            //    foreach (Letter letter in word.letters)
            //    {
            //        finalGrid.GetCell(letter.pos.x, letter.pos.y).val = letter.val;
            //    }
            //}

            //for (int y = 0; y < dimY; ++y)
            //{
            //    for (int x = 0; x < dimX; ++x)
            //    {
            //        char val = finalGrid.GetCell(x, y).val;
            //        if (val == '\0')
            //            Console.Write(". ");
            //        else
            //            Console.Write(val.ToString() + " ");
            //    }
            //    Console.Write("\n");
            //}
            dimensions = new Vector2Int(dimX, dimY);

            return finalWords.ToArray();
        }

        private bool CheckPlacement(int gridSize, Grid grid, char_bool[] word, char letter, int index, List<Word> words)
        {
            for (int y = 0; y < gridSize; ++y)
            {
                for (int x = 0; x < gridSize; ++x)
                {
                    Grid.Cell cell = grid.GetCell(x, y);
                    if (cell.val == letter) // Potential cell
                    {
                        if (cell.connectHoriz && cell.connectVert) // already in both Dirs
                            continue;

                        // get direction word will travel
                        Dir dir = (cell.connectHoriz) ? Dir.Vert : Dir.Horiz;

                        // check neighbours
                        if (dir == Dir.Vert && x > 0)                   // check non-edge LEFT
                        {
                            if (grid.GetCell(x - 1, y).connectVert)
                                continue;
                        }
                        if (dir == Dir.Vert && x < gridSize - 1)        // check non-edge RIGHT
                        {
                            if (grid.GetCell(x + 1, y).connectVert)
                                continue;
                        }
                        if (dir == Dir.Horiz && y > 0)                   // check non-edge TOP
                        {
                            if (grid.GetCell(x, y - 1).connectHoriz)
                                continue;
                        }
                        if (dir == Dir.Horiz && y < gridSize - 1)        // check non-edge BOTTOM
                        {
                            if (grid.GetCell(x, y + 1).connectHoriz)
                                continue;
                        }

                        //check length of word
                        int iter = (dir == Dir.Horiz) ? x - 1 : y - 1;
                        bool isValid = true;
                        //front of word
                        for (int c = index - 1; c >= -1; --c) // one past
                        {
                            if (iter < 0)
                            {
                                isValid = false;
                                continue;
                            }
                            // is not off the board
                            Grid.Cell peek = (dir == Dir.Horiz) ? grid.GetCell(iter, y) : grid.GetCell(x, iter);
                            if (peek.val != '\0') // space is occupied
                            {
                                isValid = false;
                            }
                            //check cells to side
                            int side = (dir == Dir.Horiz) ? y - 1 : x - 1;
                            if (side >= 0) // Side -'ve (not offboard
                            {
                                peek = (dir == Dir.Horiz) ? grid.GetCell(iter, side) : grid.GetCell(side, iter);
                                if (peek.val != '\0')
                                    isValid = false;
                            } else isValid = false;
                            side = (dir == Dir.Horiz) ? y + 1 : x + 1;
                            if (side < gridSize) // Side +'ve (not off board)
                            {
                                peek = (dir == Dir.Horiz) ? grid.GetCell(iter, side) : grid.GetCell(side, iter);
                                if (peek.val != '\0')
                                    isValid = false;
                            } else isValid = false;
                            iter--;
                        }
                        //back of word
                        iter = (dir == Dir.Horiz) ? x + 1 : y + 1;
                        for (int c = index + 1; c <= word.Length; ++c) // one past
                        {
                            if (iter >= gridSize)
                            {
                                isValid = false;
                                continue;
                            }
                            // is not off the board
                            Grid.Cell peek = (dir == Dir.Horiz) ? grid.GetCell(iter, y) : grid.GetCell(x, iter);
                            if (peek.val != '\0') // space is occupied
                            {
                                isValid = false;
                            }
                            //check cells to side
                            int side = (dir == Dir.Horiz) ? y - 1 : x - 1;
                            if (side >= 0) // Side -'ve (not offboard
                            {
                                peek = (dir == Dir.Horiz) ? grid.GetCell(iter, side) : grid.GetCell(side, iter);
                                if (peek.val != '\0')
                                    isValid = false;
                            }
                            else isValid = false;
                            side = (dir == Dir.Horiz) ? y + 1 : x + 1;
                            if (side < gridSize) // Side +'ve (not off board)
                            {
                                peek = (dir == Dir.Horiz) ? grid.GetCell(iter, side) : grid.GetCell(side, iter);
                                if (peek.val != '\0')
                                    isValid = false;
                            }
                            else isValid = false;
                            iter++;
                        }

                        if (!isValid)
                            continue;

                        //Passed all checks
                        //Set current cell connection
                        if (dir == Dir.Horiz)
                            grid.GetCell(x, y).connectHoriz = true;
                        else
                            grid.GetCell(x, y).connectVert = true;

                        string w = "";
                        foreach (char_bool cb in word)
                            w += cb.val;

                        words.Add(new Word(w, (dir == Dir.Horiz) ? new Vector2Int(x - index, y) : new Vector2Int(x, y - index), (dir == Dir.Horiz)));

                        //stamp front of word
                        iter = (dir == Dir.Horiz) ? x - 1 : y - 1;
                        for (int c = index - 1; c >= 0; --c) // one past
                        {
                            Grid.Cell peek = (dir == Dir.Horiz) ? grid.GetCell(iter--, y) : grid.GetCell(x, iter--);
                            peek.val = word[c].val;
                            if (dir == Dir.Horiz)
                                peek.connectHoriz = true;
                            else
                                peek.connectVert = true;
                        }
                        //stamp back of word
                        iter = (dir == Dir.Horiz) ? x + 1 : y + 1;
                        for (int c = index + 1; c < word.Length; ++c) // one past
                        {
                            Grid.Cell peek = (dir == Dir.Horiz) ? grid.GetCell(iter++, y) : grid.GetCell(x, iter++);
                            peek.val = word[c].val;
                            if (dir == Dir.Horiz)
                                peek.connectHoriz = true;
                            else
                                peek.connectVert = true;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool WordComp(Dictionary<char, int> charInfo, string word)
        {
            foreach (char c in word)
            {
                if (!charInfo.ContainsKey(c))
                    return false;
                if (charInfo[c] <= 0)
                    return false;

                charInfo[c]--;
            }
            return true;
        }

        private void TrimCoords(List<Word> words, out int dimX, out int dimY)
        {
            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int max = new Vector2Int(0, 0);

            foreach(Word word in words)
            {
                foreach(Letter letter in word.letters)
                {
                    if (letter.pos.x < min.x)
                        min.x = letter.pos.x;
                    if (letter.pos.y < min.y)
                        min.y = letter.pos.y;

                    if (letter.pos.x > max.x)
                        max.x = letter.pos.x;
                    if (letter.pos.y > max.y)
                        max.y = letter.pos.y;
                }
            }
            foreach (Word word in words)
            {
                foreach (Letter letter in word.letters)
                {
                    letter.pos.x -= min.x;
                    letter.pos.y -= min.y;
                }
            }
            dimX = max.x - min.x + 1;
            dimY = max.y - min.y + 1;
        }
    }
}
