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

namespace SVM
{
    /// <remarks>
    /// Deals with the scaling of Problems so they have uniform ranges across all dimensions in order to
    /// result in better SVM performance.
    /// </remarks>
    public static class Scaling
    {
        /// <summary>
        /// Default lower bound for scaling (-1).
        /// </summary>
        public const int DEFAULT_LOWER_BOUND = -1;
        /// <summary>
        /// Default upper bound for scaling (1).
        /// </summary>
        public const int DEFAULT_UPPER_BOUND = 1;

        /// <summary>
        /// Determines the Range transform for the provided problem.  Uses the default lower and upper bounds.
        /// </summary>
        /// <param name="prob">The Problem to analyze</param>
        /// <returns>The Range transform for the problem</returns>
        public static RangeTransform DetermineRange(Problem prob)
        {
            return DetermineRangeTransform(prob, DEFAULT_LOWER_BOUND, DEFAULT_UPPER_BOUND);
        }
        /// <summary>
        /// Determines the Range transform for the provided problem.
        /// </summary>
        /// <param name="prob">The Problem to analyze</param>
        /// <param name="lowerBound">The lower bound for scaling</param>
        /// <param name="upperBound">The upper bound for scaling</param>
        /// <returns>The Range transform for the problem</returns>
        public static RangeTransform DetermineRangeTransform(Problem prob, double lowerBound, double upperBound)
        {
            double[] minVals = new double[prob.MaxIndex];
            double[] maxVals = new double[prob.MaxIndex];
            for (int i = 0; i < prob.MaxIndex; i++)
            {
                minVals[i] = double.MaxValue;
                maxVals[i] = double.MinValue;
            }
            for (int i = 0; i < prob.Count; i++)
            {
                for (int j = 0; j < prob.X[i].Length; j++)
                {
                    int index = prob.X[i][j].Index-1;
                    double value = prob.X[i][j].Value;
                    minVals[index] = Math.Min(minVals[index], value);
                    maxVals[index] = Math.Max(maxVals[index], value);
                }
            }
            for (int i = 0; i < prob.MaxIndex; i++)
            {
                if (minVals[i] == double.MaxValue || maxVals[i] == double.MinValue)
                {
                    minVals[i] = 0;
                    maxVals[i] = 0;
                }
            }
            return new RangeTransform(minVals, maxVals, lowerBound, upperBound);
        }
        /// <summary>
        /// Scales a problem using the provided range.  This will not affect the parameter.
        /// </summary>
        /// <param name="prob">The problem to scale</param>
        /// <param name="range">The Range transform to use in scaling</param>
        /// <returns>The Scaled problem</returns>
        public static Problem Scale(Problem prob, IRangeTransform range)
        {
            Problem scaledProblem = new Problem(prob.Count, new double[prob.Count], new Node[prob.Count][], prob.MaxIndex);
            for (int i = 0; i < scaledProblem.Count; i++)
            {
                scaledProblem.X[i] = new Node[prob.X[i].Length];
                for (int j = 0; j < scaledProblem.X[i].Length; j++)
                    scaledProblem.X[i][j] = new Node(prob.X[i][j].Index, range.Transform(prob.X[i][j].Value, prob.X[i][j].Index));
                scaledProblem.Y[i] = prob.Y[i];
            }
            return scaledProblem;
        }
    }
}
