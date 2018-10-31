using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using lbaseDotNetCustomControls.Helpers;
using lbaseDotNetCustomControls.Exceptions;
using TricentisLibs;

namespace lbaseDotNetCustomControls.Reflection
{
    public abstract class RibbonCollectionWrapper<T> : IEnumerable, IWrappedObject where T : IWrappedObjectWithText
    {
        object childCollection;


        public RibbonCollectionWrapper(object collection)
        {
            childCollection = collection;
        }

        public T this[String text]
        {
            get
            {
                T foundElement = FindElement(text);
                if (foundElement == null)
                { 
                    throw new ItemNotFoundException("Das Element mit dem Text >" + text + "< wurde nicht gefunden!");
                }
                else
                {
                    return foundElement;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                return WrapChildElement(ReflectionHelper.ReflectionHelper.GetProperty(childCollection, "Item", new object[] { index }));
            }
        }

        public T FindElement(String text)
        {
            String textWithoutIndex;
            int matchIndex;
            if (StringHelpers.StringContainsIndex(text, out textWithoutIndex, out matchIndex))
                return FindElement(textWithoutIndex, --matchIndex);  // -- because TOSCA is 1 based and C# is 0 based
            else return FindElement(text, 0);
        }

        private T FindElement(String text, int matchIndex)
        {
            int currMatchIndex = 0;
            foreach (Object currChild in this)
            {
                T currChildElementWrapper = WrapChildElement(currChild);
                if (currChild.GetType() == typeof(C1.Win.C1Ribbon.RibbonComboBox))
                {
                    if (((C1.Win.C1Ribbon.RibbonComboBox)currChild).Items.IndexOf(text) != -1)
                    {
                        RibbonComboBoxWrapper.Value = text;
                        if (matchIndex <= currMatchIndex)
                            return currChildElementWrapper;
                        else currMatchIndex++;
                    }
                }

                if (currChild.GetType() == typeof(C1.Win.C1Ribbon.RibbonControlHost))
                {
                    // if (((C1.Win.C1Ribbon.RibbonControlHost)currChild).Name == "") //.Items.IndexOf(text) != -1)
                    // {
                    //  RibbonComboBoxWrapper.Value = text;
                    // if (((C1.Win.C1Ribbon.RibbonControlHost)currChild).
                    if (matchIndex <= currMatchIndex)
                            return currChildElementWrapper;
                        else currMatchIndex++;
                    // }
                }

                if (currChild.GetType() == typeof(C1.Win.C1Ribbon.RibbonToggleButton))
                {
                    if (((C1.Win.C1Ribbon.RibbonToggleButton)currChild).Name.Contains("riBackstage"))
                    {
                        // wenn in [MENU]DATEI
                        // soll in weiterem Verlauf erledigt werden, nicht hier
                    }
                    else
                    { 
                        // RibbonToggleButton
                        var buttons = ((C1.Win.C1Ribbon.RibbonToggleButton)currChild).Group.Items;
                        int nButtons = buttons.Count;

                        // if (buttons.IndexOf(text) != -1)
                        while (currMatchIndex < nButtons)
                        {
                            if (typeof(C1.Win.C1Ribbon.RibbonToggleButton) == buttons.GetType())
                            {
                                if (StringHelpers.DoStringsMatch(((C1.Win.C1Ribbon.RibbonToggleButton)currChild).Text, text))
                                {
                                    if (matchIndex <= currMatchIndex)
                                        // currChildElementWrapper = WrapChildElement(buttons[currMatchIndex]);
                                        return currChildElementWrapper;
                                }
                                else
                                {
                                    currMatchIndex++;
                                }
                            }
                            else
                            {
                                currMatchIndex++;
                            }
                        }
                    }
                }

                // var RibbonTyp = currChild.GetType().Name;
                if (currChild.GetType() == typeof(C1.Win.C1Ribbon.RibbonGroup))
                {
                    // RibbonToggleGroup - z.B. OPTIONEN->Logikinterpreter->LI-Debug in Datei
                    var buttons = ((C1.Win.C1Ribbon.RibbonGroup)currChild).Items;
                    int nButtons = buttons.Count;

                    if (buttons.IndexOf(text) != -1)
                    {
                        while (currMatchIndex < nButtons)
                        {
                            // if (buttons[currMatchIndex].GetType().Name == "C1.Win.C1Ribbon.RibbonButton")
                            if (buttons[currMatchIndex].GetType().Name == "C1.Win.C1Ribbon.RibbonToggleButton")
                            {
                                if (StringHelpers.DoStringsMatch(((C1.Win.C1Ribbon.RibbonToggleButton)buttons[currMatchIndex]).Text, text))
                                {
                                    if (matchIndex <= currMatchIndex)
                                        currChildElementWrapper = WrapChildElement(buttons[currMatchIndex]);
                                    return currChildElementWrapper;
                                }
                            }
                            else if (buttons[currMatchIndex].GetType().Name == "C1.Win.C1Ribbon.RibbonButton")
                            {
                                if (StringHelpers.DoStringsMatch(((C1.Win.C1Ribbon.RibbonButton)buttons[currMatchIndex]).Text, text))
                                {
                                    if (matchIndex <= currMatchIndex)
                                        currChildElementWrapper = WrapChildElement(buttons[currMatchIndex]);
                                    return currChildElementWrapper;
                                }
                            }
                            else
                            {
                                currMatchIndex++;
                            }
                        }
                    }
                }
                if (currChild.GetType() == typeof(C1.Win.C1Ribbon.RibbonToggleGroup))
                {
                    // RibbonToggleGroup - z.B. OPTIONEN->Trace Level->Alles||Error
                    var buttons = ((C1.Win.C1Ribbon.RibbonToggleGroup)currChild).Items;                    
                    int nButtons = buttons.Count;

                    if (buttons.IndexOf(text) != -1)
                    {
                        while (currMatchIndex < nButtons)
                        {
                            if (StringHelpers.DoStringsMatch(((C1.Win.C1Ribbon.RibbonToggleButton)buttons[currMatchIndex]).Text, text))
                            {
                                if (matchIndex <= currMatchIndex)
                                    currChildElementWrapper = WrapChildElement(buttons[currMatchIndex]);
                                return currChildElementWrapper;
                            }
                            else
                            {
                                currMatchIndex++;
                            }
                        }
                    }
                }

                if (currChild.GetType() == typeof(C1.Win.C1Ribbon.RibbonLabel))
                {
                    // Ermittle, ob Label beschreibbar
                    bool label = ((C1.Win.C1Ribbon.RibbonLabel)currChild).Enabled;
                    // int nLabels = ((C1.Win.C1Ribbon.RibbonLabel)currChild);
                    if (((C1.Win.C1Ribbon.RibbonLabel)currChild).Text == "Adresse")
                    {
                        Logger.Instance.Log(this, "Das Element Adresse soll mit >" + text + "< befüllt werden!");

                        // throw new ItemNotFoundException("Das Element Adresse soll mit >" + text + "< befüllt werden!");
                    }
                }
                else
                {
                    if (StringHelpers.DoStringsMatch(currChildElementWrapper.Text, text))
                        if (matchIndex <= currMatchIndex)
                            return currChildElementWrapper;
                        else currMatchIndex++;
                }
            }
            return default(T);
        }

        public int Count { get { return (int)ReflectionHelper.ReflectionHelper.GetProperty(childCollection, "Count"); } }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)ReflectionHelper.ReflectionHelper.InvokeMethod(childCollection, "GetEnumerator");
        }

        public object WrappedObject { get { return childCollection; } }

        protected abstract T WrapChildElement(Object obj);
    }
}
