using GreySMITH.Common.Utilities.General;

namespace GreySMITH.Revit.Extensions.Applications
{
    public enum TemplateDiscipline
    {
        [StringValue("G:/Shared/CADD/BIM/Templates/2014_BR+A ELEC Template.rvt")]
        Electrical = 1,
        [StringValueAttribute("G:/Shared/CADD/BIM/Templates/2014_BR+A MECH Template.rvt")]
        Mechanical = 2,
        [StringValueAttribute("G:/Shared/CADD/BIM/Templates/2014_BR+A PFP Template.rvt")]
        PlumbingFireProtection = 3,
        [StringValueAttribute("G:/Shared/CADD/BIM/Templates/2014_BR+A TECH Template.rvt")]
        Technology = 4,
        [StringValueAttribute("G:/Shared/CADD/BIM/Templates/2014_BR+A MEPFP Template.rvt")]
        All = 5
    }
}
