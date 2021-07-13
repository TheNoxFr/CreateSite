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

        public CfgApplication GetApplication(string name)
        {
            CfgApplicationQuery query = new CfgApplicationQuery { Name = name };

            return confService.RetrieveObject<CfgApplication>(query);
        }

        public CfgDN GetDN(string dn, string switchname)
        {
            CfgSwitch sw = RetrieveSwitch(switchname);

            if (sw == null)
                return null;

            CfgDNQuery query = new CfgDNQuery { DnNumber = dn, SwitchDbid = sw.DBID };

            return confService.RetrieveObject<CfgDN>(query);
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

        public void AddValue2KVList(ref KeyValueCollection kv, string section, string option, string value)
        {
            KeyValueCollection opt = new KeyValueCollection();
            opt.Add(option, value);

            if (kv.AllKeys.Contains<string>(section))
            {
                KeyValueCollection sec = kv.GetAsKeyValueCollection(section);
                sec.Add(opt);
            }
            else
            {
                kv.Add(section, opt);
            }
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

        #region Tranasction List

        public CfgTransaction GetTransactionList(string name)
        {
            CfgTransactionQuery query = new CfgTransactionQuery { Name = name };

            return confService.RetrieveObject<CfgTransaction>(query);
        }

        public string AddTransactionList(string name, KeyValueCollection annexe, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgTransaction transaction = new CfgTransaction(confService);

            transaction.Type = CfgTransactionType.CFGTRTList;
            transaction.SetTenantDBID(folder.OwnerID.DBID);
            transaction.Name = name;
            transaction.Alias = name;
            transaction.FolderId = folder.DBID;
            transaction.UserProperties = annexe;

            try
            {
                transaction.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        // Ajout des annexes sur une TL existante
        public string UpdateTransactionList(string name, KeyValueCollection annexe)
        {
            string result = "";

            CfgTransaction tl = GetTransactionList(name);

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
                    result = "Msg : " + e.Message;
                }
            }
            else
            {
                result = "Msg : Transaction List " + name + " non trouvée.";
            }

            return result;
        }

        #endregion

        #region Interaction Queue

        public CfgScript GetInteractionQueue(string name)
        {
            CfgScriptQuery query = new CfgScriptQuery { Name = name };

            return confService.RetrieveObject<CfgScript>(query);
        }

        public string AddInteractionQueue(string name, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgScript script = new CfgScript(confService);

            script.SetTenantDBID(folder.OwnerID.DBID);
            script.FolderId = folder.DBID;
            script.Name = name;
            script.Type = CfgScriptType.CFGInteractionQueue;

            try
            {
                script.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        #endregion

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

        #region Access Group

        public CfgAccessGroup GetAccessGroup(string group)
        {
            CfgAccessGroupQuery query = new CfgAccessGroupQuery { Name = group };

            return confService.RetrieveObject<CfgAccessGroup>(query);
        }

        public string AddAccessGroup(string name, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgAccessGroup accessGroup = new CfgAccessGroup(confService);

            accessGroup.FolderId = folder.DBID;
            accessGroup.GroupInfo = new CfgGroup(confService, null);
            accessGroup.GroupInfo.Name = name;
            accessGroup.GroupInfo.SetTenantDBID(folder.OwnerID.DBID);

            try
            {
                accessGroup.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        public string AddPersonToAccessGroup(string username, string groupname)
        {
            string result = "";

            CfgAccessGroup ag = GetAccessGroup(groupname);
            CfgPerson person = GetPerson(username);

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
                    result = "Msg : " + e.Message;
                }
            }
            else
            {
                if (ag != null)
                {
                    result = "Msg : Person " + username + " non trouvée.";
                } 
                else
                {
                    result = "Msg : Access Group " + groupname + " non trouvé.";
                }
            }
                

            return result;
        }


        #endregion

        #region Person

        public CfgPerson GetPerson(string username)
        {
            CfgPersonQuery query = new CfgPersonQuery { UserName = username };

            return confService.RetrieveObject<CfgPerson>(query);
        }

        public string AddPerson(string username, string employyeid, string firstname, string lastname, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgPerson person = new CfgPerson(confService);

            person.SetTenantDBID(folder.OwnerID.DBID);
            person.FolderId = folder.DBID;
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
                result = "Msg : " + e.Message;
            }

            return result;
        }

        #endregion

        #region Virtual Queue

        // Utilise GetDN
        /*
        public CfgDN GetVirtualQueue(string name, string switchname)
        {
            CfgSwitch sw = RetrieveSwitch(switchname);

            if (sw == null)
                return null;

            CfgDNQuery query = new CfgDNQuery { DnNumber = name, SwitchDbid = sw.DBID };

            return confService.RetrieveObject<CfgDN>(query);
        }*/

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

        #region Routing Point

        public string AddRoutingPoint(string name, string alias, KeyValueCollection annexe, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgDN routingpoint = new CfgDN(confService);

            routingpoint.Number = name;
            routingpoint.Name = alias + name;
            routingpoint.SetTenantDBID(folder.OwnerID.DBID);
            routingpoint.Type = CfgDNType.CFGRoutingPoint;
            routingpoint.RouteType = CfgRouteType.CFGDefault;
            routingpoint.SwitchSpecificType = 1;
            routingpoint.FolderId = folder.DBID;
            routingpoint.UserProperties = annexe;

            string[] pathlist = folderpath.Split('\\');

            if (pathlist.Length < 3)
                return "Msg : nom de répertoire incorrect";

            string switchname = pathlist[2];

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
                    result = "Msg : " + e.Message;
                }
            }
            else
                result = "Msg : Switch non trouvé.";

            return result;
        }

        public string AddDefaultToRoutingPoint(string routingpointname, string defaultname, string folderpath)
        {
            string result = "";

            string[] pathlist = folderpath.Split('\\');

            if (pathlist.Length < 3)
                return "Msg : nom de répertoire incorrect";

            string switchname = pathlist[2];


            CfgDN rp = GetDN(routingpointname, switchname);
            CfgDN def = GetDN(defaultname, switchname);

            if (rp == null)
            {
                return "Msg : Routing Point " + routingpointname + " non trouvé.";
            }

            if (def == null)
            {
                return "Msg : Default " + defaultname + " non trouvé.";
            }

            List<int> list = new List<int>();
            list.Add(def.DBID);
            rp.SetDestDNDBIDs(list);

            try
            {
                rp.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        #endregion

        #region ERP

        public string AddExternalRoutingPoint(string name, string alias, string epn, string association, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";


            CfgDN routingpoint = new CfgDN(confService);

            routingpoint.Number = name;
            routingpoint.Name = alias + name;
            routingpoint.SetTenantDBID(folder.OwnerID.DBID);
            routingpoint.Type = CfgDNType.CFGExtRoutingPoint;
            routingpoint.RouteType = CfgRouteType.CFGDefault;
            routingpoint.SwitchSpecificType = 1;
            routingpoint.Association = association;
            routingpoint.FolderId = folder.DBID;
            KeyValueCollection kv = new KeyValueCollection();
            AddValue2KVList(ref kv, "TServer", "epn", epn);
            routingpoint.UserProperties = kv;

            string[] pathlist = folderpath.Split('\\');

            if (pathlist.Length < 3)
                return "Msg : nom de répertoire incorrect";

            string switchname = pathlist[2];
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
                    result = "Msg : " + e.Message;
                }
            }
            else
                result = "Msg : Switch " + switchname + " non trouvé";

            return result;
        }

        #endregion

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
                CfgDN dn = GetDN(queue, switchname);

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
                result = "Msg : DN Group non trouvé.";

            return result;
        }

        #endregion

        #region Agent Group

        public CfgAgentGroup GetAgentGroup(string group)
        {
            CfgAgentGroupQuery query = new CfgAgentGroupQuery { Name = group };

            return confService.RetrieveObject<CfgAgentGroup>(query);
        }

        public string AddAgentGroup(string name, string annexe, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgAgentGroup group = new CfgAgentGroup(confService);

            group.GroupInfo = new CfgGroup(confService, null);
            group.GroupInfo.SetTenantDBID(folder.OwnerID.DBID);
            group.FolderId = folder.DBID;
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
                result = "Msg : " + e.Message;
            }
            return result;
        }

        public string AddAccessGroupRightsToAgentGroup(string agentgroupname, string accessgroupname)
        {
            string result = "";

            CfgAccessGroup acg = GetAccessGroup(accessgroupname);
            CfgAgentGroup agg = GetAgentGroup(agentgroupname);

            if ((agg != null) && (acg != null))
            {
                agg.SetAccountPermissions(acg, 0, false);
            }
            else
            {
                if (acg == null)
                {
                    result = "Msg : Access Group " + accessgroupname + " non trouvé.";
                }
                else
                {
                    result = "Msg : Agent Group " + agentgroupname + " non trouvé.";
                }
            }


            return result;
        }

        #endregion

        #region CCPulse

        public string AddApplicationCCPulse(string name, string templatename, string storagefile, string liststatservers, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgApplication app = new CfgApplication(confService);
            app.Name = name;
            app.FolderId = folder.DBID;

            CfgAppPrototypeQuery query = new CfgAppPrototypeQuery(confService);
            query.Name = templatename;
            CfgAppPrototype appproto = confService.RetrieveObject<CfgAppPrototype>(query);

            if (appproto == null)
                return "Msg : Template " + templatename + " non trouvé.";

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

                CfgApplication stat = GetApplication(statserver);
                if (stat == null)
                    return "Msg : Statserver " + statserver + " non trouvé.";

                connInfo.SetAppServerDBID(stat.DBID);
                cnx.Add(connInfo);
            }
            app.AppServers = cnx;

            try
            {
                app.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }
            return result;
        }

        #endregion

        #region CS Proxy

        public string AddApplicationCSProxy(string name, string templatename, string host, string user, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgHost cfgHost = GetHost(host);
            if (cfgHost == null)
                return "Msg : Host " + host + " inconnu.";

            CfgApplication app = new CfgApplication(confService);

            app.Name = name;
            app.FolderId = folder.DBID;
            app.ServerInfo = new CfgServer(confService, null);
            app.ServerInfo.SetHostDBID(cfgHost.DBID);
            app.ServerInfo.Port = "2020";
            app.WorkDirectory = ".";
            app.CommandLine = ".";

            CfgAppPrototypeQuery query = new CfgAppPrototypeQuery(confService);
            query.Name = templatename;
            CfgAppPrototype appproto = confService.RetrieveObject<CfgAppPrototype>(query);

            if (appproto == null)
                return "Msg : Template " + templatename + " inconnu.";

            app.AppPrototype = appproto;
            app.Options = appproto.Options; // On récupère les options de base du template

            // On ajoute le confserver et le MessageServer
            List<CfgConnInfo> cnx = new List<CfgConnInfo>();
            CfgConnInfo connInfo = new CfgConnInfo(confService, null);

            CfgApplication confserv = GetApplication("confserv");
            if (confserv == null)
                return "Msg : Application confserv non trouvée.";

            connInfo.SetAppServerDBID(confserv.DBID);
            cnx.Add(connInfo);
            connInfo = new CfgConnInfo(confService, null);

            CfgApplication msgserver = GetApplication("MessageServer");
            if (msgserver == null)
                return "Msg : Application MessageServer non trouvée.";

            connInfo.SetAppServerDBID(msgserver.DBID);
            cnx.Add(connInfo);
            app.AppServers = cnx;

            try
            {
                app.Save();

                // On met à jour le user de lancement une fois que l'application est créée
                CfgApplicationQuery q = new CfgApplicationQuery { Name = name };
                CfgApplication ap = confService.RetrieveObject<CfgApplication>(q);

                CfgPerson cfgPerson = GetPerson(user);
                if (cfgPerson == null)
                    return "Msg : user " + user + " non trouvé";
                ap.UpdateLogonAs(CfgObjectType.CFGPerson, cfgPerson.DBID);
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        #endregion

        #region Host

        public CfgHost GetHost(string name)
        {
            CfgHostQuery query = new CfgHostQuery { Name = name };

            return confService.RetrieveObject<CfgHost>(query);
        }

        public string AddHost(string name, string ipaddress, string scsname, string folderpath)
        {
            string result = "";

            CfgFolder folder = GetFolder(folderpath);
            if (folder == null)
                return "Msg : " + folderpath + " : répertoire non existant";

            CfgApplication scs = GetApplication(scsname);
            if (scs == null)
                return "Msg : " + scsname + " : non existant";

            CfgHost host = new CfgHost(confService);
            host.Name = name;
            host.IPaddress = ipaddress;
            host.LCAPort = "4999";
            host.FolderId = folder.DBID;
            host.Type = CfgHostType.CFGNetworkServer;
            host.OSinfo = new CfgOS(confService, null);
            host.OSinfo.OStype = CfgOSType.CFGWindowsServer2008;

            host.SetSCSDBID(scs.DBID);

            try
            {
                host.Save();
            }
            catch (Exception e)
            {
                result = "Msg : " + e.Message;
            }

            return result;
        }

        #endregion

        #region StatServer Option

        public string AddOptionStatServer(string statserver, KeyValueCollection annexe)
        {
            string result = "";

            CfgApplication app = GetApplication(statserver);

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
                    result = "Msg : " + e.Message;
                }
            }
            else
                result = "Msg : StatServer " + statserver + " non trouvé.";

            return result;
        }

        #endregion
    }
}
