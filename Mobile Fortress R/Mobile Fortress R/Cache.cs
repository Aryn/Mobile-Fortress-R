using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile_Fortress_R
{
    class Cache
    {
        static Dictionary<string, object> directory = new Dictionary<string, object>();
        public static T Get<T>(string filename)
        {
            object data = null;
            if (!MobileFortress.TestingMode && !directory.TryGetValue(filename, out data))
            {
                data = MobileFortress.Content.Load<T>(filename);
                directory.Add(filename, data);
            }
            return (T)data;
        }
    }
}
