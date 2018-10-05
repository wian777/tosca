using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LBASE_Smoketests
{
    public class TextDatei
    {
        ///<summary>
        /// Schreibt den übergebenen Inhalt in eine Textdatei.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="sLines">zu schreibender Text</param>
        public void WriteFile(String sFilename, String sLines)
        {
            StreamWriter myFile = new StreamWriter(sFilename);
            myFile.Write(sLines);
            myFile.Close();
        }

        ///<summary>
        /// Fügt den übergebenen Text an das Ende einer Textdatei an.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="sLines">anzufügender Text</param>
        public void Append(string sFilename, string sLines)
        {
            StreamWriter myFile = new StreamWriter(sFilename, true);
            myFile.Write("\r\n" + sLines);
            myFile.Close();
        }
    }
}
