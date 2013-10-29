using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TestLibrary
{
    class Test
    {
        public static T GetInstanceVariable<T>(object source, params string[] gets)
        {
            object parent = source;
            FieldInfo info;
            foreach (string g in gets)
            {
                info = parent.GetType().GetField(g, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (info != null) parent = info.GetValue(parent);
                else throw new TargetException("No such field for "+ parent.ToString() +": " + g);
            }
            return (T)parent;
        }
        public static T GetInstanceProperty<T>(object source, params string[] gets)
        {
            object parent = source;
            FieldInfo info;
            string g;
            for (int i = 0; i < gets.Length - 1; i++)
            {
                g = gets[i];
                info = parent.GetType().GetField(g, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                parent = info.GetValue(source);
            }
            g = gets[gets.Length - 1];
            MethodInfo minfo = parent.GetType().GetProperty(g, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetGetMethod();
            if (minfo == null)
                ThrowValidMethods(parent);
            return (T)minfo.Invoke(parent, new object[]{});
        }
        public static object CallInstanceMethod(object source, object[] parameters, params string[] gets)
        {
            object parent = source;
            FieldInfo info;
            string g;
            for(int i = 0; i < gets.Length - 1; i++)
            {
                g = gets[i];
                info = parent.GetType().GetField(g, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                parent = info.GetValue(source);
            }
            g = gets[gets.Length - 1];
            MethodInfo minfo = parent.GetType().GetMethod(g, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (minfo == null)
                ThrowValidMethods(parent);
            return minfo.Invoke(parent, parameters);
        }
        public static void ThrowValidMethods(object source)
        {
            string s = "Valid Methods For " + source + ": ";
            MethodInfo[] minfos = source.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo info in minfos)
            {
                s += info.Name + ", ";
            }
            throw new ApplicationException(s);
        }
    }
}
