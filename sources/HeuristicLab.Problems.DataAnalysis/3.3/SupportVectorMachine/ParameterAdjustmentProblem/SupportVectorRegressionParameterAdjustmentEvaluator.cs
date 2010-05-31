#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Parameters;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine.ParameterAdjustmentProblem {
  [Item("SupportVectorRegressionParameterAdjustmentEvaluator", "")]
  [StorableClass]
  public class SupportVectorRegressionParameterAdjustmentEvaluator : AlgorithmOperator, ISingleObjectiveEvaluator {
    private const string ParameterVectorParameterName = "ParameterVector";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string EpsilonParameterName = "Epsilon";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string NumberOfFoldsParameterName = "NumberOfFolds";
    private const string QualityParameterName = "Quality";

    #region parameter properties
    public ILookupParameter<RealVector> ParameterVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["ParameterVector"]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public IValueLookupParameter<StringValue> SvmTypeParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IValueLookupParameter<StringValue> KernelTypeParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters[KernelTypeParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> NuParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[NuParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> CostParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[CostParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> GammaParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[GammaParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> EpsilonParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[EpsilonParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IValueLookupParameter<IntValue> NumberOfFoldsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[NumberOfFoldsParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    #endregion
    #region properties
    public RealVector ParameterVector {
      get { return ParameterVectorParameter.ActualValue; }
    }
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public StringValue SvmType {
      get { return SvmTypeParameter.Value; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.Value; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
    }
    public IntValue NumberOfFolds {
      get { return NumberOfFoldsParameter.ActualValue; }
    }
    #endregion

    public SupportVectorRegressionParameterAdjustmentEvaluator()
      : base() {
      StringValue nuSvrType = new StringValue("NU_SVR").AsReadOnly();
      StringValue rbfKernelType = new StringValue("RBF").AsReadOnly();
      Parameters.Add(new LookupParameter<RealVector>(ParameterVectorParameterName, "The parameters for the SVM encoded as a real vector."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for training."));
      Parameters.Add(new ValueLookupParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", nuSvrType));
      Parameters.Add(new ValueLookupParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", rbfKernelType));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC, one-class SVM and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC, epsilon-SVR and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(EpsilonParameterName, "The value of the epsilon parameter of epsilon-SVR."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition the support vector machine should use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the data set partition the support vector machine should use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(NumberOfFoldsParameterName, "The number of folds to use for cross-validation."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The cross validation quality reached with the given parameters."));


      SupportVectorMachineCrossValidationEvaluator evaluator = new SupportVectorMachineCrossValidationEvaluator();
      OperatorGraph.InitialOperator = evaluator;
      evaluator.Successor = null;
    }

    public override IOperation Apply() {
      var point = ParameterVector;
      NuParameter.Value = new DoubleValue(point[0]);
      CostParameter.Value = new DoubleValue(Math.Pow(2, point[1]));
      GammaParameter.Value = new DoubleValue(Math.Pow(2, point[2]));
      EpsilonParameter.Value = new DoubleValue();

      return base.Apply();
    }
  }
}
