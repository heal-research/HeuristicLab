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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("CheckedTransformationList View")]
  //[Content(typeof(RunCollectionConstraintCollection), true)]
  [Content(typeof(ICheckedItemList<ITransformation>), false)]
  public partial class CheckedTransformationListView : CheckedItemListView<ITransformation> {

    public CheckedTransformationListView() {
      InitializeComponent();
      itemsGroupBox.Text = "Transformations";
    }

    protected override ITransformation CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Transformation";
        typeSelectorDialog.TypeSelector.Caption = "Available Transformations";
        typeSelectorDialog.TypeSelector.Configure(typeof(ITransformation), showNotInstantiableTypes: true, showGenericTypes: false, typeCondition: CanInstanciateTransformation);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          // TODO: Avoid accessing parent view
          var transformationView = (TransformationView)Parent;
          var columnNames = transformationView.Content.Data.VariableNames;

          return (ITransformation)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType(new[] { columnNames });
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }

    private bool CanInstanciateTransformation(Type type) {
      foreach (ConstructorInfo ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)) {
        ParameterInfo[] parameters = ctor.GetParameters();
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IEnumerable<string>)) return true;
      }
      return false;
    }
  }
}
