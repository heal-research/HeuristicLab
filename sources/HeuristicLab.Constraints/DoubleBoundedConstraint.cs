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
  public class DoubleBoundedConstraint : ConstraintBase {
    private double lowerBound;
    public double LowerBound {
      get { return lowerBound; }
      set {
        lowerBound = value;
        OnChanged();
      }
    }
    private bool lowerBoundIncluded;
    public bool LowerBoundIncluded {
      get { return lowerBoundIncluded; }
      set {
        lowerBoundIncluded = value;
        OnChanged();
      }
    }
    private bool lowerBoundEnabled;
    public bool LowerBoundEnabled {
      get { return lowerBoundEnabled; }
      set {
        lowerBoundEnabled = value;
        OnChanged();
      }
    }
    private double upperBound;
    public double UpperBound {
      get { return upperBound; }
      set {
        upperBound = value;
        OnChanged();
      }
    }
    private bool upperBoundIncluded;
    public bool UpperBoundIncluded {
      get { return upperBoundIncluded; }
      set {
        upperBoundIncluded = value;
        OnChanged();
      }
    }
    private bool upperBoundEnabled;
    public bool UpperBoundEnabled {
      get { return upperBoundEnabled; }
      set {
        upperBoundEnabled = value;
        OnChanged();
      }
    }

    public override string Description {
      get { return "The double is limited one or two sided by a lower and/or upper boundary"; }
    }

    public DoubleBoundedConstraint()
      : this(double.MinValue, double.MaxValue) {
    }

    public DoubleBoundedConstraint(double lowerBound, double upperBound)
      : this(lowerBound, true, upperBound, true) {
    }

    public DoubleBoundedConstraint(double lowerBound, bool lowerBoundIncluded, double upperBound, bool upperBoundIncluded)
      : base() {
      this.lowerBound = lowerBound;
      this.lowerBoundIncluded = lowerBoundIncluded;
      this.lowerBoundEnabled = true;
      this.upperBound = upperBound;
      this.upperBoundIncluded = upperBoundIncluded;
      this.upperBoundEnabled = true;
    }


    public override bool Check(IItem data) {
      ConstrainedDoubleData d = (data as ConstrainedDoubleData);
      if (d == null) return false;
      if (LowerBoundEnabled && ((LowerBoundIncluded && d.CompareTo(LowerBound) < 0)
        || (!LowerBoundIncluded && d.CompareTo(LowerBound) <= 0))) return false;
      if (UpperBoundEnabled && ((UpperBoundIncluded && d.CompareTo(UpperBound) > 0)
        || (!UpperBoundIncluded && d.CompareTo(UpperBound) >= 0))) return false;
      return true;
    }

    public override IView CreateView() {
      return new DoubleBoundedConstraintView(this);
    }

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
