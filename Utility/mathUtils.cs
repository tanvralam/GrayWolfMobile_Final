using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Utility
{
    class RollingAverageDouble
    {

        bool IsReady()
        {
            return (GetCount(0) > 1);
        }

        private double[,] m_buffer;
        private double[] m_sum; // sum of each columb
        private int m_width; // how many columns
        private int m_depth; // how many datapoints for each column
        private int[] m_count; // count of points in each columne

        public RollingAverageDouble(int width, int depth) // Width is how many columns of data, depth is how many averages are used in calculation
        {
            m_width = width;
            m_depth = depth;
            m_buffer = new double[m_width, m_depth];
            m_sum = new double[m_width];
            m_count = new int[m_width];


            Initialize();
        }

        void Initialize()
        {
            for (int i = 0; i < m_width; i++)
            {
                m_sum[i] = 0.0;
                m_count[i] = 0;
                for (int j = 0; j < m_depth; j++)
                {
                    m_buffer[i, j] = 0.00;
                }
            }

        }

        public void AddValue(int col, double value)
        {
            if (col > m_width) return;

            int count = m_count[col];

            if (count < m_depth)
            {
                // adding
                m_buffer[col, count] = value;
                m_sum[col] += value;
                m_count[col] = count + 1;
            }
            else
            {
                // shifting
                m_count[col] = m_depth;
                m_sum[col] -= m_buffer[col, 0];
                // slide everything down
                for (int j = 0; j < m_depth - 1; j++)
                {
                    m_buffer[col, j] = m_buffer[col, (j + 1)];
                }
                m_buffer[col, (m_depth - 1)] = value;
                m_sum[col] += value;
            }
        }

        public double GetFiltered(int col)
        {
            if ((col < 0) || (col > m_width)) return 0;
            double count = (double)m_count[col];
            if (count == 0.00) return 0;
            return (m_sum[col] / count);
        }


        public int GetCount(int col)
        {
            if (col < 0) col = 0;
            return m_count[col];
        }


    }
}