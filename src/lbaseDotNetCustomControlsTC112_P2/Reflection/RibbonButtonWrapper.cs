using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TricentisLibs;
using System.Threading;
using lbaseDotNetCustomControls.Exceptions;
using System.Windows.Forms;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonButtonWrapper : RibbonItemWrapper, IClickableRibbonItem
    {
        public RibbonButtonWrapper(object item)
            : base(item)
        {

        }

		public void Click()
		{            
			Thread threadCockpit = new Thread(ClickInternal);
			threadCockpit.Start();
		}

		private void ClickInternal()
		{
			// Thread.Sleep(2000);

            ((this.item as C1.Win.C1Ribbon.RibbonButton).OwnerControl as Control).Invoke((MethodInvoker)delegate { ClickInternalInvoke(); });
            // ((item as C1.Win.C1Ribbon.RibbonButton).OwnerControl as Control).Invoke((MethodInvoker)delegate { ClickInternalInvoke(); });
        }

        private void ClickInternalInvoke()
        {		
            // Ist es PPJ 46?
            var isPpjV46 = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "PPJ.Runtime.46");

            if (isPpjV46)
            {
                // Es ist PPJ 46:
                if (!Enabled)
                    throw new NotEnabledException("Der Button >" + Text + "< ist nicht enabled!");

                try
                {
                    // var isMenu = (this.item as C1.Win.C1Ribbon.RibbonButton).OwnerControl;
                    // Thread.Sleep(300);
                    
                    // in german.txt wird fuer DATEI-Beenden nur Ribbon.riBackstagBeenden@Beenden verwendet
                    if (this.Name.ToLower().Contains("ribackstag"))
                    {
                        switch (this.Name.ToLower())
                        {
                            case "ribbon.ribackstagbeenden":
                            case "ribbon.ribackstagedrucken":
                            case "ribbon.ribackstagespeichernunter":
                            case "ribbon.ribackstageprogramminfo":
                            case "ribbon.ribackstagesendenininternetbrowser":
                            case "ribbon.ribackstageberechtigungsinformation":
                                ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j"); // a OnClick is invoked with the "j" Method ;-) wian
                                                                                           // nur wenn [MENU]DATEI->Beenden verwendet wurde (oder siehe obige case), zwei mal "j"!
                                ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j");
                                break;
                            default:
                                ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j");
                                break;
                        }
                    }

                    else
                    {
                        ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j");
                    }
                        
                }
                catch (Exception)
                {

                    throw;
                }                
            }
            else
            {
                // Es ist nicht PPJ 46
                // Ist es PPJ 45?
                var isPpjV45 = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "PPJ.Runtime.45");
                if (!isPpjV45)
                {
                    // Es ist nicht PPJ 45 (aeltere lbase Version):
                    if (!Enabled)
                        throw new NotEnabledException("Der Button >" + Text + "< ist nicht enabled!");
                    Logger.Instance.Log("Der Button >" + Text + "< wird geklickt.");

                    Thread.Sleep(300);
                    try
                    {
                        ReflectionHelper.ReflectionHelper.InvokeMethod(item, "k"); // a OnDoubleClick is invoked with the "k" Method ;-)
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Logger.Instance.Log(ioe.ToString());
                    }
                    // Thread.Sleep(300);
                    /*
                    * 
                     // 20151022 wian:
                     // Vor dem Wechsel ppj framework wird PPJ.Runtime.4.dll verwendet
                     * Mit folgendem Befehl während Debuggen im Direktfenster die Methoden von item ermitteln:
                    ReflectionHelper.ReflectionHelper.GetMethodNames(item)
                    ReflectionHelper.ReflectionHelper.GetMethodNames(item).Skip(100).ToArray()
                     * ...
                     * [31]: "OnClick"
                     * [32]: "k"
                     * [33]: "add_DoubleClick"
                     * [34]: "remove_DoubleClick"
                     * [35]: "OnDoubleClick"
                     * [36]: "h"
                     * ...
                     */
                }
                else
                {
                    // Es ist PPJ 45:
                    if (!Enabled)
                        throw new NotEnabledException("Der Button >" + Text + "< ist nicht enabled!");

                    // Logger.Instance.Log("Der Button >" + Text + "< wird geklickt.");

                    // 20151022 wian:
                    // Diese Änderung ist seit dem Wechsel von ppj framework auf PPJ.Runtime.45.dll mit Dateiversion 4.5.1056.646 ab lbase 6.4 nötig
                    // Thread.Sleep(600);
                    // ReflectionHelper.ReflectionHelper.GetMethodNames(item);
                    // ReflectionHelper.ReflectionHelper.GetMethodNames(item).Skip(100).ToArray()

                    Thread.Sleep(300);
                    ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j"); // a OnClick is invoked with the "j" Method ;-) wian
                    // Thread.Sleep(300);
                    /*
                     * Mit folgendem Befehl während Debuggen im Direktfenster die Methoden von item ermitteln:
                     ReflectionHelper.ReflectionHelper.GetMethodNames(item)
                     ReflectionHelper.ReflectionHelper.GetMethodNames(item).Skip(100).ToArray()
                     * ...
                     * [33]: "OnClick"
                     * [34]: "j"
                     * [35]: "add_DoubleClick"
                     * [36]: "remove_DoubleClick"
                     * [37]: "OnDoubleClick"
                     * [38]: "k"
                     * ...
                    */
                }
            }

        }


        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }

        public String Name
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Name"); }
        }
    }
}
