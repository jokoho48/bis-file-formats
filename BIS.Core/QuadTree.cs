﻿#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace BIS.Core
{
    public class QuadTree<TElement> : IEnumerable<TElement>
    {
        private const int logSizeX = 2;
        private const int logSizeY = 2;

        //static because it only depends on TElement
        private static int leafLogSizeX;
        private static int leafLogSizeY;

        private static Func<byte[], int, TElement> readElement;
        private static int elementSize;

        private readonly IEnumerable<TElement> allElementsEnumeration;

        /// <summary>
        ///     tells if the root is a leafNode or a tree
        /// </summary>
        private readonly bool flag;

        /// <summary>
        ///     root of the quadTree that represents the whole area
        /// </summary>
        private readonly IQuadTreeNode root;

        private int logSizeTotalX;
        private int logSizeTotalY;

        /// <summary>
        ///     how many virtual elements exist in X-dimension
        /// </summary>
        private int sizeTotalX;

        /// <summary>
        ///     how many virtual elements exist in Y-dimension
        /// </summary>
        private int sizeTotalY;

        /// <summary>
        ///     how many elements exist in X-dimension
        /// </summary>
        private int sizeX;

        /// <summary>
        ///     how many elements exist in Y-dimension
        /// </summary>
        private int sizeY;


        public QuadTree(int sizeX, int sizeY, BinaryReader input, Func<byte[], int, TElement> readElement,
            int elementSize)
        {
            QuadTree<TElement>.readElement = readElement;
            QuadTree<TElement>.elementSize = elementSize;

            CalculateDimensions(sizeX, sizeY);
            allElementsEnumeration = from y in Enumerable.Range(0, SizeY)
                from x in Enumerable.Range(0, SizeX)
                select Get(x, y);

            flag = input.ReadBoolean();

            if (flag)
            {
                root = new QuadTreeNode(input);
            }
            else
            {
                root = new QuadTreeLeaf(input);
            }
        }

        public int SizeX => sizeX;
        public int SizeY => sizeY;

        public IEnumerator<TElement> GetEnumerator()
        {
            return allElementsEnumeration.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allElementsEnumeration.GetEnumerator();
        }

        public TElement Get(int x, int y)
        {
            if (x < 0 || x >= sizeX)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= sizeY)
                throw new ArgumentOutOfRangeException("y");

            uint shiftedX =
                (uint) (x << (8 * sizeof(int) - logSizeTotalX)); // make highest bits accessible on left side
            uint shiftedY =
                (uint) (y << (8 * sizeof(int) - logSizeTotalY)); // make highest bits accessible on left side
            return flag ? ((QuadTreeNode) root).Get(x, y, shiftedX, shiftedY) : ((QuadTreeLeaf) root).Get(x, y);
        }

        private void CalculateDimensions(int x, int y)
        {
            sizeX = x;
            sizeY = y;

            x--;
            logSizeTotalX = 0;
            while (x != 0)
            {
                logSizeTotalX++;
                x >>= 1;
            }

            y--;
            logSizeTotalY = 0;
            while (y != 0)
            {
                logSizeTotalY++;
                y >>= 1;
            }

            switch (elementSize)
            {
                case 1:
                    leafLogSizeX = 1;
                    leafLogSizeY = 1;
                    break;
                case 2:
                    leafLogSizeX = 1;
                    leafLogSizeY = 0;
                    break;
                case 4:
                    leafLogSizeX = 0;
                    leafLogSizeY = 0;
                    break;

                default: throw new ArgumentException("Element size needs to be 1, 2 or 4");
            }

            // optimize _logSizeTotalX, _logSizeTotalY
            int numLevelsX = (logSizeTotalX - leafLogSizeX + logSizeX - 1) / logSizeX;
            int numLevelsY = (logSizeTotalY - leafLogSizeY + logSizeY - 1) / logSizeY;

            int numLevels = numLevelsX > numLevelsY ? numLevelsX : numLevelsY;

            logSizeTotalX = numLevels * logSizeX + leafLogSizeX;
            logSizeTotalY = numLevels * logSizeY + leafLogSizeY;
            sizeTotalX = 1 << logSizeTotalX;
            sizeTotalY = 1 << logSizeTotalY;

            // check if quad tree is well-formed
            Debug.Assert(sizeTotalX >= sizeX);
            Debug.Assert(sizeTotalY >= sizeY);
        }


        private interface IQuadTreeNode
        {
        }

        private class QuadTreeNode : IQuadTreeNode
        {
            private const int logSizeX = 2;
            private const int logSizeY = 2;

            private readonly IQuadTreeNode[] subTrees = new IQuadTreeNode[16];

            /// <summary>
            ///     bits determine if subTree is a leaf or not
            /// </summary>
            private short flag;

            public QuadTreeNode(BinaryReader input)
            {
                Read(input);
            }

            private void Read(BinaryReader input)
            {
                flag = input.ReadInt16();
                short bitMask = flag;
                for (int i = 0; i < 16; i++)
                {
                    if ((bitMask & 1) == 1)
                    {
                        subTrees[i] = new QuadTreeNode(input);
                    }
                    else
                    {
                        subTrees[i] = new QuadTreeLeaf(input);
                    }

                    bitMask >>= 1;
                }
            }

            public TElement Get(int x, int y, uint shiftedX, uint shiftedY)
            {
                // use (logSize) highest bits of shiftedX, shiftedX as indices
                uint indexX = shiftedX >> (8 * sizeof(int) - logSizeX);
                uint indexY = shiftedY >> (8 * sizeof(int) - logSizeY);
                // 2D to 1D array conversion
                int index = (int) ((indexY << logSizeX) + indexX);
                if ((flag & (1 << index)) != 0)
                {
                    // move shiftedX, shiftedY to make next bits available
                    return ((QuadTreeNode) subTrees[index]).Get(x, y, shiftedX << logSizeX, shiftedY << logSizeY);
                }

                // mask only lowest bits from the original x, y
                int maskX = (1 << leafLogSizeX) - 1;
                int maskY = (1 << leafLogSizeY) - 1;
                return ((QuadTreeLeaf) subTrees[index]).Get(x & maskX, y & maskY);
            }
        }

        private class QuadTreeLeaf : IQuadTreeNode
        {
            private static Func<byte[], int, int, TElement> getFunc;
            private readonly byte[] value;

            public QuadTreeLeaf(BinaryReader input)
            {
                if (getFunc == null)
                {
                    switch (elementSize)
                    {
                        case 1:
                            getFunc = (src, x, y) => readElement(src, 0);
                            break;
                        case 2:
                            getFunc = (src, x, y) => readElement(src, x * 2);
                            break;
                        case 4:
                            getFunc = (src, x, y) => readElement(src, (y << 1) + x);
                            break;
                    }
                }

                value = input.ReadBytes(4);
            }

            public TElement Get(int x, int y)
            {
                return getFunc(value, x, y);
            }
        }
    }
}