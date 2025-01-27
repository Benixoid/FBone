using Inventory.Models;
using System.Collections.Generic;

namespace FBone.Service
{
    public static class GeneralHelper
    {
        public static List<ListModel> GetDefaultPageItems()
        {
            var listCount = new List<ListModel>
            {
                new ListModel(50),
                new ListModel(200),
                new ListModel(500),
                new ListModel(1000)
            };
            return listCount;
        }
    }
}
