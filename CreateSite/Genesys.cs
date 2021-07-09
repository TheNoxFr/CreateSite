using Axl;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Objects;
using Genesyslab.Platform.Configuration.Protocols.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSite
{
    public class Genesys
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        private ConfServerProtocol confservProtocol;
        private IConfService confService;

        public Genesys(string host, int port, string login, string password)
        {
            Host = host;
            Port = port;
            Login = login;
            Password = password;
        }

        public void Init()
        {
            Endpoint endpoint = new Endpoint("default", Host, Port);
            confservProtocol = new ConfServerProtocol(endpoint);
            confservProtocol.ClientApplicationType = (int)CfgAppType.CFGSCE;
            confservProtocol.ClientName = "default";
            confservProtocol.UserName = Login;
            confservProtocol.UserPassword = Password;
            confservProtocol.Open();

            confService = ConfServiceFactory.CreateConfService(confservProtocol);
        }

        public void Disconnect()
        {
            if (confservProtocol != null)
            {
                try
                {
                    confservProtocol.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        internal CfgApplication RetrieveApplication(string name)
        {
            CfgApplicationQuery query = new CfgApplicationQuery { Name = name };

            return confService.RetrieveObject<CfgApplication>(query);
        }

        internal CfgHost RetrieveHost(string name)
        {
            CfgHostQuery query = new CfgHostQuery { Name = name };

            return confService.RetrieveObject<CfgHost>(query);
        }

        internal CfgAccessGroup RetrieveAccessGroup(string group)
        {
            CfgAccessGroupQuery query = new CfgAccessGroupQuery { Name = group };

            return confService.RetrieveObject<CfgAccessGroup>(query);
        }

        internal CfgAgentGroup RetrieveAgentGroup(string group)
        {
            CfgAgentGroupQuery query = new CfgAgentGroupQuery { Name = group };

            return confService.RetrieveObject<CfgAgentGroup>(query);
        }

        internal CfgPerson RetrievePerson(string username)
        {
            CfgPersonQuery query = new CfgPersonQuery { UserName = username };

            return confService.RetrieveObject<CfgPerson>(query);
        }

        internal CfgDN RetrieveDN(string dn, int switchid)
        {
            CfgDNQuery query = new CfgDNQuery { DnNumber = dn, SwitchDbid = switchid };

            return confService.RetrieveObject<CfgDN>(query);
        }

        internal CfgTransaction RetrieveTransactionList(string name)
        {
            CfgTransactionQuery query = new CfgTransactionQuery { Name = name };

            return confService.RetrieveObject<CfgTransaction>(query);
        }

        internal CfgSwitch RetrieveSwitch(string name)
        {
            CfgSwitchQuery query = new CfgSwitchQuery { Name = name };

            return confService.RetrieveObject<CfgSwitch>(query);
        }

        public CfgTenant GetTenant(string name)
        {
            CfgTenantQuery query = new CfgTenantQuery { Name = name, AllTenants = 1 };

            return confService.RetrieveObject<CfgTenant>(query);
        }


        #region Folder
        // Retourne un Root Folder via son nom (Persons, Places,...) et son tenant (Environment, Resources,...)
        internal CfgFolder RetrieveRootFolder(string rootname, string tenantname)
        {
            CfgFolderQuery query = new CfgFolderQuery(confService);
            query.Name = rootname;
            query.DefaultFolder = 1;

            CfgTenant tenant = GetTenant(tenantname);
            if (tenant == null)
                return null;

            query.OwnerDbid = tenant.DBID;
            
//            ICollection<CfgFolder> res = confService.RetrieveMultipleObjects<CfgFolder>(query);

            return confService.RetrieveObject<CfgFolder>(query);

        }

        // Retourne Le folder directement fils d'un switch
        internal CfgFolder RetrieveChildSwitch(string childname, int parentid)
        {
            CfgFolderQuery query = new CfgFolderQuery(confService);
            query.Name = childname;
            query.OwnerDbid = parentid;

            return confService.RetrieveObject<CfgFolder>(query);
        }

        // Retourne le Folder fils d'un dbid (Folder) via son nom
        internal CfgFolder RetrieveChildFolder(string childname, int parentid)
        {
            CfgFolderQuery query = new CfgFolderQuery(confService);
            query.Dbid= parentid;

            CfgFolder parent = confService.RetrieveObject<CfgFolder>(query);

            foreach(CfgObjectID childid in parent.ObjectIDs)
            {
                query.Dbid = childid.DBID;
                CfgFolder child = confService.RetrieveObject<CfgFolder>(query);

                if ((child != null) && (child.Name.Equals(childname)))
                    return child;
            }

            return null;
        }

        // Retourne un CfgFolder via son path complet
        public CfgFolder GetFolder(string path) 
        {
            CfgFolderQuery query = new CfgFolderQuery();
            int dbid;

            string[] pathlist = path.Split('\\');

            // Il faut au minimum le tenant et le root
            if (pathlist.Length < 2)
                return null;

            string tenantname = pathlist[0];
            string rootname = pathlist[1];
            CfgFolder folder = RetrieveRootFolder(rootname,tenantname);

            if (folder != null)
                dbid = folder.DBID;
            else
                return null;

            // Pour le Switches
            if (rootname.Equals("Switches"))
            {
                try
                {
                    CfgSwitch sw = RetrieveSwitch(pathlist[2]);
                    if (sw != null)
                    {
                        folder = RetrieveChildSwitch(pathlist[3], sw.DBID);
                        dbid = folder.DBID;
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            int idx = 0;
            foreach (string name in pathlist)
            {
                if ((idx >= 2) && (folder != null) && !(rootname.Equals("Switches") && idx <=3))
                {
                    folder = RetrieveChildFolder(name, dbid);
                    if (folder != null)
                        dbid = folder.DBID;
                }
                idx++;
            }

            return folder;
        }

        // Retourne un Cfgfolder via son DBID
        internal CfgFolder RetrieveFolderById(int id)
        {
            CfgFolderQuery query = new CfgFolderQuery(confService);
            query.Dbid = id;

            return confService.RetrieveObject<CfgFolder>(query);

        }


        public string AddFolder(string path)
        {
            string result = "";

            // On recherche le Folder parent
            string[] pathlist = path.Split('\\');

            if (pathlist.Length < 2)
                return "Msg: " + path + " : Nom de répertoire incorrect" ;

            string[] pathlistparent = new string[pathlist.Length - 1];
            Array.Copy(pathlist, pathlistparent, pathlist.Length - 1);

            CfgFolder folderparent = GetFolder(string.Join("\\", pathlistparent));

            CfgFolder folder = new CfgFolder(confService);
            folder.Name = pathlist[pathlist.Length - 1];


            if (folderparent != null)
            {
                folder.FolderClass = CfgFolderClass.CFGFCDefault;
                folder.Type = folderparent.Type;
                folder.FolderId = folderparent.DBID;
                folder.OwnerID = new CfgOwnerID(confService, null);
                folder.OwnerID.DBID = folderparent.OwnerID.DBID; 
                folder.OwnerID.Type = folderparent.OwnerID.Type; 

                try
                {
                    folder.Save();
                }
                catch (Exception e)
                {
                    result = "Msg : " + e.Message;
                }
            } else
                result = "Msg : Répertoire parent non trouvé.";

            return result;
        }

        #endregion

        public void AddValue2KVList(ref KeyValueCollection kv, string section, string option, string value)
        {
            KeyValueCollection opt = new KeyValueCollection();
            opt.Add(option, value);

            if (kv.AllKeys.Contains<string>(section))
            {
                KeyValueCollection sec = kv.GetAsKeyValueCollection(section);
                sec.Add(opt);
            } else
            {
                kv.Add(section, opt);
            }
        }

        public bool AddTransactionList(string name, int parentid, KeyValueCollection annexe)
        {
            bool result = true;

            CfgTransaction transaction = new CfgTransaction(confService);

            transaction.Type = CfgTransactionType.CFGTRTList;
            transaction.SetTenantDBID(101);
            transaction.Name = name;
            transaction.Alias = name;
            transaction.FolderId = parentid;
            transaction.UserProperties = annexe;

            transaction.Save();

            return result;
        }

        // Ajout des annexes sur une TL existante
        public bool UpdateTransactionList(string name, KeyValueCollection annexe)
        {
            bool result = true;

            CfgTransaction tl = RetrieveTransactionList(name);

            if (tl != null)
            {
                //si la section existe il faut la lire et uniquement ajouter l'option

                foreach(string sect in annexe.AllKeys)
                {
                    if (tl.UserProperties.ContainsKey(sect))
                    {
                        // Si l'option existe et qu'on a le mot clé %Value%, on met à jour la valeur. Sinon on renverra une erreur ? ou non !  (on n'écrase pas)
                        foreach(string opt in annexe.GetAsKeyValueCollection(sect).AllKeys)
                        {
                            if (annexe.GetAsKeyValueCollection(sect).GetAsString(opt).Contains("%Value%"))
                            {
                                string newValue = annexe.GetAsKeyValueCollection(sect).GetAsString(opt).Replace("%Value%", tl.UserProperties.GetAsKeyValueCollection(sect).GetAsString(opt));
                                tl.UserProperties.GetAsKeyValueCollection(sect).Set(opt,newValue);
                            } 
                            else
                            {
                                tl.UserProperties.GetAsKeyValueCollection(sect).Add(opt,annexe.GetAsKeyValueCollection(sect).GetAsString(opt));
                            }
                        }

//                        tl.UserProperties.GetAsKeyValueCollection(sect).Add(annexe.GetAsKeyValueCollection(sect));
                    }
                    else
                    {
                        tl.UserProperties.Add(annexe);
                    }
                }

                try
                {
                    tl.Save();
                }
                catch (Exception e )
                {
                    Console.WriteLine(e.Message);
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool AddInteractionQueue(string name, int parentid)
        {
            bool result = true;

            CfgScript script = new CfgScript(confService);

            script.SetTenantDBID(101);
            script.FolderId = parentid;
            script.Name = name;
            script.Type = CfgScriptType.CFGInteractionQueue;

            try
            {
                script.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }

            return result;
        }

        #region Skill

        public CfgSkill GetSkill(string name)
        {
            CfgSkillQuery query = new CfgSkillQuery { Name = name };

            return confService.RetrieveObject<CfgSkill>(query);
        }

        public string AddSkill(string name, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgSkill skill = new CfgSkill(confService);

            skill.SetTenantDBID(folder.OwnerID.DBID);
            skill.Name = name;
            skill.FolderId = folder.DBID;

            try
            {
                skill.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }
        #endregion

        #region Place
        public CfgPlace GetPlace(string name)
        {
            CfgPlaceQuery query = new CfgPlaceQuery { Name = name };

            return confService.RetrieveObject<CfgPlace>(query);
        }

        public string AddPlace(string name, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgPlace place = new CfgPlace(confService);

            place.SetTenantDBID(folder.OwnerID.DBID);
            place.FolderId = folder.DBID;
            place.Name = name;

            try
            {
                place.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }
        #endregion

        public bool AddAccessGroup(string name, int parentid)
        {
            bool result = true;

            CfgAccessGroup accessGroup = new CfgAccessGroup(confService);

            accessGroup.FolderId = parentid;
            accessGroup.GroupInfo = new CfgGroup(confService, null);
            accessGroup.GroupInfo.Name = name;
            accessGroup.GroupInfo.SetTenantDBID(101);

            try
            {
                accessGroup.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }

            return result;
        }

        public bool AddPerson(string username, string employyeid, string firstname, string lastname, int parentid)
        {
            bool result = true;

            CfgPerson person = new CfgPerson(confService);

            person.SetTenantDBID(101);
            person.FolderId = parentid;
            person.FirstName = firstname;
            person.LastName = lastname;
            person.UserName = username;
            person.EmployeeID = employyeid;
            person.IsAgent = CfgFlag.CFGFalse;

            try
            {
                person.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }

            return result;
        }

        public bool AddAccessGroupRightsToAgentGroup(string agentgroupname, string accessgroupname)
        {
            bool result = true;

            CfgAccessGroup acg = RetrieveAccessGroup(accessgroupname);
            CfgAgentGroup agg = RetrieveAgentGroup(agentgroupname);

            if ((agg != null) && (acg != null))
            {
                agg.SetAccountPermissions(acg,0, false);
            }
            else
                result = false;

            return result;
        }

        public bool AddPersonToAccessGroup(string username, string groupname)
        {
            bool result = true;

            CfgAccessGroup ag = RetrieveAccessGroup(groupname);
            CfgPerson person = RetrievePerson(username);

            if ((ag != null) && (person != null))
            {
                CfgID cfgID = new CfgID(confService, null);
                cfgID.DBID = person.DBID;
                ag.MemberIDs.Add(cfgID);

                try
                {
                    ag.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    result = false;
                }
            }
            else
                result = false;

            return result;
        }


        #region Virtual Queue

        public CfgDN GetVirtualQueue(string name, string switchname)
        {
            CfgSwitch sw = RetrieveSwitch(switchname);

            if (sw == null)
                return null;

            CfgDNQuery query = new CfgDNQuery { DnNumber = name, SwitchDbid = sw.DBID };

            return confService.RetrieveObject<CfgDN>(query);
        }

        public string AddVirutalQueue(string name, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            string[] pathlist = folderpath.Split('\\');

            if (pathlist.Length < 3)
                return "Msg : nom de répertoire incorrect";

            string switchname = pathlist[2];

            CfgDN queue = new CfgDN(confService);

            queue.Number = name;
            queue.Name = name + "_" + switchname;
            queue.SetTenantDBID(folder.OwnerID.DBID);
            queue.Type = CfgDNType.CFGVirtACDQueue;
            queue.RouteType = CfgRouteType.CFGDefault;
            queue.SwitchSpecificType = 1;
            queue.FolderId = folder.DBID;

            CfgSwitch sw = RetrieveSwitch(switchname);
            if (sw != null)
            {
                queue.SetSwitchDBID(sw.DBID);

                try
                {
                    queue.Save();
                }
                catch (Exception e)
                {
                    result = "Msg : " + e.Message;
                }
            }
            else
                result = "Msg : Switch " + switchname + " non trouvé" ;

            return result;
        }

        #endregion

        public bool AddRoutingPoint(string name, string switchname, string alias, KeyValueCollection annexe, int parentid)
        {
            bool result = true;

            CfgDN routingpoint = new CfgDN(confService);

            routingpoint.Number = name;
            routingpoint.Name = alias + name;
            routingpoint.SetTenantDBID(101);
            routingpoint.Type = CfgDNType.CFGRoutingPoint;
            routingpoint.RouteType = CfgRouteType.CFGDefault;
            routingpoint.SwitchSpecificType = 1;
            routingpoint.FolderId = parentid;
            routingpoint.UserProperties = annexe;
            

            CfgSwitch sw = RetrieveSwitch(switchname);
            if (sw != null)
            {
                routingpoint.SetSwitchDBID(sw.DBID);

                try
                {
                    routingpoint.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    result = false;
                }
            }
            else
                result = false;

            return result;
        }

        public bool AddExternalRoutingPoint(string name, string switchname, string alias, string epn, string association, int parentid)
        {
            bool result = true;

            CfgDN routingpoint = new CfgDN(confService);

            routingpoint.Number = name;
            routingpoint.Name = alias + name;
            routingpoint.SetTenantDBID(101);
            routingpoint.Type = CfgDNType.CFGExtRoutingPoint;
            routingpoint.RouteType = CfgRouteType.CFGDefault;
            routingpoint.SwitchSpecificType = 1;
            routingpoint.Association = association;
            routingpoint.FolderId = parentid;
            KeyValueCollection kv = new KeyValueCollection();
            AddValue2KVList(ref kv, "TServer", "epn", epn);
            routingpoint.UserProperties = kv;


            CfgSwitch sw = RetrieveSwitch(switchname);
            if (sw != null)
            {
                routingpoint.SetSwitchDBID(sw.DBID);

                try
                {
                    routingpoint.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    result = false;
                }
            }
            else
                result = false;

            return result;
        }

        public bool AddDefaultToRoutingPoint(string routingpointname, string defaultname, string switchname)
        {
            bool result = true;

            CfgSwitch sw = RetrieveSwitch(switchname);
            if (sw == null)
                return false;

            CfgDN rp = RetrieveDN(routingpointname,sw.DBID);
            CfgDN def = RetrieveDN(defaultname, sw.DBID);

            if ((rp == null) || (def == null))
                return false;

            List<int> list = new List<int>();
            list.Add(def.DBID);
            rp.SetDestDNDBIDs(list);

            try
            {
                rp.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }

            return result;
        }

        #region DN Group

        public CfgDNGroup GetDNGroup(string name)
        {
            CfgDNGroupQuery query = new CfgDNGroupQuery { Name = name };

            return confService.RetrieveObject<CfgDNGroup>(query);
        }

        public string AddDNGroup(string name, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgDNGroup group = new CfgDNGroup(confService);
            
            group.GroupInfo = new CfgGroup(confService, null);
            group.GroupInfo.SetTenantDBID(folder.OwnerID.DBID);
            group.FolderId = folder.DBID;
            group.Type = CfgDNGroupType.CFGACDQueues;
            group.GroupInfo.Name = name;
            
            try
            {
                group.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public string AddVirtualQueueToDNGroup(string queue, string switchname, string group)
        {
            string result = "";
            CfgDNGroup dngroup = GetDNGroup(group);

            if (dngroup != null)
            {
                CfgSwitch sw = RetrieveSwitch(switchname);
                if (sw != null)
                {
                    CfgDN dn = RetrieveDN(queue, sw.DBID);

                    if (dn != null)
                    {
                        CfgDNInfo dninfo = new CfgDNInfo(confService, null);
                        dninfo.DN = dn;
                        dngroup.DNs.Add(dninfo);

                        try
                        {
                            dngroup.Save();
                        }
                        catch (Exception e)
                        {
                            result = "Msg : " + e.Message;
                        }
                    }
                    else
                        result = "Msg : virtual queue non trouvée.";
                }
                else
                    result = "Msg : Switch non trouvé.";

            }
            else
                result = "Msg : DN Group non trouvé.";

            return result;
        }

        #endregion

        public bool AddAgentGroup(string name, string annexe, int directory)
        {
            bool result = true;

            CfgAgentGroup group = new CfgAgentGroup(confService);

            group.GroupInfo = new CfgGroup(confService, null);
            group.GroupInfo.SetTenantDBID(101);
            group.FolderId = directory;
            group.GroupInfo.Name = name;

            if (!annexe.Equals(""))
            {
                KeyValueCollection anx = new KeyValueCollection();
                anx.Add("script", annexe);
                group.GroupInfo.UserProperties = new KeyValueCollection();
                group.GroupInfo.UserProperties.Add("virtual",anx);
            }

            try
            {
                group.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }
            return result;
        }

        public bool AddApplicationCCPulse(string name, string templatename, string storagefile, string liststatservers, int directory)
        {
            bool result = true;

            CfgApplication app = new CfgApplication(confService);
            app.Name = name;
            app.FolderId = directory;

            CfgAppPrototypeQuery query = new CfgAppPrototypeQuery(confService);
            query.Name = templatename;
            CfgAppPrototype appproto = confService.RetrieveObject<CfgAppPrototype>(query);

            app.AppPrototype = appproto;

            appproto.Options.GetAsKeyValueCollection("Storage").Set("XMLActionsStorageFullPath", storagefile);
            appproto.Options.GetAsKeyValueCollection("Storage").Set("XMLTemplatesStorageFullPath", storagefile);
            appproto.Options.GetAsKeyValueCollection("Storage").Set("XMLThresholdsStorageFullPath", storagefile);
            appproto.Options.GetAsKeyValueCollection("Storage").Set("XMLWorkspacesStorageFullPath", storagefile);

            app.Options = appproto.Options; // On récupère les options de base du template

            // On ajoute les StatServer en connexion
            List<CfgConnInfo> cnx = new List<CfgConnInfo>();
            string[] statservers = liststatservers.Split(';');
            foreach(string statserver in statservers)
            {
                CfgConnInfo connInfo = new CfgConnInfo(confService, null);
                connInfo.SetAppServerDBID(RetrieveApplication(statserver).DBID);
                cnx.Add(connInfo);
            }
            app.AppServers = cnx;



            try
            {
                app.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }
            return result;
        }

        public bool AddApplicationCSProxy(string name, string templatename, string host, string user, int directory)
        {
            bool result = true;

            CfgApplication app = new CfgApplication(confService);

            app.Name = name;
            app.FolderId = directory;
            app.ServerInfo = new CfgServer(confService, null);
            app.ServerInfo.SetHostDBID(RetrieveHost(host).DBID);
            app.ServerInfo.Port = "2020";
            app.WorkDirectory = ".";
            app.CommandLine = ".";

            CfgAppPrototypeQuery query = new CfgAppPrototypeQuery(confService);
            query.Name = templatename;
            CfgAppPrototype appproto = confService.RetrieveObject<CfgAppPrototype>(query);

            app.AppPrototype = appproto;
            app.Options = appproto.Options; // On récupère les options de base du template

            // On ajoute le confserver et le MessageServer
            List<CfgConnInfo> cnx = new List<CfgConnInfo>();
            CfgConnInfo connInfo = new CfgConnInfo(confService, null);
            connInfo.SetAppServerDBID(RetrieveApplication("confserv").DBID);
            cnx.Add(connInfo);
            connInfo = new CfgConnInfo(confService, null);
            connInfo.SetAppServerDBID(RetrieveApplication("MessageServer").DBID);
            cnx.Add(connInfo);
            app.AppServers = cnx;

            try
            {
                app.Save();

                // On met à jour le user de lancement une fois que l'application est créée
                CfgApplicationQuery q = new CfgApplicationQuery { Name = name };
                CfgApplication ap = confService.RetrieveObject<CfgApplication>(q);
                ap.UpdateLogonAs(CfgObjectType.CFGPerson, RetrievePerson(user).DBID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }

            return result;
        }

        public bool AddHost(string name, string ipaddress, string scsname, int directory)
        {
            bool result = true;

            CfgHost host = new CfgHost(confService);
            host.Name = name;
            host.IPaddress = ipaddress;
            host.LCAPort = "4999";
            host.FolderId = directory;
            host.Type = CfgHostType.CFGNetworkServer;
            host.OSinfo = new CfgOS(confService, null);
            host.OSinfo.OStype = CfgOSType.CFGWindowsServer2008;

            host.SetSCSDBID(RetrieveApplication(scsname).DBID);

            try
            {
                host.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }

            return result;
        }

        public bool AddOptionStatServer(string statserver, KeyValueCollection annexe)
        {
            bool result = true;

            CfgApplication app = RetrieveApplication(statserver);

            if (app != null)
            {
                //si la section existe il faut la lire et uniquement ajouter l'option

                foreach (string sect in annexe.AllKeys)
                {
                    if (app.Options.ContainsKey(sect))
                    {
                        // Si l'option existe et qu'on a le mot clé %Value%, on met à jour la valeur. Sinon on renverra une erreur ? ou non !  (on n'écrase pas)
                        foreach (string opt in annexe.GetAsKeyValueCollection(sect).AllKeys)
                        {
                            if (annexe.GetAsKeyValueCollection(sect).GetAsString(opt).Contains("%Value%"))
                            {
                                string newValue = annexe.GetAsKeyValueCollection(sect).GetAsString(opt).Replace("%Value%", app.Options.GetAsKeyValueCollection(sect).GetAsString(opt));
                                app.Options.GetAsKeyValueCollection(sect).Set(opt, newValue);
                            }
                            else
                            {
                                app.Options.GetAsKeyValueCollection(sect).Add(opt, annexe.GetAsKeyValueCollection(sect).GetAsString(opt));
                            }
                        }
                    }
                    else
                    {
                        app.Options.Add(annexe);
                    }
                }

                try
                {
                    app.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    result = false;
                }
            }
            else
                result = false;

            return result;
        }
    }
}
