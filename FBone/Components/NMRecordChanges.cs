using DocumentFormat.OpenXml.Drawing;
using FBone.Database;
using FBone.Models.NMode;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Components
{
    public class NMRecordChanges : ViewComponent
    {
        private readonly DataManager DataManager;
        private readonly NMManager manager;
        public NMRecordChanges(DataManager dm)
        {
            DataManager = dm;
            manager = dm.NMManager;
        }

        public IViewComponentResult Invoke(int RecordId)
        {
            List<NModeChangeRecord> model = null;
            if (RecordId > -1)
            {
                var Record = manager.NModeRecords.FirstOrDefault(r => r.Id == RecordId);
                if (Record != null)
                {
                    model = Record.Changes;
                }
            }
            return View(model);
        }
    }
}
