#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Threading;
using SVM;

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorCreator : OperatorBase {
    private Thread trainingThread;
    private object locker = new object();
    private bool abortRequested = false;

    public SupportVectorCreator()
      : base() {
      //Dataset infos
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Name of the target variable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("InputVariables", "List of allowed input variable names", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "(optional) Maximal time offset for time-series prognosis", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "(optional) Minimal time offset for time-series prognosis", typeof(IntData), VariableKind.In));

      //SVM parameters
      AddVariableInfo(new VariableInfo("SVMType", "String describing which SVM type is used. Valid inputs are: C_SVC, NU_SVC, ONE_CLASS, EPSILON_SVR, NU_SVR",
        typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMKernelType", "String describing which SVM kernel is used. Valid inputs are: LINEAR, POLY, RBF, SIGMOID, PRECOMPUTED",
        typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMCost", "Cost parameter (C) of C-SVC, epsilon-SVR and nu-SVR", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMNu", "Nu parameter of nu-SVC, one-class SVM and nu-SVR", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMGamma", "Gamma parameter in kernel function", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMModel", "Represent the model learned by the SVM", typeof(SVMModel), VariableKind.New | VariableKind.Out));
    }

    public override void Abort() {
      abortRequested = true;
      lock (locker) {
        if (trainingThread != null && trainingThread.ThreadState == ThreadState.Running) {
          trainingThread.Abort();
        }
      }
    }

    public override IOperation Apply(IScope scope) {
      abortRequested = false;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      string targetVariable = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      ItemList inputVariables = GetVariableValue<ItemList>("InputVariables", scope, true);
      var inputVariableNames = from x in inputVariables
                               select ((StringData)x).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      IntData maxTimeOffsetData = GetVariableValue<IntData>("MaxTimeOffset", scope, true, false);
      int maxTimeOffset = maxTimeOffsetData == null ? 0 : maxTimeOffsetData.Data;
      IntData minTimeOffsetData = GetVariableValue<IntData>("MinTimeOffset", scope, true, false);
      int minTimeOffset = minTimeOffsetData == null ? 0 : minTimeOffsetData.Data;
      string svmType = GetVariableValue<StringData>("SVMType", scope, true).Data;
      string svmKernelType = GetVariableValue<StringData>("SVMKernelType", scope, true).Data;

      double cost = GetVariableValue<DoubleData>("SVMCost", scope, true).Data;
      double nu = GetVariableValue<DoubleData>("SVMNu", scope, true).Data;
      double gamma = GetVariableValue<DoubleData>("SVMGamma", scope, true).Data;

      SVMModel modelData = null;
      lock (locker) {
        if (!abortRequested) {
          trainingThread = new Thread(() => {
            modelData = TrainModel(dataset, targetVariable, inputVariableNames,
                                   start, end, minTimeOffset, maxTimeOffset,
                                   svmType, svmKernelType,
                                   cost, nu, gamma);
          });
          trainingThread.Start();
        }
      }
      if (!abortRequested) {
        trainingThread.Join();
        trainingThread = null;
      }


      if (!abortRequested) {
        //persist variables in scope
        scope.AddVariable(new Variable(scope.TranslateName("SVMModel"), modelData));
        return null;
      } else {
        return new AtomicOperation(this, scope);
      }
    }

    public static SVMModel TrainRegressionModel(
      Dataset dataset, string targetVariable, IEnumerable<string> inputVariables,
      int start, int end,
      double cost, double nu, double gamma) {
      return TrainModel(dataset, targetVariable, inputVariables, start, end, 0, 0, "NU_SVR", "RBF", cost, nu, gamma);
    }

    public static SVMModel TrainModel(
      Dataset dataset, string targetVariable, IEnumerable<string> inputVariables,
      int start, int end,
      int minTimeOffset, int maxTimeOffset,
      string svmType, string kernelType,
      double cost, double nu, double gamma) {
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);

      //extract SVM parameters from scope and set them
      SVM.Parameter parameter = new SVM.Parameter();
      parameter.SvmType = (SVM.SvmType)Enum.Parse(typeof(SVM.SvmType), svmType, true);
      parameter.KernelType = (SVM.KernelType)Enum.Parse(typeof(SVM.KernelType), kernelType, true);
      parameter.C = cost;
      parameter.Nu = nu;
      parameter.Gamma = gamma;
      parameter.CacheSize = 500;
      parameter.Probability = false;


      SVM.Problem problem = SVMHelper.CreateSVMProblem(dataset, targetVariableIndex, inputVariables, start, end, minTimeOffset, maxTimeOffset);
      SVM.RangeTransform rangeTransform = SVM.RangeTransform.Compute(problem);
      SVM.Problem scaledProblem = rangeTransform.Scale(problem);
      var model = new SVMModel();

      model.Model = SVM.Training.Train(scaledProblem, parameter);
      model.RangeTransform = rangeTransform;

      return model;
    }
  }
}
