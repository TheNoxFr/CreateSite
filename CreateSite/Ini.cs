using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSite
{
    public class Ini
    {
        private IniType iniType { get; set; }
        public List<Section> Sections { get; set; }

        public Ini(IniType iniType)
        {
            Sections = new List<Section>();
            this.iniType = iniType;
        }
    }

    public enum IniType { Cisco, Genesys };

    public enum SectionType { Config, Folder, AccessGroup, AgentGroup, VirtualQueue, GroupOfQueue, Place, TransactionList, Person, RoutingPoint, ERP, Skill, CCPulse, Host, CSProxy, Options, Erreur, PhysicalLocation, CallingSearchSpace }; 

    public class Section
    {
        public string Name { get; set; }
        public SectionType Type { get; set; }

        public List<CleValeur> Options { get; set; }

        public Section(string name)
        {
            // Initialisation par défaut
            Name = name;
            Type = SectionType.Erreur;
            Options = new List<CleValeur>();

            if (Name.Equals("Config"))
            {
                Type = SectionType.Config;
                return;
            }

            // Section Cisco
            if (Name.Equals("PHYSICAL_LOCATION"))
            {
                Type = SectionType.PhysicalLocation;
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("CSS")))
            {
                Type = SectionType.CallingSearchSpace;
                Name = name.Substring(4, name.Length - 4);
                return;
            }

            // Section Genesys

            if (Name.Equals("Folder"))
            {
                Type = SectionType.Folder;
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("AG")))
            {
                Type = SectionType.AccessGroup;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("VAG")))
            {
                Type = SectionType.AgentGroup;
                Name = name.Substring(4, name.Length - 4);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("VQ")))
            {
                Type = SectionType.VirtualQueue;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("SK")))
            {
                Type = SectionType.Skill;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("GQ")))
            {
                Type = SectionType.GroupOfQueue;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("PL")))
            {
                Type = SectionType.Place;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("TL")))
            {
                Type = SectionType.TransactionList;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("PE")))
            {
                Type = SectionType.Person;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 2) && (Name.Substring(0, 2).Equals("RP")))
            {
                Type = SectionType.RoutingPoint;
                Name = name.Substring(3, name.Length - 3);
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("ERP")))
            {
                Type = SectionType.ERP;
                Name = name.Substring(4, name.Length - 4);
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("CCP")))
            {
                Type = SectionType.CCPulse;
                Name = name.Substring(4, name.Length - 4);
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("CSP")))
            {
                Type = SectionType.CSProxy;
                Name = name.Substring(4, name.Length - 4);
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("HOS")))
            {
                Type = SectionType.Host;
                Name = name.Substring(4, name.Length - 4);
                return;
            }

            if ((Name.Length >= 3) && (Name.Substring(0, 3).Equals("OPT")))
            {
                Type = SectionType.Options;
                Name = name.Substring(4, name.Length - 4);
                return;
            }


        }
    }

    public class CleValeur
    {
        public string Cle { get; set; }
        public string Valeur { get; set; }

        public CleValeur(string cle, string valeur)
        {
            Cle = cle.Trim();
            Valeur = valeur.Trim();
        }

    }
}
