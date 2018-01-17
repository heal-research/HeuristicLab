# add hint path for DataVisualization assembly
# define __MonoCS__
# --------------------------------------------
orig="<Reference Include=\"System.Windows.Forms.DataVisualization\" \/>"
repl="<Reference Include=\"System.Windows.Forms.DataVisualization\"><HintPath>..\/..\/bin\/System.Windows.Forms.DataVisualization.dll<\/HintPath><\/Reference>"

for csprojfile in $(find . -name '*.csproj')
do    
  sed -e "s/$orig/$repl/g" \
      -e "s@<DefineConstants>\(.*\)</DefineConstants>@<DefineConstants>\1;__MonoCS__</DefineConstants>@g" \
      $csprojfile > tmp
  mv tmp $csprojfile
done;

# remove projects that do not build
# ---------------------------------
UNAMESTR=`uname`

if [[ "$UNAMESTR" == 'Linux' || "$UNAMESTR" == 'Darwin' ]]; then
  sed `# projects` \
      -e '/HeuristicLab.AvalonEdit-5.0.1/{N;d;}' \
      -e '/ICSharpCode.AvalonEdit-5.0.1/{N;d;}' \
      `# project configurations` \
      -e '/644B1CCE-1B2A-4C61-B0E3-A2EDB89DF872/d' `# HeuristicLab.AvalonEdit-5.0.1` \
      -e '/255C7DEB-3C98-4BC2-92D4-B683F82A7E52/d' `# ICSharpCode.AvalonEdit-5.0.1` \
      "HeuristicLab.ExtLibs.sln" > tmp
  mv tmp "HeuristicLab.ExtLibs.sln"

  sed `# projects` \
      -e '/HeuristicLab.Problems.BinPacking.Views-3.3/{N;d;}' \
      -e '/HeuristicLab.Problems.ExternalEvaluation-3.4/{N;d;}' \
      -e '/HeuristicLab.Problems.ExternalEvaluation.GP-3.5/{N;d;}' \
      -e '/HeuristicLab.Problems.ExternalEvaluation.Matlab-3.3/{N;d;}' \
      -e '/HeuristicLab.Problems.ExternalEvaluation.Views-3.4/{N;d;}' \
      `# project configurations` \
      -e '/8CFC7A61-E214-44DC-96B3-4CEA9B8E958E/d' `# HeuristicLab.Problems.BinPacking.Views-3.3` \
      -e '/8A0B2A2B-47A7-410D-97A0-4296293BB00D/d' `# HeuristicLab.Problems.ExternalEvaluation-3.4` \
      -e '/64CC53AC-156B-4D8A-8DB0-B68990BEA4D3/d' `# HeuristicLab.Problems.ExternalEvaluation.GP-3.5` \
      -e '/362A5DC3-969D-43FB-A552-D2F52B780188/d' `# HeuristicLab.Problems.ExternalEvaluation.Matlab-3.3` \
      -e '/F7E5B975-FDF2-45A4-91CB-FF6D3C33D65E/d' `# HeuristicLab.Problems.ExternalEvaluation.Views-3.4` \
      "HeuristicLab 3.3.sln" > tmp
  mv tmp "HeuristicLab 3.3.sln"

  # remove code from HeuristicLab.CodeEditor that depends on WPF and therefore does not work with Mono
  sed -e '/ITextMarker.cs/d' \
      -e '/MethodDefinitionReadOnlySectionProvider.cs/d' \
      -e '/GoToLineDialog/{N;N;d;}' \
      -e '/TextMarkerService.cs/d' \
      -e '/Compile Include\=\"LanguageFeatures/d' \
      -e '/AvalonEditWrapper.xaml.cs/{N;N;d;}' \
      -e '/AvalonEditWrapper.xaml/{N;N;N;d;}' \
      -e '/CodeViewer/{N;N;d;}' \
      -e '/\"CodeEditor.cs/{N;N;d;}' \
      -e '/\"CodeEditor.Designer.cs/{N;N;d;}' \
      "HeuristicLab.CodeEditor/3.4/HeuristicLab.CodeEditor-3.4.csproj" > tmp
  mv tmp "HeuristicLab.CodeEditor/3.4/HeuristicLab.CodeEditor-3.4.csproj"
else 
  echo "Unsupported operating system, compiling HeuristicLab may not work!"
fi

# switch to MultipleDocumentMainForm type as DockingMainForm does not properly work with Mono
sed "s/DockingMainForm/MultipleDocumentMainForm/g" "HeuristicLab.Optimizer/3.3/Properties/Settings.settings" > tmp
mv tmp "HeuristicLab.Optimizer/3.3/Properties/Settings.settings"

sed "s/DockingMainForm/MultipleDocumentMainForm/g" "HeuristicLab.Optimizer/3.3/Properties/Settings.Designer.cs" > tmp
mv tmp "HeuristicLab.Optimizer/3.3/Properties/Settings.Designer.cs"
