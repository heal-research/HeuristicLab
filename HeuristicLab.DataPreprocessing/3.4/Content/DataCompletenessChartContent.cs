using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.DataPreprocessing {
  [Item("DataCompletenessChart", "Represents a datacompleteness chart.")]

  public class DataCompletenessChartContent : Item, IViewChartShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.EditBrightnessContrast; }
    }

    //public DataGridLogic DataGridLogic { get; private set; }
    public SearchLogic SearchLogic { get; private set; }

    public DataCompletenessChartContent(SearchLogic searchLogic) {
      SearchLogic = searchLogic;
    }

    public DataCompletenessChartContent(DataCompletenessChartContent content, Cloner cloner)
      : base(content, cloner) {
      SearchLogic = content.SearchLogic;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataCompletenessChartContent(this, cloner);
    }
  }
}
