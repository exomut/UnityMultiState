using System;
using System.Collections.Generic;
using System.Linq;

namespace EXOMUT.MultiState
{
    public enum NoEnumStates { }
    
    public static class Enums
    {
        public static IEnumerable<T> FlagsToList<T>(Enum flags)
        {
            if (typeof(T).IsSubclassOf(typeof(Enum)) == false)
                throw new ArgumentException();

            return Enum.GetValues(typeof(T)).Cast<Enum>()
                .Where(m => flags.HasFlag(m)).Cast<T>();
        }

        public static IEnumerable<string> FlagsToStringList<T>(Enum flags)
        {
            var list = FlagsToList<T>(flags);

            return list.Select(item => item.ToString()).ToList();
        }

        public static T StringToEnum<T>(string value, bool ignoreCase = true)
        {
            return (T) Enum.Parse(typeof(T), value, ignoreCase);
        }
        
        public static T StringsToEnum<T>(string[] values, bool ignoreCase = true)
        {
            return StringToEnum<T>(string.Join(", ", values), ignoreCase);
        }
    }
}