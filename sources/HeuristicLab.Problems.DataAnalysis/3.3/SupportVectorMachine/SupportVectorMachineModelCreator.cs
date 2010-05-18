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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Threading;
using HeuristicLab.LibSVM;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using SVM;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine {
  /// <summary>
  /// Represents an operator that creates a support vector machine model.
  /// </summary>
  [StorableClass]
  [Item("SupportVectorMachineModelCreator", "Represents an operator that creates a support vector machine model.")]
  public class SupportVectorMachineModelCreator : SingleSuccessorOperator {
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string ModelParameterName = "SupportVectorMachineModel";

    #region parameter properties
    public IValueLookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public IValueParameter<StringValue> SvmTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IValueParameter<StringValue> KernelTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[KernelTypeParameterName]; }
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
    public ILookupParameter<SupportVectorMachineModel> SupportVectorMachineModelParameter {
      get { return (ILookupParameter<SupportVectorMachineModel>)Parameters[ModelParameterName]; }
    }
    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public StringValue SvmType {
      get { return SvmTypeParameter.Value; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.Value; }
    }
    public DoubleValue Nu {
      get { return NuParameter.ActualValue; }
    }
    public DoubleValue Cost {
      get { return CostParameter.ActualValue; }
    }
    public DoubleValue Gamma {
      get { return GammaParameter.ActualValue; }
    }
    #endregion

    public SupportVectorMachineModelCreator()
      : base() {
      #region svm types
      StringValue cSvcType = new StringValue("C_SVC").AsReadOnly();
      StringValue nuSvcType = new StringValue("NU_SVC").AsReadOnly();
      StringValue eSvrType = new StringValue("EPSILON_SVR").AsReadOnly();
      StringValue nuSvrType = new StringValue("NU_SVR").AsReadOnly();
      ItemSet<StringValue> allowedSvmTypes = new ItemSet<StringValue>();
      allowedSvmTypes.Add(cSvcType);
      allowedSvmTypes.Add(nuSvcType);
      allowedSvmTypes.Add(eSvrType);
      allowedSvmTypes.Add(nuSvrType);
      #endregion
      #region kernel types
      StringValue rbfKernelType = new StringValue("RBF").AsReadOnly();
      StringValue linearKernelType = new StringValue("LINEAR").AsReadOnly();
      StringValue polynomialKernelType = new StringValue("POLY").AsReadOnly();
      StringValue sigmoidKernelType = new StringValue("SIGMOID").AsReadOnly();
      ItemSet<StringValue> allowedKernelTypes = new ItemSet<StringValue>();
      allowedKernelTypes.Add(rbfKernelType);
      allowedKernelTypes.Add(linearKernelType);
      allowedKernelTypes.Add(polynomialKernelType);
      allowedKernelTypes.Add(sigmoidKernelType);
      #endregion
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for training."));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", allowedSvmTypes, nuSvrType));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", allowedKernelTypes, rbfKernelType));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC, one-class SVM and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC, epsilon-SVR and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function."));
      Parameters.Add(new LookupParameter<SupportVectorMachineModel>(ModelParameterName, "The result model generated by the SVM."));
    }

    public override IOperation Apply() {

      SupportVectorMachineModel model = TrainModel(DataAnalysisProblemData,
                             SvmType.Value, KernelType.Value,
                             Cost.Value, Nu.Value, Gamma.Value);
      SupportVectorMachineModelParameter.ActualValue = model;

      return base.Apply();
    }

    public static SupportVectorMachineModel TrainModel(
      DataAnalysisProblemData problemData,
      string svmType, string kernelType,
      double cost, double nu, double gamma) {
      int targetVariableIndex = problemData.Dataset.GetVariableIndex(problemData.TargetVariable.Value);

      //extract SVM parameters from scope and set them
      SVM.Parameter parameter = new SVM.Parameter();
      parameter.SvmType = (SVM.SvmType)Enum.Parse(typeof(SVM.SvmType), svmType, true);
      parameter.KernelType = (SVM.KernelType)Enum.Parse(typeof(SVM.KernelType), kernelType, true);
      parameter.C = cost;
      parameter.Nu = nu;
      parameter.Gamma = gamma;
      parameter.CacheSize = 500;
      parameter.Probability = false;


      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(problemData, problemData.TrainingSamplesStart.Value, problemData.TrainingSamplesEnd.Value);
      SVM.RangeTransform rangeTransform = SVM.RangeTransform.Compute(problem);
      SVM.Problem scaledProblem = Scaling.Scale(rangeTransform, problem);
      var model = new SupportVectorMachineModel();

      model.Model = SVM.Training.Train(scaledProblem, parameter);
      model.RangeTransform = rangeTransform;

      return model;
    }
  }
}
