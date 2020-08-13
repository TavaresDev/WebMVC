using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropetyValue("Name"),
                       Value = item.GetPropetyValue("Id"),
                       Selected = item.GetPropetyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}
