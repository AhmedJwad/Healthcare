using HealthCare.Common.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthCare.API.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboBloodtypes();
        IEnumerable<SelectListItem> GetCities();
        IEnumerable<SelectListItem> GetNationalities();
        IEnumerable<SelectListItem> Getgendres();
        IEnumerable<SelectListItem> GetCombodiagnosic();
        IEnumerable<SelectListItem> GetUserPatients();
        IEnumerable<SelectListItem> GetUserTypes();

    }
}
