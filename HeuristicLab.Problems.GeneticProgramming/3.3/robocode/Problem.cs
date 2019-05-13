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
 */
#endregion

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableType("0B4FE44B-3044-4531-8CA9-3C4D3BB3A4BB")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 360)]
  [Item("Robocode Problem", "Evolution of a robocode program in java using genetic programming.")]
  public class Problem : SymbolicExpressionTreeProblem {
    #region Parameter Names
    private const string RobocodePathParamaterName = "RobocodePath";
    private const string NrOfRoundsParameterName = "NrOfRounds";
    private const string EnemiesParameterName = "Enemies";
    #endregion

    #region Parameters
    public IFixedValueParameter<DirectoryValue> RobocodePathParameter {
      get { return (IFixedValueParameter<DirectoryValue>)Parameters[RobocodePathParamaterName]; }
    }
    public IFixedValueParameter<IntValue> NrOfRoundsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NrOfRoundsParameterName]; }
    }
    public IValueParameter<EnemyCollection> EnemiesParameter {
      get { return (IValueParameter<EnemyCollection>)Parameters[EnemiesParameterName]; }
    }

    public string RobocodePath {
      get { return RobocodePathParameter.Value.Value; }
      set { RobocodePathParameter.Value.Value = value; }
    }

    public int NrOfRounds {
      get { return NrOfRoundsParameter.Value.Value; }
      set { NrOfRoundsParameter.Value.Value = value; }
    }

    public EnemyCollection Enemies {
      get { return EnemiesParameter.Value; }
      set { EnemiesParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected Problem(StorableConstructorFlag _) : base(_) { }
    protected Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public Problem() : base(new SymbolicExpressionTreeEncoding()) {
      DirectoryValue robocodeDir = new DirectoryValue { Value = @"robocode" };

      var robotList = EnemyCollection.ReloadEnemies(robocodeDir.Value);
      robotList.RobocodePath = robocodeDir.Value;


      Parameters.Add(new FixedValueParameter<DirectoryValue>(RobocodePathParamaterName, "Path of the Robocode installation.", robocodeDir));
      Parameters.Add(new FixedValueParameter<IntValue>(NrOfRoundsParameterName, "Number of rounds a robot has to fight against each opponent.", new IntValue(3)));
      Parameters.Add(new ValueParameter<EnemyCollection>(EnemiesParameterName, "The enemies that should be battled.", robotList));

      Encoding.TreeLength = 1000;
      Encoding.TreeDepth = 10;
      Encoding.Grammar = new Grammar();
      Encoding.GrammarParameter.ReadOnly = true;
      Encoding.FunctionArguments = 0;
      Encoding.FunctionDefinitions = 0;

      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Problem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { RegisterEventHandlers(); }

    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      return Interpreter.EvaluateTankProgram(tree, RobocodePath, Enemies, null, false, NrOfRounds);
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[] qualities, ResultCollection results, IRandom random) {
      // find the tree with the best quality
      double maxQuality = double.NegativeInfinity;
      ISymbolicExpressionTree bestTree = null;
      for (int i = 0; i < qualities.Length; i++) {
        if (qualities[i] > maxQuality) {
          maxQuality = qualities[i];
          bestTree = trees[i];
        }
      }

      // create a solution instance
      var bestSolution = new Solution(bestTree, RobocodePath, NrOfRounds, Enemies);

      // also add the best solution as a result to the result collection
      // or alternatively update the existing result
      if (!results.ContainsKey("BestSolution")) {
        results.Add(new Result("BestSolution", "The best tank program", bestSolution));
      } else {
        results["BestSolution"].Value = bestSolution;
      }
    }

    public override bool Maximization {
      get { return true; }
    }

    private void RegisterEventHandlers() {
      RobocodePathParameter.Value.StringValue.ValueChanged += RobocodePathParameter_ValueChanged;
    }

    private void RobocodePathParameter_ValueChanged(object sender, System.EventArgs e) {
      EnemiesParameter.Value.RobocodePath = RobocodePathParameter.Value.Value;
    }
  }
}