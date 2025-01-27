using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PISDK;

namespace FBone.Service.WriteToPI
{
    public class PIData : List<PITagData>
    {
        private List<Server> PIServers(string PICollectiveName)
        {
            List<Server> ServerList = new List<Server>();
            try
            {
                PISDK.PISDK sdk = new PISDK.PISDK();
                Server srv = sdk.Servers[PICollectiveName];
                IPICollective picoll = (IPICollective)srv;
                if (picoll.IsCollectiveMember)
                {
                    var pimembers = picoll.ListMembers();
                    ServerList = new List<Server>();
                    List<string> pisrvrnames = new List<string>();

                    //int i = 0;
                    Server pisrv = null;
                    foreach (CollectiveMember member in pimembers)
                    {
                        pisrv = picoll.MemberOpen(member, "");
                        pisrvrnames.Add(pisrv.Name);
                        ServerList.Add(pisrv);
                    }
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return ServerList;
        }

        public bool WriteToPI(string PIServerName)
        {
            List<Server> servers = PIServers(PIServerName);
            foreach (PITagData ptd in this)
            {
                try
                {
                    DateTime ts;
                    PIValues vals = new PIValues
                    {
                        ReadOnly = false
                    };
                    foreach (PIEvent evnt in ptd.Events)
                    {
                        ts = Convert.ToDateTime(evnt.TimeStamp);
                        vals.Add(ts, evnt.Value, null);
                    }

                    PIPoint pt = null;
                    foreach (Server srv in servers)
                    {
                        bool IsConnected = srv.Connected;
                        string srvname = srv.Name;

                        pt = srv.PIPoints[ptd.Tagname];
                        if (pt != null)
                        {
                            pt.Data.UpdateValues(vals);
                        }
                    }
                }
                catch (Exception)
                {
                    //return false;
                    //throw ex;
                }
            }
            return true;
        }
    }

}
