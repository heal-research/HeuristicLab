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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.GeneticProgramming.ArtificialAnt;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [Item("Grammatical Evolution Artificial Ant Problem (GE)", "Represents the Artificial Ant problem, implemented in Grammatical Evolution.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 170)]
  [StorableType("B6F0EBC4-FA3B-42E6-958F-404FA89C81FA")]
  public sealed class GEArtificialAntProblem : SingleObjectiveBasicProblem<IntegerVectorEncoding>, IStorableContent {

    #region Parameter Properties
    public IValueParameter<BoolMatrix> WorldParameter {
      get { return (IValueParameter<BoolMatrix>)Parameters["World"]; }
    }
    public IFixedValueParameter<IntValue> MaxTimeStepsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaximumTimeSteps"]; }
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
    public int MaxTimeSteps {
      get { return MaxTimeStepsParameter.Value.Value; }
      set { MaxTimeStepsParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private GEArtificialAntProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override bool Maximization {
      get { return true; }
    }

    [Storable]
    // parameters of the wrapped problem cannot be changed therefore it is not strictly necessary to clone and store it
    private readonly HeuristicLab.Problems.GeneticProgramming.ArtificialAnt.Problem wrappedAntProblem;

    private GEArtificialAntProblem(GEArtificialAntProblem original, Cloner cloner)
      : base(original, cloner) {
      this.wrappedAntProblem = cloner.Clone(original.wrappedAntProblem);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GEArtificialAntProblem(this, cloner);
    }

    public GEArtificialAntProblem()
      : base() {
      wrappedAntProblem = new HeuristicLab.Problems.GeneticProgramming.ArtificialAnt.Problem();
      Parameters.Add(new ValueParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items.", wrappedAntProblem.World));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumTimeSteps", "The number of time steps the artificial ant has available to collect all food items.", new IntValue(600)));
      Parameters.Add(new ValueParameter<IGenotypeToPhenotypeMapper>("GenotypeToPhenotypeMapper", "Maps the genotype (an integer vector) to the phenotype (a symbolic expression tree).", new DepthFirstMapper()));

      Encoding = new IntegerVectorEncoding(30) { Bounds = new IntMatrix(new int[,] { { 0, 100 } }) };

      BestKnownQuality = wrappedAntProblem.BestKnownQuality;
    }

    private readonly object syncRoot = new object();
    public override double Evaluate(Individual individual, IRandom random) {
      var vector = individual.IntegerVector();

      var bounds = Encoding.Bounds;
      var len = Encoding.Length;
      var grammar = wrappedAntProblem.Encoding.Grammar;
      var mapper = GenotypeToPhenotypeMapperParameter.Value;

      // Evaluate might be called concurrently therefore access to random has to be synchronized.
      // However, results depend on the order of execution. Therefore, results might be different for the same seed when using the parallel engine.
      IRandom fastRand;
      lock (syncRoot) {
        fastRand = new FastRandom(random.Next());
      }
      var tree = mapper.Map(fastRand, bounds, len, grammar, vector);

      Interpreter interpreter = new Interpreter(tree, World, MaxTimeSteps);
      interpreter.Run();

      return interpreter.FoodEaten;
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      var bounds = Encoding.Bounds;
      var len = Encoding.Length;
      var grammar = wrappedAntProblem.Encoding.Grammar;
      var mapper = GenotypeToPhenotypeMapperParameter.Value;

      var trees = individuals
        .Select(ind => mapper.Map(random, bounds, len, grammar, ind.IntegerVector()))
        .ToArray();

      wrappedAntProblem.Analyze(trees, qualities, results, random);
    }
  }
}