using System;
using System.Collections.Generic;
using System.Text;

namespace ApartamentBotCore.WebModules
{
    class DictionaryKeyAttribute : Attribute
    {
        public DictionaryKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
