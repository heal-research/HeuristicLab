if [ "${TargetDir}" == "" ]; then
  TargetDir=bin
fi

echo Recreating HeuristicLab 3.3.exe.config...
cp $SolutionDir/HeuristicLab/3.3/app.config "$TargetDir/HeuristicLab 3.3.exe.config"

echo Merging...
for f in $(ls $TargetDir/*.dll.config); do 
		mono $SolutionDir/ConfigMerger.exe $f "$TargetDir/HeuristicLab 3.3.exe.config"
done
