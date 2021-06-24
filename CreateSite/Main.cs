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
                lblCiscoCodeUGS.Text = config.Options.Find(cv => cv.Cle.Equals("Site")).Valeur;
                lblCiscoVille.Text = config.Options.Find(cv => cv.Cle.Equals("Nom")).Valeur;
                lblCiscoNumPrefixe.Text = config.Options.Find(cv => cv.Cle.Equals("PlanNum")).Valeur;

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
                TreeNode phyloc = treeCisco.Nodes.Add("Physical Location", "Physical Location");
                foreach (CleValeur clevaleur in liste[0].Options) // On considère une seule section dans tout le fichier
                {
                    phyloc.Nodes.Add(clevaleur.Valeur); // Une seule option : le nom
                }
            }

            // Ajout CSS
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CallingSearchSpace);
            if (liste.Count > 0)
            {
                TreeNode css = treeCisco.Nodes.Add("Calling Search Space", "Calling Search Space");
                foreach (Section section in liste) // Une section par CSS
                {
                    css.Nodes.Add(section.Name); // Le nom est dans la section
                }
            }

            tabControlGeneral.SelectTab("tabPageCisco");


        }


        private void BtnCiscoVerifExiste_Click(object sender, EventArgs e)
        {
            cisco = new Cisco("https://192.168.1.46:8443/axl/", "ccmadministrator", "BcH1Kf0T");
            cisco.Init();

            // Vérif Physical Location
            TreeNode phyloc = treeCisco.Nodes["Physical Location"];
            if (phyloc != null)
            {
                foreach(TreeNode node in phyloc.Nodes)
                {
                    string name = node.Text;
                    GetPhysicalLocationRes ph = cisco.GetPhysicalLocation(name);
                    if (ph != null) // existe déjà
                    {
                        node.BackColor = Color.Red;
                    }
                }
            }

            // Vérif CSS
            TreeNode css = treeCisco.Nodes["Calling Search Space"];
            if (css != null)
            {
                foreach (TreeNode node in css.Nodes)
                {
                    string name = node.Text;
                    GetCssRes cs = cisco.GetCSS(name);
                    if (cs != null) // existe déjà
                    {
                        node.BackColor = Color.Red;
                    }
                }
            }

        }

        private void BtnCiscoCreer_Click(object sender, EventArgs e)
        {
            // Crée Physical Location
            TreeNode phyloc = treeCisco.Nodes["Physical Location"];
            if (phyloc != null)
            {
                foreach (TreeNode node in phyloc.Nodes)
                {
                    string name = node.Text;
                    cisco.AddPhysicalLocation(name);
                }
            }


        }
    }
}
