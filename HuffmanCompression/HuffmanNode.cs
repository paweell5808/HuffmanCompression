using System;

namespace HuffmanCompression
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public string symbol;
        public int frequency;
        public string code;
        public HuffmanNode parentNode;
        public HuffmanNode leftTree;
        public HuffmanNode rightTree;
        public bool isLeaf;

        public HuffmanNode(string value)
        {
            symbol = value;
            frequency = 1;

            rightTree = leftTree = parentNode = null;

            code = "";
            isLeaf = true;
        }

        public HuffmanNode(HuffmanNode node1, HuffmanNode node2)
        {
            code = "";
            isLeaf = false;
            parentNode = null;

            if (node1.frequency >= node2.frequency)
            {
                rightTree = node1;
                leftTree = node2;
                rightTree.parentNode = leftTree.parentNode = this;
                symbol = node1.symbol + node2.symbol;
                frequency = node1.frequency + node2.frequency;
            }
            else if (node1.frequency < node2.frequency)
            {
                rightTree = node2;
                leftTree = node1;
                leftTree.parentNode = rightTree.parentNode = this;
                symbol = node2.symbol + node1.symbol;
                frequency = node2.frequency + node1.frequency;
            }
        }

        public int CompareTo(HuffmanNode otherNode)
        {
            return this.frequency.CompareTo(otherNode.frequency);
        }
    }
}
