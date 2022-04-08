//using System;
//using System.ComponentModel;
//using System.Windows.Forms;

//namespace HeuristicLab.JsonInterface.OptimizerIntegration {
//  public interface IJsonItemVM : INotifyPropertyChanged 
//  {
//    event Action ItemChanged;

//    Type TargetedJsonItemType { get; }

//    UserControl Control { get; }
//    bool Selected { get; set; }

//    string Name { get; set; }

//    string Description { get; set; }
    
//    TreeNode TreeNode { get; set; }

//    TreeView TreeView { get; set; }
//    JsonItem Item { get; set; }

//  }

//  public interface IJsonItemVM<JsonItemType> : IJsonItemVM
//    where JsonItemType : JsonItem
//  {
//    new JsonItemType Item { get; set; }
//  }
//}
