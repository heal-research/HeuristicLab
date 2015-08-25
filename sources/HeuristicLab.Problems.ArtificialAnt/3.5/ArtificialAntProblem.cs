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
 */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.ArtificialAnt.Analyzers;

namespace HeuristicLab.Problems.ArtificialAnt {
  [Item("Artificial Ant Problem", "Represents the Artificial Ant problem.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 170)]
  [StorableClass]
  public sealed class ArtificialAntProblem : SingleObjectiveBasicProblem<SymbolicExpressionTreeEncoding>, IStorableContent {
    public string Filename { get; set; }

    #region constant for default world (Santa Fe)

    private static readonly char[][] santaFeAntTrail = new[] {
      " ###                            ".ToCharArray(),
      "   #                            ".ToCharArray(),
      "   #                    .###..  ".ToCharArray(),
      "   #                    #    #  ".ToCharArray(),
      "   #                    #    #  ".ToCharArray(),
      "   ####.#####       .##..    .  ".ToCharArray(),
      "            #       .        #  ".ToCharArray(),
      "            #       #        .  ".ToCharArray(),
      "            #       #        .  ".ToCharArray(),
      "            #       #        #  ".ToCharArray(),
      "            .       #        .  ".ToCharArray(),
      "            #       .        .  ".ToCharArray(),
      "            #       .        #  ".ToCharArray(),
      "            #       #        .  ".ToCharArray(),
      "            #       #  ...###.  ".ToCharArray(),
      "            .   .#...  #        ".ToCharArray(),
      "            .   .      .        ".ToCharArray(),
      "            #   .      .        ".ToCharArray(),
      "            #   #      .#...    ".ToCharArray(),
      "            #   #          #    ".ToCharArray(),
      "            #   #          .    ".ToCharArray(),
      "            #   #          .    ".ToCharArray(),
      "            #   .      ...#.    ".ToCharArray(),
      "            #   .      #        ".ToCharArray(),
      " ..##..#####.   #               ".ToCharArray(),
      " #              #               ".ToCharArray(),
      " #              #               ".ToCharArray(),
      " #     .#######..               ".ToCharArray(),
      " #     #                        ".ToCharArray(),
      " .     #                        ".ToCharArray(),
      " .####..                        ".ToCharArray(),
      "                                ".ToCharArray()
    };


    #endregion

    #region Parameter Properties
    public IValueParameter<BoolMatrix> WorldParameter {
      get { return (IValueParameter<BoolMatrix>)Parameters["World"]; }
    }
    public IValueParameter<IntValue> MaxTimeStepsParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumTimeSteps"]; }
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
    #endregion

    public override bool Maximization {
      get { return true; }
    }

    public ArtificialAntProblem()
      : base() {
      BoolMatrix world = new BoolMatrix(ToBoolMatrix(santaFeAntTrail));
      Parameters.Add(new ValueParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items.", world));
      Parameters.Add(new ValueParameter<IntValue>("MaximumTimeSteps", "The number of time steps the artificial ant has available to collect all food items.", new IntValue(600)));

      base.BestKnownQuality = 89;
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new string[] { "IfFoodAhead", "Prog2" }, 2, 2);
      g.AddSymbols(new string[] { "Prog3" }, 3, 3);
      g.AddTerminalSymbols(new string[] { "Left", "Right", "Move" });
      base.Encoding = new SymbolicExpressionTreeEncoding(g, 20, 10);

      InitializeOperators();
      RegisterEventHandlers();
    }


    public override double Evaluate(Individual individual, IRandom random) {
      var interpreter = new AntInterpreter(individual.SymbolicExpressionTree(), World, MaxTimeSteps.Value);
      interpreter.Run();
      return interpreter.FoodEaten;
    }

    // persistence
    [StorableConstructor]
    private ArtificialAntProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    // cloning 
    private ArtificialAntProblem(ArtificialAntProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArtificialAntProblem(this, cloner);
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
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
      Encoding.SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    private void InitializeOperators() {
      Operators.Add(new BestAntTrailAnalyzer());
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void ParameterizeAnalyzers() {
      foreach (var analyzer in Operators.OfType<BestAntTrailAnalyzer>()) {
        analyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        analyzer.SymbolicExpressionTreeParameter.ActualName = Encoding.SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        analyzer.WorldParameter.ActualName = WorldParameter.Name;
        analyzer.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
      }
    }

    private void ParameterizeOperators() {
      // no problem-specific operators to parameterize
    }

    private bool[,] ToBoolMatrix(char[][] ch) {
      var rows = ch.Length;
      var cols = ch[0].Length;
      var b = new bool[rows, cols];
      for (int r = 0; r < rows; r++) {
        Contract.Assert(ch[r].Length == cols); // all rows must have the same number of columns
        for (int c = 0; c < cols; c++) {
          b[r, c] = ch[r][c] == '#';
        }
      }
      return b;
    }
    #endregion
  }
}
