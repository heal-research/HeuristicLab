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

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorCreator : OperatorBase {
    private Thread trainingThread;
    private object locker = new object();
    private bool abortRequested = false;

    public override bool SupportsAbort {
      get {
        return true;
      }
    }

    public SupportVectorCreator()
      : base() {
      //Dataset infos
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("AllowedFeatures", "List of indexes of allowed features", typeof(ItemList<IntData>), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));

      //SVM parameters
      AddVariableInfo(new VariableInfo("SVMType", "String describing which SVM type is used. Valid inputs are: C_SVC, NU_SVC, ONE_CLASS, EPSILON_SVR, NU_SVR",
        typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMKernelType", "String describing which SVM kernel is used. Valid inputs are: LINEAR, POLY, RBF, SIGMOID, PRECOMPUTED",
        typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMCost", "Cost parameter (C) of C-SVC, epsilon-SVR and nu-SVR", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMNu", "Nu parameter of nu-SVC, one-class SVM and nu-SVR", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMGamma", "Gamma parameter in kernel function", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMModel", "Represent the model learned by the SVM", typeof(SVMModel), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SVMRangeTransform", "The applied transformation during the learning the model", typeof(SVMRangeTransform), VariableKind.New | VariableKind.Out));
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
      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>("AllowedFeatures", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;

      string svmType = GetVariableValue<StringData>("SVMType", scope, true).Data;
      string svmKernelType = GetVariableValue<StringData>("SVMKernelType", scope, true).Data;

      //extract SVM parameters from scope and set them
      SVM.Parameter parameter = new SVM.Parameter();
      parameter.SvmType = (SVM.SvmType)Enum.Parse(typeof(SVM.SvmType), svmType, true);
      parameter.KernelType = (SVM.KernelType)Enum.Parse(typeof(SVM.KernelType), svmKernelType, true);
      parameter.C = GetVariableValue<DoubleData>("SVMCost", scope, true).Data;
      parameter.Nu = GetVariableValue<DoubleData>("SVMNu", scope, true).Data;
      parameter.Gamma = GetVariableValue<DoubleData>("SVMGamma", scope, true).Data;

      SVM.Problem problem = SVMHelper.CreateSVMProblem(dataset, allowedFeatures, targetVariable, start, end);
      SVM.RangeTransform rangeTransform = SVM.Scaling.DetermineRange(problem);
      SVM.Problem scaledProblem = SVM.Scaling.Scale(problem, rangeTransform);

      SVM.Model model = StartTraining(scaledProblem, parameter);
      if (!abortRequested) {
        //persist variables in scope
        SVMModel modelData = new SVMModel();
        modelData.Data = model;
        scope.AddVariable(new Variable(scope.TranslateName("SVMModel"), modelData));
        SVMRangeTransform rangeTransformData = new SVMRangeTransform();
        rangeTransformData.Data = rangeTransform;
        scope.AddVariable(new Variable(scope.TranslateName("SVMRangeTransform"), rangeTransformData));
      }
      return null;
    }

    private SVM.Model StartTraining(SVM.Problem scaledProblem, SVM.Parameter parameter) {
      SVM.Model model = null;
      lock (locker) {
        if (!abortRequested) {
          trainingThread = new Thread(() => {
              model = SVM.Training.Train(scaledProblem, parameter);
          });
          trainingThread.Start();
        }
      }
      trainingThread.Join();
      trainingThread = null;
      return model;
    }
  }
}
