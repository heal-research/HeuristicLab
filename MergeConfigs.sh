if [ "${TargetDir}" == "" ]; then
  TargetDir=bin
fi

echo Recreating HeuristicLab 3.3.dll.config...
cp $SolutionDir/HeuristicLab/3.3/app.config "$TargetDir/HeuristicLab 3.3.dll.config"

echo Merging...
for f in $(ls $TargetDir/*.dll.config); do 
		mono $SolutionDir/ConfigMerger.exe $f "$TargetDir/HeuristicLab 3.3.dll.config"
done
