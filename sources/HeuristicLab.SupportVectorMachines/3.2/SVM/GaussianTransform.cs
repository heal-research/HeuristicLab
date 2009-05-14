/*
 * SVM.NET Library
 * Copyright (C) 2008 Matthew Johnson
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;
using System.IO;

namespace SVM
{
    /// <remarks>
    /// A transform which learns the mean and variance of a sample set and uses these to transform new data
    /// so that it has zero mean and unit variance.
    /// </remarks>
    public class GaussianTransform : IRangeTransform
    {
        private List<Node[]> _samples;
        private int _maxIndex;

        private double[] _means;
        private double[] _stddevs;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxIndex">The maximum index of the vectors to be transformed</param>
        public GaussianTransform(int maxIndex)
        {
            _samples = new List<Node[]>();
        }
        private GaussianTransform(double[] means, double[] stddevs, int maxIndex)
        {
            _means = means;
            _stddevs = stddevs;
            _maxIndex = maxIndex;
        }

        /// <summary>
        /// Adds a sample to the data.  No computation is performed.  The maximum index of the
        /// sample must be less than MaxIndex.
        /// </summary>
        /// <param name="sample">The sample to add</param>
        public void Add(Node[] sample)
        {
            _samples.Add(sample);
        }

        /// <summary>
        /// Computes the statistics for the samples which have been obtained so far.
        /// </summary>
        public void ComputeStatistics()
        {
            int[] counts = new int[_maxIndex];
            _means = new double[_maxIndex];
            foreach(Node[] sample in _samples)
            {
                for (int i = 0; i < sample.Length; i++)
                {
                    _means[sample[i].Index] += sample[i].Value;
                    counts[sample[i].Index]++;
                }
            }
            for (int i = 0; i < _maxIndex; i++)
            {
                if (counts[i] == 0)
                    counts[i] = 2;
                _means[i] /= counts[i];
            }

            _stddevs = new double[_maxIndex];
            foreach(Node[] sample in _samples)
            {
                for (int i = 0; i < sample.Length; i++)
                {
                    double diff = sample[i].Value - _means[sample[i].Index];
                    _stddevs[sample[i].Index] += diff * diff;
                }
            }
            for (int i = 0; i < _maxIndex; i++)
            {
                if (_stddevs[i] == 0)
                    continue;
                _stddevs[i] /= (counts[i]-1);
                _stddevs[i] = Math.Sqrt(_stddevs[i]);
            }
        }

        /// <summary>
        /// Saves the transform to the disk.  The samples are not stored, only the 
        /// statistics.
        /// </summary>
        /// <param name="stream">The destination stream</param>
        /// <param name="transform">The transform</param>
        public static void Write(Stream stream, GaussianTransform transform)
        {
            StreamWriter output = new StreamWriter(stream);
            output.WriteLine(transform._maxIndex);
            for (int i = 0; i < transform._maxIndex; i++)
                output.WriteLine("{0} {1}", transform._means[i], transform._stddevs[i]);
            output.Flush();
        }

        /// <summary>
        /// Reads a GaussianTransform from the provided stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The transform</returns>
        public static GaussianTransform Read(Stream stream)
        {
            StreamReader input = new StreamReader(stream);
            int length = int.Parse(input.ReadLine());
            double[] means = new double[length];
            double[] stddevs = new double[length];
            for (int i = 0; i < length; i++)
            {
                string[] parts = input.ReadLine().Split();
                means[i] = double.Parse(parts[0]);
                stddevs[i] = double.Parse(parts[1]);
            }
            return new GaussianTransform(means, stddevs, length);
        }

        /// <summary>
        /// Saves the transform to the disk.  The samples are not stored, only the 
        /// statistics.
        /// </summary>
        /// <param name="filename">The destination filename</param>
        /// <param name="transform">The transform</param>
        public static void Write(string filename, GaussianTransform transform)
        {
            FileStream output = File.Open(filename, FileMode.Create);
            try
            {
                Write(output, transform);
            }
            finally
            {
                output.Close();
            }
        }

        /// <summary>
        /// Reads a GaussianTransform from the provided stream.
        /// </summary>
        /// <param name="filename">The source filename</param>
        /// <returns>The transform</returns>
        public static GaussianTransform Read(string filename)
        {
            FileStream input = File.Open(filename, FileMode.Open);
            try
            {
                return Read(input);
            }
            finally
            {
                input.Close();
            }
        }

        #region IRangeTransform Members

        /// <summary>
        /// Transform the input value using the transform stored for the provided index.
        /// <see cref="ComputeStatistics"/> must be called first, or the transform must
        /// have been read from the disk.
        /// </summary>
        /// <param name="input">Input value</param>
        /// <param name="index">Index of the transform to use</param>
        /// <returns>The transformed value</returns>
        public double Transform(double input, int index)
        {
            if (_stddevs[index] == 0)
                return 0;
            double diff = input - _means[index];
            diff /= _stddevs[index];
            return diff;
        }
        /// <summary>
        /// Transforms the input array.  <see cref="ComputeStatistics"/> must be called 
        /// first, or the transform must have been read from the disk.
        /// </summary>
        /// <param name="input">The array to transform</param>
        /// <returns>The transformed array</returns>
        public Node[] Transform(Node[] input)
        {
            Node[] output = new Node[input.Length];
            for (int i = 0; i < output.Length; i++)
            {
                int index = input[i].Index;
                double value = input[i].Value;
                output[i] = new Node(index, Transform(value, index));
            }
            return output;
        }

        #endregion
    }
}
