using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Helpers
{
    public class StringHelpers
    {
        public static bool DoStringsMatch(String value, String pattern)
        {
            if (value == null || pattern == null)
                return false;
            pattern = pattern.Trim().ToLower();
            value=value.Trim().ToLower();
            if (pattern.Contains("*"))
            {
                String[] patterns = pattern.Split(new char[]{'*'}, StringSplitOptions.RemoveEmptyEntries);
                int currIndex = 0;
                foreach (String currPattern in patterns)
                {
                    int index = value.IndexOf(currPattern, currIndex);
                    if (index >= 0)
                        currIndex = index;
                    else
                        return false;
                }
                if (patterns.Length > 0 && patterns[patterns.Length - 1].EndsWith(TricentisLibs.Settings.Instance.ExactMatchDelimiter))
                {
                    String lastPattern = patterns[patterns.Length - 1];
                    lastPattern = lastPattern.Substring(0, lastPattern.Length);
                    return value.EndsWith(lastPattern);
                }
                return true;
            }
            else
            {
                if (pattern.EndsWith(TricentisLibs.Settings.Instance.ExactMatchDelimiter))
                {
                    pattern = pattern.Substring(0, pattern.Length - 1);
                    return pattern == value;
                }
                else
                {
                    return value.StartsWith(pattern);
                }
            }
        }

        public static bool StringContainsIndex(String str, out String strWithoutIndex, out int index)
        {
            str = str.Trim();
            if (str.Contains("[") && str.EndsWith("]"))
            {
                String[] items = str.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 2)
                {
                    strWithoutIndex = items[0];
                    String indexString = items[1].Replace("]", String.Empty);
                    if(int.TryParse(indexString, out index))
                    {
                        return true;
                    }
                }
            }
            strWithoutIndex = str;
            index = -1;
            return false;
        }

    }
}
