using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Authorization_Authentication.Services.ServicesModel
{
    public class ViewModelToCreateTicket
    {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Detail { get; set; }

            [Required]
            public string Priority { get; set; } 

            [Required]
            public int ModuleId { get; set; }

            public List<SelectListItem> Modules { get; set; } 
        

    }
}
