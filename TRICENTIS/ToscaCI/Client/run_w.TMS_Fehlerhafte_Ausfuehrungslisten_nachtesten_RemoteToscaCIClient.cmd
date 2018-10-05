@echo off
:: wian 20181003
:: reason=TA-84 Tosca Distributed Execution und Tosca Event Monitor

cd %tricentis_home%\toscaci\client
:: AgentWS_w.TMS_Fehlerhafte_Ausfuehrungslisten_nachtesten.xml
ToscaCIClient.exe -m distributed -c AgentWS_w.TMS_Fehlerhafte_Ausfuehrungslisten_nachtesten.xml -r ResultRemote_w.TMS_FehlerNachtests_%date:~6,4%%date:~3,2%%date:~0,2%.xml