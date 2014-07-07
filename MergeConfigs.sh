if [ "${Outdir}" == "" ]; then
  Outdir=bin
fi

echo Recreating HeuristicLab 3.3.exe.config...
cp $Outdir/app.config "$Outdir/HeuristicLab 3.3.exe.config"

echo Merging...
for f in $(ls $Outdir/*.dll.config); do 
		mono $SolutionDir/ConfigMerger.exe $f "$Outdir/HeuristicLab 3.3.exe.config"
done
