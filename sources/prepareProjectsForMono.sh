# add hint path for DataVisualization assembly
ORIG="<Reference Include=\"System.Windows.Forms.DataVisualization\" \/>"
REP="<Reference Include=\"System.Windows.Forms.DataVisualization\" > \n <HintPath>..\/..\/bin\/System.Windows.Forms.DataVisualization.dll<\/HintPath> \n <\/Reference>"

for filename in $(find . -name '*.csproj')
do    
    sed "s/$ORIG/$REP/g" $filename > tmp
    mv tmp $filename
done;


# remove projects that do not build
unamestr=`uname`
if [[ "$unamestr" == 'Darwin' ]]; then
   awk '/ICSharpCode.AvalonEdit-5.0.1|HeuristicLab.AvalonEdit-5.0.1/ {while (/ICSharpCode.AvalonEdit-5.0.1|HeuristicLab.AvalonEdit-5.0.1/ && getline>0) ; next} 1' HeuristicLab.ExtLibs.sln > tmp
   mv tmp HeuristicLab.ExtLibs.sln
   awk '/HeuristicLab.Problems.ExternalEvaluation-3.3|HeuristicLab.Problems.ExternalEvaluation.GP-3.4|HeuristicLab.Problems.ExternalEvaluation.Views-3.3/ {while (/HeuristicLab.Problems.ExternalEvaluation-3.3|HeuristicLab.Problems.ExternalEvaluation.GP-3.4|HeuristicLab.Problems.ExternalEvaluation.Views-3.3/ && getline>0) ; next} 1' "HeuristicLab 3.3.sln" > tmp
   mv tmp "HeuristicLab 3.3.sln"
elif [[ "$unamestr" == 'Linux' ]]; then
   sed -e '/ICSharpCode.AvalonEdit-5.0.1/,+1d' -e '/HeuristicLab.AvalonEdit-5.0.1/,+1d' HeuristicLab.ExtLibs.sln > tmp
   mv tmp HeuristicLab.ExtLibs.sln
   sed -e '/HeuristicLab.Problems.ExternalEvaluation-3.3/,+1d' -e '/HeuristicLab.Problems.ExternalEvaluation.GP-3.4/,+1d' -e '/HeuristicLab.Problems.ExternalEvaluation.Views-3.3/,+1d' "HeuristicLab 3.3.sln" > tmp
   mv tmp "HeuristicLab 3.3.sln"
else 
   echo "Unsupported operating system, compiling HeuristicLab may not work!"
fi


# switch to MultiDocument MainForm type as Docking doesn't properly work on Linux
sed "s/DockingMainForm/MultipleDocumentMainForm/g" HeuristicLab.Optimizer/3.3/Properties/Settings.settings > tmp
mv tmp HeuristicLab.Optimizer/3.3/Properties/Settings.settings

sed "s/DockingMainForm/MultipleDocumentMainForm/g" HeuristicLab.Optimizer/3.3/Properties/Settings.Designer.cs > tmp
mv tmp HeuristicLab.Optimizer/3.3/Properties/Settings.Designer.cs
