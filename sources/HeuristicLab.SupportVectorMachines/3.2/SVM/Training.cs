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

namespace SVM
{
    /// <remarks>
    /// Class containing the routines to train SVM models.
    /// </remarks>
    public static class Training
    {
        private static double doCrossValidation(Problem problem, Parameter parameters, int nr_fold)
        {
            int i;
            double[] target = new double[problem.Count];
            Dictionary<int, double>[] confidence = new Dictionary<int, double>[problem.Count];
            Procedures.svm_cross_validation(problem, parameters, nr_fold, target, confidence);
            if (parameters.Probability)
            {
                List<RankPair> ranks = new List<RankPair>();
                for (i = 0; i < target.Length; i++)
                    ranks.Add(new RankPair(confidence[i][1], problem.Y[i]));
                PerformanceEvaluator eval = new PerformanceEvaluator(ranks);
                return eval.AuC*eval.AP;
            }
            else
            {
                int total_correct = 0;
                double total_error = 0;
                double sumv = 0, sumy = 0, sumvv = 0, sumyy = 0, sumvy = 0;
                if (parameters.SvmType == SvmType.EPSILON_SVR || parameters.SvmType == SvmType.NU_SVR)
                {
                    for (i = 0; i < problem.Count; i++)
                    {
                        double y = problem.Y[i];
                        double v = target[i];
                        total_error += (v - y) * (v - y);
                        sumv += v;
                        sumy += y;
                        sumvv += v * v;
                        sumyy += y * y;
                        sumvy += v * y;
                    }
                }
                else
                    for (i = 0; i < problem.Count; i++)
                        if (target[i] == problem.Y[i])
                            ++total_correct;
                return (double)total_correct / problem.Count;
            }

        }
        /// <summary>
        /// Legacy.  Allows use as if this was svm_train.  See libsvm documentation for details on which arguments to pass.
        /// </summary>
        /// <param name="args"></param>
        [Obsolete("Provided only for legacy compatibility, use the other Train() methods")]
        public static void Train(params string[] args)
        {
            Parameter parameters;
            Problem problem;
            bool crossValidation;
            int nrfold;
            string modelFilename;
            parseCommandLine(args, out parameters, out problem, out crossValidation, out nrfold, out modelFilename);
            if (crossValidation)
                PerformCrossValidation(problem, parameters, nrfold);
            else Model.Write(modelFilename, Train(problem, parameters));
        }

        /// <summary>
        /// Performs cross validation.
        /// </summary>
        /// <param name="problem">The training data</param>
        /// <param name="parameters">The parameters to test</param>
        /// <param name="nrfold">The number of cross validations to use</param>
        /// <returns>The cross validation score</returns>
        public static double PerformCrossValidation(Problem problem, Parameter parameters, int nrfold)
        {
            Procedures.svm_check_parameter(problem, parameters);
            return doCrossValidation(problem, parameters, nrfold);
        }

        /// <summary>
        /// Trains a model using the provided training data and parameters.
        /// </summary>
        /// <param name="problem">The training data</param>
        /// <param name="parameters">The parameters to use</param>
        /// <returns>A trained SVM Model</returns>
        public static Model Train(Problem problem, Parameter parameters)
        {
            Procedures.svm_check_parameter(problem, parameters);

            return Procedures.svm_train(problem, parameters);
        }

        private static void parseCommandLine(string[] args, out Parameter parameters, out Problem problem, out bool crossValidation, out int nrfold, out string modelFilename)
        {
            int i;

            parameters = new Parameter();
            // default values

            crossValidation = false;
            nrfold = 0;

            // parse options
            for (i = 0; i < args.Length; i++)
            {
                if (args[i][0] != '-')
                    break;
                ++i;
                switch (args[i - 1][1])
                {

                    case 's':
                        parameters.SvmType = (SvmType)int.Parse(args[i]);
                        break;

                    case 't':
                        parameters.KernelType = (KernelType)int.Parse(args[i]);
                        break;

                    case 'd':
                        parameters.Degree = int.Parse(args[i]);
                        break;

                    case 'g':
                        parameters.Gamma = double.Parse(args[i]);
                        break;

                    case 'r':
                        parameters.Coefficient0 = double.Parse(args[i]);
                        break;

                    case 'n':
                        parameters.Nu = double.Parse(args[i]);
                        break;

                    case 'm':
                        parameters.CacheSize = double.Parse(args[i]);
                        break;

                    case 'c':
                        parameters.C = double.Parse(args[i]);
                        break;

                    case 'e':
                        parameters.EPS = double.Parse(args[i]);
                        break;

                    case 'p':
                        parameters.P = double.Parse(args[i]);
                        break;

                    case 'h':
                        parameters.Shrinking = int.Parse(args[i]) == 1;
                        break;

                    case 'b':
                        parameters.Probability = int.Parse(args[i]) == 1;
                        break;

                    case 'v':
                        crossValidation = true;
                        nrfold = int.Parse(args[i]);
                        if (nrfold < 2)
                        {
                            throw new ArgumentException("n-fold cross validation: n must >= 2");
                        }
                        break;

                    case 'w':
                        ++parameters.WeightCount;
                        {
                            int[] old = parameters.WeightLabels;
                            parameters.WeightLabels = new int[parameters.WeightCount];
                            Array.Copy(old, 0, parameters.WeightLabels, 0, parameters.WeightCount - 1);
                        }
                        {
                            double[] old = parameters.Weights;
                            parameters.Weights = new double[parameters.WeightCount];
                            Array.Copy(old, 0, parameters.Weights, 0, parameters.WeightCount - 1);
                        }

                        parameters.WeightLabels[parameters.WeightCount - 1] = int.Parse(args[i - 1].Substring(2));
                        parameters.Weights[parameters.WeightCount - 1] = double.Parse(args[i]);
                        break;

                    default:
                        throw new ArgumentException("Unknown Parameter");
                }
            }

            // determine filenames

            if (i >= args.Length)
                throw new ArgumentException("No input file specified");

            problem = Problem.Read(args[i]);

            if (parameters.Gamma == 0)
                parameters.Gamma = 1.0 / problem.MaxIndex;

            if (i < args.Length - 1)
                modelFilename = args[i + 1];
            else
            {
                int p = args[i].LastIndexOf('/') + 1;
                modelFilename = args[i].Substring(p) + ".model";
            }
        }
    }
}