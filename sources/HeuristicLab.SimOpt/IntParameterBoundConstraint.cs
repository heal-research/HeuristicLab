#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Globalization;
using System.Text;
using System.Xml;
using HeuristicLab.Constraints;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class IntParameterBoundConstraint : ConstraintBase {
    private int lowerBound;
    public int LowerBound {
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
    private int upperBound;
    public int UpperBound {
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
    private string parameterName;
    public string ParameterName {
      get { return parameterName; }
      set {
        parameterName = value;
        OnChanged();
      }
    }

    public override string Description {
      get { return "The double is limited one or two sided by a lower and/or upper boundary"; }
    }

    public IntParameterBoundConstraint()
      : this(int.MinValue, int.MaxValue, "undefined") {
    }

    public IntParameterBoundConstraint(int lowerBound, int upperBound, string parameterName)
      : this(lowerBound, true, upperBound, true, parameterName) {
    }

    public IntParameterBoundConstraint(int lowerBound, bool lowerBoundIncluded, int upperBound, bool upperBoundIncluded, string parameterName)
      : base() {
      this.lowerBound = lowerBound;
      this.lowerBoundIncluded = lowerBoundIncluded;
      this.lowerBoundEnabled = true;
      this.upperBound = upperBound;
      this.upperBoundIncluded = upperBoundIncluded;
      this.upperBoundEnabled = true;
      this.parameterName = parameterName;
    }


    public override bool Check(IItem data) {
      ConstrainedItemList list = (data as ConstrainedItemList);
      if (list == null) return false;
      for (int i = 0; i < list.Count; i++) {
        if (list[i] is IVariable && ((IVariable)list[i]).Name.Equals(parameterName) && ((IVariable)list[i]).Value is IntData) {
          if (LowerBoundEnabled && ((LowerBoundIncluded && ((IComparable)((IVariable)list[i]).Value).CompareTo(LowerBound) < 0)
            || (!LowerBoundIncluded && ((IComparable)((IVariable)list[i]).Value).CompareTo(LowerBound) <= 0))) return false;
          if (UpperBoundEnabled && ((UpperBoundIncluded && ((IComparable)((IVariable)list[i]).Value).CompareTo(UpperBound) > 0)
            || (!UpperBoundIncluded && ((IComparable)((IVariable)list[i]).Value).CompareTo(UpperBound) >= 0))) return false;
        }
      }
      return true;
    }

    public override IView CreateView() {
      return new IntParameterBoundConstraintView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      IntParameterBoundConstraint clone = new IntParameterBoundConstraint();
      clonedObjects.Add(Guid, clone);
      clone.parameterName = (string)parameterName.Clone();
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
      XmlAttribute pm = document.CreateAttribute("ParameterName");
      pm.Value = parameterName;
      XmlAttribute lb = document.CreateAttribute("LowerBound");
      lb.Value = LowerBound.ToString();
      XmlAttribute lbi = document.CreateAttribute("LowerBoundIncluded");
      lbi.Value = lowerBoundIncluded.ToString();
      XmlAttribute lbe = document.CreateAttribute("LowerBoundEnabled");
      lbe.Value = lowerBoundEnabled.ToString();
      XmlAttribute ub = document.CreateAttribute("UpperBound");
      ub.Value = upperBound.ToString();
      XmlAttribute ubi = document.CreateAttribute("UpperBoundIncluded");
      ubi.Value = upperBoundIncluded.ToString();
      XmlAttribute ube = document.CreateAttribute("UpperBoundEnabled");
      ube.Value = upperBoundEnabled.ToString();
      node.Attributes.Append(pm);
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
      parameterName = node.Attributes["ParameterName"].Value;
      lowerBound = int.Parse(node.Attributes["LowerBound"].Value, CultureInfo.InvariantCulture);
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

      upperBound = int.Parse(node.Attributes["UpperBound"].Value, CultureInfo.InvariantCulture);
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
