using Axl;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateSite
{
    public partial class Main : Form
    {
        public Genesys genesys;
        public Ini ini;
        public Suivi genesysSuivi;
        public Cisco cisco;
        public Section config;

        public const string LBL_PHYSICAL_LOCATION = "Physical Location";
        public const string LBL_LOCATION = "Location";
        public const string LBL_CSS = "Calling Search Space";
        public const string LBL_HUNT_LIST = "Hunt List";
        public const string LBL_GEOLOC = "Geolocation";
        public const string LBL_LINEGROUP = "Line Group";
        public const string LBL_HUNT_PILOT = "Hunt Pilot";
        public const string LBL_TRANSLATION_PATTERN = "Translation Pattern";
        public const string LBL_CALG_PARTY_TFP = "Calling Party TFPattern";
        public const string LBL_CALD_PARTY_TFP = "Called Party TFPattern";
        public const string LBL_DEVICE_MOBILITY = "Device Mobility";
        public const string LBL_REGION = "Region";
        public const string LBL_DEVICE_POOL = "Device Pool";
        public const string LBL_LINE = "Line";
        public const string LBL_CTI_ROUTEPOINT = "CTI RoutePoint";

        public enum TraceLevel { INFO, WARNING, ERROR };
        public enum Target { Cisco, Genesys };

        public Main()
        {
            InitializeComponent();
        }

        private void BtnGO_Click(object sender, EventArgs e)
        {
            genesys = new Genesys("genserv", 2020, txtGenesysLogin.Text, txtGenesysPassword.Text);
            genesysSuivi = new Suivi();

          //  genesys.Connect();
            //genesys.AddAgentGroup("Groupe1", "Skill(\"T_Profil_Jabber\") > 0", 168);
            //genesys.AddDNGroup("VQ_BANDEAU_S13", 171);

            //genesys.AddVirutalQueue("VQ_BANDEAU_IARD_ACPL_S13","Cisco",172);
            //genesys.AddVirutalQueue("VQ_BANDEAU_IARD_ACPL_S13", "Generic", 173);

            //genesys.AddVirtualQueueToDNGroup("VQ_BANDEAU_IARD_ACPL_S13", "Cisco", "VQ_BANDEAU_S13");
            //genesys.AddVirtualQueueToDNGroup("VQ_BANDEAU_IARD_ACPL_S13", "Generic", "VQ_BANDEAU_S13");

            /*
            KeyValueCollection kv = new KeyValueCollection();
            genesys.AddValue2KVList(ref kv, "section1", "option1", "value1");
            genesys.AddValue2KVList(ref kv, "section1", "option2", "value2");
            genesys.AddValue2KVList(ref kv, "section2", "option1", "value1");
            genesys.AddTransactionList("TL1", 189, kv);
            */

            /*
            KeyValueCollection kv = new KeyValueCollection();
            genesys.AddValue2KVList(ref kv, "section1", "option3", "%Value%;value4");
            genesys.AddValue2KVList(ref kv, "section3", "option1", "value2");
            genesys.UpdateTransactionList("TL1", kv);
            */

            //genesys.AddSkill("T_Site_S13", 190);

            //genesys.AddInteractionQueue("S14 MAF", 191);

            //genesys.AddPlace("S14", 186);

            //genesys.AddPerson("cs-proxy.s14", "cs-proxy.s14", "CPProxy", "S14", 177);

            //genesys.AddAccessGroup("CSPROXY14", 192);

            /*
            genesys.AddPersonToAccessGroup("cs-proxy.s14", "CSPROXY14");
            genesys.AddPersonToAccessGroup("cs-proxy.s14", "Super Administrators");
            */

            //genesys.AddAccessGroupRightsToAgentGroup("Zero", "CSPROXY14");

            /*
            CfgFolder folder = genesys.RetrieveFolder("Agent Groups\\SINISTRE\\PJ"); // 169
            CfgFolder folder2 = genesys.RetrieveFolder("Places\\SINISTRE\\IARD"); // 186
            CfgFolder folder3 = genesys.RetrieveFolder("Switches\\Cisco\\DNs\\IARD"); // 172
            CfgFolder folder4 = genesys.RetrieveFolder("Persons\\CC"); // null
            CfgFolder folder5 = genesys.RetrieveFolder("Person\\AA"); // null
            */

            //genesys.AddFolder("Places\\SINISTRE\\IARD\\S14");
            //CfgFolder folder = genesys.RetrieveFolder("Agent Groups\\SINISTRE\\PJ"); // 169
            //CfgFolder folder3 = genesys.RetrieveFolder("Switches\\Simu\\Agent Logins\\AA"); // 193
            //genesys.AddFolder("Switches\\Simu\\Agent Logins\\AA\\IARD");

            /*
            KeyValueCollection kv = new KeyValueCollection();
            genesys.AddValue2KVList(ref kv, "__ROUTER__", "Categorie_Type", "*");
            genesys.AddValue2KVList(ref kv, "__ROUTER__", "Metier", "IARD");
            genesys.AddValue2KVList(ref kv, "TServer", "smloc", "SM_S99");
            genesys.AddRoutingPoint("205701", "Cisco", "S05_STD_RP_", kv, 172);
            genesys.AddRoutingPoint("205709", "Cisco", "S05_Def-DN_RP_", kv, 172);

            genesys.AddDefaultToRoutingPoint("205701", "205709","Cisco");
            */
            //genesys.AddExternalRoutingPoint("205702", "Cisco", "S05_ERP-1_","S05","153744815",172) ;

            TraiteFichierGenesys();

            /*
            KeyValueCollection kv = new KeyValueCollection();
            genesys.AddValue2KVList(ref kv, "Filters", "Filtre2", "Value2");
            genesys.AddOptionStatServer("StatServerCCP", kv);
            */

            //genesys.AddApplicationCCPulse("CCPulse_S14_MA", "CCPulse_800", "\\\\%Site%ste350\\ccpulse\\%Site%\\MA", "StatServerCCP;StatServer_URS", 207);
            //genesys.AddApplicationCSProxy("CSProxy_S14", "CSProxy_8", "S14ste350", "csproxy.S14", 207);
            //            genesys.Disconnect();

            cisco = new Cisco("https://192.168.1.46:8443/axl/", "ccmadministrator", "BcH1Kf0T");
            //cisco.Connect();

            //cisco.AddPartition("ALL_Interne_P", "Mon premier test");
            //cisco.AddPartition("ALL_Genesys_P", "Mon deuxieme test");

            //cisco.AddGeolocation("S01_Geoloc", "+33111111111", "+33222222222");
            //cisco.AddPhysicalLocation("S01_Orleans_PHYLOC");
            //cisco.AddRegion("S01_Rouen_RGN");

        }

        private void ParseInifile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            DialogResult res = dialog.ShowDialog();

            if (res != DialogResult.OK)
                return;


            string iniFileName = dialog.FileName;
            ini = new Ini(IniType.Cisco);

            // On lit le fichier de config
            StreamReader file = new StreamReader(iniFileName);
            string line;
            Section section = null;

            // On importe les données brutes
            while ((line = file.ReadLine()) != null)
            {
                // Si la ligne n'est pas vide ou que ce n'est pas un commentaire
                if ((line.Length > 0) && (line[0] != '#'))
                {
                    // Si c'est une nouvelle section
                    if (line[0] == '[')
                    {
                        section = new Section(line.Trim().Substring(1, line.Trim().Length - 2));
                        ini.Sections.Add(section);
                    }
                    else
                    {
                        int idx = line.IndexOf('=');
                        if (idx != -1)
                        {
                            section.Options.Add(new CleValeur(line.Substring(0, idx), line.Substring(idx + 1, line.Length - (idx + 1))));
                        }
                    }
                }
            }

            file.Close();

            // Si on n'a pas reconnu une section => Waring
            List<Section> err = ini.Sections.FindAll(s => s.Type == SectionType.Erreur);
            if (err.Count > 0)
            {
                // Comment on remonte les erreurs ??
            }

            // On met à jour les champs "wildcard"
            config = ini.Sections.Find(s => s.Type == SectionType.Config);
            if (config == null)
            {
                // Comment on remonte les erreurs ??
                return;
            }

            foreach (CleValeur clevaleur in config.Options)
            {
                foreach (Section sect in ini.Sections)
                {
                    if (sect.Name.Contains("%" + clevaleur.Cle + "%"))
                    {
                        sect.Name = sect.Name.Replace("%" + clevaleur.Cle + "%", clevaleur.Valeur);
                    }

                    foreach (CleValeur opt in sect.Options)
                    {
                        if (opt.Valeur.Contains("%" + clevaleur.Cle + "%"))
                        {
                            opt.Valeur = opt.Valeur.Replace("%" + clevaleur.Cle + "%", clevaleur.Valeur);
                        }
                    }
                }
            }


        }

        private void TraiteFichierCisco()
        {
            ParseInifile();
        }

        private void TraiteFichierGenesys()
        {
            // Si on n'a pas reconnu une section => Waring
            /*
            List<Section> err = genesysIni.Sections.FindAll(s => s.Type == SectionType.Erreur);
            if (err.Count > 0)
            {
                foreach(Section s in err)
                {
                    genesysSuivi.AddEvenement(SectionType.Erreur, Instant.Config, Level.Warning, s.Name);
                }
                lblFichierInconnu.BackColor = Color.Orange;
            }
            */

            ParseInifile();

            CheckConfigGenesys();

            AfficheEvenements();

        }

        // Affiche les évènements
        private void AfficheEvenements()
        {
            foreach(Evenement evt in genesysSuivi.listeEvenements)
            {
                if (evt.sectionType == SectionType.Erreur)
                {
                    if (evt.instant == Instant.Config)
                    {
                        int debut = rtbConfigEvtInconnu.Text.Length;
                        rtbConfigEvtInconnu.Text += evt.texte;
                        rtbConfigEvtInconnu.Select(debut, evt.texte.Length);
                        switch (evt.level)
                        {
                            case Level.Error:
                                rtbConfigEvtInconnu.SelectionColor = Color.Red;
                                break;
                            case Level.Information:
                                rtbConfigEvtInconnu.SelectionColor = Color.Green;
                                break;
                            case Level.Warning:
                                rtbConfigEvtInconnu.SelectionColor = Color.Yellow;
                                break;
                        }
                        //rtbConfigEvtInconnu.Select(0, 0);
                    }
                }
            }
        }

        private bool IsWildcard(List<Section> liste)
        {
            bool result = false;
            Regex regex = new Regex("(%.*%)");

            foreach(Section s in liste)
            {
                if (regex.IsMatch(s.Name))
                    return true;

                foreach(CleValeur cleValeur in s.Options)
                {
                    if (regex.IsMatch(cleValeur.Valeur.Replace("%Value%","")))
                            return true;
                }
            }

            return result;
        }

        private void CheckConfigGenesys()
        {
            List<Section> liste;

            // Check Folder
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Folder);
            if (IsWildcard(liste))
            {
                lblFichierFolder.BackColor = Color.Red;
            }

            // Check Access Group
            liste = ini.Sections.FindAll(s => s.Type == SectionType.AccessGroup);
            if (IsWildcard(liste))
            {
                lblFichierAccessGroup.BackColor = Color.Red;
            }

        }

        private void BtnNEXT_Click(object sender, EventArgs e)
        {
            List<Section> liste;
            int folderid;
            string folder;
            CleValeur cleFolder;
            string alias;
            KeyValueCollection kv;
            string templatename;
            string valeur;

            // Folder
            if (cbFolder.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.Folder);
                foreach(Section sect in liste)
                {
                    foreach(CleValeur clevaleur in sect.Options)
                    {
                        if (!genesys.AddFolder(clevaleur.Valeur))
                        {
                            lblResultFolder.BackColor = Color.Red;
                        }
                    }
                }
            }

            // Person
            if (cbPerson.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.Person);
                foreach (Section sect in liste)
                {
                    string firstName = sect.Options.Find(o => o.Cle.Equals("FirstName")).Valeur;
                    string lastName = sect.Options.Find(o => o.Cle.Equals("LastName")).Valeur;
                    string emloyeeId = sect.Options.Find(o => o.Cle.Equals("EmployeeId")).Valeur;
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    if (!genesys.AddPerson(sect.Name,emloyeeId,firstName,lastName,folderid))
                    {
                        lblResultPerson.BackColor = Color.Red;
                    }
                }

            }

            // Access Group
            if (cbAccessGroup.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.AccessGroup);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    if (!genesys.AddAccessGroup(sect.Name, folderid))
                    {
                        lblResultAccessGroup.BackColor = Color.Red;
                    }
                    else
                    {
                        string username = sect.Options.Find(o => o.Cle.Equals("Person")).Valeur;
                        if (!genesys.AddPersonToAccessGroup(username,sect.Name))
                        {
                            lblResultAccessGroup.BackColor = Color.Red;
                        }
                    }
                }
            }

            // Agent Group
            if (cbAgentGroup.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.AgentGroup);
                foreach (Section sect in liste)
                {
                    cleFolder = sect.Options.Find(o => o.Cle.Equals("Folder"));
                    
                    // Si il y a pas un Folder c'est qu'on doit créer le groupe. sinon on mettre que les Access Group à jour
                    if (cleFolder != null)
                    {
                        folder = cleFolder.Valeur;
                        folderid = genesys.RetrieveFolder(folder).DBID;
                        string annexe = sect.Options.Find(o => o.Cle.Equals("Script")).Valeur;
                        if (!genesys.AddAgentGroup(sect.Name, annexe, folderid))
                        {
                            lblResultAgentGroup.BackColor = Color.Red;
                        }
                    }

                    // On ajout les Access Group (qu'on ait créé le groupe on non)

                    string[] acgs = sect.Options.Find(o => o.Cle.Equals("NoAccess")).Valeur.Split(';');
                    foreach(string acg in acgs)
                    {
                        if (!genesys.AddAccessGroupRightsToAgentGroup(sect.Name,acg))
                        {
                            lblResultAgentGroup.BackColor = Color.Red;
                        }
                    }
                }
            }

            // Skill
            if (cbSkill.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.Skill);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    if (!genesys.AddSkill(sect.Name, folderid))
                    {
                        lblResultSkill.BackColor = Color.Red;
                    }
                }
            }

            // Virtual Queue
            if (cbVirtualQueue.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.VirtualQueue);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    if (!genesys.AddVirutalQueue(sect.Name, folder.Split('\\')[1], folderid))
                    {
                        lblResultVirtualQueue.BackColor = Color.Red;
                    }
                }
            }

            // Group Of Queue
            if (cbGroupOfQueue.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.GroupOfQueue);
                foreach (Section sect in liste)
                {
                    cleFolder = sect.Options.Find(o => o.Cle.Equals("Folder"));

                    if (cleFolder != null)
                    {
                        folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                        folderid = genesys.RetrieveFolder(folder).DBID;
                        if (!genesys.AddDNGroup(sect.Name, folderid))
                        {
                            lblResultGroupOfQueue.BackColor = Color.Red;
                        }
                    }

                    string[] queues = sect.Options.Find(o => o.Cle.Equals("VQ")).Valeur.Split(';');
                    foreach (string queue in queues)
                    {
                        string[] qlong = queue.Split('_');
                        string sw = qlong[qlong.Length - 1];

                        string[] qshort = new string[qlong.Length - 1];
                        Array.Copy(qlong, qshort, qlong.Length - 1);

                        if (!genesys.AddVirtualQueueToDNGroup(string.Join("_", qshort), sw, sect.Name))
                        {
                            lblResultGroupOfQueue.BackColor = Color.Red;
                        }
                    }

                }
            }

            // Place
            if (cbPlace.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.Place);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    if (!genesys.AddPlace(sect.Name, folderid))
                    {
                        lblResultPlace.BackColor = Color.Red;
                    }
                }
            }

            // Routing Point
            if(cbRoutingPoint.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.RoutingPoint);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    alias = sect.Options.Find(o => o.Cle.Equals("Alias")).Valeur;
                    kv = new KeyValueCollection();
                    genesys.AddValue2KVList(ref kv, "__ROUTER__", "Categorie_Type", sect.Options.Find(o => o.Cle.Equals("Categorie_Type")).Valeur);
                    genesys.AddValue2KVList(ref kv, "__ROUTER__", "Metier", sect.Options.Find(o => o.Cle.Equals("Metier")).Valeur);
                    genesys.AddValue2KVList(ref kv, "__ROUTER__", "Perimetre", sect.Options.Find(o => o.Cle.Equals("Perimetre")).Valeur);
                    genesys.AddValue2KVList(ref kv, "__ROUTER__", "Zone", sect.Options.Find(o => o.Cle.Equals("Zone")).Valeur);
                    genesys.AddValue2KVList(ref kv, "TServer", "smloc", sect.Options.Find(o => o.Cle.Equals("smloc")).Valeur);
                    if (!genesys.AddRoutingPoint(sect.Name, folder.Split('\\')[1], alias, kv, folderid))
                    {
                        lblResultRoutingPoint.BackColor = Color.Red;
                    }

                    // Si il y a un defaut
                    CleValeur defaultrp = sect.Options.Find(o => o.Cle.Equals("Default"));
                    if (defaultrp != null)
                    {
                        if (!genesys.AddDefaultToRoutingPoint(sect.Name,defaultrp.Valeur, folder.Split('\\')[1]))
                        {
                            lblResultRoutingPoint.BackColor = Color.Red;
                        }
                    }
                }
            }

            // ERP
            if (cbERP.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.ERP);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    alias = sect.Options.Find(o => o.Cle.Equals("Alias")).Valeur;
                    string epn = sect.Options.Find(o => o.Cle.Equals("epn")).Valeur;
                    string association = sect.Options.Find(o => o.Cle.Equals("association")).Valeur;
                    if (!genesys.AddExternalRoutingPoint(sect.Name, folder.Split('\\')[1],alias,epn, association,folderid))
                    {
                        lblResultERP.BackColor = Color.Red;
                    }
                }
            }

            // Transaction List
            if (cbTransactionList.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.TransactionList);
                foreach (Section sect in liste)
                {
                    cleFolder = sect.Options.Find(o => o.Cle.Equals("Folder"));

                    // On ajoute une TL
                    if (cleFolder != null)
                    {
                        folder = cleFolder.Valeur;
                        folderid = genesys.RetrieveFolder(folder).DBID;
                        kv = new KeyValueCollection();
                        foreach(CleValeur clevaleur in sect.Options)
                        {
                            if (!clevaleur.Cle.Equals("Folder"))
                            {
                                string[] opt = clevaleur.Valeur.Split('=');   // peut on avoir des valeurs d'options avec des = ?????????
                                genesys.AddValue2KVList(ref kv, clevaleur.Cle, opt[0], opt[1]);
                            }
                        }
                        if (!genesys.AddTransactionList(sect.Name, folderid, kv))
                        {
                            lblResultTransactionList.BackColor = Color.Red;
                        }
                    } else // on en met à jour une
                    {
                        kv = new KeyValueCollection();
                        foreach (CleValeur clevaleur in sect.Options)
                        {
                            string[] opt = clevaleur.Valeur.Split('=');
                            valeur = clevaleur.Valeur.Substring(opt[0].Length + 1);
                            genesys.AddValue2KVList(ref kv, clevaleur.Cle, opt[0], valeur);
                        }
                        if (!genesys.UpdateTransactionList(sect.Name, kv))
                        {
                            lblResultTransactionList.BackColor = Color.Red;
                        }
                    }
                }
            }

            // Options Application 
            if (cbOptionsApp.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.Options);
                foreach (Section sect in liste)
                {
                    kv = new KeyValueCollection();
                    foreach (CleValeur clevaleur in sect.Options)
                    {
                        string[] opt = clevaleur.Valeur.Split('=');
                        valeur = clevaleur.Valeur.Substring(opt[0].Length + 1);
                        genesys.AddValue2KVList(ref kv, clevaleur.Cle, opt[0], valeur);
                    }
                    if (!genesys.AddOptionStatServer(sect.Name,kv))
                    {
                        lblResultOptionsApp.BackColor = Color.Red;
                    }
                }
            }


            // CCpulse
            if (cbCCPulse.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.CCPulse);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    templatename = sect.Options.Find(o => o.Cle.Equals("Template")).Valeur;
                    string storagefile = sect.Options.Find(o => o.Cle.Equals("Storage")).Valeur;
                    string liststatservers = sect.Options.Find(o => o.Cle.Equals("StatServer")).Valeur;
                    if (!genesys.AddApplicationCCPulse(sect.Name,templatename,storagefile,liststatservers, folderid))
                    {
                        lblResultCCPulse.BackColor = Color.Red;
                    }
                }
            }

            // Host
            if (cbHost.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.Host);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    string ipaddress = sect.Options.Find(o => o.Cle.Equals("IPAddress")).Valeur;
                    string scsname = sect.Options.Find(o => o.Cle.Equals("SCS")).Valeur;
                    if (!genesys.AddHost(sect.Name,ipaddress, scsname, folderid))
                    {
                        lblResultHost.BackColor = Color.Red;
                    }
                }
            }

            // CS Proxy
            if (cbCSProxy.Checked)
            {
                liste = ini.Sections.FindAll(s => s.Type == SectionType.CSProxy);
                foreach (Section sect in liste)
                {
                    folder = sect.Options.Find(o => o.Cle.Equals("Folder")).Valeur;
                    folderid = genesys.RetrieveFolder(folder).DBID;
                    templatename = sect.Options.Find(o => o.Cle.Equals("Template")).Valeur;
                    string host = sect.Options.Find(o => o.Cle.Equals("Host")).Valeur;
                    string user = sect.Options.Find(o => o.Cle.Equals("User")).Valeur;
                    if (!genesys.AddApplicationCSProxy(sect.Name, templatename, host, user, folderid))
                    {
                        lblResultCSProxy.BackColor = Color.Red;
                    }
                }
            }

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (genesys != null)
                genesys.Disconnect();
        }

        private void BtnCisco_Click(object sender, EventArgs e)
        {
            TraiteFichierCisco();

            try
            {
                lblCiscoCodeUGS.Text = config.getOption("Site");
                lblCiscoVille.Text = config.getOption("Nom");
                lblCiscoNumPrefixe.Text = config.getOption("PlanNum");

            }
            catch (Exception)
            {
                // Les infos de base sont non présentes (à bloquer via une pré-vérif ?
            }

            List<Section> liste;

            // Ajout Physical Location
            liste = ini.Sections.FindAll(s => s.Type == SectionType.PhysicalLocation);
            if (liste.Count > 0)
            {
                TreeNode phyloc = treeCisco.Nodes.Add(LBL_PHYSICAL_LOCATION, LBL_PHYSICAL_LOCATION);
                foreach (Section section in liste)
                {
                    phyloc.Nodes.Add(section.Name); 
                }
            }

            // Ajout Location
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Location);
            if (liste.Count > 0)
            {
                TreeNode loc = treeCisco.Nodes.Add(LBL_LOCATION, LBL_LOCATION);
                foreach (Section section in liste)
                {
                    loc.Nodes.Add(section.Name);
                }
            }

            // Ajout CSS
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CallingSearchSpace);
            if (liste.Count > 0)
            {
                TreeNode css = treeCisco.Nodes.Add(LBL_CSS, LBL_CSS);
                foreach (Section section in liste) // Une section par CSS
                {
                    css.Nodes.Add(section.Name); // Le nom est dans la section
                }
            }

            // Ajout Geolocation
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Geolocation);
            if (liste.Count > 0)
            {
                TreeNode css = treeCisco.Nodes.Add(LBL_GEOLOC, LBL_GEOLOC);
                foreach (Section section in liste) // Une section par Geoloc
                {
                    css.Nodes.Add(section.Name); // Le nom est dans la section
                }
            }

            // Ajout Line Group
            liste = ini.Sections.FindAll(s => s.Type == SectionType.LineGroup);
            if (liste.Count > 0)
            {
                TreeNode lg = treeCisco.Nodes.Add(LBL_LINEGROUP, LBL_LINEGROUP);
                foreach (Section section in liste) // Une section par Line Group
                {
                    lg.Nodes.Add(section.Name); // Le nom est dans la section
                }
            }

            // Ajout Hunt List
            liste = ini.Sections.FindAll(s => s.Type == SectionType.HuntList);
            if (liste.Count > 0)
            {
                TreeNode hl = treeCisco.Nodes.Add(LBL_HUNT_LIST, LBL_HUNT_LIST);
                foreach (Section section in liste) // Une section par Line Group
                {
                    hl.Nodes.Add(section.Name); // Le nom est dans la section
                }
            }

            // Ajout Hunt Pilot
            liste = ini.Sections.FindAll(s => s.Type == SectionType.HuntPilot);
            if (liste.Count > 0)
            {
                TreeNode hp = treeCisco.Nodes.Add(LBL_HUNT_PILOT, LBL_HUNT_PILOT);
                foreach (Section section in liste) 
                {
                    hp.Nodes.Add(section.Name); // Le numéro est dans la section
                }
            }

            // Ajout Translation Pattern
            liste = ini.Sections.FindAll(s => s.Type == SectionType.TranslationPattern);
            if (liste.Count > 0)
            {
                TreeNode tp = treeCisco.Nodes.Add(LBL_TRANSLATION_PATTERN, LBL_TRANSLATION_PATTERN);
                foreach (Section section in liste)
                {
                    tp.Nodes.Add(section.Name); // Le numéro est dans la section
                }
            }

            // Ajout Calling Party Tranformation Pattern
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CallingPartyTransformationPattern);
            if (liste.Count > 0)
            {
                TreeNode cg = treeCisco.Nodes.Add(LBL_CALG_PARTY_TFP, LBL_CALG_PARTY_TFP);
                foreach (Section section in liste)
                {
                    cg.Nodes.Add(section.Name); 
                }
            }

            // Ajout Called Party Tranformation Pattern
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CalledPartyTransformationPattern);
            if (liste.Count > 0)
            {
                TreeNode cd = treeCisco.Nodes.Add(LBL_CALD_PARTY_TFP, LBL_CALD_PARTY_TFP);
                foreach (Section section in liste)
                {
                    cd.Nodes.Add(section.Name);
                }
            }

            // Ajout Device Mobility Info
            liste = ini.Sections.FindAll(s => s.Type == SectionType.DeviceMobilityInfo);
            if (liste.Count > 0)
            {
                TreeNode dm = treeCisco.Nodes.Add(LBL_DEVICE_MOBILITY, LBL_DEVICE_MOBILITY);
                foreach (Section section in liste)
                {
                    dm.Nodes.Add(section.Name);
                }
            }

            // Ajout Region
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Region);
            if (liste.Count > 0)
            {
                TreeNode re = treeCisco.Nodes.Add(LBL_REGION, LBL_REGION);
                foreach (Section section in liste)
                {
                    re.Nodes.Add(section.Name);
                }
            }

            // Ajout Device Pool
            liste = ini.Sections.FindAll(s => s.Type == SectionType.DevicePool);
            if (liste.Count > 0)
            {
                TreeNode dp = treeCisco.Nodes.Add(LBL_DEVICE_POOL, LBL_DEVICE_POOL);
                foreach (Section section in liste)
                {
                    dp.Nodes.Add(section.Name);
                }
            }

            // Ajout Line
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Line);
            if (liste.Count > 0)
            {
                TreeNode li = treeCisco.Nodes.Add(LBL_LINE, LBL_LINE);
                foreach (Section section in liste)
                {
                    li.Nodes.Add(section.Name);
                }
            }

            // Ajout CTI RoutePoint
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CTIRoutePoint);
            if (liste.Count > 0)
            {
                TreeNode cti = treeCisco.Nodes.Add(LBL_CTI_ROUTEPOINT, LBL_CTI_ROUTEPOINT);
                foreach (Section section in liste)
                {
                    cti.Nodes.Add(section.Name);
                }
            }


            treeCisco.ExpandAll();

            tabControlGeneral.SelectTab("tabPageCisco");


        }


        private void BtnCiscoVerifExiste_Click(object sender, EventArgs e)
        {
            cisco = new Cisco("https://192.168.1.46:8443/axl/", "ccmadministrator", "BcH1Kf0T");
            cisco.Init();

            Trace(Target.Cisco, TraceLevel.INFO, "------------------------------");
            Trace(Target.Cisco, TraceLevel.INFO, "Début Vérification.");

            // Vérif Physical Location
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification "+ LBL_PHYSICAL_LOCATION);
            TreeNode phyloc = treeCisco.Nodes[LBL_PHYSICAL_LOCATION];
            if (phyloc != null)
            {
                foreach(TreeNode node in phyloc.Nodes)
                {
                    string name = node.Text;
                    GetPhysicalLocationRes ph = cisco.GetPhysicalLocation(name);
                    if (ph != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Location
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_LOCATION);
            TreeNode loc = treeCisco.Nodes[LBL_LOCATION];
            if (loc != null)
            {
                foreach (TreeNode node in loc.Nodes)
                {
                    string name = node.Text;
                    GetLocationRes lo = cisco.GetLocation(name);
                    if (lo != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Geoloc
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_GEOLOC);
            TreeNode geo = treeCisco.Nodes[LBL_GEOLOC];
            if (geo != null)
            {
                foreach (TreeNode node in geo.Nodes)
                {
                    string name = node.Text;
                    GetGeoLocationRes ge = cisco.GetGeolocation(name);
                    if (ge != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Line Group
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_LINEGROUP);
            TreeNode lg = treeCisco.Nodes[LBL_LINEGROUP];
            if (lg != null)
            {
                foreach (TreeNode node in lg.Nodes)
                {
                    string name = node.Text;
                    GetLineGroupRes l = cisco.GetLineGroup(name);
                    if (l != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Hunt List
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_HUNT_LIST);
            TreeNode hl = treeCisco.Nodes[LBL_HUNT_LIST];
            if (hl != null)
            {
                foreach (TreeNode node in hl.Nodes)
                {
                    string name = node.Text;
                    GetHuntListRes h = cisco.GetHuntList(name);
                    if (h != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Hunt Pilot
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_HUNT_PILOT);
            TreeNode hp = treeCisco.Nodes[LBL_HUNT_PILOT];
            if (hp != null)
            {
                foreach (TreeNode node in hp.Nodes)
                {
                    string numero = node.Text;
                    GetHuntPilotRes h = cisco.GetHuntPilot(numero,"ALL_Interne_P");
                    if (h != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " ALL_Interne_P");
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Translation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_TRANSLATION_PATTERN);
            TreeNode tp = treeCisco.Nodes[LBL_TRANSLATION_PATTERN];
            if (tp != null)
            {
                foreach (TreeNode node in tp.Nodes)
                {
                    string numero = node.Text;
                    GetTransPatternRes t = cisco.GetTranslationPattern(numero, "ALL_Interne_P");
                    if (t != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " ALL_Interne_P");
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Calling Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_CALG_PARTY_TFP);
            TreeNode cg = treeCisco.Nodes[LBL_CALG_PARTY_TFP];
            if (cg != null)
            {
                foreach (TreeNode node in cg.Nodes)
                {
                    string numero = node.Text;
                    Section section = ini.Sections.Find(s => s.Type == SectionType.CallingPartyTransformationPattern && s.Name.Equals(numero));
                    string partition = section.getOption("Partition");
                    GetCallingPartyTransformationPatternRes c = cisco.GetCallingPartyTransformationPattern(numero, partition);
                    if (c != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Called Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_CALD_PARTY_TFP);
            TreeNode cd = treeCisco.Nodes[LBL_CALD_PARTY_TFP];
            if (cd != null)
            {
                foreach (TreeNode node in cd.Nodes)
                {
                    string numero = node.Text;
                    Section section = ini.Sections.Find(s => s.Type == SectionType.CalledPartyTransformationPattern && s.Name.Equals(numero));
                    string partition = section.getOption("Partition");
                    GetCalledPartyTransformationPatternRes c = cisco.GetCalledPartyTransformationPattern(numero, partition);
                    if (c != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Device Mobility Info
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_DEVICE_MOBILITY);
            TreeNode dm = treeCisco.Nodes[LBL_DEVICE_MOBILITY];
            if (dm != null)
            {
                foreach (TreeNode node in dm.Nodes)
                {
                    string name = node.Text;
                    GetDeviceMobilityRes d = cisco.GetDeviceMobilityInfo(name);
                    if (d != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Region
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_REGION);
            TreeNode re = treeCisco.Nodes[LBL_REGION];
            if (re != null)
            {
                foreach (TreeNode node in re.Nodes)
                {
                    string name = node.Text;
                    GetRegionRes r = cisco.GetRegion(name);
                    if (r != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Device Pool
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_DEVICE_POOL);
            TreeNode dp = treeCisco.Nodes[LBL_DEVICE_POOL];
            if (dp != null)
            {
                foreach (TreeNode node in dp.Nodes)
                {
                    string name = node.Text;
                    GetDevicePoolRes d = cisco.GetDevicePool(name);
                    if (d != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif Line
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_LINE);
            TreeNode li = treeCisco.Nodes[LBL_LINE];
            if (li != null)
            {
                foreach (TreeNode node in li.Nodes)
                {
                    string numero = node.Text;
                    Section section = ini.Sections.Find(s => s.Type == SectionType.Line && s.Name.Equals(numero));
                    string partition = section.getOption("Partition");
                    GetLineRes l = cisco.GetLine(numero, partition);
                    if (l != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif CTI Route Point
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_CTI_ROUTEPOINT);
            TreeNode cti = treeCisco.Nodes[LBL_CTI_ROUTEPOINT];
            if (cti != null)
            {
                foreach (TreeNode node in cti.Nodes)
                {
                    string name = node.Text;
                    GetCtiRoutePointRes c = cisco.GetCtiRoutingPoint(name);
                    if (c != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.BackColor = Color.Red;
                    }
                }
            }


        }

        private void BtnCiscoCreer_Click(object sender, EventArgs e)
        {
            Trace(Target.Cisco, TraceLevel.INFO, "------------------------------");
            Trace(Target.Cisco, TraceLevel.INFO, "Début Création.");

            // Crée Physical Location
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_PHYSICAL_LOCATION);
            TreeNode phyloc = treeCisco.Nodes[LBL_PHYSICAL_LOCATION];
            if (phyloc != null)
            {
                foreach (TreeNode node in phyloc.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        string res = cisco.AddPhysicalLocation(name);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        } else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée Location
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_LOCATION);
            TreeNode loc = treeCisco.Nodes[LBL_LOCATION];
            if (loc != null)
            {
                foreach (TreeNode node in loc.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        string res = cisco.Addlocation(name);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Geolocation
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_GEOLOC);
            TreeNode geo = treeCisco.Nodes[LBL_GEOLOC];
            if (geo != null)
            {
                foreach (TreeNode node in geo.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.Geolocation && s.Name.Equals(name));
                        string NAM = section.getOption("NAM");
                        string PC = section.getOption("PC");
                        string res = cisco.AddGeolocation(name, NAM, PC);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Line Group
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_LINEGROUP);
            TreeNode lg = treeCisco.Nodes[LBL_LINEGROUP];
            if (lg != null)
            {
                foreach (TreeNode node in lg.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        string res = cisco.AddLineGroup(name);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Hunt List
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_HUNT_LIST);
            TreeNode hl = treeCisco.Nodes[LBL_HUNT_LIST];
            if (hl != null)
            {
                foreach (TreeNode node in hl.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.HuntList && s.Name.Equals(name));
                        string cucmg = section.getOption("CUCMG");
                        string linegroup = section.getOption("LineGroup");
                        string res = cisco.AddHuntList(name, name, cucmg, linegroup);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Hunt Pilot
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_HUNT_PILOT);
            TreeNode hp = treeCisco.Nodes[LBL_HUNT_PILOT];
            if (hp != null)
            {
                foreach (TreeNode node in hp.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.HuntPilot && s.Name.Equals(numero));
                        string description = section.getOption("Description");
                        string huntlist = section.getOption("HuntList");
                        string res = cisco.AddHuntPilot(numero, "ALL_Interne_P", description, huntlist);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, numero + " ALL_Interne_P" + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + numero + " ALL_Interne_P");
                        }
                    }
                }
            }

            // Créer Translation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_TRANSLATION_PATTERN);
            TreeNode tp = treeCisco.Nodes[LBL_TRANSLATION_PATTERN];
            if (tp != null)
            {
                foreach (TreeNode node in tp.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.TranslationPattern && s.Name.Equals(numero));
                        string description = section.getOption("Description");
                        string calledmask = section.getOption("CalledMask");
                        string callingmask = section.getOption("CallingMask");
                        string res = cisco.AddTranslationPattern(numero, "ALL_Interne_P", description, "SYS_Technique_CSS", calledmask, callingmask);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, numero + " ALL_Interne_P" + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + numero + " ALL_Interne_P");
                        }
                    }
                }
            }

            // Créer Calling Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_CALG_PARTY_TFP);
            TreeNode cg = treeCisco.Nodes[LBL_CALG_PARTY_TFP];
            if (cg != null)
            {
                foreach (TreeNode node in cg.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.CallingPartyTransformationPattern && s.Name.Equals(numero));
                        string partition = section.getOption("Partition");
                        string description = section.getOption("Description");
                        string mask = section.getOption("Mask");
                        string res = cisco.AddCallingPartyTransformationPattern(numero, partition, description, mask);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, numero + " " + partition + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + numero + " " + partition);
                        }
                    }
                }
            }

            // Créer Called Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_CALD_PARTY_TFP);
            TreeNode cd = treeCisco.Nodes[LBL_CALD_PARTY_TFP];
            if (cd != null)
            {
                foreach (TreeNode node in cd.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.CalledPartyTransformationPattern && s.Name.Equals(numero));
                        string partition = section.getOption("Partition");
                        string description = section.getOption("Description");
                        string mask = section.getOption("Mask");
                        string res = cisco.AddCalledPartyTransformationPattern(numero, partition, description, mask);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, numero + " " + partition + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + numero + " " + partition);
                        }
                    }
                }
            }

            // Créer Device Mobility Info
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_DEVICE_MOBILITY);
            TreeNode dm = treeCisco.Nodes[LBL_DEVICE_MOBILITY];
            if (dm != null)
            {
                foreach (TreeNode node in dm.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.DeviceMobilityInfo && s.Name.Equals(name));
                        string subnet = section.getOption("Subnet");
                        string devicepool = section.getOption("DevicePool");
                        string mask = section.getOption("Mask");
                        string res = cisco.AddDeviceMobilityInfo(name, subnet, mask, devicepool);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Region
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_REGION);
            TreeNode re = treeCisco.Nodes[LBL_REGION];
            if (re != null)
            {
                foreach (TreeNode node in re.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.Region && s.Name.Equals(name));
                        string codeclist = section.getOption("CodecList");
                        string res = cisco.AddRegion(name, codeclist);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Device Pool
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_DEVICE_POOL);
            TreeNode dp = treeCisco.Nodes[LBL_DEVICE_POOL];
            if (dp != null)
            {
                foreach (TreeNode node in dp.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.DevicePool && s.Name.Equals(name));
                        string ccmg = section.getOption("CCMG");
                        //string datetimegroup = section.getOption("DateTimeGroup");
                        string region = section.getOption("Region");
                        string mrgl = section.getOption("MRGL");
                        string location = section.getOption("Location");
                        string physicallocation = section.getOption("PhysicalLocation");
                        //string devicemobilitygroup = section.getOption("DeviceMobilityGroup");
                        string primarylrg = section.getOption("PrimaryLocalRouteGroup");
                        string secondarylrg = section.getOption("SecondaryLocalRouteGroup");
                        string devicemobilitycss = section.getOption("DeviceMobilityCSS");
                        string geolocation = section.getOption("Geolocation");
                        string res = cisco.AddDevicePool(name, ccmg, region, mrgl, location, physicallocation, primarylrg, secondarylrg, devicemobilitycss, geolocation);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Créer Line
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_LINE);
            TreeNode li = treeCisco.Nodes[LBL_LINE];
            if (li != null)
            {
                foreach (TreeNode node in li.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.Line && s.Name.Equals(numero));
                        string partition = section.getOption("Partition");
                        string voicemailprofile = section.getOption("VoiceMailProfile");
                        string css = section.getOption("CSS");
                        string destinationerror = section.getOption("DestinationError");
                        string csserror = section.getOption("CssError");
                        string destinationfwdall = section.getOption("DestinationForwardAll");
                        string cssfwdall = section.getOption("CssForwardAll");
                        string res = cisco.AddLine(numero, partition, voicemailprofile, css, destinationerror, csserror, destinationfwdall, cssfwdall);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, numero + " " + partition + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + numero + " " + partition);
                        }
                    }
                }
            }

            // Créer CTI Route Point
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_CTI_ROUTEPOINT);
            TreeNode cti = treeCisco.Nodes[LBL_CTI_ROUTEPOINT];
            if (cti != null)
            {
                foreach (TreeNode node in cti.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = ini.Sections.Find(s => s.Type == SectionType.CTIRoutePoint && s.Name.Equals(name));
                        string description = section.getOption("Description");
                        string line = section.getOption("Line");
                        string css = section.getOption("CSS");
                        string partition = section.getOption("Partition");
                        string devicepool = section.getOption("DevicePool");
                        string location = section.getOption("Location");
                        string res = cisco.AddCtiRoutingPoint(name, description, devicepool, css, location, line, partition);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Cisco, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }


        }

        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void treeCisco_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Quand on coche ou décoche une case
            if (e.Action != TreeViewAction.Unknown)
            {
                // si il y a des fils
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                } else // si on était sur un fils
                {
                    // si on décoche alors on décoche le parent
                    if (!e.Node.Checked)
                    {
                        e.Node.Parent.Checked = false;
                    } else
                    {
                        // Si tous les autres fils sont cochés, alos on coche le parent
                        TreeNode parent = e.Node.Parent;
                        bool coche = true;
                        foreach(TreeNode fils in parent.Nodes)
                        {
                            if (!fils.Checked)
                                coche = false;
                        }
                        if (coche)
                            parent.Checked = true;
                    }
                }
            }
        }

        public void Trace(Target target, TraceLevel level, string text)
        {
            RichTextBox richText = null;

            switch (target)
            {
                case Target.Cisco:
                    richText = rtCiscoLog;
                    break;
                case Target.Genesys:
                    richText = null;
                    break;
            }


            richText.SelectionStart = richText.TextLength;
            richText.SelectionLength = 0;

            switch (level)
            {
                case TraceLevel.INFO:
                    richText.SelectionColor = Color.Green;
                    break;
                case TraceLevel.WARNING:
                    richText.SelectionColor = Color.Orange;
                    break;
                case TraceLevel.ERROR:
                    richText.SelectionColor = Color.Red;
                    break;
            }

            richText.AppendText(DateTime.Now.ToString("HH:mm"));
            richText.AppendText(" : ");
            richText.AppendText(text);
            richText.AppendText("\r\n");

            richText.SelectionStart = richText.Text.Length;
            richText.ScrollToCaret();
        }
    }
}
