#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Represents the frequency of an allele.
  /// </summary>
  [Item("AlleleFrequency", "Represents the frequency of an allele.")]
  [StorableClass]
  public class AlleleFrequency : Item {
    private string id;
    public string Id {
      get { return id; }
    }
    private double frequency;
    public double Frequency {
      get { return frequency; }
    }
    private double averageImpact;
    public double AverageImpact {
      get { return averageImpact; }
    }
    private double averageSolutionQuality;
    public double AverageSolutionQuality {
      get { return averageSolutionQuality; }
    }
    private bool containedInBestKnownSolution;
    public bool ContainedInBestKnownSolution {
      get { return containedInBestKnownSolution; }
    }
    private bool containedInBestSolution;
    public bool ContainedInBestSolution {
      get { return containedInBestSolution; }
    }

    #region Storable Properties
    [Storable(Name = "Id")]
    private string StorableId {
      get { return id; }
      set { id = value; }
    }
    [Storable(Name = "Frequency")]
    private double StorableFrequency {
      get { return frequency; }
      set { frequency = value; }
    }
    [Storable(Name = "AverageImpact")]
    private double StorableAverageImpact {
      get { return averageImpact; }
      set { averageImpact = value; }
    }
    [Storable(Name = "AverageSolutionQuality")]
    private double StorableAverageSolutionQuality {
      get { return averageSolutionQuality; }
      set { averageSolutionQuality = value; }
    }
    [Storable(Name = "ContainedInBestKnownSolution")]
    private bool StorableContainedInBestKnownSolution {
      get { return containedInBestKnownSolution; }
      set { containedInBestKnownSolution = value; }
    }
    [Storable(Name = "ContainedInBestSolution")]
    private bool StorableContainedInBestSolution {
      get { return containedInBestSolution; }
      set { containedInBestSolution = value; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    protected AlleleFrequency(bool deserializing) : base(deserializing) { }
    protected AlleleFrequency(AlleleFrequency original, Cloner cloner)
      : base(original, cloner) {
      this.id = original.id;
      this.frequency = original.frequency;
      this.averageImpact = original.averageImpact;
      this.averageSolutionQuality = original.averageSolutionQuality;
      this.containedInBestKnownSolution = original.containedInBestKnownSolution;
      this.containedInBestSolution = original.containedInBestSolution;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlleleFrequency(this, cloner);
    }
    #endregion
    public AlleleFrequency()
      : base() {
      id = string.Empty;
      frequency = 0;
      averageImpact = 0;
      averageSolutionQuality = 0;
      containedInBestKnownSolution = false;
      containedInBestSolution = false;
    }
    public AlleleFrequency(string id, double frequency, double averageImpact, double averageSolutionQuality, bool containedInBestKnownSolution, bool containedInBestSolution)
      : base() {
      this.id = id;
      this.frequency = frequency;
      this.averageImpact = averageImpact;
      this.averageSolutionQuality = averageSolutionQuality;
      this.containedInBestKnownSolution = containedInBestKnownSolution;
      this.containedInBestSolution = containedInBestSolution;
    }

    public override string ToString() {
      return id;
    }
  }
}
