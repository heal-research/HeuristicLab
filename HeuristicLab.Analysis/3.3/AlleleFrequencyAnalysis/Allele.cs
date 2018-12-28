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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Represents an allele of a solution which is used for allele analysis.
  /// </summary>
  [Item("Allele", "Represents an allele of a solution which is used for allele analysis.")]
  [StorableType("27AE2AD2-D4A3-4187-A773-CDA518B8B28A")]
  public class Allele : Item {
    private string id;
    public string Id {
      get { return id; }
    }
    private double impact;
    public double Impact {
      get { return impact; }
    }

    #region Storable Properties
    [Storable(Name = "Id")]
    private string StorableId {
      get { return id; }
      set { id = value; }
    }
    [Storable(Name = "Impact")]
    private double StorableImpact {
      get { return impact; }
      set { impact = value; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    protected Allele(StorableConstructorFlag _) : base(_) { }
    protected Allele(Allele original, Cloner cloner)
      : base(original, cloner) {
      this.id = original.id;
      this.impact = original.impact;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Allele(this, cloner);
    }
    #endregion
    public Allele()
      : base() {
      this.id = string.Empty;
      this.impact = 0;
    }
    public Allele(string id)
      : base() {
      this.id = id;
      this.impact = 0;
    }
    public Allele(string id, double impact)
      : base() {
      this.id = id;
      this.impact = impact;
    }

    public override string ToString() {
      return id + " (" + impact + ")";
    }
  }
}
