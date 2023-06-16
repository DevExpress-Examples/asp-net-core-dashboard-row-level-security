using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebDashboard.Models {
    public class LoginScreenModel {
        public string EmployeeId { get; set; }
        public IList<SelectListItem> Employees { get; set; }
    }
}
