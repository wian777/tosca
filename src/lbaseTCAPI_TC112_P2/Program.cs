using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using LBASE_Smoketests;
// using Tricentis
using Tricentis.TCAPI;
using Tricentis.TCAPIObjects.Objects;
using Tricentis.TCAPINativeConnector;

namespace LBASE_Smoketests
{

    /// <summary>
    /// Takes a fullscreen screenshot of the monitor and saves the specified file in a directory with custom name.
    /// It expects the Format of the file.
    /// </summary>
    /// <param name="filepath"></param>
    /// <param name="filename"></param>
    /// <param name="format"></param>
    public class FullScreenshot
    {
        public void GetFullScreenshot(String filepath, String filename, ImageFormat format)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                string fullpath = filepath + "\\" + filename;

                bitmap.Save(fullpath, format);
            }
        }
    }

    /// <summary>
    /// Startet TCApi und führt Ausführungslisten aus, welche die in LBASE_Smoketest.exe.config enthaltene ExecutionProperty auf True haben.
    /// Je nach Ergebniss der Ausführungsliste wird reagiert und in Logdateien geschrieben:
    /// EXCEPTION: Ausführung wird mit Erstellung eines Screenshots und Logdatei mit Benennung ReportLogfile + _EXCEPTION.jpg bzw. ReportLogfile + "_EXCEPTION.log" beendet.
    /// ERRROR: Ausführung wird mit Erstellung eines Screenshots und Logdatei mit Endung *_ERROR.log beendet.
    /// FAILURE: Ausführung wird mit Erstellung eines Screenshots und Logdatei mit Endung *_FAILURE.log beendet.
    /// NORESULT: Ausführung wird mit Erstellung eines Screenshots mit Endung *_NORESULT.jpg weiter ausgeführt.
    /// </summary>
    /// <param name=""></param>
    class Program
    {
        static int Main(string[] args)
        {
            // TODO: Set Tosca Settings to Engine.ExceptionHandling = Ignore (aber wie?)
            // String ttt = TCAPI.Instance.MetaSettingsPath;
            String ExecutionProperty = Properties.Settings.Default.ExecutionlistProperties;
            TextDatei c_textdatei = new TextDatei();
            List<string> l_savesettings = null;
            l_savesettings = new List<string> { Properties.Settings.Default.LogfilePath + Properties.Settings.Default.WorkspaceName + @"\Settings\ProjectSettings.xml" };
            // l_savesettings.Add("" + "" + "Tricentis.TCAPIObjects.Objects" );
            // SaveSettingsOfTCExecution c_savesettings = new SaveSettingsOfTCExecution();
            int logCount = 0;
            // FullScreenshot 
            FullScreenshot fss = new FullScreenshot();
            // printScreen // ( Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "screenshot.jpg", ImageFormat.Jpeg);
            String t_DateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // String Logfile = Properties.Settings.Default.LogfilePath + Properties.Settings.Default.LogfileName + "_" + time.ToString(format) + ".log";
            // String Logfile = Properties.Settings.Default.LogfilePath + Properties.Settings.Default.LogfileName + "_" + t_DateTime + ".log";
            String Logfile = Properties.Settings.Default.LogfilePath + Properties.Settings.Default.LogfileName + "_" + logCount + "_" + t_DateTime + ".log";
            String origLogfile = Logfile;
            String computername = null;
            String tricentis_allusers_appdata = null;
            String tc_user_appdata = null;
            String copysettingsdestination = null;
            int returnValue = 0;
            // Boolean isSplit = false;
            bool isCheckedOut = false;
            bool isSplit = false;
            String t_CheckinList = "Ausführungsliste(n): ";
            String t_projectSearchSplitFolder = "";
            // File.OpenWrite(Logfile);
            c_textdatei.WriteFile(origLogfile, "BEGIN Logfile DATE and TIME: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") +
                                       "\r\n---------------------------------------------------" + "\r\n");
            #region Console einrichten
            // Console.SetWindowSize(100, 55);
            Console.SetWindowSize(100, 51);
            Console.Title = "TC-API execution, do not close!";
            String ConsoleTitle = Console.Title;

            #region Versuchsbereich
            /*
             * Versuchsbereich vor dem Start:
            Console.Title = ConsoleTitle + " Task: Directories exists?";
            if (Directory.Exists(Path.GetDirectoryName(@"C:\temp\")))
            {
                Console.Title = ConsoleTitle + @" Task: Directory c:\temp\ exists: OK";
            }
            else
            {
                Console.Title = ConsoleTitle + @" Task: Directory c:\temp\ NOT exists!";
            }
            */
            /*
            Thread.Sleep(100000);
            Console.ReadKey();
            */
            #endregion Versuchsbereich

            #endregion Console einrichten

            #region Umgebungsvariablen ermitteln und ausgeben
            Console.Title = ConsoleTitle + " Task: GetEnvironmentVariables";
            c_textdatei.Append(origLogfile, " Task: GetEnvironmentVariables\r\n---------------------------------------------------\r\n");
            foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables())
            {
                if (variable.Key.ToString().ToLower() == "computername")
                {
                    computername = variable.Value.ToString().ToLower();
                }
                if (variable.Key.ToString().ToLower() == "tricentis_allusers_appdata")
                {
                    tricentis_allusers_appdata = variable.Value.ToString().ToLower();
                }
                if (variable.Key.ToString().ToLower() == "appdata")
                {
                    tc_user_appdata = variable.Value.ToString().ToLower() + @"\TRICENTIS\TOSCA TestSuite\7.0.0";
                }
                Console.WriteLine(variable.Key + "=" + variable.Value);
                //Text an eine Datei anhängen
                c_textdatei.Append(origLogfile, variable.Key + "=" + variable.Value + "\r");
            }
            #endregion Umgebungsvariablen

            #region Log End- Zielverzeichnisse
            // Unterverzeichnis für Report-Logfiles anlegen, wenn noch nicht vorhanden:
            String ReportLogfilePath = Properties.Settings.Default.ReportPath + (t_DateTime).Substring(0, 8) + @"\";
            if (!Directory.Exists(ReportLogfilePath))
            {
                Directory.CreateDirectory(ReportLogfilePath);
            }
            String ReportLogfile = ReportLogfilePath + Properties.Settings.Default.LogfileName + "_" +
                t_DateTime + "_" + computername;
            String ExecReportLogfile = ReportLogfilePath + "tmp.log";
            #endregion Log End- Zielverzeichnisse

            l_savesettings.Add(tricentis_allusers_appdata + @"\Settings\XML\Settings.xml");
            l_savesettings.Add(tc_user_appdata + @"\dataexchange\xml\TestSet3c6d653e.xml");
            l_savesettings.Add(tc_user_appdata + @"\output.xml");

            TCAPI api = null;
            TCWorkspace workspace = null;

            try
            {
                #region open TCAPI, workspace and update workspace
                // wegen Exception Licence für ToscaCommanderAPI ins try versetzt
                copysettingsdestination = Properties.Settings.Default.LogfilePath + t_DateTime + "_" + logCount;
                Console.Title = ConsoleTitle + " Task: CreateInstance";
                c_textdatei.Append(origLogfile, " Task: CreateInstance\r\n---------------------------------------------------\r\n");
                // ACHTUNG: Die Lizenz für TCAPI muss natürlich vorhanden sein, sonst:
                // Tricentis.TCAPIObjects.Exceptions.TCApiException was unhandled
                // TCAPI api = TCAPI.CreateInstance();
                api = TCAPI.CreateInstance();
                // Stoppuhr initial starten
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long lastTimeStamp = 0;
                Console.Title = ConsoleTitle + " Task: OpenWorkspace";
                c_textdatei.Append(origLogfile, " Task: OpenWorkspace\r\nExistiert der Workspace " + Properties.Settings.Default.WorkspacePath + "?\r\n");
                if (!File.Exists(Properties.Settings.Default.WorkspacePath))
                {
                    Console.Title = ConsoleTitle + " Workspace " + Properties.Settings.Default.WorkspacePath + " existiert nicht!";
                    c_textdatei.Append(origLogfile, "Fehler: Der Workspace " + Properties.Settings.Default.WorkspacePath + " existiert nicht!\r\n");

                    // WrapUp before end: copy logfile to ReportLogfilePath
                    Console.Title = ConsoleTitle + " Task: copy Logfile";
                    c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                    if (File.Exists(ReportLogfile + ".log"))
                    {
                        File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                    }
                    else
                    {
                        File.Copy(origLogfile, ReportLogfile + ".log");
                    }
                    TCAPI.CloseInstance();
                    return -1;
                }
                else
                {
                    c_textdatei.Append(origLogfile, "Workspace:  OK\r\n");
                    if (File.Exists((Properties.Settings.Default.WorkspacePath + ".txt")))
                    {
                        if (Properties.Settings.Default.LockFileDelete == true)
                        {
                            Console.Title = ConsoleTitle + " Workspace " + Properties.Settings.Default.WorkspacePath +
                            " ist gesperrt oder die Lock-Datei <WorkspaceName.tws.txt> ist noch im Verzeichnis vorhanden! Die Ausführung wird daher beendet.";
                            c_textdatei.Append(origLogfile, "!!! FAILED: Workspace " + Properties.Settings.Default.WorkspacePath +
                            " ist gesperrt \r\n oder die Lock-Datei <WorkspaceName.tws.txt> ist noch im Verzeichnis vorhanden! Die Ausführung wird daher beendet.\r\n" +
                            "Execution ended with failure returncode -1 at " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nreturncode: -1\r\nEND");

                        // WrapUp before end: copy logfile to ReportLogfilePath
                            Console.Title = ConsoleTitle + " Task: copy Logfile";
                            c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                            if (File.Exists(ReportLogfile + ".log"))
                            {
                                File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                            }
                            else
                            {
                                File.Copy(origLogfile, ReportLogfile + ".log");
                            }
                            TCAPI.CloseInstance();
                            return -1;
                        }
                        else
                        {
                            try
                            {
                                Console.Title = ConsoleTitle + " Workspace " + Properties.Settings.Default.WorkspacePath +
                                " ist gesperrt oder die Lock-Datei " + Properties.Settings.Default.WorkspaceName + ".txt ist noch im Verzeichnis vorhanden! Dieses wird gelöscht und die Ausführung weiter durchgeführt.";
                                c_textdatei.Append(origLogfile, "! WARN: Workspace " + Properties.Settings.Default.WorkspacePath +
                                    " ist gesperrt.\r\n Die Lock-Datei " + Properties.Settings.Default.WorkspaceName + ".txt ist noch im Verzeichnis vorhanden!\r\n" +
                                    "Die Lockdatei wird gelöscht und die Ausführung wird vortgesetzt.\r\n");
                                File.Delete((Properties.Settings.Default.WorkspacePath + ".txt"));
                                if (!File.Exists((Properties.Settings.Default.WorkspacePath + ".txt")))
                                {
                                    workspace = OpenWSMeth(api);
                                    Console.Title = ConsoleTitle + " OpenWorkspace: OK";
                                    c_textdatei.Append(origLogfile, "OpenWorkspace: OK\r\n---------------------------------------------------\r\n");
                                }
                                
                            }
                            catch (NullReferenceException e)
                            {
                                Console.Title = ConsoleTitle + " Workspace " + Properties.Settings.Default.WorkspacePath +
                                " ist gesperrt oder die Lock-Datei <WorkspaceName.tws.txt> ist noch im Verzeichnis vorhanden und konnte nicht gelöscht werden! Die Ausführung wird daher beendet.";
                                c_textdatei.Append(origLogfile, "!!! FAILED: Workspace " + Properties.Settings.Default.WorkspacePath +
                                " ist gesperrt, da die Lock-Datei "+ Properties.Settings.Default.WorkspaceName + ".txt noch im Verzeichnis vorhanden ist und nicht gelöscht werden konnte! Die Ausführung wird daher beendet.\r\n" +
                                "Execution ended with failure returncode -1 at " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\n" +
                                "Fehler Exception bei OpenWorkspace: " + e + "\r\nreturncode: -1\r\nEND");
                                Console.Title = ConsoleTitle + "Fehler Exception bei OpenWorkspace: " + e;
                                
                                Console.Title = ConsoleTitle + " Task: copy Logfile";
                                c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                                if (File.Exists(ReportLogfile + ".log"))
                                {
                                    File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                                }
                                else
                                {
                                    File.Copy(origLogfile, ReportLogfile + ".log");
                                }
                                TCAPI.CloseInstance();
                                throw e;
                            }

                        }
                    }
                    else
                    {
                        workspace = OpenWSMeth(api);
                        Console.Title = ConsoleTitle + " OpenWorkspace: OK";
                        c_textdatei.Append(origLogfile, "OpenWorkspace: OK\r\n---------------------------------------------------\r\n");
                    }

                }
                Console.WriteLine("Creating the API Instance and open workspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "Creating the API Instance and open workspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                /*
                Console.Title = ConsoleTitle + " Task: exists Worspace";
                c_textdatei.Append(origLogfile, " Task: exists Worspace\r\n---------------------------------------------------\r\n");
                if (!File.Exists(Properties.Settings.Default.WorkspacePath))
                {
                    Console.WriteLine("Der Workspace existiert nicht!");
                    c_textdatei.Append(origLogfile, "Der Workspace " + Properties.Settings.Default.WorkspacePath + " existiert nicht!" + "\r\n");
                    // WrapUp before end: copy logfile to ReportLogfilePath
                    Console.Title = ConsoleTitle + " Task: copy Logfile";
                    c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                    if (File.Exists(ReportLogfile + ".log"))
                    {
                        File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                    }
                    else
                    {
                        File.Copy(origLogfile, ReportLogfile + ".log");
                    }
                    TCAPI.CloseInstance();
                    return -1;
                }
                */
                Console.Title = ConsoleTitle + " Task: open Workspace";
                c_textdatei.Append(origLogfile, " Task: open Workspace\r\n---------------------------------------------------\r\n");
                Console.WriteLine("Opening Workspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "Opening Workspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                Console.Title = ConsoleTitle + " Task: update Workspace";
                c_textdatei.Append(origLogfile, " Task: update Workspace\r\n---------------------------------------------------\r\n");
                workspace.UpdateAll();
                Console.WriteLine("Updating Workspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "Updating Workspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;

                /*
                Console.WriteLine("CompactWorkspace start");
                workspace.CompactWorkspace();
                Console.WriteLine("CompactWorkspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "CompactWorkspace took " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                */
                #endregion open TCAPI, workspace and update workspace

                // Eintrag ExecutionlistProperties in LBASE_Smoketests.exe.config finden, z.B. OvernightExecution
                Console.Title = ConsoleTitle + " Task: search project with " + ExecutionProperty;
                c_textdatei.Append(origLogfile, " Task: search project with " + ExecutionProperty +
                    " (set in LBASE_Smoketests.config ExecutionlistProperties)\r\n" +
                    "---------------------------------------------------\r\n");
                // ExecutionProperty
                TCProject project = workspace.GetProject();
                // TCFolder.List foundFolders             = new TCFolder.List();

                ExecutionList.List foundExecutionLists = new ExecutionList.List();
                // suche alle Executionlists, die das Property "OvernightExecution" (oder andere) auf true haben:
                // Ausgabe des Ergebnisses von foundExecutionLists.AddAllFrom

                #region CheckOutState abfragen
                String ExecutionListsFound = "=>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState != \"CheckedOut\")]";
                foundExecutionLists.AddAllFrom(project.Search("->SORT(" + ExecutionListsFound + ",\"Name\")"));
                if (project.Search(ExecutionListsFound).Count >= 1)
                {

                    if (Properties.Settings.Default.checkoutable == true)
                    {
                        // Ausführungsliste darf natürlich auch nicht von anderen ausgecheckt sein
                        if (project.Search("=>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState == \"CheckedOut\")]").Count >= 1)
                        {
                            // Ausgabe der ausgecheckten Executionlists per Suche:
                            // =>SORT(=>SUBPARTS:ExecutionList[(OvernightExecutionGuiTests=="true")and(CheckOutState=="CheckedOut")],"Name")
                            Console.Title = ConsoleTitle + " Executionlists with property " + ExecutionProperty + " were found, but some of them are checked out!";
                            Console.WriteLine("Executionlists with property " + ExecutionProperty + " were found, but some of them are checked out!");
                            Console.WriteLine("SearchString: =>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState == \"CheckedOut\")]");
                            c_textdatei.Append(origLogfile, "Executionlists with property " + ExecutionProperty + " were found, but some of them are checked out!" + "\r\n");
                            c_textdatei.Append(origLogfile, "SearchString: =>SUBPARTS:ExecutionList[(" + ExecutionProperty +
                                        "=i=\"True\") and (CheckOutState == \"CheckedOut\")]" + "\r\n");

                            foreach (ExecutionList foundExecutionList in foundExecutionLists)
                            {
                                // bool chckState = foundExecutionList.IsCheckedOutByMe;
                                c_textdatei.Append(origLogfile, "if (foundExecutionList.IsCheckedOutByMe == true )");
                                if (foundExecutionList.IsCheckedOutByMe == true)
                                {
                                    Console.WriteLine("Workspace.CheckInAll, because IsCheckedOutByMe: " + foundExecutionList.NodePath);
                                    c_textdatei.Append(origLogfile, "Workspace.CheckInAll, because IsCheckedOutByMe: " + "\r\n   Nodepath: " +
                                        foundExecutionList.NodePath + "\r\n   UniqueId: " +
                                        foundExecutionList.UniqueId + "\r\n");
                                    workspace.CheckInAll("CheckinAll, because IsCheckedOutByMe was found at executing " + ExecutionProperty + "!");
                                }
                            }
                            // wenn immer noch ausgecheckte vorhanden, dann nicht vom aktuellen Benutzer oder aktuellen Workspace
                            if (project.Search("=>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState == \"CheckedOut\")]").Count >= 1)
                            {
                                // returnValue = -1;
                                Console.Title = ConsoleTitle + " Executionlists with property " + ExecutionProperty + " were found, but some of them are checked out!";
                                Console.WriteLine("INFO: Executionlists with property " + ExecutionProperty + " were found, but some of them are checked out!");
                                Console.WriteLine("SearchString: =>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState == \"CheckedOut\")]");
                                c_textdatei.Append(origLogfile, "Executionlists with property " + ExecutionProperty + " were found, but some of them are checked out!" + "\r\n");
                                c_textdatei.Append(origLogfile, "SearchString: =>SUBPARTS:ExecutionList[(" + ExecutionProperty +
                                            "=i=\"True\") and (CheckOutState == \"CheckedOut\")]" + "\r\n");
                                c_textdatei.Append(origLogfile, "returncode: -1" + "\r\n");
                                // ExecutionListsFound = "=>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState == \"CheckedOut\")]";
                                // WrapUp before end: copy logfile to 
                                Console.Title = ConsoleTitle + " Task: copy Logfile";
                                c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                                if (File.Exists(ReportLogfile + "_CHECKEDOUTNodePath.log"))
                                {
                                    File.Replace(origLogfile, ReportLogfile + "_CHECKEDOUTNodePath.log", ReportLogfile + "_CHECKEDOUTNodePath.bkp");
                                }
                                else
                                {
                                    File.Copy(origLogfile, ReportLogfile + "_CHECKEDOUTNodePath.log");
                                }
                                // TCAPI.CloseInstance();
                                // return -1;
                            }
                        }
                    }
                }

                ExecutionListsFound = "=>SUBPARTS:TCFolder[(" + ExecutionProperty + "=i=\"True\")]=>SUBPARTS:ExecutionList"; // alle unter TCFolder mit Property finden
                if (project.Search(ExecutionListsFound).Count >= 1)
                {
                    if (project.Search("=>SUBPARTS:TCFolder[(" + ExecutionProperty + "=i=\"True\")]=>SUBPARTS:ExecutionList[(CheckOutState == \"CheckedOut\")]").Count >= 1)
                    {
                        returnValue = -1;
                        // Ausgabe der ausgecheckten Executionlists per Suche:                        
                        // =>SORT(=>SUBPARTS:TCFolder[(OvernightExecutionSplit=i="true")and(CheckOutState=="CheckedOut")],"Name")
                        // da in Split (OvernightExecutionSplit), sind die Ausführungslisten voneinander abhängig, also run TCAPI.CloseInstance():
                        Console.Title = ConsoleTitle + " Executionlists were found with property " + ExecutionProperty + ", but some of them are checked out!";
                        Console.WriteLine("Executionlists were found with property " + ExecutionProperty + ", but some of them are checked out!");
                        Console.WriteLine("SearchString: =>SUBPARTS:TCFolder[(" + ExecutionProperty + "=i=\"True\")]=>SUBPARTS:ExecutionList[(CheckOutState == \"CheckedOut\")]");
                        c_textdatei.Append(origLogfile, "Executionlists were found with property " + ExecutionProperty + ", but some of them are checked out!" + "\r\n");
                        c_textdatei.Append(origLogfile, "SearchString: =>SUBPARTS:TCFolder[(" + ExecutionProperty +
                            "=i=\"True\")]=>SUBPARTS:ExecutionList[(CheckOutState == \"CheckedOut\")]" + "\r\n");
                        // WrapUp before end: copy logfile to ReportLogfilePath
                        Console.Title = ConsoleTitle + " Task: copy Logfile";
                        c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                        if (File.Exists(ReportLogfile + ".log"))
                        {
                            File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                        }
                        else
                        {
                            File.Copy(origLogfile, ReportLogfile + ".log");
                        }
                        TCAPI.CloseInstance();
                        return -1;
                    }
                }
                #endregion CheckOutState abfragen

                // Ausführungsliste suchen mit ExecutionProperty und nicht von anderen ausgecheckt
                String t_projectSearchSplit = "=>SUBPARTS:TCFolder[(" + ExecutionProperty + "=i=\"True\")]=>SORT(=>SUBPARTS:ExecutionList[(CheckOutState != \"CheckedOut\")],\"Name\")";
                /*
                if (t_projectSearchSplit != null)
                {
                    isSplit = true;
                    t_projectSearchSplitFolder = "=>SUBPARTS:TCFolder[(" + ExecutionProperty + "=i=\"True\")]";
                    Console.Title = ConsoleTitle + " No Executionlists found with property " + ExecutionProperty;
                    Console.WriteLine("Executionlists in Split-Folder " + t_projectSearchSplitFolder );
                    c_textdatei.Append(origLogfile, "Executionlists in Split-Folder " + t_projectSearchSplitFolder + "\r\n");
                }
                */

                #region wird eine Executionlist mit ExecutionProperty gefunden?
                String t_projectSearch = "=>SUBPARTS:ExecutionList[(" + ExecutionProperty + "=i=\"True\") and (CheckOutState != \"CheckedOut\")]";
                // if (project.Search("=>SUBPARTS:ExecutionList[OvernightExecution=i=\"True\"]").Count < 1)

                if (project.Search(t_projectSearch).Count < 1)
                {
                    if (project.Search(t_projectSearchSplit).Count < 1)
                    {
                        returnValue = -1;
                        Console.Title = ConsoleTitle + " No Executionlists found with property " + ExecutionProperty;
                        Console.WriteLine("No Executionlists found with property " + ExecutionProperty + "=\"True\"!");
                        Console.WriteLine("SearchString: " + t_projectSearchSplit);
                        c_textdatei.Append(origLogfile, "No Executionlists found with property " + ExecutionProperty + "=\"True\"!" + "\r\n");
                        c_textdatei.Append(origLogfile, "SearchString: " + t_projectSearchSplit + "\r\n");
                        // WrapUp before end: copy logfile to ReportLogfilePath
                        Console.Title = ConsoleTitle + " Task: copy Logfile";
                        c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                        if (File.Exists(ReportLogfile + ".log"))
                        {
                            File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                        }
                        else
                        {
                            File.Copy(origLogfile, ReportLogfile + ".log");
                        }
                        TCAPI.CloseInstance();
                        return -1;
                    }
                    else
                    {

                        // Ausgabe der Menge und alle gefundenen Pfade:
                        // Ordner mit Property OvernightExecutionSplit wurde gefunden und ist nicht ausgecheckt, daher in t_projectSearch
                        isSplit = true;
                        t_projectSearch = t_projectSearchSplit;
                        Console.Title = ConsoleTitle + " Task: split list about to execute";
                        Console.WriteLine("Executionlists Count: " + project.Search(t_projectSearchSplit).Count);
                        c_textdatei.Append(origLogfile, "Executionlists Count: " + project.Search(t_projectSearchSplit).Count + "\r\n");
                        c_textdatei.Append(origLogfile, " Task: split list about to execute\r\n" +
                            "---------------------------------------------------\r\n");
                    }
                }
                else
                {
                    // Ausgabe der Menge und alle gefundenen Pfade:
                    Console.Title = ConsoleTitle + " Task: list about to execute";
                    Console.WriteLine("Executionlists Count: " + project.Search(t_projectSearch).Count);
                    c_textdatei.Append(origLogfile, "Executionlists Count: " + project.Search(t_projectSearch).Count + "\r\n");
                    c_textdatei.Append(origLogfile, " Task: list about to execute\r\n" +
                        "---------------------------------------------------\r\n");
                }

                // foundExecutionLists.AddAllFrom(project.Search("=>SUBPARTS:ExecutionList[OvernightExecution=i=\"True\"]"));
                // foundExecutionLists.Clear();
                // foundExecutionLists.AddAllFrom(project.Search("->SORT(" + t_projectSearch + ",\"Name\")"));
                // foundExecutionLists
                Console.WriteLine("Search all " + t_projectSearch + " Executionlists " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "Search all " + t_projectSearch + " Executionlists " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                foreach (ExecutionList foundExecutionList in foundExecutionLists)
                {
                    Console.WriteLine("About to execute: " + foundExecutionList.NodePath);
                    c_textdatei.Append(origLogfile, "About to execute: " + "\r\n   Nodepath: " + foundExecutionList.NodePath + "\r\n   UniqueId: " +
                        foundExecutionList.UniqueId + "\r\n");
                    t_CheckinList = t_CheckinList + "\r\nNodepath: " + foundExecutionList.NodePath + " UniqueId: " + foundExecutionList.UniqueId + "";
                }
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                #endregion wird eine Executionlist mit ExecutionProperty gefunden?

                #region IsTaskApplicable
                // Prüfe, ob Executionlisten frei zum checkout sind:
                Console.Title = ConsoleTitle + " Task: IsTaskApplicable";
                c_textdatei.Append(origLogfile, " Task: IsTaskApplicable\r\n---------------------------------------------------\r\n");
                if (Properties.Settings.Default.checkoutable == true)
                {
                    if (!foundExecutionLists.IsTaskApplicable(TCTasks.Checkout))
                    {
                        returnValue = -1;
                        Console.Title = ConsoleTitle + "Executionlist kann nicht ausgecheckt werden!";
                        Console.WriteLine("Eine Executionlist kann nicht ausgecheckt werden!");
                        c_textdatei.Append(origLogfile, "Eine Executionlist kann nicht ausgecheckt werden!" + "\r\n");
                        // WrapUp before end: copy logfile to ReportLogfilePath
                        Console.Title = ConsoleTitle + " Task: copy Logfile";
                        c_textdatei.Append(origLogfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                        if (File.Exists(ReportLogfile + ".log"))
                        {
                            File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                        }
                        else
                        {
                            File.Copy(origLogfile, ReportLogfile + ".log");
                        }
                        TCAPI.CloseInstance();
                        return -1;
                    }
                    else
                    {
                        Console.WriteLine("Executionlists are checkout applicable ... " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                        c_textdatei.Append(origLogfile, "Executionlists are checkout applicable ... " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                        lastTimeStamp = stopwatch.ElapsedMilliseconds;
                        Console.Title = ConsoleTitle + " Task: Executionlist checkout";
                        c_textdatei.Append(origLogfile, " Task: Executionlist checkout\r\n---------------------------------------------------\r\n");
                        Console.WriteLine("Executionlist checkout: ");
                        c_textdatei.Append(origLogfile, "Executionlist checkout: \r\n");
                    }
                #endregion IsTaskApplicable

                #region Checkout
                // ALLES IN EXECUTIONLISTS AKTUALISIEREN
                // Checkout der gefundenen ExecutionLists
                // foundExecutionLists.Checkout();
                if (isCheckedOut == false) foundExecutionLists.CheckoutTree();
                isCheckedOut = true;
                Console.WriteLine("Executionlists checked out " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "Executionlists checked out " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                }
                // Include all necessary Items:
                // foundExecutionLists.IncludeAllNecessaryItems();
                    #endregion Checkout
                #region eventuelles senden von EMailAdresse
                // Sind vor der Ausführung Änderungen nötig?
                foreach (ExecutionList executionList in foundExecutionLists)
                {
                    /*
                    // eventuelles senden von EMailAdresse an ändern, wenn in LBASE_Smoketests.config ein Eintrag enthalten ist
                    if (Properties.Settings.Default.EMailAdresse != "")
                    {
                        c_textdatei.Append(origLogfile, "Settings EMailAdresse ist vorhanden: " + Properties.Settings.Default.EMailAdresse + "\r\n");
                        // c_textdatei.Append(Logfile, "ExecutionList PropertyNames: " + entry.ExecutionList.GetPropertyNames() + "\r\n");
                        c_textdatei.Append(origLogfile, "ExecutionList Name: " + entry.ExecutionList.Name + "\r\n");
                        // c_textdatei.Append(origLogfile, "ExecutionList PropertyNames: " + entry.ExecutionList + "\r\n");
                        // entry.GetPropertyValue("SendEMailTo");
                            
                        if (entry.GetPropertyValue("SendEMailTo").Count() > 0)
                        {
                            entry.SetAttibuteValue("SendEMailTo", Properties.Settings.Default.EMailAdresse);
                        }
                    }
                    else
                    {
                        c_textdatei.Append(origLogfile, "Settings EMailAdresse ist nicht vorhanden. " + "\r\n");
                    }
                    */

                    // momentane Mailadresse in TestConfigurationParameters SendEMailTo wegsichern, da später wieder zurückgesetzt
                    String o_ChangeMail = TestConfigurationHelper.GetTestConfigurationParameterValue(executionList, "SendEMailTo");

                    // Zur Fehlervermeidung sollte hier noch abgefangen werden, dass es bei erster Ausführung auch Executionlists ohne ActualExecutionLog geben kann!
                    // folgende Ausführung geht nur nach der zweiten Ausführung der Ausführungsliste gut (sobald ein Log vorhanden):
                    /*
                    if (executionList.ExecutionLogs.Count() > 0)
                    { 
                        // Bisheriges ExecutionLog archivieren:
                        Console.Title = ConsoleTitle + " Task: ArchiveActualExecutionLog";
                        c_textdatei.Append(origLogfile, " Task: ArchiveActualExecutionLog\r\n---------------------------------------------------\r\n");
                        
                        foundExecutionLists.ArchiveActualExecutionLog(DateTime.Now.ToString(),MsgBoxResult_YesNo.Yes, MsgBoxResult_YesNo.Yes);
                        // foundExecutionLists.ArchiveActualExecutionLog(DateTime.Now.ToString(), MsgBoxResult_YesNo.Yes);
                        Console.WriteLine("Executionlists archived " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                        c_textdatei.Append(origLogfile, "Executionlists archived " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                        lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    }
                    */
                }
                #endregion eventuelles senden von EMailAdresse

                #region Synchronize old
                /*
                // foundExecutionLists.CheckoutTree();
                Console.WriteLine("START foundExecutionLists.CheckoutTree()");
                c_textdatei.Append(origLogfile, "\r\nSTART foundExecutionLists.CheckoutTree() \r\n");
                foundExecutionLists.CheckoutTree();

                    // Archive actual ExecutionLog: z.B. API_20170928_1515
                    Console.WriteLine("START foundExecutionLists.ArchiveActualExecutionLog ...");
                    c_textdatei.Append(origLogfile, "\r\nfoundExecutionLists.ArchiveActualExecutionLog(Properties.Settings.Default.WorkspaceUser " +
                            "\"_\" + t_DateTime, \"Yes\", \"Yes\") \r\n");
                    foundExecutionLists.ArchiveActualExecutionLog(Properties.Settings.Default.WorkspaceUser +"_" + t_DateTime, "Yes", "Yes");
                */
                /*
                // Synchronize:
                // foundExecutionLists.Synchronize();
                Console.WriteLine("START foundExecutionLists.Synchronize() ...");
                c_textdatei.Append(origLogfile, "START foundExecutionLists.Synchronize() \r\n");
                foundExecutionLists.Synchronize();
                */
                #endregion Synchronize old

                // Jetzt geht's los
                foreach (ExecutionList executionList in foundExecutionLists) // alle gefundenen Ausführungslisten
                {
                    #region Synchronize
                    // foundExecutionLists.CheckoutTree();
                    Console.WriteLine("executionList.Name: " + executionList.Name);

                    c_textdatei.Append(origLogfile, "executionList.Name: " + executionList.Name + "\r\n");

                    if (Properties.Settings.Default.checkoutable == true)
                    {
                        Console.WriteLine("START executionList.CheckoutTree()");
                        Console.Title = "START executionList.CheckoutTree()";
                        c_textdatei.Append(origLogfile, "START executionList.CheckoutTree() \r\n");
                        executionList.IsTaskApplicable(executionList.Name);
                        executionList.CheckoutTree();                        
                    }

                    Console.WriteLine("executionList.IncludeAllNecessaryItems");
                    Console.Title = "executionList.IncludeAllNecessaryItems";
                    c_textdatei.Append(origLogfile, "\r\nexecutionList.IncludeAllNecessaryItems\r\n");
                    lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    executionList.IncludeAllNecessaryItems();
                    c_textdatei.Append(origLogfile, "Duration: " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                    Console.WriteLine("executionList.Synchronize");
                    Console.Title = "executionList.Synchronize";
                    c_textdatei.Append(origLogfile, "\r\nexecutionList.Synchronize\r\n");
                    lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    executionList.Synchronize();
                    c_textdatei.Append(origLogfile, "Duration: " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                    lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    // foundExecutionLists.Synchronize();
                    // executionList.Synchronize();
                    // Ist Synchronize
                    #endregion Synchronize

                    //* wian beginn
                    #region Logfiles managen
                    logCount = logCount + 1;
                    Logfile = Properties.Settings.Default.LogfilePath + Properties.Settings.Default.LogfileName + "_" + logCount + "_" + t_DateTime + ".log";
                    // c_textdatei
                    c_textdatei.Append(origLogfile, "\r\nSTART execute Executionlist " + logCount + "\r\n");
                    File.Copy(origLogfile, Logfile); // der Beginn des origLogfile soll an den Beginn des neuen Logfiles
                    // String ReportLogfile = ReportLogfilePath + Properties.Settings.Default.LogfileName + "_" +
                    // t_DateTime + "_" + computername;
                    ExecReportLogfile = ReportLogfile + "_" + logCount + "_" + executionList.Name + ".log";
                    copysettingsdestination = Properties.Settings.Default.LogfilePath + @"\" + t_DateTime + "_" + logCount + "_" + executionList.Name;
                    #endregion Logfiles managen

                    #region Mail
                    // Mailadresse ändern, wenn:
                    // 1. in Executionlist ein TestConfigurationParameters mit Name SendEMailTo enthalten ist
                    // 2. in Konfigurationsdatei LBASE_Smoketests.exe.config EMailAdresse Eintrag enthalten, also nicht leer:
                    //  String t_TestParams_SendEMailTo = "=>SUBPARTS[(TestConfigurationParameters=?\"SendEMailTo\")]";
                    // nicht SUBPARTS sondern SELF
                    String t_TestParams_SendEMailTo = "=>SELF[(TestConfigurationParameters=?\"SendEMailTo\")]";
                    // bisherige Mail danach wieder zurücksetzen
                    String o_ChangeMail = TestConfigurationHelper.GetTestConfigurationParameterValue(executionList, "SendEMailTo");
                    String ChangeMail = Properties.Settings.Default.EMailAdresse;
                    if (executionList.Search(t_TestParams_SendEMailTo).Count >= 1)
                    {
                        if ((Properties.Settings.Default.EMailAdresse) != "")
                        {


                            // Testconfiguration SendEMailTo auf Eintrag aus Konfigurationsdatei LBASE_Smoketests.exe.config EMailAdresse ändern:
                            c_textdatei.Append(Logfile, " SendEMailTo gefunden: " + executionList.Search(t_TestParams_SendEMailTo) +
                                "\r\n---------------------------------------------------\r\n");
                            // String Array allTestConfNames = TestConfigurationHelper.GetTestConfigurationParameterNames(executionList.Search(t_TestParams_SendEMailTo);
                            // public class TestConfigurationHelper 
                            TestConfigurationHelper.SetTestConfigurationParameterValue(executionList, "SendEMailTo", ChangeMail);

                            // jetzt ist TestConfigurationParameters überschrieben mit Properties.Settings.Default.EMailAdresse:
                            Console.WriteLine("TestConfigurationParameters überschrieben mit Properties.Settings.Default.EMailAdresse: " +
                                TestConfigurationHelper.GetTestConfigurationParameterValue(executionList, "SendEMailTo"));
                            c_textdatei.Append(Logfile, "TestConfigurationParameters überschrieben mit Properties.Settings.Default.EMailAdresse: " +
                                TestConfigurationHelper.GetTestConfigurationParameterValue(executionList, "SendEMailTo") + "\r\n");
                        }

                    }
                    #endregion Mail
                    // */ wian end
                    c_textdatei.Append(origLogfile, " Task: run " + executionList.Name +
                            "\r\n---------------------------------------------------\r\n");
                    c_textdatei.Append(Logfile, "BEGIN " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "\r\nTask: run " + executionList.Name +
                            "\r\n---------------------------------------------------\r\n");
                    #region foreach ExecutionEntry
                    foreach (ExecutionEntry entry in executionList.AllExecutionEntries)
                    {
                        /* // Teststeps auflisten:
                        Console.WriteLine("Teststeps in Testcase " + entry.Name + ": " + 
                                entry.Search("->AllReferences:TestCase=>SUBPARTS:TestStep[Disabled=i=\"false\"]"));
                        c_textdatei.Append(Logfile, "Teststeps in Testcase " + entry.Name + 
                                ":\r\n" + entry.Search("->AllReferences:TestCase=>SUBPARTS:TestStep[Disabled=i=\"false\"]") + "\r\n");
                        foreach (List() teststep in entry.Search("->AllReferences:TestCase=>SUBPARTS:TestStep[Disabled=i=\"false\"]"))
                        { 
                        }
                        */

                        // ab hier ausführen
                        returnValue = 0;
                        Console.Title = ConsoleTitle + " Task: run " + entry.Name;
                        c_textdatei.Append(Logfile, " Task: run " + entry.NodePath +
                            "\r\n---------------------------------------------------\r\n");

                        #region Run() nur aktive Ausführungslisten ausführen
                        if (entry.Disabled != true) // wian: keine deaktivierten ausfuehren!
                        {
                            if (entry.AnyParentDisabled != true)
                                entry.Run();
                            else
                            {
                                c_textdatei.Append(Logfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    "Do not Run: entry AnyParentDisabled!\r\n");
                            }
                        }
                        else
                        {
                            c_textdatei.Append(Logfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                "Do not Run: entry Disabled!\r\n");
                        }
                        #endregion Run() nur aktive Ausführungslisten ausführen

                        // aus entry.Name (Name des Testfalls) Leerzeichen und Umlaute bzw. ß
                        String TestCaseName_fln2utf8 = entry.Name.Replace(" ", "_").Replace("ü", "ue").Replace("Ü", "Ue").Replace("ö", "oe").Replace("Ö", "Oe").Replace("ä", "ae").Replace("Ä", "Ae").Replace("ß", "ss");

                        #region Resultate abfangen
                        switch (entry.ActualResult)
                        {
                            case ExecutionResult.Error:
                                Console.WriteLine("ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ": " +
                                    entry.NodePath + " !!! FAILED with ERROR: " + entry.ActualLog.Description);
                                Console.WriteLine("Detail: " + entry.ActualLog.Detail + " Description: " + entry.ActualLog.Description);
                                // GetFullScreenshot des aktuellen DeskTops
                                fss.GetFullScreenshot(ReportLogfilePath, TestCaseName_fln2utf8 + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + "_ERROR.jpg", ImageFormat.Jpeg);

                                c_textdatei.Append(Logfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    entry.NodePath + "\r\nRun not successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n" +
                                    "\r\n!!! FAILED with ERROR: " + entry.ActualLog.Description + "\r\n" + entry.ActualLog.Detail + "\r\n");
                                c_textdatei.Append(Logfile, "UniqueId=" + entry.UniqueId + "\r\n" +
                                    "Zusatzinformationen: Name = " + entry.Name + "\r\nDetail: " + entry.ActualLog.Detail + "\r\n"); // "\r\n TestCase: " + entry.TestCase + "\r\n");

                                c_textdatei.Append(origLogfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    entry.NodePath + "\r\nRun not successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n" +
                                    "\r\n!!! FAILED with ERROR: " + entry.ActualLog.Description + "\r\n" + entry.ActualLog.Detail + "\r\n");
                                c_textdatei.Append(origLogfile, "UniqueId=" + entry.UniqueId + "\r\n" +
                                    "Zusatzinformationen: Name = " + entry.Name + "\r\nDetail: " + entry.ActualLog.Detail + "\r\n");
                                returnValue = -1;
                                break;
                            case ExecutionResult.Failed:
                                Console.WriteLine("ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ": " + entry.NodePath + " !!! FAILED: " +
                                    entry.ActualLog.Description);
                                Console.WriteLine("Detail: " + entry.ActualLog.Detail);
                                // GetFullScreenshot des aktuellen DeskTops
                                fss.GetFullScreenshot(ReportLogfilePath, TestCaseName_fln2utf8 + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + "_FAILED.jpg", ImageFormat.Jpeg);

                                c_textdatei.Append(Logfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    entry.NodePath + "\r\nRun not successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n" +
                                    "\r\n!!! FAILED: " + entry.ActualLog.Description + "\r\n" + entry.ActualLog.Detail + "\r\n");
                                c_textdatei.Append(Logfile, "UniqueId=" + entry.UniqueId + "\r\n" +
                                    "Zusatzinformationen: Name = " + entry.Name + "\r\n"); // + "\r\n TestCase: " + entry.TestCase + "\r\n");
                                c_textdatei.Append(origLogfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    entry.NodePath + "\r\nRun not successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n" +
                                    "\r\n!!! FAILED: " + entry.ActualLog.Description + "\r\n" + entry.ActualLog.Detail + "\r\n");
                                c_textdatei.Append(origLogfile, "UniqueId=" + entry.UniqueId + "\r\n" +
                                    "Zusatzinformationen: Name = " + entry.Name + "\r\n");
                                returnValue = -1;
                                break;
                            case ExecutionResult.NoResult:
                                // Wann und warum tritt NoResult auf? Ergänzt wian am 21.09.2017
                                Console.WriteLine("ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ": " + entry.NodePath + " was run with NoResult " +
                                    (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                                // GetFullScreenshot des aktuellen DeskTops
                                fss.GetFullScreenshot(ReportLogfilePath, TestCaseName_fln2utf8 + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + "_NORESULT.jpg", ImageFormat.Jpeg);

                                c_textdatei.Append(Logfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    entry.NodePath + "\r\nWas run with NoResult " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                                break;
                            case ExecutionResult.Passed:
                                Console.WriteLine("ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ": " + entry.NodePath + " was run successfully " +
                                    (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                                c_textdatei.Append(Logfile, "ExecutionEntry " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ":\r\n" +
                                    entry.NodePath + "\r\nWas run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                                break;
                        }
                        if (returnValue != 0)
                            break;

                        #endregion Resultate abfangen
                    } // END foreach (ExecutionEntry entry in executionList.AllExecutionEntries)
                    #endregion foreach ExecutionEntry
                    // TestConfigurationHelper.SetTestConfigurationParameterValue(executionList, "SendEMailTo", o_ChangeMail);

                    if (isSplit == true)
                    {
                        if (Properties.Settings.Default.checkoutable == true)
                        {
                            Console.Title = ConsoleTitle + " Task: checkin split tree ExecutionList " + executionList.Name;
                            c_textdatei.Append(Logfile, " Task: checkin split tree ExecutionList " + executionList.Name + "\r\n");
                            executionList.CheckinTree("Executionlist with split " + executionList.Name + " in folder " + t_projectSearchSplitFolder + " executed at " +
                                DateTime.Now.ToString("dd.MM.yyyy at HH:mm") + " as user "
                                + Properties.Settings.Default.WorkspaceUser);
                            // executionList.CheckinTree("Executionlist wian test");
                        }
                    }
                    // */
                    #region nach der Ausführung Log aktualisieren
                    // Je Ausführungsliste eigene Logdatei: copy logfile to ReportLogfilePath
                    Console.Title = ConsoleTitle + "END execute Executionlist " + logCount;
                    c_textdatei.Append(origLogfile, "END execute Executionlist " + logCount + "\r\n");
                    c_textdatei.Append(Logfile, "END execute Executionlist " + logCount + "\r\n");

                    if (returnValue != 0)
                    {
                        //   break;
                        Console.Title = ConsoleTitle + "Execution ended with failure returncode " + returnValue;
                        c_textdatei.Append(origLogfile, "Execution ended with failure returncode " + returnValue + " at " +
                            DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nEND\r\n");
                        c_textdatei.Append(Logfile, "Execution ended with failure returncode " + returnValue + " at " +
                            DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nreturncode: " + returnValue + "\r\nEND\r\n");
                    }
                    else
                    {
                        Console.Title = ConsoleTitle + "Execution ended with failure returncode " + returnValue;
                        c_textdatei.Append(origLogfile, "Execution ended with returncode " + returnValue + " at " +
                            DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nEND\r\n");
                        c_textdatei.Append(Logfile, "Execution ended with returncode " + returnValue + " at " +
                            DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nreturncode: " + returnValue + "\r\nEND\r\n");
                    }

                    // Hier ein checkin der aktuellen Executionlist
                    if (Properties.Settings.Default.checkoutable == true)
                    {
                        Console.Title = ConsoleTitle + "CheckinAll Executionlist " + executionList.Name;
                        c_textdatei.Append(origLogfile, "CheckinAll Executionlist " + executionList.Name + "\r\n");
                        c_textdatei.Append(Logfile, "CheckinAll Executionlist " + executionList.Name + "\r\n");
                        #endregion nach der Ausführung Log aktualisieren

                        /* CheckinTree: 
                        executionList.CheckinTree("Executionlist " + executionList.Name + " in folder " + executionList.NodePath + " and UniqueId " + executionList.UniqueId + " executed at " +
                                DateTime.Now.ToString("dd.MM.yyyy at HH:mm") + " as user "
                                + Properties.Settings.Default.WorkspaceUser);
                         */
                        #region einchecken und speichern
                        c_textdatei.Append(origLogfile, "CheckInAll Kommentar: Executionlist " + executionList.Name + " in folder " + executionList.NodePath + " and UniqueId " + executionList.UniqueId + " executed at " +
                                DateTime.Now.ToString("dd.MM.yyyy at HH:mm") + " as user "
                                + Properties.Settings.Default.WorkspaceUser + "\r\n");
                        c_textdatei.Append(Logfile, "CheckInAll Kommentar: Executionlist " + executionList.Name + " in folder " + executionList.NodePath + " and UniqueId " + executionList.UniqueId + " executed at " +
                                DateTime.Now.ToString("dd.MM.yyyy at HH:mm") + " as user "
                                + Properties.Settings.Default.WorkspaceUser + "\r\n");
                        workspace.CheckInAll("Executionlist " + executionList.Name + " in folder " + executionList.NodePath + " and UniqueId " + executionList.UniqueId + " executed at " +
                                DateTime.Now.ToString("dd.MM.yyyy at HH:mm") + " as user "
                                + Properties.Settings.Default.WorkspaceUser);
                        Console.Title = ConsoleTitle + "CheckInAll run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.";
                        c_textdatei.Append(origLogfile, "CheckInAll run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                        c_textdatei.Append(Logfile, "CheckInAll run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                        lastTimeStamp = stopwatch.ElapsedMilliseconds;

                    }
                    workspace.Save();
                    Console.Title = ConsoleTitle + "workspace.Save() run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.";
                    c_textdatei.Append(origLogfile, "workspace.Save() run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                    c_textdatei.Append(Logfile, "workspace.Save() run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                    lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    // api.CloseWorkspace();
                    #endregion einchecken und speichern

                    #region Logdateien an Zielort speichern
                    Console.Title = ConsoleTitle + " Task: copy Logfile";
                    c_textdatei.Append(Logfile, " Task: copy Logfile\r\n---------------------------------------------------\r\n");
                    if (File.Exists(ExecReportLogfile))
                    {
                        File.Replace(Logfile, ExecReportLogfile, ExecReportLogfile + ".bkp");
                        Console.Title = ConsoleTitle + " File.Replace(Logfile, ExecReportLogfile, ExecReportLogfile + \".bkp\" run successfully ";
                        c_textdatei.Append(origLogfile, "File.Replace(Logfile, ExecReportLogfile, ExecReportLogfile + \".bkp\" run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                        c_textdatei.Append(Logfile, "File.Replace(Logfile, ExecReportLogfile, ExecReportLogfile + \".bkp\" run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                        lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    }
                    else
                    {
                        File.Copy(Logfile, ExecReportLogfile);
                        Console.Title = ConsoleTitle + " File.Copy(Logfile, ExecReportLogfile) run successfully";
                        c_textdatei.Append(origLogfile, "File.Copy(Logfile, ExecReportLogfile) run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                        c_textdatei.Append(Logfile, "File.Copy(Logfile, ExecReportLogfile) run successfully " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.\r\n");
                        lastTimeStamp = stopwatch.ElapsedMilliseconds;
                    }

                    #endregion Logdateien an Zielort speichern
                } // END foreach (ExecutionList executionList in foundExecutionLists)
                  // TODO Result abfragen

                // Checkin ausführen
                if (Properties.Settings.Default.checkoutable == true)
                {
                    #region Checkin, speichern und Close Workspace, sollte was noch offen sein, close Instance
                    Console.Title = ConsoleTitle + " Task: checkin all";
                    c_textdatei.Append(origLogfile, " Task: checkin all\r\n" + t_CheckinList + "\r\n---------------------------------------------------\r\n");
                    workspace.CheckInAll(t_CheckinList + " at " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " as user "
                        + Properties.Settings.Default.WorkspaceUser);
                    Console.WriteLine("Executionlists checkin all " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                    c_textdatei.Append(origLogfile, "Executionlists checkin all " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                    lastTimeStamp = stopwatch.ElapsedMilliseconds;
                }
                Console.Title = ConsoleTitle + " Task: workspace save";
                c_textdatei.Append(origLogfile, " Task: workspace save\r\n---------------------------------------------------\r\n");
                workspace.Save();
                Console.WriteLine("Save Workspace " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "Save Workspace " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                Console.Title = ConsoleTitle + " Task: close workspace";
                c_textdatei.Append(origLogfile, " Task: close workspace\r\n---------------------------------------------------\r\n");
                api.CloseWorkspace();
                Console.WriteLine("CloseWorkspace " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "CloseWorkspace " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                Console.Title = ConsoleTitle + " Task: close instance";
                c_textdatei.Append(origLogfile, " Task: close instance\r\n---------------------------------------------------\r\n");
                TCAPI.CloseInstance();
                Console.WriteLine("CloseInstance " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms.");
                c_textdatei.Append(origLogfile, "CloseInstance " + (stopwatch.ElapsedMilliseconds - lastTimeStamp) + " ms." + "\r\n");
                lastTimeStamp = stopwatch.ElapsedMilliseconds;
                #endregion Checkin, speichern und Close Workspace, sollte was noch offen sein, close Instance
                
                // Console.ReadKey();
                Console.Title = ConsoleTitle + " returncode: " + returnValue;
                c_textdatei.Append(origLogfile, " returncode: " + returnValue + "\r\n---------------------------------------------------\r\n");
                Console.WriteLine("Execution proper ended with returncode: " + returnValue);
                c_textdatei.Append(origLogfile, "Execution proper ended with returncode " + returnValue + " at " +
                    DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nEOF");
                // WrapUp before end: copy logfile to ReportLogfilePath
                Console.Title = ConsoleTitle + " Task: copy origLogfile";
                c_textdatei.Append(origLogfile, " Task: copy origLogfile\r\n---------------------------------------------------\r\n");
                if (File.Exists(ReportLogfile + ".log"))
                {
                    File.Replace(origLogfile, ReportLogfile + ".log", ReportLogfile + ".bkp");
                }
                else
                {
                    File.Copy(origLogfile, ReportLogfile + ".log");
                }
                return returnValue; // TODO return Wert anhand execution Status setzen                
            }
            catch (Exception ex)
            {
                #region Exception
                returnValue = -1;
                Console.Title = ConsoleTitle + " Task: Exception! Returncode: " + returnValue;
                Console.WriteLine("Execution ended with failure returncode: " + returnValue);
                c_textdatei.Append(origLogfile, "\r\n---------------------------------------------------\r\n");
                c_textdatei.Append(origLogfile, " Task: Exception!\r\n---------------------------------------------------\r\n");
                Console.WriteLine("Create Screenshot: " + Path.GetFileName(ReportLogfile).Replace(".log", "_") + 
                        DateTime.Now.ToString("ddMMyyyy_HHmmss") + "_EXCEPTION.jpg");
                c_textdatei.Append(origLogfile, "Create Screenshot: " + Path.GetFileName(ReportLogfile).Replace(".log", "_") + 
                        DateTime.Now.ToString("ddMMyyyy_HHmmss") + "_EXCEPTION.jpg\r\n");
                // GetFullScreenshot des aktuellen DeskTops
                // Schreibrechte vorausgesetzt!
                fss.GetFullScreenshot(ReportLogfilePath, Path.GetFileName(ReportLogfile).Replace(".log","_") + DateTime.Now.ToString("_yyyyMMdd_HHmmss") + "_EXCEPTION.jpg", ImageFormat.Jpeg);

                c_textdatei.Append(origLogfile, "\r\n---------------------------------------------------\r\n");
                c_textdatei.Append(origLogfile, " Task: Exception!\r\n---------------------------------------------------\r\n");
                c_textdatei.Append(origLogfile, "Execution ended with failure returncode " + returnValue + "\r\n");
                c_textdatei.Append(origLogfile, "Exception caught: " + ex.Message + "\r\nExecution ended at " +
                        DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\nEOF");

                if (Properties.Settings.Default.checkoutable == true)
                {
                    if (isCheckedOut)
                    {
                        c_textdatei.Append(origLogfile, "CheckInAll " + t_CheckinList + "\r\n");
                        workspace.CheckInAll("Exception caught: " + ex.Message + ", " + t_CheckinList + " at " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " as user "
                            + Properties.Settings.Default.WorkspaceUser);
                    }
                }
                if (workspace != null) workspace.Save();
                if (api.IsWorkspaceOpen != false) api.CloseWorkspace();
                if (TCAPI.Instance != null) TCAPI.CloseInstance();
                // return returnValue; // TODO return Wert anhand execution Status setzen

                Console.WriteLine("Exception caught: " + ex.Message);
                // Thread.Sleep(100000);
                // Console.ReadKey();

                // copysettingsdestination = Properties.Settings.Default.LogfilePath + @"\" + t_DateTime ;
                Console.WriteLine("Alle Settings-Dateien kopieren");
                // bei Exceptions alle Settings-Dateien in Liste l_savesettings sichern:
                // testc();
                // LogfileAppend(origLogfile,"Settings-Dateien kopieren nach:\r\n" + copysettingsdestination);
                c_textdatei.Append(origLogfile, "Settings-Dateien kopieren nach:\r\n" + copysettingsdestination + "\r\n");
                // SaveSettingsOfTCExecution(origLogfile,l_savesettings,copysettingsdestination);

                if (!Directory.Exists(Path.GetDirectoryName(copysettingsdestination + "\\")))
                {
                    c_textdatei.Append(origLogfile, "Verzeichnis anlegen\r\n");
                    Directory.CreateDirectory(copysettingsdestination);
                }
                for (int i = 0; i < l_savesettings.Count; ++i)
                {
                    if (File.Exists(l_savesettings[i]))
                    {
                        string f_name = l_savesettings[i].Substring(l_savesettings[i].LastIndexOf("\\") + 1);
                        c_textdatei.Append(origLogfile, "Datei: " + l_savesettings[i] + "\r\n");
                        File.Copy(l_savesettings[i], copysettingsdestination + @"\" + f_name);
                    }
                }

                // WrapUp before end: copy logfile to ReportLogfilePath
                Console.Title = ConsoleTitle + " Task: copy origLogfile";
                c_textdatei.Append(origLogfile, "\r\n---------------------------------------------------\r\n" +
                    " Task: copy origLogfile\r\n---------------------------------------------------\r\n");
                if (File.Exists(ReportLogfile + ".log"))
                {
                    File.Replace(origLogfile, ReportLogfile + "_EXCEPTION.log", ReportLogfile + "_EXCEPTION.bkp");
                }
                else
                {
                    File.Copy(origLogfile, ReportLogfile + "_EXCEPTION.log");
                }
                // Console.WriteLine("Exception after File.Copy: " + origLogfile + " to " + ReportLogfile + ".log");
                // c_textdatei.Append(origLogfile, "Exception after File.Copy: " + origLogfile + " to " + ReportLogfile + ".log" + "\r\n");
                // Console.ReadKey();

                return -1;
                #endregion Exception
            }


        }

        private static TCWorkspace OpenWSMeth(TCAPI api)
        {
            return api.OpenWorkspace(Properties.Settings.Default.WorkspacePath,
                                                                  Properties.Settings.Default.WorkspaceUser,
                                                                  Properties.Settings.Default.WorkspacePassword);
        }
    }
}
