using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FEMprog
{
    public class Pallette
    {
        Color[] colors;
        double min;
        double max;
        private const int COUNTLOCALPALLETTE = 11;

        public Pallette(double min, double max)
        {
            List<Color> listColor = new List<Color>();
            int R = 125;
            int G = 0;
            int B = 0;
            //Marron, Red
            for (int i = 0; i <= 125; i++)
            {
                R = 125 + i;
                listColor.Add(Color.FromArgb(R, G, B));
            }
            //Red, Orange, Yellow
            for (int i = 1; i <= 250; i++)
            {
                G = i;
                listColor.Add(Color.FromArgb(R, G, B));
            }

            //Yellow, Salat, Green
            for (int i = 1; i <= 250; i++)
            {
                R = 250 - i;
                listColor.Add(Color.FromArgb(R, G, B));
            }
            //Green, Aqua, LightBlue
            for (int i = 1; i <= 250; i++)
            {
                B = i;
                listColor.Add(Color.FromArgb(R, G, B));
            }
            //LightBlue, Blue, Navy
            for (int i = 1; i <= 250; i++)
            {
                G = 250 - i;
                listColor.Add(Color.FromArgb(R, G, B));
            }
            //Navy, DarkBlue
            for (int i = 1; i <= 125; i++)
            {
                B = 250 - i;
                listColor.Add(Color.FromArgb(R, G, B));
            }
            listColor.Reverse();
            colors = listColor.ToArray();

            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// v1 < v2
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public List<Color> GetColorPallette(double v1, double v2)
        {
            List<Color> res = new List<Color>();

            if (min == max)
            {
                for (int i = 0; i < COUNTLOCALPALLETTE; i++)
                {
                    res.Add(colors[colors.Length / 2]);
                }
                return res;
            }
            int colorV1 = (int)((v1 - min) * colors.Length / (max - min));
            int colorV2 = (int)((v2 - min) * colors.Length / (max - min));
            if (colorV2 >= colors.Length)
                colorV2--;
            if (colorV1 >= colors.Length)
                colorV1--;

            int h = (int)((colorV2 - colorV1) / (double)(COUNTLOCALPALLETTE));

            for (int i = 0; i < COUNTLOCALPALLETTE - 1; i++)
            {
                res.Add(colors[colorV1 + i * h]);
            }
            res.Add(colors[colorV2]);

            return res;
        }

        public float[] GetRelativaPossition()
        {
            float[] relativePositions = { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };
            return relativePositions;
        }
    }

}
