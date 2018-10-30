using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TricentisLibs;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using lbaseDotNetCustomControls.Reflection;
using lbaseDotNetCustomControls.Exceptions;
using lbaseDotNetCustomControls.Helpers;
using System.Threading;

namespace lbaseDotNetCustomControls
{

    public class C1Ribbon : OperatorGenericCtrl
    {

        // TODOLIST BEGIN
        // wian - 20130920: Alle vorhandenen Ribbons in Liste schreiben und prüfen, ob sie aktuell sind, übersetzt sind, vollständig sind 
        //                  (count, write in Datei, diff mit german.txt, american.txt).
        // 
        // TODOLIST END

        private Control ctrl;
        private C1RibbonWrapper lribbon;
        // Variablen für .listallribbons=
        private String lastRibbon = "Schließen";
        private String allRibbons = "";
        private String rTab       = "";
        private String rGroup     = "";
        private String rDelemPath = "->";
        private String rDelem     = ";";
        bool rAskEnabled = true;

        public bool isMenu = false;

        TextDatei c_textdatei = new TextDatei();

        String ModulPath = "\\\\tfdbnbld01\\LBase\\QA\\tosca\\wizardexport";
        String ModulfileName = "R_";
        public String ModulFile = "";

        public C1Ribbon(Control ctrl)
        {
            // Logdatei unter %appdata%\TRICENTIS\TOSCA TestSuite\7.0.0\logs\dotnetengine\lbaseDNCC_RibbonTrace.log
            Logger.Init(true, true, "lbaseDNCC_RibbonTrace.log", TricentisLibs.Settings.Instance.TracePath);
            Logger.Instance.Log(this, "Entered Constructor of C1Ribbon");
            this.ctrl = ctrl;
            lribbon = new C1RibbonWrapper(ctrl);
        }

        public override System.Windows.Forms.Control Ctrl
        {
            get { return ctrl; }
        }

        public override string setValue(string val, string steering)
        {
            #region IsOptional
            // wian 20130829
            // hier soll abgefangen werden, ob im String von val ".Optional=" enthalten ist
            // damit soll abgefragt werden, ob man schon in der *AUSWAHL ist oder nicht
            // wenn ja, müsste die Ribbon Auswahl nicht mehr betätigt werden,
            // ansonsten wird Ribbon Auswahl ausgeführt
            if (val.ToLower().StartsWith(".optional="))
            {
                lbaseGlobals.isOptional = true;
                Logger.Instance.Log(this, "Es wird optional abgefragt, ob das Ribbon aufgerufen werden muss: " + val);
                // hier fragen, ob in ÜBERSICHT
                IWrappedObjectWithText itemAuswahl = FindItem("*ÜBERSICHT");
                if (itemAuswahl == null) // ÜBERSICHT nicht gefunden, daher itemAuswahl auf null - wir sind womöglich schon in Auswahl
                {
                    lbaseGlobals.isOptional = false;
                    return ReturnOK;     // na dann, tschüss mit OK, also kein Fehler und keine weitere Verarbeitung
                }

                // wenn aber Übersicht gefunden, dann val ohne .Optional=
                if (itemAuswahl.Text.Substring(itemAuswahl.Text.Length - 9, 9) == "ÜBERSICHT")
                {
                    // wenn ja, Ribbon Aufruf durchführen
                    // val = val.Substring(10, (val.Length - 10));
                    val = val.ToLower().Remove(0, ((string)".optional=").Length);
                    lbaseGlobals.isOptional = false;
                    itemAuswahl = null;
                }
            /*
            else
            {
                // wenn nicht, Ribbon Aufruf abbrechen, da eh schon in AUSWAHL
                // ignore und raus aus dem Haus
            }
                 */
            lbaseGlobals.isOptional = false;
            }
            #endregion IsOptional

            #region IsEnabled
            // Ribbon, das aufgerufen werden soll auf enabled abfragen
            // .enabled=LBASE AUFTRAGSBEARBEITUNG
            if (val.ToLower().StartsWith(".enabled"))  // ab Null zählen beginnen
            {
                // val ohne .enabled
                val = val.ToLower().Remove(0, ((string)".enabled").Length);

                if (val.StartsWith("!="))
                {
                    lbaseGlobals.CheckEnabled = true;
                    rAskEnabled = false;
                    val = val.ToLower().Remove(0, ((string)"!=").Length);
                }

                if (val.StartsWith("="))
                {
                    lbaseGlobals.CheckEnabled = true;
                    rAskEnabled = true;
                    val = val.ToLower().Remove(0, ((string)"=").Length);
                }

            }
            else
            {
                lbaseGlobals.CheckEnabled = false;
            }
            #endregion IsEnabled

            #region ListAllRibbons
            // alle Ribbons der aktuellen Maske auflisten
            if (val.ToLower().StartsWith(".listallribbons="))  // Soll sich alle Ribbons der Maske merken
            {
                lbaseGlobals.isListallribbons = true;
                val = val.ToLower().Remove(0, ((string)".listallribbons=").Length);
                // Dateiname anlegen
                // ModulfileName = ModulfileName + "TEST - ÜBERSICHT->Test 1->Test - 2->Test->test mich fertig";
                ModulfileName = ModulfileName + val.ToUpper().Replace("->","-").Replace(" - ","_-_");
                // Datei anlegen
                ModulFile = ModulPath + "\\" + ModulfileName + ".txt";
                Logger.Instance.Log("Datei für listallribbons: " + ModulFile);
            }
            #endregion ListAllRibbons

            #region TryFindItem
            try
            {
                IWrappedObjectWithText item = FindItem(val);
                if (item is IClickableRibbonItem)
                {
                    #region IsEnabled
                    if (lbaseGlobals.CheckEnabled)  // wenn .enabled=, dann nur auf Item.Enabled prüfen und raus
                    {
                        // lbaseGlobals.CheckEnabled = ((RibbonItemWrapper)item).Enabled;
                        if (((RibbonItemWrapper)item).Enabled)
                        {
                            if (rAskEnabled)
                            {
                                Logger.Instance.Log("Das RibbonItem " + item.Text + " ist enabled.");
                                return ReturnOK;
                            }

                            if (!rAskEnabled) // ich frage, ob es disabled ist - wenn disabled: OK, wenn enabled, Fehler
                            {
                                // ich frage, ob es disabled ist - wenn enabled: FEHLER
                                Logger.Instance.Log("Das RibbonItem " + item.Text + " ist enabled, erwartet wurde disabled.");
                                // throw Exception
                                throw new NotEnabledException("Das RibbonItem >" + item.Text + "< ist enabled, erwartet wurde disabled.");
                            }
                        }
                        else
                        {
                            if (!rAskEnabled) // ich frage, ob es disabled ist - wenn disabled: OK
                            {
                                Logger.Instance.Log("OK: Das RibbonItem " + item.Text + " ist nicht enabled.");
                                return ReturnOK;
                            }

                            if (rAskEnabled)
                            {
                                Logger.Instance.Log("Das RibbonItem " + item.Text + " ist disabled!");
                                throw new NotEnabledException("Das RibbonItem >" + item.Text + "< ist disabled!");
                            }
                        }
                        // ReflectionHelper.ReflectionHelper.SetField(                        
                    }
                    #endregion IsEnabled

                    (item as IClickableRibbonItem).Click();
                    Logger.CloseLogger();

                }
                return ReturnOK;
            }
            #endregion TryFindItem
            catch (NotSupportedException nse)
            {
                return nse.Message;
            }
            catch (NotEnabledException nee)
            {
                return nee.Message;
            }
            catch (ItemNotFoundException infe)
            {
                return infe.Message;
            }
            catch (ListallRibbonsException lare)
            {
                return lare.Message;
            }
        }

        /*
        private IWrappedObjectWithText RibbonCount(string val)
        {
            RibbonTabWrapper tab = lribbon.SelectedTab;
            // tabcount = tab.;
            
            RibbonGroupWrapper group = tab.Groups[2];
            throw new NotImplementedException("The ribbon value does not exist.");
        }
        */
        #region GetProperty
        public override string getProperty(string name, string key)
        {
            switch (name.ToLower())
            {
                case "text":
                case "value":
                    RibbonTabWrapper selectedTab = lribbon.SelectedTab;
                    if (selectedTab == null)
                        return String.Empty;
                    return selectedTab.Text;
                case "enabled":
                    if (String.IsNullOrEmpty(key))
                        return base.getProperty(name, key);
                    else
                    {
                        IWrappedObjectWithText item = FindItem(key);
                        if (item is RibbonItemWrapper)
                        {
                            return (item as RibbonItemWrapper).Enabled.ToString();
                        }
                        throw new NotSupportedException("The given item has no Enabled property.");
                    }
                case "enabledandclick":
                    if (String.IsNullOrEmpty(key))
                        return base.getProperty(name, key);
                    else
                    {
                        IWrappedObjectWithText item = FindItem(key);
                        if (item is RibbonItemWrapper)
                        {
                            bool enabled = (item as RibbonItemWrapper).Enabled;
                            if (enabled && item is IClickableRibbonItem)
                            {
                                (item as IClickableRibbonItem).Click();
                            }
                            return enabled.ToString();
                        }
                        throw new NotSupportedException("The given item has no Enabled property.");
                    }

            }
            return base.getProperty(name, key);
        }
        #endregion GetProperty

        #region FindItem
        private IWrappedObjectWithText FindItem(String itemString)
        {
            // bool isMenu = false;
            if (itemString.ToLower().TrimStart().StartsWith("[menu]"))
            {
                isMenu = true;
                itemString = itemString.Substring(itemString.IndexOf("]") + 1); // Remove the "[MENU]" part from the path 
            }
            Regex regex = new Regex("->");
            List<String> items = new List<string>(regex.Split(itemString));
            if (isMenu)
                return FindItemInMenu(items);
            else
                return FindItemInRibbon(items);
        }
        #endregion FindItem

        #region FindItemMenu
        private IClickableRibbonItem FindItemInMenu(List<string> items)
        {
            if (items.Count < 2)
                throw new NotSupportedException("Bitte geben Sie zumindest zwei Menüebenen ein!");
            RibbonApplicationMenuWrapper applicationMenu = lribbon.ApplicationMenu;
            if (!StringHelpers.DoStringsMatch(applicationMenu.Text, items[0]))
                throw new ItemNotFoundException("Das Menü >" + items[0] + "< wurde nicht gefunden!");
            items = items.Skip(1).ToList();
            RibbonDropDownBaseWrapper currBaseWrapper = applicationMenu;
            return FindItemInSubMenu(currBaseWrapper, items);
        }
        #endregion FindItemMenu

        #region FindItemInRibbon
        private IWrappedObjectWithText FindItemInRibbon(List<string> items)
        {
            String tabText = items[0];
            RibbonTabWrapper tab;
            tab = lribbon.Tabs.FindElement(tabText);  // We search the tab in the Tabs of the ribbon

            if (tab == null)
            {
                if (lribbon.SelectedTab.Text.ToLower().Equals(tabText))
                {
                    tab = lribbon.SelectedTab;
                }
                /* else
                {
                    // if(!lribbon.SelectedTab.Text.ToLower().Equals(tabText))
                }
                */
            }

            #region Listallribbons
            if (lbaseGlobals.isListallribbons && (tab != null))
            {
                // jetzt alle Ribbons merken, die im RibbonTab vorhanden sind
                Logger.Instance.Log(this, "Ribbons, die im TAB " + this + "vorhanden sind");
                rTab = tab.Text;
                // allRibbons = allRibbons + tab.Text + rDelemPath;
                // return null;
            }
            #region TabGroup.FindElement
            if (tab == null) // The Tab was not found in the 'normal' tabs, we have to search the contextual Tabs
            {
                for (int currentGroupIndex = 0; currentGroupIndex < lribbon.ContextualTabGroups.Count; currentGroupIndex++)
                {
                    RibbonContextualTabGroupWrapper tabGroup = lribbon.ContextualTabGroups[currentGroupIndex];
                    tab = tabGroup.Tabs.FindElement(tabText);
                    if (lbaseGlobals.isListallribbons && (tab != null))
                        rTab = tab.Text;
                    // allRibbons = allRibbons + tab.Text + rDelemPath;

                    if (tab != null)
                        break;
                }
            }

            if (tab == null)  // The Tab was still not found
            {
                if (lbaseGlobals.isOptional == true)
                {
                    // throw new OptionalException("Ribbon Menü ignoriert, da optional.");
                    Logger.Instance.Log(this, "Optional Ribbon: " + this);
                    // return "Ribbon Menü ignoriert, da optional.";
                    // return ReturnOK;
                    // Exception( );
                    // throw new ItemNotFoundException("Der Tab >" + tabText + "< wurde nicht gefunden!");
                    return null;
                }
                else
                {
                    // Ribbon->RibbonBar->Der Tab >GENERISCHE TABELLEN-DATEN - ÜBERSICHT< wurde nicht gefunden!
                    throw new ItemNotFoundException("Der Tab >" + tabText + "< wurde nicht gefunden!");
                }
            }
            #endregion TabGroup.FindElement

            lribbon.SelectedTab = tab;

            #region isListallribbons
            if (lbaseGlobals.isListallribbons)
            {
                int countGroups = tab.Groups.Count;
                for (int nGroup = 0; nGroup < countGroups; nGroup++)
                {
                    RibbonGroupWrapper tGroup = tab.Groups[nGroup];
                    rGroup = tGroup.Text;
                    // allRibbons = allRibbons + rGroup + rDelemPath; // Gruppe ergänzen
                    // Alle Items unter der Gruppe ergänzen
                    int countItems = tGroup.Items.Count;
                    for (int nItem = 0; nItem < countItems; nItem++)
                    {
                        // allItemsInGroup = tGroup.Items // GetType().Name; // tab.Groups[nGroup].Items;
                        allRibbons = allRibbons + rTab + rDelemPath + rGroup + rDelemPath +
                            tGroup.Items[nItem].Text + rDelem; // Item ergänzen und Delemiter
                        if (tGroup.Items[nItem].Text == lastRibbon) // || (nGroup == countGroups  && nItem == countItems))
                        {
                            lbaseGlobals.isListallribbons = false;
                            // hier noch allRibbons vor dem Ausstieg in eine Datei schreiben
                            c_textdatei.WriteFile(ModulFile, "Alle Ribbons des TABS " + rTab + ":\r\n" + allRibbons + "\r\n");
                            /*
                            // OpenFileDialog 
                            System.IO.StreamReader sr = new
                                System.IO.StreamReader(ModulFile);

                            MessageBox.Show(sr.ReadToEnd());
                            sr.Close();
                            */

                            MessageBox.Show("Angelegt in der Datei: \r\n" + ModulFile + "\r\n\r\n" + allRibbons, "Alle Ribbons zu TAB " + rTab);
                            // ModulFile
                            allRibbons = "";
                            return null;
                            // throw new ListallRibbonsException("Alle Ribbons des TABS: + \r\n" + allRibbons);                            // return null;
                            // hier in Datei schreiben

                        }
                    }
                }
                lbaseGlobals.isListallribbons = false;
                // hier noch allRibbons vor dem Ausstieg in eine Datei schreiben
                c_textdatei.WriteFile(ModulFile, "Alle Ribbons des TABS " + rTab + ":\r\n" + allRibbons + "\r\n");
                MessageBox.Show("Angelegt in der Datei: \r\n" + ModulFile + "\r\n\r\n" + allRibbons, "Alle Ribbons zu TAB " + rTab);
                // ModulFile
                allRibbons = "";
                return null;
            }
            #endregion isListallribbons

            items = items.Skip(1).ToList();
            if (items.Count == 0 && (lbaseGlobals.isListallribbons == false))  // no more elements in the list, only the tab change is wanted
                return tab;
            #endregion Listallribbons

            String groupText = items[0];
            RibbonGroupWrapper group = tab.Groups[groupText];
            items = items.Skip(1).ToList();
            if (items.Count == 0)  //no more elements in the list, only a verification that the group exists was done
                return group;
            if (!group.Enabled)
                throw new NotEnabledException("Die Gruppe >" + groupText + "< ist nicht enabled.");
            RibbonItemWrapper foundItem = group.Items[items[0]];
            items = items.Skip(1).ToList();
            Logger.Instance.Log("foundItem, items: " + foundItem + ", " + items);
            return FindItemInSubMenu(foundItem, items);
        }
        #endregion FindItemInRibbon

        private void Exception()
        {
            Logger.Instance.Log("Weiss nicht, was los ist. Bin in C1Ribbon.cs: Ribbon Menü ignoriert, da optional." +
                " Daher möchte ich ohne Fehler raus!");
            Logger.CloseLogger();
            throw new NotImplementedException("Weiss nicht, was los ist. Bin in C1Ribbon.cs: Ribbon Menü ignoriert, da optional." +
                " Daher möchte ich ohne Fehler raus!");
        }

        private IClickableRibbonItem FindItemInSubMenu(RibbonItemWrapper startWrapper, List<String> items)
        {
            RibbonItemWrapper currWrapper = startWrapper;
            while (items.Count > 0)  // We still have elements to find
            {
                if (currWrapper is RibbonDropDownBaseWrapper)
                {
                    // Z.B. [MENU]DATEI->Beenden
                    RibbonDropDownBaseWrapper currDropDownWrapper = currWrapper as RibbonDropDownBaseWrapper;
                    currDropDownWrapper.DroppedDown = true;
                    String currItemToFind = items[0];
                    items = items.Skip(1).ToList();
                    // Thread.Sleep(2000);
                    currWrapper = currDropDownWrapper.Items[currItemToFind];
                }
                else throw new NotSupportedException("Das Item >" + currWrapper.Text + "< ist kein gültiges DropDownItem (RibbonMenu oder SplitButton)!");
            }  // We're on the last item. We'll click it.

            if (currWrapper is IClickableRibbonItem)
            {
                return currWrapper as IClickableRibbonItem;
            }
            else throw new NotSupportedException("Das Item >" + currWrapper.Text + "< ist kein gültiger Button!");
        }

        public override string RobotClass
        {
            get { return RobotClassControl; }
        }
    }
}
