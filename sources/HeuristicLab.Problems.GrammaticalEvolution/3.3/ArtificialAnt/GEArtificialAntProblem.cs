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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.ArtificialAnt;
using HeuristicLab.Problems.ArtificialAnt.Analyzers;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [Item("Grammatical Evolution Artificial Ant Problem", "Represents the Artificial Ant problem, implemented in Grammatical Evolution.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 170)]
  [StorableClass]
  public sealed class GEArtificialAntProblem : SingleObjectiveHeuristicOptimizationProblem<GEArtificialAntEvaluator, IIntegerVectorCreator>, IStorableContent {
    public string Filename { get; set; }

    #region constant for default world (Santa Fe)
    private readonly bool[,] santaFeAntTrail = new bool[,] {
      {false, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, false}, 
      {false, false, false, true, true, true, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, true, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, true, true, true, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false}, 
      {false, false, false, true, true, false, false, true, true, true, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, false, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }
    };
    #endregion

    #region Parameter Properties
    public IValueParameter<ISymbolicExpressionGrammar> ArtificialAntExpressionGrammarParameter {
      get { return (IValueParameter<ISymbolicExpressionGrammar>)Parameters["ArtificialAntExpressionGrammar"]; }
    }
    public IValueParameter<IntValue> MaxExpressionLengthParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumExpressionLength"]; }
    }
    public IValueParameter<BoolMatrix> WorldParameter {
      get { return (IValueParameter<BoolMatrix>)Parameters["World"]; }
    }
    public IValueParameter<IntValue> MaxTimeStepsParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumTimeSteps"]; }
    }
    public IValueParameter<IntMatrix> BoundsParameter {
      get { return (IValueParameter<IntMatrix>)Parameters["Bounds"]; }
    }
    public IValueParameter<IGenotypeToPhenotypeMapper> GenotypeToPhenotypeMapperParameter {
      get { return (IValueParameter<IGenotypeToPhenotypeMapper>)Parameters["GenotypeToPhenotypeMapper"]; }
    }
    #endregion

    #region Properties
    public BoolMatrix World {
      get { return WorldParameter.Value; }
      set { WorldParameter.Value = value; }
    }
    public IntValue MaxTimeSteps {
      get { return MaxTimeStepsParameter.Value; }
      set { MaxTimeStepsParameter.Value = value; }
    }
    public IntValue MaxExpressionLength {
      get { return MaxExpressionLengthParameter.Value; }
      set { MaxExpressionLengthParameter.Value = value; }
    }
    public ArtificialAntExpressionGrammar ArtificialAntExpressionGrammar {
      get { return (ArtificialAntExpressionGrammar)ArtificialAntExpressionGrammarParameter.Value; }
    }
    public IEnumerable<IAntTrailAnalyzer> AntTrailAnalyzers {
      get { return Operators.OfType<IAntTrailAnalyzer>(); }
    }
    public IntMatrix Bounds {
      get { return BoundsParameter.Value; }
      set { BoundsParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private GEArtificialAntProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private GEArtificialAntProblem(GEArtificialAntProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GEArtificialAntProblem(this, cloner);
    }

    public GEArtificialAntProblem()
      : base(new GEArtificialAntEvaluator(), new UniformRandomIntegerVectorCreator()) {
      BoolMatrix world = new BoolMatrix(santaFeAntTrail);
      Parameters.Add(new ValueParameter<IntValue>("MaximumExpressionLength", "Maximal length of the expression to control the artificial ant (genotype length).", new IntValue(30)));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("ArtificialAntExpressionGrammar", "The grammar that should be used for artificial ant expressions.", new ArtificialAntExpressionGrammar()));
      Parameters.Add(new ValueParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items.", world));
      Parameters.Add(new ValueParameter<IntValue>("MaximumTimeSteps", "The number of time steps the artificial ant has available to collect all food items.", new IntValue(600)));
      IntMatrix m = new IntMatrix(new int[,] { { 0, 100 } });
      Parameters.Add(new ValueParameter<IntMatrix>("Bounds", "The integer number range in which the single genomes of a genotype are created.", m));
      Parameters.Add(new ValueParameter<IGenotypeToPhenotypeMapper>("GenotypeToPhenotypeMapper", "Maps the genotype (an integer vector) to the phenotype (a symbolic expression tree).", new DepthFirstMapper()));

      Maximization.Value = true;
      MaximizationParameter.Hidden = true;
      BestKnownQuality = new DoubleValue(89);

      SolutionCreator.IntegerVectorParameter.ActualName = "AntTrailSolutionIntegerVector";
      Evaluator.SymbolicExpressionTreeParameter.ActualName = "AntTrailSolutionTree";
      Evaluator.SymbolicExpressionTreeGrammarParameter.ActualName = "ArtificialAntExpressionGrammar";
      Evaluator.QualityParameter.ActualName = "FoodEaten";

      InitializeOperators();
      RegisterEventHandlers();
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();

      SolutionCreator.IntegerVectorParameter.ActualName = "AntTrailSolutionIntegerVector";
      SolutionCreator.IntegerVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_IntegerVectorParameter_ActualNameChanged);

      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();

      Evaluator.SymbolicExpressionTreeParameter.ActualName = "AntTrailSolutionTree";
      Evaluator.SymbolicExpressionTreeGrammarParameter.ActualName = "ArtificialAntExpressionGrammar";
      Evaluator.QualityParameter.ActualName = "FoodEaten";

      Evaluator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(Evaluator_SymbolicExpressionTreeParameter_ActualNameChanged);
      Evaluator.SymbolicExpressionTreeGrammarParameter.ActualNameChanged += new EventHandler(Evaluator_SymbolicExpressionTreeGrammarParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);

      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void SolutionCreator_IntegerVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void Evaluator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void Evaluator_SymbolicExpressionTreeGrammarParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    #endregion

    #region Helpers
    private void RegisterEventHandlers() {
      SolutionCreator.IntegerVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_IntegerVectorParameter_ActualNameChanged);
      Evaluator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(Evaluator_SymbolicExpressionTreeParameter_ActualNameChanged);
      Evaluator.SymbolicExpressionTreeGrammarParameter.ActualNameChanged += new EventHandler(Evaluator_SymbolicExpressionTreeGrammarParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IIntegerVectorOperator>().OfType<IOperator>());
      Operators.Add(new BestAntTrailAnalyzer());
      Operators.Add(new MinAverageMaxSymbolicExpressionTreeLengthAnalyzer());
      Operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void ParameterizeAnalyzers() {
      foreach (IAntTrailAnalyzer analyzer in AntTrailAnalyzers) {
        analyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        analyzer.SymbolicExpressionTreeParameter.ActualName = Evaluator.SymbolicExpressionTreeParameter.ActualName;
        analyzer.WorldParameter.ActualName = WorldParameter.Name;
        analyzer.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
      }
      foreach (ISymbolicExpressionTreeAnalyzer analyzer in Operators.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = Evaluator.SymbolicExpressionTreeParameter.ActualName;
      }
    }

    private void ParameterizeOperators() {
      var operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators);

      foreach (ISymbolicExpressionTreeGrammarBasedOperator op in operators.OfType<ISymbolicExpressionTreeGrammarBasedOperator>()) {
        op.SymbolicExpressionTreeGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
      }
      foreach (GEArtificialAntEvaluator op in operators.OfType<GEArtificialAntEvaluator>()) {
        op.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
        op.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
        op.WorldParameter.ActualName = WorldParameter.Name;
      }
      foreach (IIntegerVectorCrossover op in operators.OfType<IIntegerVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
      }
      foreach (IIntegerVectorManipulator op in operators.OfType<IIntegerVectorManipulator>()) {
        op.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
      }
      foreach (IIntegerVectorCreator op in operators.OfType<IIntegerVectorCreator>()) {
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.LengthParameter.ActualName = MaxExpressionLengthParameter.Name;
      }
    }
    #endregion
  }
}