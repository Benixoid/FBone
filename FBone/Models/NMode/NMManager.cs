using DocumentFormat.OpenXml.ExtendedProperties;
using FastReport.Utils;
using FBone.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Models.NMode
{
    public class NMManager
    {
        private readonly AppDBContext _context;
        private readonly NModeSettings _settings;

        public NMManager(AppDBContext context, IOptions<NModeSettings> settingsOptions)
        {
            _context = context;
            _settings = settingsOptions.Value;
        }

        public NModeSettings Settings { get { return _settings; } }

        private NModeRecords Records = null;
        public NModeRecords NModeRecords
        {
            get
            {
                if (Records == null)
                {
                    //var trecs = TotalRecords;
                    if (Records == null)
                    {
                        Records = new NModeRecords(_context.NModeRecords.Include(r => r.Conditions).Include(r => r.Area).Include(r => r.Lcn).Include(r => r.Changes.OrderByDescending(chng => chng.Date)));
                        Records.ForEach(rec => rec.Settings = _settings);
                    }
                }
                return Records;
            }
            //protected set { Records = value; }
        }

        public NModeResult GetRecordResult(int RecordId, DateTime dt)
        {
            dt = _settings.ReportTimeStamp(dt);
            NModeResult result = _context.NModeResults.Where(rr => rr.RecordId == RecordId && rr.TimeStamp == dt).OrderByDescending(rr => rr.TimeStamp).Take(1).FirstOrDefault();
            return result;
        }

        public NModeRecords GetNModeRecords(DateTime dt)
        {
            dt = _settings.ReportTimeStamp(dt);
            var recs = new NModeRecords(_context.NModeRecords.Include(r => r.Conditions)
                .Include(r => r.Area).Include(r => r.Lcn).Include(r => r.Changes.OrderByDescending(chng => chng.Date)).Include(r => r.Results.Where(rr => rr.TimeStamp == dt).OrderByDescending(rr => rr.TimeStamp).Take(1)));

            recs.ForEach(rec => rec.Settings = _settings);

            return recs;
        }
        public NModeRecord GetNModeRecord(DateTime dt, int? Id = null)
        {
            dt = _settings.ReportTimeStamp(dt);
            var recs = new NModeRecords(_context.NModeRecords.Where(r => r.Id == Id).Include(r => r.Conditions)
              .Include(r => r.Area).Include(r => r.Lcn).Include(r => r.Changes.OrderByDescending(chng => chng.Date)).Include(r => r.Results.Where(rr => rr.TimeStamp == dt).OrderByDescending(rr => rr.TimeStamp).Take(1)));

            recs.ForEach(rec => rec.Settings = _settings);

            return recs.FirstOrDefault();
        }

        public NMTotalRecords GetNMTotalRecords(DateTime dt)
        {
            dt = _settings.ReportTimeStamp(dt);
            var trecs = new NMTotalRecords(_context.NMTotalRecords.Include(tr => tr.Area).Include(tr => tr.Lcn).Include(tr => tr.SubTotals).ToList());
            trecs.ForEach(trec =>
            {
                trec.NMRecords = new NModeRecords(new NModeRecords(_context.NModeRecords.Where(rec => rec.AreaId == trec.Area.Id && rec.LcnId == trec.Lcn.Id).Include(r => r.Conditions)
                .Include(r => r.Area).Include(r => r.Lcn).Include(r => r.Changes.OrderByDescending(chng => chng.Date)).Include(r => r.Results.Where(rr => rr.TimeStamp == dt).OrderByDescending(rr => rr.TimeStamp).Take(1))));
                //trec.NMRecords.ForEach(rec => rec.Settings = _settings);
                trec.Settings = _settings;
            });
            return trecs;
        }
        public NMTotalRecord GetNMTotalRecord(DateTime dt, int Id)
        {
            dt = _settings.ReportTimeStamp(dt);
            var trecs = new NMTotalRecords(_context.NMTotalRecords.Where(tr => tr.Id == Id).Include(tr => tr.Area).Include(tr => tr.Lcn).Include(tr => tr.SubTotals).ToList());
            //trecs = new NMTotalRecords(_context.NMTotalRecords.Include(tr => tr.Area).Include(tr => tr.Lcn).ToList());
            trecs.ForEach(trec =>
            {
                trec.NMRecords = new NModeRecords(new NModeRecords(_context.NModeRecords.Where(rec => rec.AreaId == trec.Area.Id && rec.LcnId == trec.Lcn.Id).Include(r => r.Conditions)
                .Include(r => r.Area).Include(r => r.Lcn).Include(r => r.Changes.OrderByDescending(chng => chng.Date)).Include(r => r.Results.Where(rr => rr.TimeStamp == dt).OrderByDescending(rr => rr.TimeStamp).Take(1))));
                trec.NMRecords.ForEach(rec => rec.Settings = _settings);
                trec.Settings = _settings;
            });

            return trecs.FirstOrDefault();
        }

        private NModeConditions Conditions = null;
        public NModeConditions NModeConditions
        {
            get
            {
                if (Conditions == null)
                {
                    Conditions = new NModeConditions(_context.NModeConditions.Include(n => n.NModeRecord));
                }
                return Conditions;
            }
            //protected set { Records = value; }
        }

        private NMTotalRecords TRecords = null;
        public NMTotalRecords TotalRecords
        {
            get
            {
                if (TRecords == null)
                {
                    TRecords = new NMTotalRecords(_context.NMTotalRecords.Include(tr => tr.Area).Include(tr => tr.Lcn).ToList());
                    TRecords.ForEach(trec =>
                    {
                        Records = new NModeRecords(_context.NModeRecords.Include(r => r.Conditions).Include(r => r.Area).Include(r => r.Lcn).Include(r => r.Changes.OrderByDescending(chng => chng.Date)));

                        trec.NMRecords = new NModeRecords(Records.Where(rec => rec.AreaId == trec.Area.Id && rec.LcnId == trec.Lcn.Id));
                        trec.NMRecords.ForEach(r => r.ParentId = trec.Id);
                        trec.Settings = _settings;
                    });
                }
                return TRecords;
            }
        }
        private List<Area> _Areas = null;
        public List<Area> Areas
        {
            get
            {
                if (_Areas == null)
                {
                    _Areas = _context.Areas.ToList();
                    _Areas.Insert(0, new Area() { Id = 0, Name = "All" });
                    _Areas.Sort();
                }
                return _Areas;
            }
        }

        public List<Area> AreasWithDependencies()
        {
            return _context.Areas.Include(a => a.NModeRecords).Include(a => a.NMTotalRecords).ToList();
        }

        private List<Lcn> _Lcns = null;
        public List<Lcn> Lcns
        {
            get
            {
                if (_Lcns == null)
                {
                    _Lcns = _context.Lcns.ToList();
                    _Lcns.Insert(0, new Lcn() { Id = 0, Name = "All" });
                    _Lcns.Sort();
                }
                return _Lcns;
            }
        }
        public List<Lcn> LcnsWithDependencies()
        {
            return _context.Lcns.Include(l => l.NModeRecords).Include(l => l.NMTotalRecords).ToList();
        }
        public void Calculate(DateTime Day)
        {
            TRecords.Calculate(Day);
        }

        #region "Context methods"
        internal void SaveObject(IdObject IdObject)
        {
            if (IdObject.Id == 0)
                _context.Add(IdObject);
            else
                _context.Update(IdObject);

            _context.SaveChanges();
        }

        internal void DeleteObject(IdObject IdObject)
        {
            _context.Remove(IdObject);
            _context.SaveChanges();
        }
        public void SetState(object obj, EntityState state)
        {
            _context.Entry(obj).State = state;
        }

        internal AppDBContext GetContext()
        { return _context; }

        public EntityState GetState(IdObject IdObject)
        {
            var obj = _context.Entry(IdObject);
            if (obj != null)
                return obj.State;
            else
                return EntityState.Detached;
        }
        #endregion 
    }

    public interface IdObject
    {
        public int Id { get; set; }
    }
}
