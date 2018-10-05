@echo off
:: wian 20181003
:: reason=TA-84 Tosca Distributed Execution und Tosca Event Monitor

cd %tricentis_home%\toscaci\client
:: Track and Trace
ToscaCIClient.exe -m distributed -c AgentWS_TRACK_TRACE.xml -r ResultRemote_TRACK_TRACE_%date:~6,4%%date:~3,2%%date:~0,2%.xml