using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Presentation;
using PISDK;
using PISDKCommon;
using PITimeServer;
using System;
using System.Collections.Generic;

namespace FBone.Service.WriteToPI
{
    public class PIFunctions
    {
        private static string _PICollectiveName = string.Empty;
        private static List<Server> _ServerList = null;
        private static PISDK.PISDK sdk = null;
        public static List<Server> PIServers(string PICollectiveName)
        {
            try
            {
                if (sdk == null) sdk = new PISDK.PISDK();
                Server srv = null;
                if (string.IsNullOrEmpty(PICollectiveName))
                {
                    srv = sdk.Servers.DefaultServer;
                }
                else
                {
                    srv = sdk.Servers[PICollectiveName];
                }
                if (_ServerList == null || _PICollectiveName != srv.Name)
                {
                    _ServerList = new List<Server>();

                    _PICollectiveName = srv.Name;
                    IPICollective picoll = (IPICollective)srv;
                    if (picoll.IsCollectiveMember)
                    {
                        var pimembers = picoll.ListMembers();
                        _ServerList = new List<Server>();
                        List<string> pisrvrnames = new List<string>();

                        //int i = 0;
                        Server pisrv = null;
                        foreach (CollectiveMember member in pimembers)
                        {
                            pisrv = picoll.MemberOpen(member, "");
                            pisrvrnames.Add(pisrv.Name);
                            _ServerList.Add(pisrv);
                        }
                    }
                    else
                        _ServerList.Add(srv);
                }
            }
            catch
            {
                return null;// _ServerList;
            }
            return _ServerList;
        }
        private static Server PIServer(string PICollectiveName)
        {
            return PIServers(PICollectiveName)[0];
        }

        public static List<TagValue> GetInterpolatedValuesByCount(string PIServerName, string TagName, object StartTime, object EndTime, int ValuesCount)
        {
            Server srv = PIServers(PIServerName)[0];

            PISDK.PIPoint pt = srv.PIPoints[TagName];
            PIValues vals = pt.Data.InterpolatedValues(StartTime, EndTime, ValuesCount);
            List<TagValue> values = new List<TagValue>();
            foreach (PIValue val in vals) { values.Add(new TagValue(val)); }
            return values;
        }

        public static TagValue GetAverageValue(string PIServerName, string TagName, object StartTime, object EndTime)
        {
            Server srv = PIServers(PIServerName)[0];

            PISDK.PIPoint pt = srv.PIPoints[TagName];
            TagValue value = null;
            try
            {
                PIValue val = pt.Data.Summary(StartTime, EndTime, ArchiveSummaryTypeConstants.astAverage, CalculationBasisConstants.cbEventWeighted);
                value = new TagValue(val);
            }
            catch
            {
                value = new TagValue();
            }
            return value;
        }

        public static List<TagValue> GetRecordedValues(string PIServerName, string TagName, object StartTime, object EndTime)
        {
            Server srv = PIServers(PIServerName)[0];

            PISDK.PIPoint pt = srv.PIPoints[TagName];
            PIValues vals = pt.Data.RecordedValues(StartTime, EndTime);
            List<TagValue> values = new List<TagValue>();
            foreach (PIValue val in vals) { values.Add(new TagValue(val)); }
            return values;
        }
        public static TagValue GetRecordedValue(string PIServerName, string TagName, object TimeStamp, RetrievalTypeConstants RetrievalType = RetrievalTypeConstants.rtCompressed)
        {
            Server srv = PIServers(PIServerName)[0];

            PISDK.PIPoint pt = srv.PIPoints[TagName];
            TagValue value = null;
            try
            {
                PIValue val = pt.Data.ArcValue(TimeStamp, RetrievalType);
                value = new TagValue(val);
            }
            catch
            {
                value = new TagValue();
            }
            return value;
        }

        public static bool IsPointExist(string PIServerName, string TagName)
        {
            Server srv = PIServers(PIServerName)[0];
            try
            {
                PISDK.PIPoint pt = srv.PIPoints[TagName];
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static PIPoint GetPIPoint(string PIServerName, string TagName, bool refresh = false)
        {
            Server srv = PIServers(PIServerName)[0];

            try
            {
                PISDK.PIPoint pt = srv.PIPoints[TagName];
                if (refresh)
                {
                    IRefresh rfr = (IRefresh)pt;
                    rfr.Refresh();
                }
                return pt;
            }
            catch
            {
                return null;
            }
        }

        public static object GetPointAttributeValue(string PIServerName, string TagName, string AttributeName, bool refresh = false)
        {
            Server srv = PIServers(PIServerName)[0];
            try
            {
                PISDK.PIPoint pt = srv.PIPoints[TagName];
                if (refresh)
                {
                    IRefresh rfr = (IRefresh)pt;
                    rfr.Refresh();
                }
                return pt.PointAttributes[AttributeName];
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static bool SaveToPI(string PIServerName, string TagName, TagValue data)
        {
            List<Server> PIServers = PIFunctions.PIServers(PIServerName);
            try
            {
                foreach (Server server in PIServers)
                {
                    PISDK.PIPoint pt = server.PIPoints[TagName];
                    pt.Data.UpdateValue(data.Value, data.TimeStamp, DataMergeConstants.dmReplaceDuplicates);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
