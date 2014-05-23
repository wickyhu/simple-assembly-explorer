@echo off

call prepare.bat

for /F "tokens=1-3 delims=/- " %%A in ('date /t') do (
    set DateYear=%%A
    set DateMonth=%%B
    set DateDay=%%C
)
set CurrentDate=%DateYear:~2,2%%DateMonth%%DateDay%
set TargetName=SAE.%CurrentDate%.7z

call delfile.bat %TargetName%

"C:\Program Files\7-Zip\7z.exe" a -r -t7z -y -xr!?svn\* -xr!TestResults\* %TargetName% *

call delfile.bat ..\%TargetName%
copy %TargetName% .. 
call delfile.bat %TargetName%

prompt
