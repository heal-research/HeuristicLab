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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.BinPacking3D {
  // in comparison to the 2d problem the 3d problem implementation also supports checking stacking constraints
  [StorableClass]
  public abstract class ProblemBase<TEnc, TSol> :
    SingleObjectiveBasicProblem<TEnc>, IProblemInstanceConsumer<BPPData>, IProblemInstanceExporter<BPPData>
    where TEnc : class, IEncoding
    where TSol : class, IItem {
    protected readonly string SolutionEvaluatorParameterName = "SolutionEvaluator";
    protected readonly string UseStackingConstraintsParameterName = "UseStackingConstraints";

    public readonly string EncodedSolutionName = "EncodedSolution";
    #region Default Instance
    private readonly BPPData defaultInstance = new BPPData() {
      Name = "3D BPP Default Instance",
      Description = "The default instance for 3D Bin Packing.",
      BinShape = new PackingShape(25, 25, 35),
      Items = new PackingItem[] {
        new PackingItem(12,5,10, new PackingShape(25,25,35)),
        new PackingItem(10,18,20, new PackingShape(25,25,35)),
        new PackingItem(9,7,7, new PackingShape(25,25,35)),
        new PackingItem(21,12,4, new PackingShape(25,25,35)),
        new PackingItem(8,8,12, new PackingShape(25,25,35)),
        new PackingItem(3,6,14, new PackingShape(25,25,35)),
        new PackingItem(20,4,9, new PackingShape(25,25,35)),
        new PackingItem(5,9,8, new PackingShape(25,25,35)),
        new PackingItem(7,17,3, new PackingShape(25,25,35)),
        new PackingItem(13,20,15, new PackingShape(25,25,35)),
        new PackingItem(9,11,9, new PackingShape(25,25,35)),
        new PackingItem(10,18,20, new PackingShape(25,25,35)),
        new PackingItem(9,7,7, new PackingShape(25,25,35)),
        new PackingItem(21,12,4, new PackingShape(25,25,35)),
        new PackingItem(8,8,12, new PackingShape(25,25,35)),
        new PackingItem(3,6,14, new PackingShape(25,25,35)),
        new PackingItem(20,4,9, new PackingShape(25,25,35)),
        new PackingItem(5,9,8, new PackingShape(25,25,35)),
        new PackingItem(7,17,3, new PackingShape(25,25,35)),
        new PackingItem(13,20,15, new PackingShape(25,25,35)),
        new PackingItem(9,11,9, new PackingShape(25,25,35)),
        new PackingItem(10,18,20, new PackingShape(25,25,35)),
        new PackingItem(9,7,7, new PackingShape(25,25,35)),
        new PackingItem(21,12,4, new PackingShape(25,25,35)),
        new PackingItem(8,8,12, new PackingShape(25,25,35)),
        new PackingItem(3,6,14, new PackingShape(25,25,35)),
        new PackingItem(20,4,9, new PackingShape(25,25,35)),
        new PackingItem(5,9,8, new PackingShape(25,25,35)),
        new PackingItem(7,17,3, new PackingShape(25,25,35)),
        new PackingItem(13,20,15, new PackingShape(25,25,35)),
        new PackingItem(9,11, 9,new PackingShape(25,25,35)),
        new PackingItem(10,18,20, new PackingShape(25,25,35)),
        new PackingItem(9,7,7, new PackingShape(25,25,35)),
        new PackingItem(21,12,4, new PackingShape(25,25,35)),
        new PackingItem(8,8,12, new PackingShape(25,25,35)),
        new PackingItem(3,6,14, new PackingShape(25,25,35)),
        new PackingItem(20,4,9, new PackingShape(25,25,35)),
        new PackingItem(5,9,8, new PackingShape(25,25,35)),
        new PackingItem(7,17,3, new PackingShape(25,25,35)),
        new PackingItem(13,20,15, new PackingShape(25,25,35)),
        new PackingItem(9,11,9, new PackingShape(25,25,35))
      },
    };
    #endregion

    #region parameter properties
    public IValueParameter<IDecoder<TSol>> DecoderParameter {
      get { return (IValueParameter<IDecoder<TSol>>)Parameters["Decoder"]; }
    }
    public IValueParameter<IEvaluator> SolutionEvaluatorParameter {
      get { return (IValueParameter<IEvaluator>)Parameters[SolutionEvaluatorParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UseStackingConstraintsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UseStackingConstraintsParameterName]; }
    }
    public IValueParameter<ReadOnlyItemList<PackingItem>> ItemsParameter {
      get { return (IValueParameter<ReadOnlyItemList<PackingItem>>)Parameters["Items"]; }
    }
    public IValueParameter<PackingShape> BinShapeParameter {
      get { return (IValueParameter<PackingShape>)Parameters["BinShape"]; }
    }
    public IValueParameter<Solution> BestKnownSolutionParameter {
      get { return (IValueParameter<Solution>)Parameters["BestKnownSolution"]; }
    }
    public IFixedValueParameter<IntValue> LowerBoundParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["LowerBound"]; }
    }
    #endregion

    #region properties
    public IDecoder<TSol> Decoder {
      get { return DecoderParameter.Value; }
      set { DecoderParameter.Value = value; }
    }
    public IEvaluator SolutionEvaluator {
      get { return SolutionEvaluatorParameter.Value; }
      set { SolutionEvaluatorParameter.Value = value; }
    }
    public bool UseStackingConstraints {
      get { return UseStackingConstraintsParameter.Value.Value; }
      set { UseStackingConstraintsParameter.Value.Value = value; }
    }
    public ReadOnlyItemList<PackingItem> Items {
      get { return ItemsParameter.Value; }
      set { ItemsParameter.Value = value; }
    }
    public PackingShape BinShape {
      get { return BinShapeParameter.Value; }
      set { BinShapeParameter.Value = value; }
    }
    public Solution BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    public int LowerBound {
      get { return LowerBoundParameter.Value.Value; }
    }
    public int NumberOfItems {
      get { return Items == null ? 0 : Items.Count; }
    }
    #endregion

    // persistence
    [StorableConstructor]
    protected ProblemBase(bool deserializing) : base(deserializing) { }

    // cloning
    protected ProblemBase(ProblemBase<TEnc, TSol> original, Cloner cloner)
      : base(original, cloner) {
    }

    protected ProblemBase()
      : base() {
      var defaultEvaluator = new PackingRatioEvaluator();
      Parameters.Add(new ValueParameter<IDecoder<TSol>>("Decoder", "The decoder translates a permutation to a packing solution candidiates"));
      Parameters.Add(new ValueParameter<IEvaluator>(SolutionEvaluatorParameterName, "The evaluator calculates qualities of solution candidates", defaultEvaluator));
      Parameters.Add(new FixedValueParameter<BoolValue>(UseStackingConstraintsParameterName, "A flag that determines if stacking constraints are considered when solving the problem.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<ReadOnlyItemList<PackingItem>>("Items", "The items which must be packed into bins"));
      Parameters.Add(new ValueParameter<PackingShape>("BinShape", "The size of bins into which items must be packed"));
      Parameters.Add(new OptionalValueParameter<Solution>("BestKnownSolution", "The best solution found so far"));
      Parameters.Add(new FixedValueParameter<IntValue>("LowerBound", "A lower bound for the number of bins that is necessary to pack all items"));

      Load(defaultInstance);
    }

    public override bool Maximization { get { return true; } }

    public override double Evaluate(Individual individual, IRandom random) {
      var encodedSolutionCand = (TSol)individual[EncodedSolutionName];
      var decoder = Decoder;
      var solution = decoder.Decode(encodedSolutionCand, BinShape, Items, UseStackingConstraints);
      return SolutionEvaluator.Evaluate(solution);
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);
      Analyze(individuals.Select(i => (TSol)i[EncodedSolutionName]).ToArray(), qualities, results, random);
    }

    // NOTE: same implementation as for 2d problem
    public virtual void Analyze(TSol[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      var bestSolutionResultName = "Best Packing Solution";
      var numContainersResultName = "Nr of Containers";
      var binUtilResultName = "Overall Bin Utilization";

      if (!results.ContainsKey(bestSolutionResultName)) results.Add(new Result(bestSolutionResultName, typeof(Solution)));
      if (!results.ContainsKey(numContainersResultName)) results.Add(new Result(numContainersResultName, typeof(IntValue)));
      if (!results.ContainsKey(binUtilResultName)) results.Add(new Result(binUtilResultName, typeof(DoubleValue)));


      // find index of item with max quality
      int bestIdx = 0;
      for (int j = 1; j < qualities.Length; j++)
        if (qualities[j] > qualities[bestIdx]) bestIdx = j;


      // update best solution so far
      var bestSolution = results[bestSolutionResultName].Value as Solution;
      if (bestSolution == null ||
        bestSolution.Quality.Value < qualities[bestIdx]) {

        var newBestSolution = Decoder.Decode(individuals[bestIdx], BinShape, Items, UseStackingConstraints);
        newBestSolution.Quality = new DoubleValue(qualities[bestIdx]);
        results[bestSolutionResultName].Value = newBestSolution;
        results[numContainersResultName].Value = new IntValue(newBestSolution.NrOfBins);
        results[binUtilResultName].Value = new DoubleValue(BinUtilizationEvaluator.CalculateBinUtilization(newBestSolution));

        // update best known solution
        var bestKnownQuality = BestKnownQualityParameter.Value;
        if (bestKnownQuality == null ||
            bestKnownQuality.Value < qualities[bestIdx]) {
          BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[bestIdx]);
          BestKnownSolutionParameter.ActualValue = newBestSolution;
        }
      }
    }


    #region Problem instance handling
    public void Load(BPPData data) {
      BestKnownSolutionParameter.Value = null;
      BestKnownQualityParameter.Value = null;

      if (data.BestKnownQuality.HasValue)
        BestKnownQuality = data.BestKnownQuality.Value;

      BinShape = data.BinShape;
      var items = new ItemList<PackingItem>(data.Items);
      items.Sort((x, y) => y.CompareTo(x));
      Items = items.AsReadOnly();

      ApplyHorizontalOrientation();
      LowerBoundParameter.Value.Value = CalculateLowerBound();
    }


    public BPPData Export() {
      return new BPPData {
        Name = Name,
        Description = Description,
        BinShape = BinShape,
        Items = Items.ToArray()
      };
    }
    #endregion


    #region helpers
    private void ApplyHorizontalOrientation() {
      BinShape.ApplyHorizontalOrientation();
      foreach (var shape in Items) {
        shape.ApplyHorizontalOrientation();
      }
    }

    private int CalculateLowerBound() {
      //This is the obvious continuous lower bound calculation; Martello and Vigo proposed a better way but it has not been implemented yet;
      int itemsVol = Items.Select(x => x.Volume).Sum();
      int binVol = BinShape.Volume;
      return (itemsVol + binVol - 1) / (binVol);
    }

    #endregion
  }
}
