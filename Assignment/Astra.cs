using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace Assignment
{
    class Astra
    {
        private int row;
        private int col;
        private int[,] iMap;
        private bool[,] bMap;
        private Vector2[] iOpenList;
        private const int initValue = 99999;

        public Astra()
        {
            row = col = 0;
        }
        public int[,] retMapInformation()
        {
            return iMap;
        }
        public int retRow()
        {
            return row;
        }
        public int retCol()
        {
            return col;
        }

        public void readMapInformation()
        {
            var path = @"Content\map.txt";
            int index = 0;

            using (var stream = TitleContainer.OpenStream(path))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (row == 0)
                    {
                        String[] strArr = line.ToString().Split('\t');
                        row = int.Parse(strArr[0]);
                        col = int.Parse(strArr[1]);
                        iMap = new int[row, col];
                    }
                    else
                    {
                        String[] strArr = line.ToString().Split('\t');
                        for (int c = 0; c < col; c++)
                        {
                            iMap[index, c] = (strArr[c].CompareTo("1") == 0 ? 1 : 0);
                        }
                        index++;
                    }
                }
            }

        }

        public void calcMapValue(Vector2 curPosition)
        {
            int ix = (int)curPosition.X;
            int iy = (int)curPosition.Y;

            iMap[ix, iy] = 0;

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    int icol = (int)curPosition.Y;
                    if (!bMap[r, c] && icol != 0 && iMap[r, c - 1] < iMap[r, c] + 1)
                    {
                        iMap[r, c - 1] = iMap[r, c] + 1;
                    }

                    if (!bMap[r, c] && icol != col - 1 && iMap[r, c + 1] < iMap[r, c] + 1)
                    {
                        iMap[r, c + 1] = iMap[r, c] + 1;
                    }

                    int irow = (int)curPosition.X;
                    if (!bMap[r, c] && irow != 0 && iMap[r - 1, c] < iMap[r, c] + 1)
                    {
                        iMap[r - 1, c] = iMap[r, c] + 1;
                    }

                    if (!bMap[r, c] && irow != row - 1 && iMap[r + 1, c] < iMap[r, c] + 1)
                    {
                        iMap[r + 1, c] = iMap[r, c] + 1;
                    }

                    /////////////////////////////
                    /*if (!bMap[r, c] && irow != 0 && icol != 0 && iMap[r - 1, c - 1] < iMap[r, c] + sqrt2)
                    {
                        iMap[r-1, c - 1] = iMap[r, c] + sqrt2;
                    }

                    if (!bMap[r, c] && irow != row - 1 && icol != 0 && iMap[r + 1, c - 1] < iMap[r, c] + sqrt2)
                    {
                        iMap[r+1, c - 1] = iMap[r,c] + sqrt2;
                    }

                    if (!bMap[r, c] && irow != 0 && icol != col - 1 && iMap[r - 1, c + 1] < iMap[r, c] + sqrt2)
                    {
                        iMap[r - 1, c + 1] = iMap[r, c] + sqrt2;
                    }

                    if (!bMap[r, c] && irow != row - 1 && icol != col - 1 && iMap[r + 1, c + 1] < iMap[r, c] + sqrt2)
                    {
                        iMap[r + 1, c + 1] = iMap[r, c] + sqrt2;
                    }
                    */
                    bMap[r, c] = true;
                }
            }


        }
    }
}
