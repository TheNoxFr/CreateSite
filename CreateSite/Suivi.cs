using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSite
{
    public enum Instant { Config, Verif, Creation, Modification };
    public enum Level { Information, Warning, Error};

    public class Evenement
    {
        public SectionType sectionType { get; set; }
        public Instant instant { get; set; }
        public Level level { get; set; }
        public string texte { get; set; }

        public Evenement() { }

        public Evenement(SectionType sec, Instant ins, Level lev, string tex)
        {
            sectionType = sec;
            instant = ins;
            level = lev;
            texte = tex;
        }

    }
    public class Suivi
    {
        public List<Evenement> listeEvenements;

        public Suivi()
        {
            listeEvenements = new List<Evenement>();
        }

        public void AddEvenement(SectionType sec, Instant ins, Level lev, string tex)
        {
            Evenement ev = new Evenement { sectionType = sec, instant = ins, level = lev, texte = tex };
            listeEvenements.Add(ev);
        }
    }
}
