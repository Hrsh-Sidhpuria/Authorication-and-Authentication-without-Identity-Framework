using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Authorization_Authentication.Services.ServicesModel
{
    public class Tickets
    {
        
            public string TicketId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Detail { get; set; }
        [Required]
        public string Priority { get; set; }
        
        public string Status { get; set; }
        public string Reply { get; set; }
        [Required]
        public int ModuleId { get; set; }

            public Modules Module { get; set; }

            public string UserId { get; set; }
             public string UserName { get; set; }

              public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }

    }
}
