@echo off
:: 20181029 wian
:: Source: \\tfdbnbld01\LBase\INTERNAL\client\LBaseInternal\lbaseDotNetCustomControls_TC11.2_P2\lbaseDotNetCustomControls.*
:: Target: \\cl-qlab-3\TRICENTIS\dll\DotNetEngine\CustomControls\.

set sourceHost=\\tfdbnbld01
set sourceDir=\LBase\INTERNAL\client\LBaseInternal\lbaseDotNetCustomControls_TC11.2_P2\
set sourceFile=lbaseDotNetCustomControls.*
set source=%sourceHost%%sourceDir%%sourceFile%
set targetHost=\\cl-qlab-3
set targetDir=\TRICENTIS\dll\DotNetEngine\CustomControls\
set targetFile=.
set target=%targetHost%%targetDir%%targetFile%
xcopy %source% %target% /y