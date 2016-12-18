using Infrastructure.Helpers;
using System;
using System.Collections.Generic;

namespace Infrastructure.Helpers
{
    public class Utilities
    {
        private static IEnumerable<SelectizeItem<TKeyValue>> ToSelectizeItemsList<T, TKeyValue>() where T : struct, IConvertible
        {
            if (typeof(T).IsEnum)
            {
                var values = Enum.GetValues(typeof(T));
                var result = new List<SelectizeItem<TKeyValue>>();
                foreach (var value in values)
                {
                    result.Add(new SelectizeItem<TKeyValue>(EnumHelpers<T>.GetDisplayValue((T)value), (TKeyValue)value));
                }
                return result;
            }
            throw new Exception("Can not convert to Selectize items list because the given type is not an ENUM");
        }

        public static IEnumerable<SelectizeItem<int>> ToSelectizeItemsList<T>() where T : struct, IConvertible
        {
            return ToSelectizeItemsList<T, int>();
        }
    }
}