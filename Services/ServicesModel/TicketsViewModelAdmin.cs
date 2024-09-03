using Microsoft.AspNetCore.Mvc.Rendering;

namespace Authorization_Authentication.Services.ServicesModel
{
    public class TicketsViewModelAdmin
    {
        public List<Tickets> Tickets { get; set; } 
        public List<SelectListItem> Statuses { get; set; } 
        public List<SelectListItem> Modules { get; set; } 
        public string SelectedStatus { get; set; } 
        public int? SelectedModuleId { get; set; }
    }
}
