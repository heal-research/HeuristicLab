FOR /F "tokens=*" %%A IN ('dir /B "%Outdir%*.dll.config"') DO (
  ConfigMerger "%%A" "HeuristicLab 3.3.exe.config"
)