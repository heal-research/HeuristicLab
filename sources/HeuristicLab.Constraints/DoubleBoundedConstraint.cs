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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Globalization;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint where a double value is limited by a one or two sided boundary.
  /// </summary>
  public class DoubleBoundedConstraint : ConstraintBase {
    private double lowerBound;
    /// <summary>
    /// Gets or sets the lower bound of the limit.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public double LowerBound {
      get { return lowerBound; }
      set {
        lowerBound = value;
        OnChanged();
      }
    }
    private bool lowerBoundIncluded;
    /// <summary>
    /// Gets or sets the boolean flag whether the lower bound should be included.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool LowerBoundIncluded {
      get { return lowerBoundIncluded; }
      set {
        lowerBoundIncluded = value;
        OnChanged();
      }
    }
    private bool lowerBoundEnabled;
    /// <summary>
    /// Gets or sets the boolean flag whether the lower bound should be enabled.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool LowerBoundEnabled {
      get { return lowerBoundEnabled; }
      set {
        lowerBoundEnabled = value;
        OnChanged();
      }
    }
    private double upperBound;
    /// <summary>
    /// Gets or sets the upper bound of the limit.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public double UpperBound {
      get { return upperBound; }
      set {
        upperBound = value;
        OnChanged();
      }
    }
    private bool upperBoundIncluded;
    /// <summary>
    /// Gets or sets the boolean flag whether the upper bound should be included.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool UpperBoundIncluded {
      get { return upperBoundIncluded; }
      set {
        upperBoundIncluded = value;
        OnChanged();
      }
    }
    private bool upperBoundEnabled;
    /// <summary>
    /// Gets or sets the boolean flag whether the upper bound should be enabled.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool UpperBoundEnabled {
      get { return upperBoundEnabled; }
      set {
        upperBoundEnabled = value;
        OnChanged();
      }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "The double is limited one or two sided by a lower and/or upper boundary"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleBoundedConstraint"/>.
    /// </summary>
    public DoubleBoundedConstraint()
      : this(double.MinValue, double.MaxValue) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleBoundedConstraint"/> with the two given boundaries.
    /// </summary>
    /// <param name="lowerBound">The lower bound of the constraint.</param>
    /// <param name="upperBound">The upper bound of the constraint.</param>
    public DoubleBoundedConstraint(double lowerBound, double upperBound)
      : this(lowerBound, true, upperBound, true) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleBoundedConstraint"/> with the given parameters.
    /// </summary>
    /// <param name="lowerBound">The lower bound of the constraint.</param>
    /// <param name="lowerBoundIncluded">Boolean flag whether the lower bound should be included.</param>
    /// <param name="upperBound">The upper bound of the constraint.</param>
    /// <param name="upperBoundIncluded">Boolean flag whether the upper bound should be included.</param>
    public DoubleBoundedConstraint(double lowerBound, bool lowerBoundIncluded, double upperBound, bool upperBoundIncluded)
      : base() {
      this.lowerBound = lowerBound;
      this.lowerBoundIncluded = lowerBoundIncluded;
      this.lowerBoundEnabled = true;
      this.upperBound = upperBound;
      this.upperBoundIncluded = upperBoundIncluded;
      this.upperBoundEnabled = true;
    }


    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      ConstrainedDoubleData d = (data as ConstrainedDoubleData);
      if (d == null) return false;
      if (LowerBoundEnabled && ((LowerBoundIncluded && d.CompareTo(LowerBound) < 0)
        || (!LowerBoundIncluded && d.CompareTo(LowerBound) <= 0))) return false;
      if (UpperBoundEnabled && ((UpperBoundIncluded && d.CompareTo(UpperBound) > 0)
        || (!UpperBoundIncluded && d.CompareTo(UpperBound) >= 0))) return false;
      return true;
    }

    /// <summary>
    /// Creates a new instance of <see cref="DoubleBoundedConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="DoubleBoundedConstraintView"/>.</returns>
    public override IView CreateView() {
      return new DoubleBoundedConstraintView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="DoubleBoundedConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      DoubleBoundedConstraint clone = new DoubleBoundedConstraint();
      clonedObjects.Add(Guid, clone);
      clone.upperBound = UpperBound;
      clone.upperBoundIncluded = UpperBoundIncluded;
      clone.upperBoundEnabled = UpperBoundEnabled;
      clone.lowerBound = LowerBound;
      clone.lowerBoundIncluded = LowerBoundIncluded;
      clone.lowerBoundEnabled = LowerBoundEnabled;
      return clone;
    }

    #region persistence
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The properties of the current instance are saved as attributes with special tag names.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute lb = document.CreateAttribute("LowerBound");
      lb.Value = LowerBound.ToString("r", CultureInfo.InvariantCulture);
      XmlAttribute lbi = document.CreateAttribute("LowerBoundIncluded");
      lbi.Value = lowerBoundIncluded.ToString();
      XmlAttribute lbe = document.CreateAttribute("LowerBoundEnabled");
      lbe.Value = lowerBoundEnabled.ToString();
      XmlAttribute ub = document.CreateAttribute("UpperBound");
      ub.Value = upperBound.ToString("r", CultureInfo.InvariantCulture);
      XmlAttribute ubi = document.CreateAttribute("UpperBoundIncluded");
      ubi.Value = upperBoundIncluded.ToString();
      XmlAttribute ube = document.CreateAttribute("UpperBoundEnabled");
      ube.Value = upperBoundEnabled.ToString();
      node.Attributes.Append(lb);
      if (!lowerBoundIncluded) node.Attributes.Append(lbi);
      if (!lowerBoundEnabled) node.Attributes.Append(lbe);
      node.Attributes.Append(ub);
      if (!upperBoundIncluded) node.Attributes.Append(ubi);
      if (!upperBoundEnabled) node.Attributes.Append(ube);
      return node;
    }

    /// <summary>
    /// Loads the persisted constraint from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The constraint must be saved in a specific way, see <see cref="GetXmlNode"/> for 
    /// more information.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the instance is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      lowerBound = double.Parse(node.Attributes["LowerBound"].Value, CultureInfo.InvariantCulture);
      if (node.Attributes["LowerBoundIncluded"] != null) {
        lowerBoundIncluded = bool.Parse(node.Attributes["LowerBoundIncluded"].Value);
      } else {
        lowerBoundIncluded = true;
      }
      if (node.Attributes["LowerBoundEnabled"] != null) {
        lowerBoundEnabled = bool.Parse(node.Attributes["LowerBoundEnabled"].Value);
      } else {
        lowerBoundEnabled = true;
      }

      upperBound = double.Parse(node.Attributes["UpperBound"].Value, CultureInfo.InvariantCulture);
      if (node.Attributes["UpperBoundIncluded"] != null) {
        upperBoundIncluded = bool.Parse(node.Attributes["UpperBoundIncluded"].Value);
      } else {
        upperBoundIncluded = true;
      }
      if (node.Attributes["UpperBoundEnabled"] != null) {
        upperBoundEnabled = bool.Parse(node.Attributes["UpperBoundEnabled"].Value);
      } else {
        upperBoundEnabled = true;
      }
    }
    #endregion
  }
}
