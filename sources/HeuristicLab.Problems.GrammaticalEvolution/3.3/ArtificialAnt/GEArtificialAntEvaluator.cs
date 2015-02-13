#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ArtificialAnt;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [Item("GEArtificialAntEvaluator", "Evaluates an artificial ant solution for grammatical evolution.")]
  [StorableClass]
  public class GEArtificialAntEvaluator : SingleSuccessorOperator,
    ISingleObjectiveEvaluator, ISymbolicExpressionTreeGrammarBasedOperator {

    #region Parameter Properties
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    // genotype:
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }
    // phenotype:
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters["SymbolicExpressionTree"]; }
    }
    public ILookupParameter<BoolMatrix> WorldParameter {
      get { return (ILookupParameter<BoolMatrix>)Parameters["World"]; }
    }
    public ILookupParameter<IntValue> MaxTimeStepsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaxTimeSteps"]; }
    }
    public IValueLookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionGrammar>)Parameters["SymbolicExpressionTreeGrammar"]; }
    }
    // genotype-to-phenotype-mapper:
    public ILookupParameter<IGenotypeToPhenotypeMapper> GenotypeToPhenotypeMapperParameter {
      get { return (ILookupParameter<IGenotypeToPhenotypeMapper>)Parameters["GenotypeToPhenotypeMapper"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<IntMatrix> BoundsParameter {
      get { return (ILookupParameter<IntMatrix>)Parameters["Bounds"]; }
    }
    public ILookupParameter<IntValue> MaxExpressionLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaximumExpressionLength"]; }
    }
    #endregion

    [StorableConstructor]
    protected GEArtificialAntEvaluator(bool deserializing) : base(deserializing) { }
    protected GEArtificialAntEvaluator(GEArtificialAntEvaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new GEArtificialAntEvaluator(this, cloner); }
    public GEArtificialAntEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the evaluated artificial ant solution."));
      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The artificial ant solution encoded as an integer vector genome."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>("SymbolicExpressionTree", "The artificial ant solution encoded as a symbolic expression tree that should be evaluated"));
      Parameters.Add(new LookupParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items."));
      Parameters.Add(new LookupParameter<IntValue>("MaxTimeSteps", "The maximal number of time steps that the artificial ant should be simulated."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>("SymbolicExpressionTreeGrammar", "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));
      Parameters.Add(new LookupParameter<IGenotypeToPhenotypeMapper>("GenotypeToPhenotypeMapper", "Maps the genotype (an integer vector) to the phenotype (a symbolic expression tree)."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "Random number generator for the genotype creation and the genotype-to-phenotype mapping."));

      Parameters.Add(new LookupParameter<IntMatrix>("Bounds", "The integer number range in which the single genomes of a genotype are created."));
      Parameters.Add(new LookupParameter<IntValue>("MaximumExpressionLength", "Maximal length of the expression to control the artificial ant (genotype length)."));
    }

    public sealed override IOperation Apply() {
      SymbolicExpressionTree tree = GenotypeToPhenotypeMapperParameter.ActualValue.Map(
        RandomParameter.ActualValue,
        BoundsParameter.ActualValue,
        MaxExpressionLengthParameter.ActualValue.Value,
        SymbolicExpressionTreeGrammarParameter.ActualValue,
        IntegerVectorParameter.ActualValue
      );
      SymbolicExpressionTreeParameter.ActualValue = tree;
      BoolMatrix world = WorldParameter.ActualValue;
      IntValue maxTimeSteps = MaxTimeStepsParameter.ActualValue;

      AntInterpreter interpreter = new AntInterpreter();
      interpreter.MaxTimeSteps = maxTimeSteps.Value;
      interpreter.World = world;
      interpreter.Expression = tree;
      interpreter.Run();

      QualityParameter.ActualValue = new DoubleValue(interpreter.FoodEaten);
      return null;
    }
  }
}