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
        public Target cible;

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

        public const string LBL_FOLDER = "Folder";
        public const string LBL_PLACE = "Place";
        public const string LBL_SKILL = "Skill";
        public const string LBL_VQUEUE = "Virtual Queue";
        public const string LBL_GROUPOFQUEUE = "Group of Queues";
        public const string LBL_PERSON = "Person";
        public const string LBL_ACCESSGROUP = "Access Group";
        public const string LBL_ROUTINGPOINT = "Routing Point";
        public const string LBL_HOST = "Host";
        public const string LBL_CSPROXY = "CSProxy";
        public const string LBL_CCPULSE = "CCPulse";
        public const string LBL_TRANSACTIONLIST = "Transaction List";
        public const string LBL_STATOPTION = "StatServer Option";
        public const string LBL_IXNQUEUE = "Interaction Queue";
        public const string LBL_AGENTGROUP = "Agent Group";
        public const string LBL_ERP = "External Routing Point";

        public enum TraceLevel { INFO, WARNING, ERROR };
        public enum Target { Cisco, Genesys };

        public Main()
        {
            InitializeComponent();
        }

        private void BtnGO_Click(object sender, EventArgs e)
        {

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

            CiscoAjout();

            

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

            GenesysAjout();
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

        private void GenesysAjout()
        {
            try
            {
                lblCodeUGS.Text = config.getOption("Site");
                lblVille.Text = config.getOption("Nom");

            }
            catch (Exception)
            {
                // Les infos de base sont non présentes (à bloquer via une pré-vérif) ?
            }

            List<Section> liste;

            // Ajout paramètres config
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Config);
            foreach (CleValeur cv in liste[0].Options)
            {
                rtConfig.AppendText(cv.Cle + " : ");
                rtConfig.SelectionStart = rtConfig.TextLength;
                rtConfig.SelectionLength = 0;
                rtConfig.SelectionFont = new Font(rtConfig.Font, FontStyle.Bold);
                rtConfig.AppendText(cv.Valeur);
                rtConfig.AppendText("\r\n");
            }

            // Ajout Folder
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Folder);
            if (liste.Count > 0)
            {
                TreeNode fol = treeObjects.Nodes.Add(LBL_FOLDER, LBL_FOLDER);
                foreach (CleValeur cv in liste[0].Options)
                {
                    TreeNode t = fol.Nodes.Add(cv.Valeur);
                }
            }

            // Ajout Place
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Place);
            if (liste.Count > 0)
            {
                TreeNode pla = treeObjects.Nodes.Add(LBL_PLACE, LBL_PLACE);
                foreach (Section section in liste)
                {
                    TreeNode t = pla.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Skill
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Skill);
            if (liste.Count > 0)
            {
                TreeNode ski = treeObjects.Nodes.Add(LBL_SKILL, LBL_SKILL);
                foreach (Section section in liste)
                {
                    TreeNode t = ski.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Virtual Queue
            liste = ini.Sections.FindAll(s => s.Type == SectionType.VirtualQueue);
            if (liste.Count > 0)
            {
                TreeNode vq = treeObjects.Nodes.Add(LBL_VQUEUE, LBL_VQUEUE);
                foreach (Section section in liste)
                {
                    string folder = section.getOption("Folder");
                    string[] pathlist = folder.Split('\\');
                    if (pathlist.Length < 3)
                        Trace(Target.Genesys, TraceLevel.WARNING, "Virtual Queue : " + section.Name + " répertoire incorrect : " + folder);
                    else
                    {
                        string switchname = pathlist[2];
                        TreeNode t = vq.Nodes.Add(section.Name + " (" + switchname+ ")");
                        t.Tag = section;
                    }
                }
            }

            // Ajout Group of Queues
            liste = ini.Sections.FindAll(s => s.Type == SectionType.GroupOfQueue);
            if (liste.Count > 0)
            {
                TreeNode vq = treeObjects.Nodes.Add(LBL_GROUPOFQUEUE, LBL_GROUPOFQUEUE);
                foreach (Section section in liste)
                {
                    TreeNode t = vq.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Person
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Person);
            if (liste.Count > 0)
            {
                TreeNode pe = treeObjects.Nodes.Add(LBL_PERSON, LBL_PERSON);
                foreach (Section section in liste)
                {
                    TreeNode t = pe.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Access Group
            liste = ini.Sections.FindAll(s => s.Type == SectionType.AccessGroup);
            if (liste.Count > 0)
            {
                TreeNode ag = treeObjects.Nodes.Add(LBL_ACCESSGROUP, LBL_ACCESSGROUP);
                foreach (Section section in liste)
                {
                    TreeNode t = ag.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Routing Point
            liste = ini.Sections.FindAll(s => s.Type == SectionType.RoutingPoint);
            if (liste.Count > 0)
            {
                TreeNode rp = treeObjects.Nodes.Add(LBL_ROUTINGPOINT, LBL_ROUTINGPOINT);
                foreach (Section section in liste)
                {
                    string folder = section.getOption("Folder");
                    string[] pathlist = folder.Split('\\');
                    if (pathlist.Length < 3)
                        Trace(Target.Genesys, TraceLevel.WARNING, "Routing Point : " + section.Name + " répertoire incorrect : " + folder);
                    else
                    {
                        TreeNode t = rp.Nodes.Add(section.Name);
                        t.Tag = section;
                    }
                }
            }

            // Ajout Host
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Host);
            if (liste.Count > 0)
            {
                TreeNode hos = treeObjects.Nodes.Add(LBL_HOST, LBL_HOST);
                foreach (Section section in liste)
                {
                    TreeNode t = hos.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout CSProxy
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CSProxy);
            if (liste.Count > 0)
            {
                TreeNode csp = treeObjects.Nodes.Add(LBL_CSPROXY, LBL_CSPROXY);
                foreach (Section section in liste)
                {
                    TreeNode t = csp.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout CCPulse
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CCPulse);
            if (liste.Count > 0)
            {
                TreeNode ccp = treeObjects.Nodes.Add(LBL_CCPULSE, LBL_CCPULSE);
                foreach (Section section in liste)
                {
                    TreeNode t = ccp.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Transaction List
            liste = ini.Sections.FindAll(s => s.Type == SectionType.TransactionList);
            if (liste.Count > 0)
            {
                TreeNode tl = treeObjects.Nodes.Add(LBL_TRANSACTIONLIST, LBL_TRANSACTIONLIST);
                foreach (Section section in liste)
                {
                    TreeNode t = tl.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout StatServer Option
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Options);
            if (liste.Count > 0)
            {
                TreeNode sso = treeObjects.Nodes.Add(LBL_STATOPTION, LBL_STATOPTION);
                foreach (Section section in liste)
                {
                    TreeNode t = sso.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Interaction Queue
            liste = ini.Sections.FindAll(s => s.Type == SectionType.InteractionQueue);
            if (liste.Count > 0)
            {
                TreeNode iq = treeObjects.Nodes.Add(LBL_IXNQUEUE, LBL_IXNQUEUE);
                foreach (Section section in liste)
                {
                    TreeNode t = iq.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Agent Group
            liste = ini.Sections.FindAll(s => s.Type == SectionType.AgentGroup);
            if (liste.Count > 0)
            {
                TreeNode ag = treeObjects.Nodes.Add(LBL_AGENTGROUP, LBL_AGENTGROUP);
                foreach (Section section in liste)
                {
                    TreeNode t = ag.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout ERP
            liste = ini.Sections.FindAll(s => s.Type == SectionType.ERP);
            if (liste.Count > 0)
            {
                TreeNode erp = treeObjects.Nodes.Add(LBL_ERP, LBL_ERP);
                foreach (Section section in liste)
                {
                    TreeNode t = erp.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }



            treeObjects.ExpandAll();

            cible = Target.Genesys;

        }

        private void GenesysVerifExiste()
        {
            genesys = new Genesys("genserv", 2020, txtGenesysLogin.Text, txtGenesysPassword.Text);
            genesys.Init();

            Trace(Target.Genesys, TraceLevel.INFO, "------------------------------");
            Trace(Target.Genesys, TraceLevel.INFO, "Début Vérification.");

            // Vérif Folder
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_FOLDER);
            TreeNode fol = treeObjects.Nodes[LBL_FOLDER];
            if (fol != null)
            {
                foreach (TreeNode node in fol.Nodes)
                {
                    string name = node.Text;
                    CfgFolder fo = genesys.GetFolder(name);
                    if (fo != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }


            // Vérif Place
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_PLACE);
            TreeNode pla = treeObjects.Nodes[LBL_PLACE];
            if (pla != null)
            {
                foreach (TreeNode node in pla.Nodes)
                {
                    string name = node.Text;
                    CfgPlace pl = genesys.GetPlace(name);
                    if (pl != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Skill
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_SKILL);
            TreeNode ski = treeObjects.Nodes[LBL_SKILL];
            if (ski != null)
            {
                foreach (TreeNode node in ski.Nodes)
                {
                    string name = node.Text;
                    CfgSkill sk = genesys.GetSkill(name);
                    if (sk != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Virtual Queue
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_VQUEUE);
            TreeNode vq = treeObjects.Nodes[LBL_VQUEUE];
            if (vq != null)
            {
                foreach (TreeNode node in vq.Nodes)
                {
                    Section section = (Section)node.Tag;
                    string[] pathlist = section.getOption("Folder").Split('\\');
                    string switchname = pathlist[2];
                    CfgDN v = genesys.GetDN(section.Name, switchname);
                    if (v != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + section.Name + " dans " + switchname);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Group of Queues  (attenton c'est normal que certaines existent)
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_GROUPOFQUEUE);
            TreeNode goq = treeObjects.Nodes[LBL_GROUPOFQUEUE];
            if (goq != null)
            {
                foreach (TreeNode node in goq.Nodes)
                {
                    Section section = (Section)node.Tag;
                    CfgDNGroup g = genesys.GetDNGroup(section.Name);
                    if (g != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + section.Name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Person
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_PERSON);
            TreeNode per = treeObjects.Nodes[LBL_PERSON];
            if (per != null)
            {
                foreach (TreeNode node in per.Nodes)
                {
                    Section section = (Section)node.Tag;
                    CfgPerson pe = genesys.GetPerson(section.Name);
                    if (pe != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + section.Name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Access Group
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_ACCESSGROUP);
            TreeNode ag = treeObjects.Nodes[LBL_ACCESSGROUP];
            if (ag != null)
            {
                foreach (TreeNode node in ag.Nodes)
                {
                    Section section = (Section)node.Tag;
                    CfgAccessGroup a = genesys.GetAccessGroup(section.Name);
                    if (a != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + section.Name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Routing Point
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_ROUTINGPOINT);
            TreeNode rp = treeObjects.Nodes[LBL_ROUTINGPOINT];
            if (rp != null)
            {
                foreach (TreeNode node in rp.Nodes)
                {
                    Section section = (Section)node.Tag;
                    string[] pathlist = section.getOption("Folder").Split('\\');
                    string switchname = pathlist[2];
                    CfgDN r = genesys.GetDN(section.Name, switchname);
                    if (r != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + section.Name + " dans " + switchname);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Host
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_HOST);
            TreeNode hos = treeObjects.Nodes[LBL_HOST];
            if (hos != null)
            {
                foreach (TreeNode node in hos.Nodes)
                {
                    string name = node.Text;
                    CfgHost ho = genesys.GetHost(name);
                    if (ho != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif CSProxy
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_CSPROXY);
            TreeNode csp = treeObjects.Nodes[LBL_CSPROXY];
            if (csp != null)
            {
                foreach (TreeNode node in csp.Nodes)
                {
                    string name = node.Text;
                    CfgApplication cs = genesys.GetApplication(name);
                    if (cs != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif CCPulse
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_CCPULSE);
            TreeNode ccp = treeObjects.Nodes[LBL_CCPULSE];
            if (ccp != null)
            {
                foreach (TreeNode node in ccp.Nodes)
                {
                    string name = node.Text;
                    CfgApplication cc = genesys.GetApplication(name);
                    if (cc != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Tranaction List (attenton c'est normal que certaines existent)
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_TRANSACTIONLIST);
            TreeNode tl = treeObjects.Nodes[LBL_TRANSACTIONLIST];
            if (tl != null)
            {
                foreach (TreeNode node in tl.Nodes)
                {
                    string name = node.Text;
                    CfgTransaction t = genesys.GetTransactionList(name);
                    if (t != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif StatServer (!! Il doit exister !)
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_STATOPTION);
            TreeNode sso = treeObjects.Nodes[LBL_STATOPTION];
            if (sso != null)
            {
                foreach (TreeNode node in sso.Nodes)
                {
                    string name = node.Text;
                    CfgApplication t = genesys.GetApplication(name);
                    if (t == null) // n'existe pas
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Non Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Interaction Queue
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_IXNQUEUE);
            TreeNode iq = treeObjects.Nodes[LBL_IXNQUEUE];
            if (iq != null)
            {
                foreach (TreeNode node in iq.Nodes)
                {
                    string name = node.Text;
                    CfgScript i = genesys.GetInteractionQueue(name);
                    if (i != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Agent Group (normal que certains existent si on garde les droits via Access Group
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_AGENTGROUP);
            TreeNode vag = treeObjects.Nodes[LBL_AGENTGROUP];
            if (vag != null)
            {
                foreach (TreeNode node in vag.Nodes)
                {
                    string name = node.Text;
                    CfgAgentGroup v = genesys.GetAgentGroup(name);
                    if (v != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif ERP
            Trace(Target.Genesys, TraceLevel.INFO, "Vérification " + LBL_ERP);
            TreeNode erp = treeObjects.Nodes[LBL_ERP];
            if (erp != null)
            {
                foreach (TreeNode node in erp.Nodes)
                {
                    string name = node.Text;
                    Section section = (Section)node.Tag;
                    string[] pathlist = section.getOption("Folder").Split('\\');
                    string switchname = pathlist[2];
                    CfgDN e = genesys.GetDN(section.Name, switchname);
                    if (e != null) // existe déjà
                    {
                        Trace(Target.Genesys, TraceLevel.WARNING, "Existant : " + section.Name + " dans " + switchname);
                        node.ForeColor = Color.Red;
                    }
                }
            }


            BtnCreer.Enabled = true;
        }

        private void GenesysCreer()
        {
            Trace(Target.Genesys, TraceLevel.INFO, "------------------------------");
            Trace(Target.Genesys, TraceLevel.INFO, "Début Création.");

            // Crée Folder
            TreeNode fol = treeObjects.Nodes[LBL_FOLDER];
            if (IsChildChecked(fol))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_FOLDER);
            if (fol != null)
            {
                foreach (TreeNode node in fol.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        string res = genesys.AddFolder(name);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée Place
            TreeNode pla = treeObjects.Nodes[LBL_PLACE];
            if (IsChildChecked(pla))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_PLACE);
            if (pla != null)
            {
                foreach (TreeNode node in pla.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        string res = genesys.AddPlace(name, section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée Skill
            TreeNode ski = treeObjects.Nodes[LBL_SKILL];
            if (IsChildChecked(ski))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_SKILL);
            if (ski != null)
            {
                foreach (TreeNode node in ski.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        string res = genesys.AddSkill(name, section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }


            // Crée Virtual Queue
            TreeNode vq = treeObjects.Nodes[LBL_VQUEUE];
            if (IsChildChecked(vq))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_VQUEUE);
            if (vq != null)
            {
                foreach (TreeNode node in vq.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string res = genesys.AddVirutalQueue(section.Name, section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                        }
                    }
                }
            }

            // Crée Group of Queues
            TreeNode goq = treeObjects.Nodes[LBL_GROUPOFQUEUE];
            if (IsChildChecked(goq))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_GROUPOFQUEUE);
            if (goq != null)
            {
                foreach (TreeNode node in goq.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string folder = section.getOption("Folder");
                        // Si on a un répertoire : on crée le DN Group (sinon ça peut être juste une mise à jour de ses queues)
                        if (!folder.Equals(""))
                        {
                            string res = genesys.AddDNGroup(section.Name, folder);
                            if (!res.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                            }
                        }

                        // On ajout les queues à l'intérieur
                        string[] queues = section.Options.Find(o => o.Cle.Equals("VQ")).Valeur.Split(';');
                        foreach (string queue in queues)
                        {
                            string[] qlong = queue.Split('_');
                            string sw = qlong[qlong.Length - 1];

                            string[] qshort = new string[qlong.Length - 1];
                            Array.Copy(qlong, qshort, qlong.Length - 1);

                            string res = genesys.AddVirtualQueueToDNGroup(string.Join("_", qshort), sw, section.Name);
                            if (!res.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "DN "+ queue + " ajouté à " + section.Name);
                            }
                        }
                    }
                }
            }

            // Crée Person
            TreeNode pe = treeObjects.Nodes[LBL_PERSON];
            if (IsChildChecked(pe))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_PERSON);
            if (pe != null)
            {
                foreach (TreeNode node in pe.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string res = genesys.AddPerson(section.Name, section.getOption("EmployeeId"), section.getOption("FirstName"), section.getOption("LastName"), section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                        }
                    }
                }
            }

            // Crée Access Group
            TreeNode ag = treeObjects.Nodes[LBL_ACCESSGROUP];
            if (IsChildChecked(ag))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_ACCESSGROUP);
            if (ag != null)
            {
                foreach (TreeNode node in ag.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string res = genesys.AddAccessGroup(section.Name, section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                        }

                        // On ajoute la personne dans le groupe 
                        string username = section.getOption("Person");
                        if (!username.Equals(""))
                        {
                            string res2 = genesys.AddPersonToAccessGroup(username, section.Name);
                            if (!res2.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res2);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Person : " + username + " ajouté à " + section.Name);
                            }
                        }
                    }
                }
            }


            // Crée Routing Point
            TreeNode rp = treeObjects.Nodes[LBL_ROUTINGPOINT];
            if (IsChildChecked(rp))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_ROUTINGPOINT);
            if (rp != null)
            {
                foreach (TreeNode node in rp.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        KeyValueCollection kv = new KeyValueCollection();
                        genesys.AddValue2KVList(ref kv, "__ROUTER__", "Categorie_Type", section.getOption("Categorie_Type"));
                        genesys.AddValue2KVList(ref kv, "__ROUTER__", "Metier", section.getOption("Metier"));
                        genesys.AddValue2KVList(ref kv, "__ROUTER__", "Perimetre", section.getOption("Perimetre"));
                        genesys.AddValue2KVList(ref kv, "__ROUTER__", "Zone", section.getOption("Zone"));
                        genesys.AddValue2KVList(ref kv, "TServer", "smloc", section.getOption("smloc"));

                        string res = genesys.AddRoutingPoint(section.Name, section.getOption("Alias"), kv, section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                        }

                        // Si il y a un defaut
                        string defaultrp = section.getOption("Default");
                        if (!defaultrp.Equals(""))
                        {
                            string res2 = genesys.AddDefaultToRoutingPoint(section.Name, defaultrp, section.getOption("Folder"));
                            if (!res2.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, defaultrp + " " + res);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Default ajouté : " + defaultrp);
                            }
                        }

                    }
                }
            }


            // Crée Host
            TreeNode hos = treeObjects.Nodes[LBL_HOST];
            if (IsChildChecked(hos))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_HOST);
            if (hos != null)
            {
                foreach (TreeNode node in hos.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        string res = genesys.AddHost(name, section.getOption("IPAddress"), section.getOption("SCS"), section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée CSProxy
            TreeNode csp = treeObjects.Nodes[LBL_CSPROXY];
            if (IsChildChecked(csp))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_CSPROXY);
            if (csp != null)
            {
                foreach (TreeNode node in csp.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        string res = genesys.AddApplicationCSProxy(name, section.getOption("Template"), section.getOption("Host"), section.getOption("User"), section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée CCPulse
            TreeNode ccp = treeObjects.Nodes[LBL_CCPULSE];
            if (IsChildChecked(ccp))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_CCPULSE);
            if (ccp != null)
            {
                foreach (TreeNode node in ccp.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        string res = genesys.AddApplicationCCPulse(name, section.getOption("Template"), section.getOption("Storage"), section.getOption("StatServer"), section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée Transaction List
            TreeNode tl = treeObjects.Nodes[LBL_TRANSACTIONLIST];
            if (IsChildChecked(tl))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_TRANSACTIONLIST);
            if (tl != null)
            {
                foreach (TreeNode node in tl.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        KeyValueCollection kv = new KeyValueCollection();
                        foreach (CleValeur clevaleur in section.Options)
                        {
                            if (!clevaleur.Cle.Equals("Folder"))
                            {
                                string[] opt = clevaleur.Valeur.Split('=');   
                                string valeur = clevaleur.Valeur.Substring(opt[0].Length + 1);
                                genesys.AddValue2KVList(ref kv, clevaleur.Cle, opt[0], valeur);
                            }
                        }

                        string folder = section.getOption("Folder");

                        // On créer une TL s'il y a un répertoire
                        if (!folder.Equals(""))
                        {
                            string res = genesys.AddTransactionList(name, kv, folder);
                            if (!res.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + name);
                            }
                        } else // On met à jour une TL existante
                        {
                            string res = genesys.UpdateTransactionList(name, kv);
                            if (!res.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Mis à jour : " + name);
                            }
                        }
                    }
                }
            }

            // Crée StatServer Option
            TreeNode sso = treeObjects.Nodes[LBL_STATOPTION];
            if (IsChildChecked(sso))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_STATOPTION);
            if (sso != null)
            {
                foreach (TreeNode node in sso.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        KeyValueCollection kv = new KeyValueCollection();
                        foreach (CleValeur clevaleur in section.Options)
                        {
                            string[] opt = clevaleur.Valeur.Split('=');   
                            string valeur = clevaleur.Valeur.Substring(opt[0].Length + 1);
                            genesys.AddValue2KVList(ref kv, clevaleur.Cle, opt[0], valeur);
                        }

                        string res = genesys.AddOptionStatServer(name, kv);
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Mis à jour : " + name);
                        }
                    }
                }
            }

            // Crée Interaction Queue
            TreeNode iq = treeObjects.Nodes[LBL_IXNQUEUE];
            if (IsChildChecked(iq))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_IXNQUEUE);
            if (iq != null)
            {
                foreach (TreeNode node in iq.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string res = genesys.AddInteractionQueue(section.Name, section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                        }
                    }
                }
            }

            // Crée Agent Group
            TreeNode vag = treeObjects.Nodes[LBL_AGENTGROUP];
            if (IsChildChecked(vag))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_AGENTGROUP);
            if (vag != null)
            {
                foreach (TreeNode node in vag.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string folder = section.getOption("Folder");
                        if (!folder.Equals(""))
                        {
                            string res = genesys.AddAgentGroup(section.Name, section.getOption("Script"), folder);
                            if (!res.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                            }
                        }

                        string[] acgs = section.getOption("NoAccess").Split(';');
                        foreach (string acg in acgs)
                        {
                            string res2 = genesys.AddAccessGroupRightsToAgentGroup(section.Name, acg);
                            if (!res2.Equals(""))
                            {
                                Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res2);
                                node.BackColor = Color.Blue;
                            }
                            else
                            {
                                Trace(Target.Genesys, TraceLevel.INFO, "Mis à jour : " + section.Name);
                            }
                        }

                    }
                }
            }

            // Crée External Routing Point
            TreeNode erp = treeObjects.Nodes[LBL_ERP];
            if (IsChildChecked(erp))
                Trace(Target.Genesys, TraceLevel.INFO, "Création " + LBL_ERP);
            if (erp != null)
            {
                foreach (TreeNode node in erp.Nodes)
                {
                    if (node.Checked)
                    {
                        Section section = (Section)node.Tag;
                        string res = genesys.AddExternalRoutingPoint(section.Name, section.getOption("Alias"), section.getOption("Epn"), section.getOption("Association"), section.getOption("Folder"));
                        if (!res.Equals(""))
                        {
                            Trace(Target.Genesys, TraceLevel.ERROR, section.Name + " " + res);
                            node.BackColor = Color.Blue;
                        }
                        else
                        {
                            Trace(Target.Genesys, TraceLevel.INFO, "Créé : " + section.Name);
                        }
                    }
                }
            }

        }

        /*
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                        folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                        folderid = genesys.GetFolder(folder).DBID;
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
                    folder = sect.getOption("Folder");
                    folderid = genesys.GetFolder(folder).DBID;
                    string res = genesys.AddPlace(sect.Name, folderid);
                    if (!res.Equals(""))
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                        folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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
                    folderid = genesys.GetFolder(folder).DBID;
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

        */

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (genesys != null)
                genesys.Disconnect();
        }

        /*
        private void BtnCisco_Click(object sender, EventArgs e)
        {
            TraiteFichierCisco();

            try
            {
                lblCiscoCodeUGS.Text = config.getOption("Site");
                lblCiscoVille.Text = config.getOption("Nom");

            }
            catch (Exception)
            {
                // Les infos de base sont non présentes (à bloquer via une pré-vérif) ?
            }

            List<Section> liste;

            // Ajout paramètres config
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Config);
            foreach(CleValeur cv in liste[0].Options)
            {
                rtCiscoConfig.AppendText(cv.Cle + " : ");
                rtCiscoConfig.SelectionStart = rtCiscoConfig.TextLength;
                rtCiscoConfig.SelectionLength = 0;
                rtCiscoConfig.SelectionFont = new Font(rtCiscoConfig.Font, FontStyle.Bold);
                rtCiscoConfig.AppendText(cv.Valeur);
                rtCiscoConfig.AppendText("\r\n");
            }

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


*/

        private void CiscoAjout()
        {
            try
            {
                lblCodeUGS.Text = config.getOption("Site");
                lblVille.Text = config.getOption("Nom");

            }
            catch (Exception)
            {
                // Les infos de base sont non présentes (à bloquer via une pré-vérif) ?
            }

            List<Section> liste;

            // Ajout paramètres config
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Config);
            foreach (CleValeur cv in liste[0].Options)
            {
                rtConfig.AppendText(cv.Cle + " : ");
                rtConfig.SelectionStart = rtConfig.TextLength;
                rtConfig.SelectionLength = 0;
                rtConfig.SelectionFont = new Font(rtConfig.Font, FontStyle.Bold);
                rtConfig.AppendText(cv.Valeur);
                rtConfig.AppendText("\r\n");
            }

            // Ajout Physical Location
            liste = ini.Sections.FindAll(s => s.Type == SectionType.PhysicalLocation);
            if (liste.Count > 0)
            {
                TreeNode phyloc = treeObjects.Nodes.Add(LBL_PHYSICAL_LOCATION, LBL_PHYSICAL_LOCATION);
                foreach (Section section in liste)
                {
                    TreeNode t = phyloc.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Location
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Location);
            if (liste.Count > 0)
            {
                TreeNode loc = treeObjects.Nodes.Add(LBL_LOCATION, LBL_LOCATION);
                foreach (Section section in liste)
                {
                    TreeNode t = loc.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout CSS
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CallingSearchSpace);
            if (liste.Count > 0)
            {
                TreeNode css = treeObjects.Nodes.Add(LBL_CSS, LBL_CSS);
                foreach (Section section in liste) // Une section par CSS
                {
                    TreeNode t = css.Nodes.Add(section.Name); // Le nom est dans la section
                    t.Tag = section;
                }
            }

            // Ajout Geolocation
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Geolocation);
            if (liste.Count > 0)
            {
                TreeNode css = treeObjects.Nodes.Add(LBL_GEOLOC, LBL_GEOLOC);
                foreach (Section section in liste) // Une section par Geoloc
                {
                    TreeNode t = css.Nodes.Add(section.Name); // Le nom est dans la section
                    t.Tag = section;
                }
            }

            // Ajout Line Group
            liste = ini.Sections.FindAll(s => s.Type == SectionType.LineGroup);
            if (liste.Count > 0)
            {
                TreeNode lg = treeObjects.Nodes.Add(LBL_LINEGROUP, LBL_LINEGROUP);
                foreach (Section section in liste) // Une section par Line Group
                {
                    TreeNode t = lg.Nodes.Add(section.Name); // Le nom est dans la section
                    t.Tag = section;
                }
            }

            // Ajout Hunt List
            liste = ini.Sections.FindAll(s => s.Type == SectionType.HuntList);
            if (liste.Count > 0)
            {
                TreeNode hl = treeObjects.Nodes.Add(LBL_HUNT_LIST, LBL_HUNT_LIST);
                foreach (Section section in liste) // Une section par Line Group
                {
                    TreeNode t = hl.Nodes.Add(section.Name); // Le nom est dans la section
                    t.Tag = section;
                }
            }

            // Ajout Hunt Pilot
            liste = ini.Sections.FindAll(s => s.Type == SectionType.HuntPilot);
            if (liste.Count > 0)
            {
                TreeNode hp = treeObjects.Nodes.Add(LBL_HUNT_PILOT, LBL_HUNT_PILOT);
                foreach (Section section in liste)
                {
                    TreeNode t = hp.Nodes.Add(section.Name); // Le numéro est dans la section
                    t.Tag = section;
                }
            }

            // Ajout Translation Pattern
            liste = ini.Sections.FindAll(s => s.Type == SectionType.TranslationPattern);
            if (liste.Count > 0)
            {
                TreeNode tp = treeObjects.Nodes.Add(LBL_TRANSLATION_PATTERN, LBL_TRANSLATION_PATTERN);
                foreach (Section section in liste)
                {
                    TreeNode t = tp.Nodes.Add(section.Name); // Le numéro est dans la section
                    t.Tag = section;
                }
            }

            // Ajout Calling Party Tranformation Pattern
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CallingPartyTransformationPattern);
            if (liste.Count > 0)
            {
                TreeNode cg = treeObjects.Nodes.Add(LBL_CALG_PARTY_TFP, LBL_CALG_PARTY_TFP);
                foreach (Section section in liste)
                {
                    TreeNode t = cg.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Called Party Tranformation Pattern
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CalledPartyTransformationPattern);
            if (liste.Count > 0)
            {
                TreeNode cd = treeObjects.Nodes.Add(LBL_CALD_PARTY_TFP, LBL_CALD_PARTY_TFP);
                foreach (Section section in liste)
                {
                    TreeNode t = cd.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Device Mobility Info
            liste = ini.Sections.FindAll(s => s.Type == SectionType.DeviceMobilityInfo);
            if (liste.Count > 0)
            {
                TreeNode dm = treeObjects.Nodes.Add(LBL_DEVICE_MOBILITY, LBL_DEVICE_MOBILITY);
                foreach (Section section in liste)
                {
                    TreeNode t = dm.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Region
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Region);
            if (liste.Count > 0)
            {
                TreeNode re = treeObjects.Nodes.Add(LBL_REGION, LBL_REGION);
                foreach (Section section in liste)
                {
                    TreeNode t = re.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Device Pool
            liste = ini.Sections.FindAll(s => s.Type == SectionType.DevicePool);
            if (liste.Count > 0)
            {
                TreeNode dp = treeObjects.Nodes.Add(LBL_DEVICE_POOL, LBL_DEVICE_POOL);
                foreach (Section section in liste)
                {
                    TreeNode t = dp.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout Line
            liste = ini.Sections.FindAll(s => s.Type == SectionType.Line);
            if (liste.Count > 0)
            {
                TreeNode li = treeObjects.Nodes.Add(LBL_LINE, LBL_LINE);
                foreach (Section section in liste)
                {
                    TreeNode t = li.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }

            // Ajout CTI RoutePoint
            liste = ini.Sections.FindAll(s => s.Type == SectionType.CTIRoutePoint);
            if (liste.Count > 0)
            {
                TreeNode cti = treeObjects.Nodes.Add(LBL_CTI_ROUTEPOINT, LBL_CTI_ROUTEPOINT);
                foreach (Section section in liste)
                {
                    TreeNode t = cti.Nodes.Add(section.Name);
                    t.Tag = section;
                }
            }


            treeObjects.ExpandAll();

            cible = Target.Cisco;

        }

        private void CiscoVerifExiste()
        {
            cisco = new Cisco("https://192.168.1.46:8443/axl/", "ccmadministrator", "BcH1Kf0T");
            cisco.Init();

            Trace(Target.Cisco, TraceLevel.INFO, "------------------------------");
            Trace(Target.Cisco, TraceLevel.INFO, "Début Vérification.");

            // Vérif Physical Location
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_PHYSICAL_LOCATION);
            TreeNode phyloc = treeObjects.Nodes[LBL_PHYSICAL_LOCATION];
            if (phyloc != null)
            {
                foreach (TreeNode node in phyloc.Nodes)
                {
                    string name = node.Text;
                    GetPhysicalLocationRes ph = cisco.GetPhysicalLocation(name);
                    if (ph != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Location
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_LOCATION);
            TreeNode loc = treeObjects.Nodes[LBL_LOCATION];
            if (loc != null)
            {
                foreach (TreeNode node in loc.Nodes)
                {
                    string name = node.Text;
                    GetLocationRes lo = cisco.GetLocation(name);
                    if (lo != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Geoloc
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_GEOLOC);
            TreeNode geo = treeObjects.Nodes[LBL_GEOLOC];
            if (geo != null)
            {
                foreach (TreeNode node in geo.Nodes)
                {
                    string name = node.Text;
                    GetGeoLocationRes ge = cisco.GetGeolocation(name);
                    if (ge != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Line Group
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_LINEGROUP);
            TreeNode lg = treeObjects.Nodes[LBL_LINEGROUP];
            if (lg != null)
            {
                foreach (TreeNode node in lg.Nodes)
                {
                    string name = node.Text;
                    GetLineGroupRes l = cisco.GetLineGroup(name);
                    if (l != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Hunt List
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_HUNT_LIST);
            TreeNode hl = treeObjects.Nodes[LBL_HUNT_LIST];
            if (hl != null)
            {
                foreach (TreeNode node in hl.Nodes)
                {
                    string name = node.Text;
                    GetHuntListRes h = cisco.GetHuntList(name);
                    if (h != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Hunt Pilot
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_HUNT_PILOT);
            TreeNode hp = treeObjects.Nodes[LBL_HUNT_PILOT];
            if (hp != null)
            {
                foreach (TreeNode node in hp.Nodes)
                {
                    string numero = node.Text;
                    GetHuntPilotRes h = cisco.GetHuntPilot(numero, "ALL_Interne_P");
                    if (h != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " ALL_Interne_P");
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Translation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_TRANSLATION_PATTERN);
            TreeNode tp = treeObjects.Nodes[LBL_TRANSLATION_PATTERN];
            if (tp != null)
            {
                foreach (TreeNode node in tp.Nodes)
                {
                    string numero = node.Text;
                    Section section = (Section)node.Tag;
                    string partition = section.getOption("Partition");
                    GetTransPatternRes t = cisco.GetTranslationPattern(numero, partition);
                    if (t != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Calling Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_CALG_PARTY_TFP);
            TreeNode cg = treeObjects.Nodes[LBL_CALG_PARTY_TFP];
            if (cg != null)
            {
                foreach (TreeNode node in cg.Nodes)
                {
                    string numero = node.Text;
                    Section section = (Section)node.Tag;
                    string partition = section.getOption("Partition");
                    GetCallingPartyTransformationPatternRes c = cisco.GetCallingPartyTransformationPattern(numero, partition);
                    if (c != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Called Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_CALD_PARTY_TFP);
            TreeNode cd = treeObjects.Nodes[LBL_CALD_PARTY_TFP];
            if (cd != null)
            {
                foreach (TreeNode node in cd.Nodes)
                {
                    string numero = node.Text;
                    Section section = (Section)node.Tag;
                    string partition = section.getOption("Partition");
                    GetCalledPartyTransformationPatternRes c = cisco.GetCalledPartyTransformationPattern(numero, partition);
                    if (c != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Device Mobility Info
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_DEVICE_MOBILITY);
            TreeNode dm = treeObjects.Nodes[LBL_DEVICE_MOBILITY];
            if (dm != null)
            {
                foreach (TreeNode node in dm.Nodes)
                {
                    string name = node.Text;
                    GetDeviceMobilityRes d = cisco.GetDeviceMobilityInfo(name);
                    if (d != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Region
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_REGION);
            TreeNode re = treeObjects.Nodes[LBL_REGION];
            if (re != null)
            {
                foreach (TreeNode node in re.Nodes)
                {
                    string name = node.Text;
                    GetRegionRes r = cisco.GetRegion(name);
                    if (r != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Device Pool
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_DEVICE_POOL);
            TreeNode dp = treeObjects.Nodes[LBL_DEVICE_POOL];
            if (dp != null)
            {
                foreach (TreeNode node in dp.Nodes)
                {
                    string name = node.Text;
                    GetDevicePoolRes d = cisco.GetDevicePool(name);
                    if (d != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif Line
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_LINE);
            TreeNode li = treeObjects.Nodes[LBL_LINE];
            if (li != null)
            {
                foreach (TreeNode node in li.Nodes)
                {
                    string numero = node.Text;
                    Section section = (Section)node.Tag;
                    string partition = section.getOption("Partition");
                    GetLineRes l = cisco.GetLine(numero, partition);
                    if (l != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + numero + " " + partition);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            // Vérif CTI Route Point
            Trace(Target.Cisco, TraceLevel.INFO, "Vérification " + LBL_CTI_ROUTEPOINT);
            TreeNode cti = treeObjects.Nodes[LBL_CTI_ROUTEPOINT];
            if (cti != null)
            {
                foreach (TreeNode node in cti.Nodes)
                {
                    string name = node.Text;
                    GetCtiRoutePointRes c = cisco.GetCtiRoutingPoint(name);
                    if (c != null) // existe déjà
                    {
                        Trace(Target.Cisco, TraceLevel.WARNING, "Existant : " + name);
                        node.ForeColor = Color.Red;
                    }
                }
            }

            BtnCreer.Enabled = true;


        }

        private void CiscoCreer()
        {
            Trace(Target.Cisco, TraceLevel.INFO, "------------------------------");
            Trace(Target.Cisco, TraceLevel.INFO, "Début Création.");

            // Crée Physical Location
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_PHYSICAL_LOCATION);
            TreeNode phyloc = treeObjects.Nodes[LBL_PHYSICAL_LOCATION];
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
                        }
                        else
                        {
                            Trace(Target.Cisco, TraceLevel.INFO, "Créé : " + name);
                        }
                    }
                }
            }

            // Crée Location
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_LOCATION);
            TreeNode loc = treeObjects.Nodes[LBL_LOCATION];
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
            TreeNode geo = treeObjects.Nodes[LBL_GEOLOC];
            if (geo != null)
            {
                foreach (TreeNode node in geo.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode lg = treeObjects.Nodes[LBL_LINEGROUP];
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
            TreeNode hl = treeObjects.Nodes[LBL_HUNT_LIST];
            if (hl != null)
            {
                foreach (TreeNode node in hl.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode hp = treeObjects.Nodes[LBL_HUNT_PILOT];
            if (hp != null)
            {
                foreach (TreeNode node in hp.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode tp = treeObjects.Nodes[LBL_TRANSLATION_PATTERN];
            if (tp != null)
            {
                foreach (TreeNode node in tp.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = (Section)node.Tag;
                        string description = section.getOption("Description");
                        string calledmask = section.getOption("CalledMask");
                        string callingmask = section.getOption("CallingMask");
                        string partition = section.getOption("Partition");
                        string css = section.getOption("CSS");
                        string res = cisco.AddTranslationPattern(numero, partition, description, css, calledmask, callingmask);
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

            // Créer Calling Party Transformation Pattern
            Trace(Target.Cisco, TraceLevel.INFO, "Création " + LBL_CALG_PARTY_TFP);
            TreeNode cg = treeObjects.Nodes[LBL_CALG_PARTY_TFP];
            if (cg != null)
            {
                foreach (TreeNode node in cg.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode cd = treeObjects.Nodes[LBL_CALD_PARTY_TFP];
            if (cd != null)
            {
                foreach (TreeNode node in cd.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode dm = treeObjects.Nodes[LBL_DEVICE_MOBILITY];
            if (dm != null)
            {
                foreach (TreeNode node in dm.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode re = treeObjects.Nodes[LBL_REGION];
            if (re != null)
            {
                foreach (TreeNode node in re.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
                        string codeclist = section.getOption("CodecList");
                        string audiobitrate = section.getOption("AudioBitRate");
                        string videobitrate = section.getOption("VideoBitRate");
                        string immersivevideobitrate = section.getOption("ImmersiveVideoBitRate");
                        string res = cisco.AddRegion(name, codeclist, audiobitrate, videobitrate, immersivevideobitrate);
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
            TreeNode dp = treeObjects.Nodes[LBL_DEVICE_POOL];
            if (dp != null)
            {
                foreach (TreeNode node in dp.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode li = treeObjects.Nodes[LBL_LINE];
            if (li != null)
            {
                foreach (TreeNode node in li.Nodes)
                {
                    if (node.Checked)
                    {
                        string numero = node.Text;
                        Section section = (Section)node.Tag;
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
            TreeNode cti = treeObjects.Nodes[LBL_CTI_ROUTEPOINT];
            if (cti != null)
            {
                foreach (TreeNode node in cti.Nodes)
                {
                    if (node.Checked)
                    {
                        string name = node.Text;
                        Section section = (Section)node.Tag;
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

        private bool IsChildChecked(TreeNode tree)
        {
            bool result = false;

            foreach(TreeNode fils in tree.Nodes)
            {
                if (fils.Checked)
                    result = true;
            }

            return result;
        }

        public void Trace(Target target, TraceLevel level, string text)
        {
            rtLog.SelectionStart = rtLog.TextLength;
            rtLog.SelectionLength = 0;

            switch (level)
            {
                case TraceLevel.INFO:
                    rtLog.SelectionColor = Color.Green;
                    break;
                case TraceLevel.WARNING:
                    rtLog.SelectionColor = Color.DarkOrange;
                    break;
                case TraceLevel.ERROR:
                    rtLog.SelectionColor = Color.Red;
                    break;
            }

            rtLog.AppendText(DateTime.Now.ToString("HH:mm"));
            rtLog.AppendText(" : ");
            rtLog.AppendText(text);
            rtLog.AppendText("\r\n");

            rtLog.SelectionStart = rtLog.Text.Length;
            rtLog.ScrollToCaret();
        }

        private void BtnCiscoFile_Click(object sender, EventArgs e)
        {
            TraiteFichierCisco();

        }

        private void BtnVerifExiste_Click(object sender, EventArgs e)
        {
            switch (cible)
            {
                case Target.Cisco:
                    CiscoVerifExiste();
                    break;
                case Target.Genesys:
                    GenesysVerifExiste();
                    break;
            }
        }

        private void BtnGenesysFile_Click(object sender, EventArgs e)
        {
            TraiteFichierGenesys();
        }

        private void treeObjects_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Quand on coche ou décoche une case
            if (e.Action != TreeViewAction.Unknown)
            {
                // si il y a des fils
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                else // si on était sur un fils
                {
                    // si on décoche alors on décoche le parent
                    if (!e.Node.Checked)
                    {
                        e.Node.Parent.Checked = false;
                    }
                    else
                    {
                        // Si tous les autres fils sont cochés, alos on coche le parent
                        TreeNode parent = e.Node.Parent;
                        bool coche = true;
                        foreach (TreeNode fils in parent.Nodes)
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

        private void BtnCreer_Click(object sender, EventArgs e)
        {
            switch (cible)
            {
                case Target.Cisco:
                    CiscoCreer();
                    break;
                case Target.Genesys:
                    GenesysCreer();
                    break;
            }

        }


    }
}
