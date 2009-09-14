#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Logging;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.SupportVectorMachines {
  public class TimeSeriesSupportVectorRegression : SupportVectorRegression, ITimeSeriesAlgorithm {

    public override string Name { get { return "SupportVectorRegression - Time Series Prognosis"; } }

    public int MaxTimeOffset {
      get { return GetVariableInjector().GetVariable("MaxTimeOffset").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTimeOffset").GetValue<IntData>().Data = value; }
    }

    public int MinTimeOffset {
      get { return GetVariableInjector().GetVariable("MinTimeOffset").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MinTimeOffset").GetValue<IntData>().Data = value; }
    }

    public TimeSeriesSupportVectorRegression()
      : base() {
      MaxTimeOffset = 0;
      MinTimeOffset = 0;
    }

    protected override IOperator CreateInitialization() {
      SequentialProcessor seq = new SequentialProcessor();
      seq.Name = "Initialization";
      seq.AddSubOperator(CreateGlobalInjector());
      ProblemInjector probInjector = new ProblemInjector();
      seq.AddSubOperator(probInjector);
      seq.AddSubOperator(new RandomInjector());
      return seq;
    }

    protected override VariableInjector CreateGlobalInjector() {
      VariableInjector injector = base.CreateGlobalInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTimeOffset", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MinTimeOffset", new IntData()));
      return injector;
    }

    protected override IOperator CreateModelAnalyser() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Model Analyzer";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(base.CreateModelAnalyser());
      SequentialSubScopesProcessor seqSubScopeProc = new SequentialSubScopesProcessor();
      SequentialProcessor seqProc = new SequentialProcessor();

      SupportVectorEvaluator trainingEvaluator = new SupportVectorEvaluator();
      trainingEvaluator.Name = "TrainingEvaluator";
      trainingEvaluator.GetVariableInfo("SVMModel").ActualName = "Model";
      trainingEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      trainingEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";

      SimpleTheilInequalityCoefficientEvaluator trainingTheilUCalculator = new SimpleTheilInequalityCoefficientEvaluator();
      trainingTheilUCalculator.Name = "TrainingTheilInequalityEvaluator";
      trainingTheilUCalculator.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingTheilUCalculator.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TrainingTheilInequalityCoefficient";
      SimpleTheilInequalityCoefficientEvaluator validationTheilUCalculator = new SimpleTheilInequalityCoefficientEvaluator();
      validationTheilUCalculator.Name = "ValidationTheilInequalityEvaluator";
      validationTheilUCalculator.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationTheilUCalculator.GetVariableInfo("TheilInequalityCoefficient").ActualName = "ValidationTheilInequalityCoefficient";
      SimpleTheilInequalityCoefficientEvaluator testTheilUCalculator = new SimpleTheilInequalityCoefficientEvaluator();
      testTheilUCalculator.Name = "TestTheilInequalityEvaluator";
      testTheilUCalculator.GetVariableInfo("Values").ActualName = "TestValues";
      testTheilUCalculator.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TestTheilInequalityCoefficient";

      seqProc.AddSubOperator(trainingEvaluator);
      seqProc.AddSubOperator(trainingTheilUCalculator);
      seqProc.AddSubOperator(validationTheilUCalculator);
      seqProc.AddSubOperator(testTheilUCalculator);

      seq.AddSubOperator(seqSubScopeProc);
      seqSubScopeProc.AddSubOperator(seqProc);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }


    protected override IAnalyzerModel CreateSVMModel(IScope bestModelScope) {
      IAnalyzerModel model = base.CreateSVMModel(bestModelScope);

      model.SetResult("TrainingTheilInequalityCoefficient", bestModelScope.GetVariableValue<DoubleData>("TrainingTheilInequalityCoefficient", false).Data);
      model.SetResult("ValidationTheilInequalityCoefficient", bestModelScope.GetVariableValue<DoubleData>("ValidationTheilInequalityCoefficient", false).Data);
      model.SetResult("TestTheilInequalityCoefficient", bestModelScope.GetVariableValue<DoubleData>("TestTheilInequalityCoefficient", false).Data);

      return model;
    }
  }
}
