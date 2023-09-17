using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Kutyak
{
    public class Dog
    {
        // Properties
        public int ID { get; }
        public int SpeciesID { get; }
        public int NameID { get; }
        public int Age { get; }
        public DateTime LastMedicalCheck { get; }

        public Dog(int iD, int speciesID, int nameID, int age, DateTime lastMedicalCheck)
        {
            ID = iD;
            SpeciesID = speciesID;
            NameID = nameID;
            Age = age;
            LastMedicalCheck = lastMedicalCheck;
        }
        // Debuggolásra
        public override string ToString()
        {
            return $"ID :{this.ID}, FajtaID: {this.SpeciesID}, " +
                $"NévID: {this.NameID}, Életkor: {this.Age}, Ellenörzés: {this.LastMedicalCheck}";
        }
    }
}
