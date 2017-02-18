using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DAL.Enums;

namespace Infrastructure.Helpers
{
    public static class Utilities
    {
        public static IEnumerable<Message<MessageType, string>> GetModelStateErrors(ModelStateDictionary modelState)
        {
            var result = new List<Message<MessageType, string>>();
            foreach (var state in modelState.Values)
            {
                foreach (var modelError in state.Errors)
                {
                    result.AddMessage(MessageType.Error, modelError.ErrorMessage);
                }
            }

            return result;
        }

        public static string GetModelStateErrorsAsString(this ModelStateDictionary modelState)
        {
            return modelState.Values.Aggregate(string.Empty,
                (current, state) => current + string.Join(Environment.NewLine, state.Errors.Select(c => c.ErrorMessage).ToList()));
        }


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

        public static bool HasImageExtension(string source)
        {
            return (source.EndsWith(".png") || source.EndsWith(".jpg") ||
                source.EndsWith(".jpeg") || source.EndsWith(".tif") || source.EndsWith(".bmp"));
        }
    }
}