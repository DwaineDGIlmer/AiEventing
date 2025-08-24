@echo off
setlocal

REM Define source path
set SourcePath=%~dp0bin\Release\Core.1.2.8.nupkg

REM Define target directories
set TargetDirs=C:\Users\dwain\source\repos\EzLeadGenerator\.nuget-local;C:\Users\dwain\source\repos\FaultAnalysisEngine\.nuget-local;C:\Users\dwain\source\repos\IntelligentLogging\.nuget-local

REM Check if source file exists
if not exist "%SourcePath%" (
    echo Source file does not exist: %SourcePath%
    exit /b 0
)

REM Loop through each target directory
for %%D in (%TargetDirs%) do (
    if exist "%%D" (
        echo Source directory %SourcePath%
        echo Target directory %%D exists.
        copy /Y "%SourcePath%" "%%D"
        if "%errorlevel%" NEQ "0" (
            echo Failed to copy to %%D
            exit /b %errorlevel%
        )
    ) else (
        echo Target directory %%D does not exist. Skipping copy...
    )
)

REM Exit successfully
exit /b 0