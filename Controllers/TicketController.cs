using Authorization_Authentication.Services;
using Authorization_Authentication.Services.BidirectionalChat;
using Authorization_Authentication.Services.ServicesModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System;
using System.Reflection;

namespace Authorization_Authentication.Controllers
{
    [Authorize(Roles ="RegularUser")]
    public class TicketController : Controller
    {
       
        private readonly ITicketManagementServices _ticketManager;
        private readonly IHubContext<AdminSideNotification> _chatContext;
        private readonly IWebHostEnvironment _env;

        public TicketController(ITicketManagementServices ticketManager, IHubContext<AdminSideNotification> chatContext, IWebHostEnvironment env)
        {
            this._ticketManager = ticketManager;
            this._chatContext = chatContext;
            this._env = env;
        }
        public async Task<IActionResult> Index()
        {
           string userId = HttpContext.Session.GetString("UserId");
            List<Tickets> TicketList = await _ticketManager.GetticketsList(userId);
            return View(TicketList);
        }

        //ticket creation
        [HttpGet]
        public async Task<IActionResult> IssueTicket()
        {
            var modules = await _ticketManager.GetModuleList();

            
            var moduleSelectList = modules.Select(m => new SelectListItem
            {
                Value = m.ModuleId.ToString(),
                Text = m.ModuleName
            }).ToList();

            var model = new ViewModelToCreateTicket
            {
                Modules = moduleSelectList
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IssueTicket(ViewModelToCreateTicket Createticket)
        {
                string userId = HttpContext.Session.GetString("UserId");
                
            var ticket = new Tickets
                {
                    Title = Createticket.Title,
                    Detail = Createticket.Detail,
                    Priority = Createticket.Priority,
                    ModuleId = Createticket.ModuleId,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Status = "Pending"
                };
            string ticketId = await  _ticketManager.issueticket(ticket);
            if(ticketId !=null)
                {
                string sender = "";
                if (User.IsInRole("RegularUser"))
                {
                     sender = "RegularUser";
                }
                else if (User.IsInRole("admin"))
                {
                     sender = "admin";
                }
                if (sender != null)
                {
                    _ticketManager.AddToTicketReplies(sender, Createticket.Detail, ticketId);
                    HttpContext.Session.SetString("ticketId", ticketId);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
                }
                else
                {
                return View();
                }
        }

        public async Task<IActionResult> TicketDetail(string TicketId)
        {
           Tickets ticketDetail = await _ticketManager.getTicketDetail(TicketId);
            ViewData["TicketId"] = TicketId;
            return View(ticketDetail);
        }

        public async Task<IActionResult> Chat(string ticketId)
        {
            bool AbleTOSendMsg = true;
            string status = await _ticketManager.getStatusFromTicketId(ticketId);
            if (status == "Closed" || status == "Resolved")
            {
                AbleTOSendMsg = false;
            }
            ViewData["AbleTOSendMsg"] = AbleTOSendMsg;
            var chatMessages = await _ticketManager.GetChatMessagesAsync(ticketId);
            ViewData["TicketId"] = ticketId;
            return View(chatMessages);
            
        }

        public async Task<IActionResult> Reply(string ticketId ,string message, IFormFile Image)
        {
            bool replyAdded = false;
            if (message != null || Image != null)
            {
                
                if (User.IsInRole("RegularUser"))
                {

                    string imageUrl = null;
                    if (Image != null && Image.Length > 0)
                    {
                        var fileExt = Path.GetExtension(Image.FileName).ToLowerInvariant();
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                        if (!allowedExtensions.Contains(fileExt))
                        {
                            return BadRequest("Invalid image format. Only .jpg and .png formats are allowed.");
                        }
                        string sender = "RegularUser";
                        var uploadPath = Path.Combine(_env.WebRootPath, "ChatImages");
                        Directory.CreateDirectory(uploadPath);
                        string imgMsg = "This is an image";

                        var fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                        var fileExtension = Path.GetExtension(Image.FileName);
                        var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadPath, newFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        imageUrl = $"/ChatImages/{newFileName}";

                        replyAdded = await _ticketManager.UploadChatImage(ticketId, sender, imgMsg, imageUrl);
                         await _chatContext.Clients.All.SendAsync("ReceiveMessageFromUser", sender, message);
                    }

                    else
                    {

                        string sender = "RegularUser";
                        replyAdded = await _ticketManager.AddReply(sender, ticketId, message);
                        await _chatContext.Clients.All.SendAsync("ReceiveMessageFromUser", sender, message);
                    }

                }
            }
            return RedirectToAction("Chat", new { ticketId = ticketId });
        }

        [HttpGet]
        public async Task<IActionResult> SendMessage(string message, string sender, string ticketId)
        {
            bool replyAdded = false;
            try
            {

                
                /*replyAdded = await _ticketManager.AddReply(sender, ticketId, message);*/
                await _chatContext.Clients.All.SendAsync("ReceiveMessageFromUser", sender, message);
                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
              
        }
    }
}
