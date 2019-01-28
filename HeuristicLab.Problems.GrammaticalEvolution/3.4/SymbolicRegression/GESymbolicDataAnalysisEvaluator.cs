#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 * 
 * Author: Sabine Winkler
 */
#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [StorableType("3E723725-9141-4259-BB1D-BACE36657086")]
  public abstract class GESymbolicDataAnalysisEvaluator<T> : SingleSuccessorOperator,
    IGESymbolicDataAnalysisEvaluator<T>, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator, IStochasticOperator
  where T : class, IDataAnalysisProblemData {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string IntegerVectorParameterName = "IntegerVector";
    private const string GenotypeToPhenotypeMapperParameterName = "GenotypeToPhenotypeMapper";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";

    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EvaluationPartitionParameterName = "EvaluationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    private const string ValidRowIndicatorParameterName = "ValidRowIndicator";

    public override bool CanChangeName { get { return false; } }

    #region parameter properties
    ILookupParameter<IRandom> IStochasticOperator.RandomParameter {
      get { return RandomParameter; }
    }

    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<T> ProblemDataParameter {
      get { return (IValueLookupParameter<T>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters[IntegerVectorParameterName]; }
    }
    public ILookupParameter<IGenotypeToPhenotypeMapper> GenotypeToPhenotypeMapperParameter {
      get { return (ILookupParameter<IGenotypeToPhenotypeMapper>)Parameters[GenotypeToPhenotypeMapperParameterName]; }
    }
    public IValueLookupParameter<ISymbolicDataAnalysisGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicDataAnalysisGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }

    public IValueLookupParameter<IntRange> EvaluationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[EvaluationPartitionParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    public ILookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    public IValueLookupParameter<StringValue> ValidRowIndicatorParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters[ValidRowIndicatorParameterName]; }
    }
    #endregion


    [StorableConstructor]
    protected GESymbolicDataAnalysisEvaluator(StorableConstructorFlag _) : base(_) { }
    protected GESymbolicDataAnalysisEvaluator(GESymbolicDataAnalysisEvaluator<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public GESymbolicDataAnalysisEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic data analysis solution encoded as a symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<T>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new LookupParameter<IntegerVector>(IntegerVectorParameterName, "The symbolic data analysis solution encoded as an integer vector genome."));
      Parameters.Add(new LookupParameter<IGenotypeToPhenotypeMapper>(GenotypeToPhenotypeMapperParameterName, "Maps the genotype (an integer vector) to the phenotype (a symbolic expression tree)."));
      Parameters.Add(new ValueLookupParameter<ISymbolicDataAnalysisGrammar>(SymbolicExpressionTreeGrammarParameterName, "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));

      Parameters.Add(new ValueLookupParameter<IntRange>(EvaluationPartitionParameterName, "The start index of the dataset partition on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The upper and lower limit that should be used as cut off value for the output values of symbolic data analysis trees."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
      Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating."));
      Parameters.Add(new ValueLookupParameter<StringValue>(ValidRowIndicatorParameterName, "An indicator variable in the data set that specifies which rows should be evaluated (those for which the indicator <> 0) (optional)."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
  }
}
