using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApartamentBotCore.WebModules
{
    static class Utils
    {
        static public string GetNumbers(string src)
        {
            if (string.IsNullOrEmpty(src))
                return null;

            return new string(src.Where(c => char.IsDigit(c) || (c == ',' || c == '.')).ToArray());
        }
    }
}
