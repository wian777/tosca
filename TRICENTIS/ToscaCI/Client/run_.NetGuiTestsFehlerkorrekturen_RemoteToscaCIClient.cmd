@echo off
:: wian 20181003
:: reason=TA-84 Tosca Distributed Execution und Tosca Event Monitor

cd %tricentis_home%\toscaci\client
:: AgentWS_.NetGuiTestsFehlerkorrekturen.xml
ToscaCIClient.exe -m distributed -c AgentWS_.NetGuiTestsFehlerkorrekturen.xml -r ResultRemote_AgentWS_.NetGuiTestsFehlerkorrekturen_%date:~6,4%%date:~3,2%%date:~0,2%.xml