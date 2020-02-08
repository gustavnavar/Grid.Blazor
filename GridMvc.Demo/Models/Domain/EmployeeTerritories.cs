using System.ComponentModel.DataAnnotations;

namespace GridMvc.Demo.Models
{
    public class EmployeeTerritories
    {
        [Display(Name = "Employee Id")]
        public int EmployeeID { get; set; }
        [Display(Name = "Employee")]
        public Employee Employee { get; set; }

        [Display(Name = "Territory Id")]
        public string TerritoryID { get; set; }
        [Display(Name = "Territory")]
        public Territory Territory { get; set; }
    }
}
