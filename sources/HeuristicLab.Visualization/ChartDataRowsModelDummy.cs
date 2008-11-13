using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Visualization{
  public class ChartDataRowsModelDummy : ChartDataRowsModel  {
    public ChartDataRowsModelDummy(){
      Random rand = new Random();

   
      // test rows
      for (int i = 0; i < 10; i++){
        AddDataRow(i);
        PushData(i, rand.NextDouble() * 1000);
        PushData(i, rand.NextDouble() * 1000);
        PushData(i, rand.NextDouble() * 1000);
      }
    }
  }
}