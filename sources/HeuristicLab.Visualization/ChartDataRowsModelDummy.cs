using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Visualization {
  public class ChartDataRowsModelDummy : ChartDataRowsModel {

     public ChartDataRowsModelDummy(){

        // test rows
        AddDataRow(1);
        AddDataRow(2);
        AddDataRow(3);
        AddDataRow(4);

       PushData(1, 1.2);
       PushData(1, 2.0);
       PushData(1, 4.5);
       PushData(1, 8.4);
       PushData(1, 5.0);
       PushData(1, 6.0);
       PushData(2, 2.0);
       PushData(3, 8.3);
       PushData(3, 7.3);
       PushData(3, 9.7);
       PushData(3, 2.3);
       PushData(3, 1.7);
       PushData(3, 0.3);
       PushData(3, 0.1);
       PushData(3, 2.0);
       PushData(3, 8.8);
       PushData(3, 9.9);

     }
    
  }
}
