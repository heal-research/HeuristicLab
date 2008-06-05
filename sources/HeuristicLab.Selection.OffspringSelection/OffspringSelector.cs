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
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Selection.OffspringSelection {
  public class OffspringSelector : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public OffspringSelector() {
      AddVariableInfo(new VariableInfo("SuccessfulChild", "True if the child was successful", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SelectionPressureLimit", "Maximum selection pressure", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SuccessRatioLimit", "Maximum success ratio", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SelectionPressure", "Current selection pressure", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SuccessRatio", "Current success ratio", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("GoodChildren", "Temporarily store successful children", typeof(ItemList<IScope>), VariableKind.New | VariableKind.Out | VariableKind.In | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("BadChildren", "Temporarily store unsuccessful children", typeof(ItemList<IScope>), VariableKind.New | VariableKind.Out | VariableKind.In | VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      double selectionPressureLimit = GetVariableValue<DoubleData>("SelectionPressureLimit", scope, true).Data;
      double successRatioLimit = GetVariableValue<DoubleData>("SuccessRatioLimit", scope, true).Data;

      IScope parents = scope.SubScopes[0];
      IScope children = scope.SubScopes[1];

      // retrieve actual selection pressure and success ratio
      DoubleData selectionPressure = GetVariableValue<DoubleData>("SelectionPressure", scope, false, false);
      if (selectionPressure == null) {
        IVariableInfo selectionPressureInfo = GetVariableInfo("SelectionPressure");
        selectionPressure = new DoubleData(0);
        if (selectionPressureInfo.Local)
          AddVariable(new Variable(selectionPressureInfo.ActualName, selectionPressure));
        else
          scope.AddVariable(new Variable(scope.TranslateName(selectionPressureInfo.FormalName), selectionPressure));
      }
      DoubleData successRatio = GetVariableValue<DoubleData>("SuccessRatio", scope, false, false);
      if (successRatio == null) {
        IVariableInfo successRatioInfo = GetVariableInfo("SuccessRatio");
        successRatio = new DoubleData(0);
        if (successRatioInfo.Local)
          AddVariable(new Variable(successRatioInfo.ActualName, successRatio));
        else
          scope.AddVariable(new Variable(scope.TranslateName(successRatioInfo.FormalName), successRatio));
      }

      // retrieve good and bad children
      ItemList<IScope> goodChildren = GetVariableValue<ItemList<IScope>>("GoodChildren", scope, false, false);
      if (goodChildren == null) {
        goodChildren = new ItemList<IScope>();
        IVariableInfo goodChildrenInfo = GetVariableInfo("GoodChildren");
        if (goodChildrenInfo.Local)
          AddVariable(new Variable(goodChildrenInfo.ActualName, goodChildren));
        else
          scope.AddVariable(new Variable(scope.TranslateName(goodChildrenInfo.FormalName), goodChildren));

        // no good children available -> first iteration of this generation -> initialize selection pressure
        selectionPressure.Data = 0;
      }
      ItemList<IScope> badChildren = GetVariableValue<ItemList<IScope>>("BadChildren", scope, false, false);
      if (badChildren == null) {
        badChildren = new ItemList<IScope>();
        IVariableInfo badChildrenInfo = GetVariableInfo("BadChildren");
        if (badChildrenInfo.Local)
          AddVariable(new Variable(badChildrenInfo.ActualName, badChildren));
        else
          scope.AddVariable(new Variable(scope.TranslateName(badChildrenInfo.FormalName), badChildren));
      }

      // separate new children in good and bad children
      int goodCount = 0;
      int badCount = 0;
      IVariableInfo successfulInfo = GetVariableInfo("SuccessfulChild");
      while (children.SubScopes.Count > 0) {
        IScope child = children.SubScopes[0];
        bool successful = child.GetVariableValue<BoolData>(successfulInfo.FormalName, false).Data;
        if (successful) {
          goodCount++;
          goodChildren.Add(child);
        } else {
          badCount++;
          // only keep the child if we have not filled up the pool or if we reached the
          // selection pressure limit in which case we have to keep more lucky losers than usual
          if ((1 - successRatioLimit) * parents.SubScopes.Count >= badChildren.Count ||
            selectionPressure.Data >= selectionPressureLimit) {
            badChildren.Add(child);
          }
        }
        children.RemoveSubScope(child);
      }

      // calculate actual selection pressure and success ratio
      selectionPressure.Data += (goodCount + badCount) / ((double)parents.SubScopes.Count);
      successRatio.Data = goodCount / ((double)parents.SubScopes.Count);

      // check if enough children have been generated
      if (((selectionPressure.Data < selectionPressureLimit) && (successRatio.Data < successRatioLimit)) ||
          ((goodChildren.Count + badChildren.Count) < parents.SubScopes.Count)) {
        // more children required -> reduce left and start children generation again
        scope.RemoveSubScope(parents);
        scope.RemoveSubScope(children);
        for (int i = 0; i < parents.SubScopes.Count; i++)
          scope.AddSubScope(parents.SubScopes[i]);

        return new AtomicOperation(SubOperators[0], scope);
      } else {
        // enough children generated
        while (children.SubScopes.Count < parents.SubScopes.Count) {
          if (goodChildren.Count > 0) {
            children.AddSubScope((IScope)goodChildren[0]);
            goodChildren.RemoveAt(0);
          } else {
            children.AddSubScope((IScope)badChildren[0]);
            badChildren.RemoveAt(0);
          }
        }

        // remove good and bad children again
        IVariableInfo goodChildrenInfo = GetVariableInfo("GoodChildren");
        if (goodChildrenInfo.Local)
          RemoveVariable(goodChildrenInfo.ActualName);
        else
          scope.RemoveVariable(scope.TranslateName(goodChildrenInfo.FormalName));
        IVariableInfo badChildrenInfo = GetVariableInfo("BadChildren");
        if (badChildrenInfo.Local)
          RemoveVariable(badChildrenInfo.ActualName);
        else
          scope.RemoveVariable(scope.TranslateName(badChildrenInfo.FormalName));
        IVariableInfo badCountInfo = GetVariableInfo("BadCount");
        if(badCountInfo.Local)
          RemoveVariable(badCountInfo.ActualName);
        else
          scope.RemoveVariable(scope.TranslateName("BadCount"));
        return null;
      }
    }
  }
}
