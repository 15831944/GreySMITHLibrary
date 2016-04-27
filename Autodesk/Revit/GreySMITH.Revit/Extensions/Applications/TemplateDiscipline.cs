using GreySMITH.Common.Extensions;

namespace GreySMITH.Revit.Commands.Extensions.Applications
{
    public enum TemplateDiscipline
    {
        [StringValue("G:/Shared/CADD/BIM/Templates/2014_BR+A ELEC Template.rvt")]
        Electrical = 1,
        [StringValue("G:/Shared/CADD/BIM/Templates/2014_BR+A MECH Template.rvt")]
        Mechanical = 2,
        [StringValue("G:/Shared/CADD/BIM/Templates/2014_BR+A PFP Template.rvt")]
        PlumbingFireProtection = 3,
        [StringValue("G:/Shared/CADD/BIM/Templates/2014_BR+A TECH Template.rvt")]
        Technology = 4,
        [StringValue("G:/Shared/CADD/BIM/Templates/2014_BR+A MEPFP Template.rvt")]
        All = 5
    }
}
