#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Support vector machine classification data analysis algorithm.
  /// </summary>
  [Item("Support Vector Classification", "Support vector machine classification data analysis algorithm (wrapper for libSVM).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class SupportVectorClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";

    #region parameter properties
    public IValueParameter<StringValue> SvmTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IValueParameter<StringValue> KernelTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[KernelTypeParameterName]; }
    }
    public IValueParameter<DoubleValue> NuParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[NuParameterName]; }
    }
    public IValueParameter<DoubleValue> CostParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[CostParameterName]; }
    }
    public IValueParameter<DoubleValue> GammaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[GammaParameterName]; }
    }
    #endregion
    #region properties
    public StringValue SvmType {
      get { return SvmTypeParameter.Value; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.Value; }
    }
    public DoubleValue Nu {
      get { return NuParameter.Value; }
    }
    public DoubleValue Cost {
      get { return CostParameter.Value; }
    }
    public DoubleValue Gamma {
      get { return GammaParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    private SupportVectorClassification(bool deserializing) : base(deserializing) { }
    private SupportVectorClassification(SupportVectorClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public SupportVectorClassification()
      : base() {
      Problem = new ClassificationProblem();

      List<StringValue> svrTypes = (from type in new List<string> { "NU_SVC", "C_SVC" }
                                    select new StringValue(type).AsReadOnly())
                                   .ToList();
      ItemSet<StringValue> svrTypeSet = new ItemSet<StringValue>(svrTypes);
      List<StringValue> kernelTypes = (from type in new List<string> { "LINEAR", "POLY", "SIGMOID", "RBF" }
                                       select new StringValue(type).AsReadOnly())
                                   .ToList();
      ItemSet<StringValue> kernelTypeSet = new ItemSet<StringValue>(kernelTypes);
      Parameters.Add(new ConstrainedValueParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", svrTypeSet, svrTypes[0]));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", kernelTypeSet, kernelTypes[3]));
      Parameters.Add(new ValueParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC.", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function.", new DoubleValue(1.0)));
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorClassification(this, cloner);
    }

    #region support vector classification
    protected override void Run() {
      IClassificationProblemData problemData = Problem.ProblemData;
      IEnumerable<string> selectedInputVariables = problemData.AllowedInputVariables;
      var solution = CreateSupportVectorClassificationSolution(problemData, selectedInputVariables, SvmType.Value, KernelType.Value, Cost.Value, Nu.Value, Gamma.Value);

      Results.Add(new Result("Support vector classification solution", "The support vector classification solution.", solution));
    }

    public static SupportVectorClassificationSolution CreateSupportVectorClassificationSolution(IClassificationProblemData problemData, IEnumerable<string> allowedInputVariables,
      string svmType, string kernelType, double cost, double nu, double gamma) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndizes;

      //extract SVM parameters from scope and set them
      SVM.Parameter parameter = new SVM.Parameter();
      parameter.SvmType = (SVM.SvmType)Enum.Parse(typeof(SVM.SvmType), svmType, true);
      parameter.KernelType = (SVM.KernelType)Enum.Parse(typeof(SVM.KernelType), kernelType, true);
      parameter.C = cost;
      parameter.Nu = nu;
      parameter.Gamma = gamma;
      parameter.CacheSize = 500;
      parameter.Probability = false;

      foreach (double c in problemData.ClassValues) {
        double wSum = 0.0;
        foreach (double otherClass in problemData.ClassValues) {
          if (!c.IsAlmost(otherClass)) {
            wSum += problemData.GetClassificationPenalty(c, otherClass);
          }
        }
        parameter.Weights.Add((int)c, wSum);
      }


      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      SVM.RangeTransform rangeTransform = SVM.RangeTransform.Compute(problem);
      SVM.Problem scaledProblem = SVM.Scaling.Scale(rangeTransform, problem);
      var model = new SupportVectorMachineModel(SVM.Training.Train(scaledProblem, parameter), rangeTransform, targetVariable, allowedInputVariables, problemData.ClassValues);

      return new SupportVectorClassificationSolution(model, (IClassificationProblemData)problemData.Clone());
    }
    #endregion
  }
}
