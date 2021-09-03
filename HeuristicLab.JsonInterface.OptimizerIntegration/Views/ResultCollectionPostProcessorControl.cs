using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Collections;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ResultCollectionPostProcessorControl : Core.Views.CheckedItemListView<IResultCollectionPostProcessor> {
    public ResultCollectionPostProcessorControl() {
      InitializeComponent();
    }

    #region Content Events
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        SetNumberOfCheckItems();
      }
    }

    protected override void Content_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<IResultCollectionPostProcessor>> e) {
      base.Content_CheckedItemsChanged(sender, e);
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<IResultCollectionPostProcessor>>(Content_CheckedItemsChanged), sender, e);
      else {
        SetNumberOfCheckItems();
      }
    }
    protected override void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IResultCollectionPostProcessor>> e) {
      base.Content_CollectionReset(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IResultCollectionPostProcessor>> e) {
      base.Content_ItemsAdded(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IResultCollectionPostProcessor>> e) {
      base.Content_ItemsMoved(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IResultCollectionPostProcessor>> e) {
      base.Content_ItemsRemoved(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IResultCollectionPostProcessor>> e) {
      base.Content_ItemsReplaced(sender, e);
      SetNumberOfCheckItems();
    }
    #endregion

    private void SetNumberOfCheckItems() { // TODO: eigene override für "Post Processors" -> Item Naming
      if (InvokeRequired) {
        Invoke((Action)SetNumberOfCheckItems);
      } else {
        this.itemsGroupBox.Text = String.Format("Post Processors (Checked: {0}/{1})", Content.CheckedItems.Count(), Content.Count);
      }
    }
  }
}
