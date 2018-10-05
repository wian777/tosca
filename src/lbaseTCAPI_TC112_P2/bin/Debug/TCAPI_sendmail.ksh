#!/usr/bin/ksh
# scriptfile: TCAPI_sendmail.ksh
# porpose:    sendet nach Ausfuehrung einer Tosca Ausfuehrungsliste die Logdatei per Mail an QA Postfach
# start:      regelmaessig mit crontab:
# 45 05 * * 1-5 echo `date` 1>/tmp/TCAPI_sendmail.ksh.log;/home/scm/bin/TCAPI_sendmail.ksh 1.mailadress@lbase.software 2.mailadress@lbase.software 1>>/tmp/TCAPI_sendmail.ksh.log 2>&1
# author:     wian
# date:       20160816
# parameters: -a fuer mail alltimes, auch wenn Logdateien keine Fehler- oder Errormeldung enthalten
# description:
# Suche die letzte Logdatei des aktuellen Datums
# Wird eine heutige Datei gefunden, Pruefung auf returncode ungleich 0
# Wenn ungleich 0, hole die Fehlermeldung aus der Logdatei
# Sende Mail mit Fehlermeldung an QA.
# Wird keine heutige Logdatei gefunden, Mail an QA
# Sende bei Parameter -a immer ein Mail, auch wenn kein Fehler oder Error gefunden wurde
# date:       20170530
# change:     wian
# tc760 auskommentiert, stattdessen qlab-2
# date:       20170921
# change:     wian
# Unterverzeichnisse Smoketest und GuiTests beruecksichtigen
# An mehrere Mailadressen versenden ermoeglichen
# date:       20171222
# change:     wian
# parameters: -sd searchdirectory: nur in bestimmtem Verzeichnis durchsuchen
# set -x

# TCAPI Tosca Returncode in logfile
RC=0
# local Returncode
l_RC=0
# debug=1
# usage=TCAPI_sendmail.ksh -a mailadresse@lbase.software -sd=GuiTests
[ "$1" == "-a" ] && p1=$1 && shift
[ "`echo $1|cut -f2 -d@`" == "lbase.software" ] && addMailAdress=$1 && shift
[ "`echo $1|cut -f1 -d=`" == "-sd" ] && p2="`echo $1|cut -f2 -d=`" && shift
param=$*

debug() {
    [ "$debug" -eq 1 ] && echo $@
}

debug DEBUG: start $0
[ "$debug" -eq 1 ] && set -x
today=`date '+%Y%m%d'`
SourceDir=/LBase/QA/tosca/Reports
TCAPImailtxt=/tmp/TCAPImailtxt.txt
file2send=${TCAPImailtxt}
[ "$debug" -eq 1 ] && SourceDir=~/tmp

[ "${param}" == "" ] && sendto="qualityassurance@lbase.software willi.andric@lbase.software"
# [ "${param}" == "" ] && sendto="willi.andric@lbase.software"
if [ "${param}" != "" ] ; then
   while [ $# -gt 0 ]       #Solange die Anzahl der Parameter ($#) groesser 0
   do
      [ "`echo $1|cut -f2 -d@`" == "lbase.software" ] && sendto="${sendto} ${1}"
      shift
   done
fi
[ "${addMailAdress}" != "" ] && sendto="${sendto} ${addMailAdress}"

[ "$debug" -eq 1 ] && sendto="willi.andric@lbase.software"
debug DEBUG: sendto=willi.andric@lbase.software
# sendtoCC=claus.vogelmann@lbase.software
LogfileErr=
Msg=
# tcString=tc760
tcString=qlab-
# Fehlermeldung="Fehler in der Ausfuehrung"
Fehlermeldung="\!\!\! FAILED:"
# Errormeldung="Error in der Ausfuehrung"
Errormeldung="\!\!\! ERROR:"
# TCExceptionmeldung= Task: Exception!
TCExceptionmeldung=" Task: Exception!"
# Verzeichnisse, in denen gesucht werden soll:
# DIRPATH[0]="GuiTests";DIRPATH[1]="Smoketests";DIRPATH[2]="TestAPI"
DIRPATH[0]="GuiTests";DIRPATH[1]="Smoketests";DIRPATH[2]="GuiFeatureTests";DIRPATH[3]="WebGuiTests"
# -sd=searchdirectory: wenn gesetzt, nur dieses Verzeichnis durchsuchen
[ "${p2}" != "" ] && unset DIRPATH && DIRPATH[0]="${p2}"
typeset cntDIRPATH=${#DIRPATH[*]}
typeset currDIRPATH=0

# Fehler oder Error an Mail
FehlerMailMsg="Fehler in der Tosca Ausfuehrung"
ErrMailMsg="Error in der Tosca Ausfuehrung"

send_email() {
    debug DEBUG: in function send_email
    if [ "${RC}" -eq 0 -a "${p1}" = "-a" ] ; then
        Msg="OK - ${p2}: die Tosca Ausfuehrung war erfolgreich."
        echo "Returncode      : ${RC}" >>${TCAPImailtxt}
        debug DEBUG: mpack -s \"${Msg}  ${file2send}\" -d \"${TCAPImailtxt}\" ${file2send} ${sendto}
        debug DEBUG: ---------------- BEGIN: Mail Text in ${TCAPImailtxt} ----------------
        debug `cat ${TCAPImailtxt}`
        debug DEBUG: ---------------- END: Mail Text in ${TCAPImailtxt} ----------------
        debug DEBUG: ...
        file2send=${TCAPImailtxt}
        echo "mpack -s \"${Msg}  ${file2send}\" -d \"${TCAPImailtxt}\" ${file2send} ${sendto}"
        mpack -s "${Msg}  `basename ${file2send}`" -d "${TCAPImailtxt}" ${file2send} ${sendto}
        l_RC=$?
        echo "Returncode      : ${RC}" >>${TCAPImailtxt}
    fi

    if [ "${RC}" -ne 0 ] ; then
        debug DEBUG: mpack -s \"${Msg}  `basename ${file2send}`\" -d \"${TCAPImailtxt}\" ${file2send} ${sendto}
        debug DEBUG: ---------------- BEGIN: Mail Text in `basename ${TCAPImailtxt}` ----------------
        debug `cat ${TCAPImailtxt}`
        debug DEBUG: ---------------- END: Mail Text in `basename ${TCAPImailtxt}` ----------------
        debug ...
        echo "Returncode      : ${RC}" >>${TCAPImailtxt}
        mpack -s "${Msg} ${p2}: `basename ${file2send}`" -d "${TCAPImailtxt}" ${file2send} ${sendto}
        l_RC=$?
        [ "${l_RC}" -ne 0 ] && echo "Scriptfehler beim Versenden mit mpack in $0, returncode: ${l_RC}"
        [ "${l_RC}" -ne 0 ] && echo "Scriptfehler beim Versenden mit mpack in $0, returncode: ${l_RC}" >>${TCAPImailtxt}
    fi
}
    
MeldungErmitteln() {
# Fehlermeldung ermitteln
    debug DEBUG: in function MeldungErmitteln
    meldung=$1
    file=$2
    msg=$3
    currDir=$4
    n_f=
    Msg="${p2}:"
    if [ -n "`grep \"${meldung}\" \"${file}\"`" ] ; then # nicht leer
        debug DEBUG: Logdatei ${file} mit $meldung		
        Msg="${msg} zu Executionlist in ${currDir} - Logdatei: "
		# Msg in TCAPImailtxt ergaenzen
		echo ""  >>${TCAPImailtxt}
		Msg="${Msg} ${msg} zu Executionlist in ${currDir} - Logdatei: "
		echo "$Msg"  >>${TCAPImailtxt}
        # und Zeilennummer ermitteln
        n_f=`grep -in "${meldung}" "${file}" | cut -f1 -d:|tail -1`
        # ab Zeilennummer bis Ende in Mail ausgeben
        echo "Ausschnitt der Meldung in ${file}:" >>${TCAPImailtxt}
        tail -$((ln-$n_f +4)) "${file}" >>${TCAPImailtxt}
        # Mail senden, da Fehler gefunden
        debug DEBUG: vor Mail mit Fehlermeldung senden
        RC=1
        # send_email erst, wenn alle Dateien durch
        debug DEBUG: Meldung: $Msg
    fi
}

## main() #{
cd ${SourceDir}
## wenn 'wievieltes Verzeichnis' < 'Anzahl Verzeichisse'
echo "Wie viele Verzeichnisse gesamt: $cntDIRPATH" >>${TCAPImailtxt}
typeset cntMax=15
while (( currDIRPATH < cntDIRPATH ))
do
    echo `date` >${TCAPImailtxt}
	# suche in $DIRPATH[$currDIRPATH]/<heutiges Datum>
    currSourceDir="${DIRPATH[${currDIRPATH}]}/${today}"
    p2=${currSourceDir}
    # Suche die letzte Logdatei des aktuellen Datums
    # if [ "`find ${SourceDir}/${currSourceDir} -type f -mtime -1 -name '*${tcString}*.log'|tail -1`" = "" ] ; then
    debug DEBUG: cd ${SourceDir}/${currSourceDir}
    # existiert ${SourceDir}/${currSourceDir}? Wenn nicht, raus!
    if [ ! -d "${SourceDir}/${currSourceDir}" ] ; then
        echo Verzeichnis ${SourceDir}/${currSourceDir} fuer die Logdateien zu Ausfuehrungen mit Tosca existiert nicht! > ${TCAPImailtxt}
        echo Wahrscheinlich war die Ausfuehrung nicht moeglich und wurde daher gar nicht durchgefuehrt \(oder ist in Warteschleife\). >> ${TCAPImailtxt}
        Msg="${p2} ACHTUNG: Es wurde kein Verzeichnis ${SourceDir}/${currSourceDir} fuer die Logdateien zu Ausfuehrungen mit Tosca gefunden!"
        file2send=${TCAPImailtxt}
        RC=1
        send_email
        echo "Returncode      : ${l_RC}"
		# ((cntDIRPATH--))
		[ $cntDIRPATH > 1 ] && ((currDIRPATH++))
        # exit ${l_RC}
    # fi
	 else
    # Wechsle in Unterverzeichnis von heute ${currSourceDir} und suche Logdateien von heute
		 cd ${SourceDir}/${currSourceDir}
		 if [ -z `find . -type f -mtime -1 -name "*${tcString}*.log"|tail -1` ] ; then
			  # Wenn keine heutige gefunden, Mail
			  # send message: no logfile
			  echo Es wurde keine Logdatei zu einer in den letzten 24 h ausgefuehrten Tosca Ausfuehrungsliste gefunden. > ${TCAPImailtxt}
			  echo Moeglicherweise war die Ausfuehrung nicht erfolgreich oder sie wurde gar nicht durchgefuehrt. >> ${TCAPImailtxt}
			  Msg="${p2} ACHTUNG: Es wurde keine Logdatei zu Ausfuehrungen mit Tosca gefunden!"
			  [ "${debug}" -eq 1 ] && echo DEBUG: $Msg
			  [ "${debug}" -eq 1 ] && echo DEBUG: vor Mail mit Fehlermeldung senden
			  file2send=${TCAPImailtxt}
			  RC=1
			  send_email
			  echo "Returncode      : ${l_RC}"
			  # [ $cntDIRPATH > 1 ] && ((currDIRPATH++))
			  # exit ${l_RC}
		 else
			 # Zuerst pruefen, ob ueberhaupt eine Zeile mit "returncode: " im Log vorhanden ist, wenn nicht, Mail senden.
			 # for i in `find ${SourceDir}/${currSourceDir} -type f -mtime -1 -exec grep -il "returncode: [1-9]" {} \;`
			 # countRC_notZero=`find . -type f -mtime -1 -name "*${tcString}*.log" -exec grep -il "returncode: -[1-9]" {} \;|wc -l`
			 foundfiles=foundFilesRC_notZero_${DIRPATH[${currDIRPATH}]}_${today}
			 find . -type f -mtime -1 -name "*${tcString}*[A-Za-z].log" -exec grep -il "returncode: -[1-9]" {} \; >/tmp/${foundfiles}.txt
			 if [ -s /tmp/${foundfiles}.txt ] ; then
				  file2send=${foundfiles}.zip
				  zip -r -9 -j -q ${foundfiles}.zip /tmp/${foundfiles}.txt
				  for i in `cat /tmp/${foundfiles}.txt`
				  do
						debug DEBUG: bin in for- Schleife, die Dateien findet, welche returncode: -[1-9] enthalten
						zip -u -9 -j -q ${foundfiles}.zip ${i} 
						# gesamt Zeilen in $i
						ln=`wc -l ${i}|cut -f1 -d' '`
						# Fehler- oder Errormeldung filtern, wenn vorhanden
						MeldungErmitteln "${Fehlermeldung}"      "${i}" "${FehlerMailMsg}" "${DIRPATH[${currDIRPATH}]}"
						MeldungErmitteln "${Errormeldung}"       "${i}" "${ErrMailMsg}"    "${DIRPATH[${currDIRPATH}]}"
						MeldungErmitteln "${TCExceptionmeldung}" "${i}" "${ErrMailMsg}"    "${DIRPATH[${currDIRPATH}]}"
				  done
				# Wenn JPeg Dateien vorhanden, in Zipdatei ergaenzen
				find . -type f -iname "*.jpg" >/dev/null
				[ "$?" == "0" ] && zip -u -9 -j -q ${foundfiles}.zip *.jpg
				Msg="${p2} Mail zu `wc -l /tmp/${foundfiles}.txt|cut -f1 -d' '` Fehlern bei Tosca Executionlists in ${DIRPATH[${currDIRPATH}]} - Logdateien: "
				zip -u -9 -j -q ${foundfiles}.zip /tmp/TCAPI_sendmail.ksh.log
				send_email
				debug DEBUG: nach Mail mit Fehlermeldungen aus ${DIRPATH[${currDIRPATH}]} senden
				rm /tmp/${foundfiles}.txt
			 fi
			 # Wenn nichts fehlerhaftes gefunden, dann keine Fehler und keine Error in Logdateien
			 # Mail nur, wenn Parameter -a
			 if [ ${RC} == "0" ] ; then
				  Msg="OK - ${p2}: die Tosca Ausfuehrung war erfolgreich."
				  debug DEBUG: keine Logdatei mit Fehler- oder Errormeldung im Verzeichnis:
				  debug DEBUG: ${SourceDir}/${currSourceDir}
				  echo $Msg >>${TCAPImailtxt}
				  echo Keine Logdatei mit Fehler- oder Errormeldung im Verzeichnis: >>${TCAPImailtxt}
				  echo ${SourceDir}/${currSourceDir} >>${TCAPImailtxt}
				  debug DEBUG: vor Mail ohne Fehler/Error senden, wenn Parameter -a
				  [ "${p1}" = "-a" ] && send_email
			 fi
			 ##### WIAN ####
			 # find . -type f -mtime -1 -name "*${tcString}*.log" > /tmp/fnd_tcString_TCAPI.lst
			 # grep -hic `find . -type f -name "*.sh"`|sort -n|tail -1 2>/dev/null
			 # rtcdfile=`grep -f /tmp/fnd_tcString_TCAPI.lst -ilc "returncode: -[1-9]" `
			 # if [ "${rtcdfile}" = "" -a "${p1}" = "-a" ] ; then
				  # # Wenn keine Datei mit returncode != 0 gefunden, dann keine Fehler und keine Error in Logdateien
				  # Msg="OK - die Tosca Ausfuehrung war erfolgreich."
				  # debug DEBUG: keine Logdatei mit Fehler- oder Errormeldung gefunden
				  # echo $Msg >>${TCAPImailtxt}
				  # debug DEBUG: vor Mail ohne Fehler/Error senden, wenn Parameter -a
				  # [ "${p1}" = "-a" ] && send_email
			 # fi

			 debug DEBUG: Ende der Verarbeitung in Verzeichnis:
			 debug DEBUG: ${SourceDir}/${currSourceDir}
		 fi
		 ((cntDIRPATH--))
		 [ $cntDIRPATH > 1 ] && ((currDIRPATH++))

	 fi
	((cntMax--))
	[ $cntMax < 1 ] && exit -1

done
# } main END

# [ "${l_RC}" -eq 0 -a "$debug" -ne 1 ] && rm ${TCAPImailtxt}

echo "Returncode      : ${l_RC}"
exit ${l_RC}

