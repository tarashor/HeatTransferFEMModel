using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMprog
{
    public class Config
    {
        private const int defaultQuality = 3;

        private int quality;

        public int Quality
        {
            get { return quality; }
            set { quality = value; }
        }
        private bool isNumberedNode;

        public bool IsNumberedNode
        {
            get { return isNumberedNode; }
            set { isNumberedNode = value; }
        }
        private bool hasAdditionalPoint;

        public bool HasAdditionalPoint
        {
            get { return hasAdditionalPoint; }
            set { hasAdditionalPoint = value; }
        }

        public Config()
        {
            quality = defaultQuality;
            isNumberedNode = true;
            hasAdditionalPoint = true;
        }

        public Config(int quality, bool isNumberedNode, bool hasAdditionalPoint)
        {
            this.quality = quality;
            this.isNumberedNode = isNumberedNode;
            this.hasAdditionalPoint = hasAdditionalPoint;
        }

        public override string ToString()
        {
            return quality.ToString() + isNumberedNode.ToString() + hasAdditionalPoint.ToString();
        }

        public static Config Parse(string configStr)
        {
            string[] mas = configStr.Split();
            return new Config(int.Parse(mas[0]), bool.Parse(mas[1]), bool.Parse(mas[2]));
        }



    }
}
