# add hint path for DataVisualization assembly
ORIG="<Reference Include=\"System.Windows.Forms.DataVisualization\" \/>"
REP="<Reference Include=\"System.Windows.Forms.DataVisualization\" > \n <HintPath>..\/..\/bin\/System.Windows.Forms.DataVisualization.dll<\/HintPath> \n <\/Reference>"

for filename in $(find -name '*.csproj')
do    
    sed "s/$ORIG/$REP/g" $filename > tmp
    mv tmp $filename
done;


# remove projects that do not build
sed -e '/ProtocolBuffers-2.4.1.473/,+1d' -e '/ProtoGen-2.4.1.473/,+1d' -e '/HeuristicLab.ProtobufCS-2.4.1.473/,+1d' HeuristicLab.ExtLibs.sln > tmp
mv tmp HeuristicLab.ExtLibs.sln

sed -e '/HeuristicLab.Problems.ExternalEvaluation-3.3/,+1d' -e '/HeuristicLab.Problems.ExternalEvaluation.GP-3.4/,+1d' -e '/HeuristicLab.Problems.ExternalEvaluation.Views-3.3/,+1d' "HeuristicLab 3.3.sln" > tmp
mv tmp "HeuristicLab 3.3.sln"


# switch to MultiDocument MainForm type as Docking doesn't properly work on Linux
sed "s/DockingMainForm/MultipleDocumentMainForm/g" HeuristicLab.Optimizer/3.3/Properties/Settings.settings > tmp
mv tmp HeuristicLab.Optimizer/3.3/Properties/Settings.settings

sed "s/DockingMainForm/MultipleDocumentMainForm/g" HeuristicLab.Optimizer/3.3/Properties/Settings.Designer.cs > tmp
mv tmp HeuristicLab.Optimizer/3.3/Properties/Settings.Designer.cs
