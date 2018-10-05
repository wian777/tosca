@echo off
:: Date:   20151023
:: User:   wian
:: Script: getnewdll.cmd
:: get new dll from compiled version
:: dir "%tricentis_home%\dll\DotNetEngine\CustomControls\"
:: pause
xcopy ".\lbaseDotNetCustomControls.*" "%tricentis_home%\dll\DotNetEngine\CustomControls\" /y
:: xcopy ".\lbaseDotNetCustomControls.*" "\\tfdbnbld01\LBase\INTERNAL\client\LBaseInternal\lbaseDotNetClientControl\" /y
:: also copy to cl-qlab-2
:: xcopy ".\lbaseDotNetCustomControls.*" "\\CL-QLAB-2\tricentis\dll\DotNetEngine\CustomControls" /y
:: xcopy ".\lbaseDotNetCustomControls.*" "\\cl-qlab-2\c$\TRICENTIS\TOSCATestsuite\dll\DotNetEngine\CustomControls" /y
:: also copy to cl-qlab-3
:: xcopy ".\lbaseDotNetCustomControls.*" "\\CL-QLAB-3\tricentis\dll\DotNetEngine\CustomControls" /y
:: xcopy ".\lbaseDotNetCustomControls.*" "\\cl-qlab-3\C\TRICENTIS\TOSCATestsuite\dll\DotNetEngine\CustomControls" /y
pause

