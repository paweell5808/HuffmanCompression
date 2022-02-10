using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HuffmanCompression
{
    public class Huffman
    {
        public List<byte> compressedBytes = new List<byte>();
        public List<string> decompressedBits = new List<string>();
        public long fileSize = 0;
        public int[] charFrequency;

        public List<HuffmanNode> getListFromFile(int[] file)
        {
            List<HuffmanNode> nodeList = new List<HuffmanNode>();
            try
            {
                for (int i = 0; i < file.Length; i++)
                {
                    string read = Convert.ToChar(file[i]).ToString();
                    if (nodeList.Exists(x => x.symbol == read))
                    {
                        int index = nodeList.FindIndex(y => y.symbol == read);
                        nodeList[index].frequency = charFrequency[file[i]];
                    }
                    else
                    {
                        HuffmanNode newItem = new HuffmanNode(read);
                        nodeList.Add(newItem);
                    }
                }
                fileSize = file.Length;
                nodeList.Sort();
                return nodeList;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void getTreeFromList(List<HuffmanNode> nodeList)
        {
            while (nodeList.Count > 1)
            {
                HuffmanNode node1 = nodeList[0];
                nodeList.RemoveAt(0);
                HuffmanNode node2 = nodeList[0];
                nodeList.RemoveAt(0);
                nodeList.Add(new HuffmanNode(node1, node2));
                nodeList.Sort();
            }
        }

        public void setCodeToTheTree(string code, HuffmanNode Nodes)
        {
            if (Nodes == null)
                return;
            if (Nodes.leftTree == null && Nodes.rightTree == null)
            {
                Nodes.code = code;
                return;
            }
            setCodeToTheTree(code + "0", Nodes.leftTree);
            setCodeToTheTree(code + "1", Nodes.rightTree);
        }

        private byte[] convertBitToBytes(int[] file, HuffmanNode node)
        {
            List<byte> resultBytes = new List<byte>();
            int byteIndex = 0, bitIndex = 0;
            byte[] bytes = new byte[1];

            for (int i = 0; i < file.Length; i++)
            {
                string read = Convert.ToChar(file[i]).ToString();
                string code = getCodeFromChar(node, read).code;

                var bitsToArray = new BitArray(code.Select(s => s == '1').ToArray());

                for (int c = 0; c < bitsToArray.Length; c++)
                {
                    if (bitsToArray[c])
                    {
                        bytes[0] |= (byte)(1 << (7 - bitIndex));
                    }

                    bitIndex++;
                    if (bitIndex == 8)
                    {
                        bitIndex = 0;
                        resultBytes.AddRange(bytes);
                        bytes = new byte[1];
                    }
                }
            }

            if (bitIndex > 0)
            {
                resultBytes.AddRange(bytes);
            }

            return resultBytes.ToArray();
        }

        public HuffmanNode getCodeFromChar(HuffmanNode nodeList, string searchChar)
        {
            if (nodeList != null)
            {
                if (nodeList.symbol == searchChar)
                {
                    return nodeList;
                }

                if (nodeList.rightTree.symbol.Contains(searchChar))
                {
                    return getCodeFromChar(nodeList.rightTree, searchChar);
                }
                else if (nodeList.leftTree.symbol.Contains(searchChar))
                {
                    return getCodeFromChar(nodeList.leftTree, searchChar);
                }

            }

            return nodeList;
        }

        public HuffmanNode getNodeFromBits(HuffmanNode nodeList, string searchBitsState)
        {
            if (searchBitsState.Length == 0 || nodeList.isLeaf == true)
            {
                return nodeList;
            }

            if (searchBitsState.Substring(0, 1) == "0")
            {
                return getNodeFromBits(nodeList.leftTree, searchBitsState.Remove(0, 1));
            }
            else
            {
                return getNodeFromBits(nodeList.rightTree, searchBitsState.Remove(0, 1));
            }
        }

        public void saveCompressedTree(int[] file, HuffmanNode node, string fileName)
        {
            File.WriteAllBytes(fileName, convertBitToBytes(file, node));
        }

        public void saveDecompressedTree(HuffmanNode nodeList, string fileName)
        {
            HuffmanNode searchedNode;
            string fullBits = String.Join("", decompressedBits);
            string fileResult = "";
            while (fullBits.Length > 0)
            {
                searchedNode = getNodeFromBits(nodeList, fullBits);
                if (searchedNode != null)
                {
                    fullBits = fullBits.Remove(0, searchedNode.code.Length);
                    fileResult += searchedNode.symbol;
                }
                if (fileResult.Length == fileSize)
                {
                    break;
                }
            }

            File.WriteAllText(fileName, fileResult);
        }

        public int[] getIntsArrayFromFile(string path)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                List<int> intsList = new List<int>();
                for (int i = 0; i < stream.Length; i++)
                {
                    int readAsInt = Convert.ToInt32(stream.ReadByte());
                    intsList.Add(readAsInt);
                }

                int[] intsArray = intsList.ToArray();
                int length = intsArray.Length;

                return intsArray;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void convertByteToBits(byte[] bytes)
        {
            foreach (byte element in bytes)
            {
                decompressedBits.Add(Convert.ToString(element, 2).PadLeft(8, '0'));
            }
        }

        public int[] countCharFrequency(int[] file, int[] resultFrequency)
        {
            foreach (var bytes in file)
            {
                resultFrequency[bytes] += 1;
            }

            return resultFrequency;
        }
    }
}
