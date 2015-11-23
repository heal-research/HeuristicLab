using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.Core;
using HeuristicLab.DataPreprocessing.Filter;

namespace HeuristicLab.DataPreprocessing.Views
{
  [View("CheckedFilterCollection View")]
  [Content(typeof(ICheckedItemCollection<>), false)]
  [Content(typeof(CheckedItemCollection<>), false)]
  public partial class CheckedFilterCollectionView : CheckedItemCollectionView<IFilter>
  {
    public CheckedFilterCollectionView()
    {
      InitializeComponent();
    }

    protected override void addButton_Click(object sender, EventArgs e)
    {
      IFilter filter = new ComparisonFilter();
      Content.Add(filter);
      Content.SetItemCheckedState(filter, false);
    }

  }
}
