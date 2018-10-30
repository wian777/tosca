@echo off 
:: wian 20181029

:: echo %1%
set param=%1%
:: echo %param%
set counter=0
set counter_add=0
set maxcount=10
if "%param%"=="-p" goto getparam
if "%param%"=="-c" goto count
if "%param%"=="-ch" goto counthorizontal
if "%param%"=="-ico" goto icon
if "%param%"=="-speak" goto speak
echo Erster Parameter ist kein -p daher ENDE
goto ENDE
echo komisch dass das weitergeht statt nach getparam!?

echo %0%
echo %$
echo %#
echo %1%
echo %2%
echo %?
echo %!
echo.
echo --------------------------------------

:getparam
:: echo trage den 1. parameter ein:
set /p p1=Gib den 1. Parameter ein: 
:: echo %p1%

:: echo trage den 2. parameter ein:
set /p p2=Gib den 2. Parameter ein: 
:: echo %p2%

goto showparam

:showparam
echo %p1%
echo %p2%
goto ENDE

:count
if %counter% equ %maxcount% goto ENDE
set /a counter=%counter% + 1
:: echo.
ping localhost /n 2 >nul
echo %counter%
goto count

:counthorizontal
if %counter% equ %maxcount% goto ENDE
set /a counter=%counter% + 1
set counter_add=%counter_add% %counter%
:: echo.
ping localhost /n 1 >nul
cls
echo %counter_add%
goto counthorizontal

:icon
echo ########
echo ##
echo ##
echo ######
echo       ##
echo       ##
echo       ##
echo ######
goto ENDE

:speak
Dim Message, Speak
Message=InputBox("Tipp eine Nachricht ein","Hello")
Set Speak=CreateObject("sapi.spvoice")
Speak.Speak Message

:unset
echo Folgende Variablen werden wieder auf null gesetzt:
echo --------------------------------------
set p1
set p2
set counter
set param
set maxcount

set p1=
set p2=
set counter=
set param=
set maxcount=

:ENDE
