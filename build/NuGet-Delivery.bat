@echo off
setlocal enabledelayedexpansion


:: ================================================
:: Variables
set version=6.1.13

set buildDir=%~dp0
set srcDir=%~dp0..\src
set outputDir=%~dp0packed

set endsMatchFilter[0]=.Lab
set endsMatchFilterLength=0


:: ================================================
:: OutputDir
if not exist %outputDir% mkdir %outputDir%

:: Pack
for /r %srcDir% %%f in (*.csproj) do (
  :: Variables
  set projectName=%%~nf
  set packFlag=1

  :: EndsMatchFilter
  for /l %%i in (0, 1, !endsMatchFilterLength!) do (
    set filterValue=!endsMatchFilter[%%i]!
    call :endsMatch isMatched !projectName! !filterValue!
    if /i !isMatched!==1 set packFlag=0
  )
    
  :: Execute
  if !packFlag! equ 1 (
    echo Packing project:%%~nf
    dotnet pack "%%f" --configuration Release --output "%outputDir%" /p:PackageVersion=%version%
  )
)

:: Push
for /r %outputDir% %%f in (*.nupkg) do (
  echo Pushing package:%%~nf
  nuget push "%%f" -src https://api.nuget.org/v3/index.json
)

:: Clear
rmdir /s /q %outputDir%


:: ================================================
:: End
echo Completed.
exit /b
endlocal


:: ================================================
:: Function
:endsMatch
set str=%~2
set ending=%~3
if not defined str ( set %1=0&exit /b )
if not defined ending ( set %1=1&exit /b )
if %str:~-1%==%ending:~-1% (
    call :endsMatch %1 %str:~0,-1% %ending:~0,-1%
) else (
    set %1=0&exit /b
)