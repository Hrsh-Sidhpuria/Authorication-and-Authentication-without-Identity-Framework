using Authorization_Authentication.Services;
using Authorization_Authentication.Services.BidirectionalChat;
using Authorization_Authentication.Services.ServicesModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Authorization_Authentication.Controllers
{
    public class AdminTicketController : Controller
    {
        private readonly ITicketManagementServices _ticketManager;
        private readonly IHubContext<ClientSideNotification> _chatContext;

        public AdminTicketController(ITicketManagementServices ticketManager, IHubContext<ClientSideNotification> chatContext)
        {
            this._ticketManager = ticketManager;
            this._chatContext = chatContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AdminListTickets(string status, int? moduleId)
        {

            var tickets = await _ticketManager.GetAllTickets();


            if (!string.IsNullOrEmpty(status))
            {
                tickets = tickets.Where(t => t.Status == status).ToList();
            }

            // Filter by module if selected
            if (moduleId.HasValue)
            {
                tickets = tickets.Where(t => t.ModuleId == moduleId.Value).ToList();
            }

            var statusList = new List<string> { "Pending", "Resolved", "Closed" };
            var statusSelectList = statusList.Select(s => new SelectListItem
            {
                Value = s,
                Text = s
            }).ToList();

            // Get the list of modules
            var modules = await _ticketManager.GetModuleList();
            var moduleSelectList = modules.Select(m => new SelectListItem
            {
                Value = m.ModuleId.ToString(),
                Text = m.ModuleName
            }).ToList();

            // Create the ViewModel
            var viewModel = new TicketsViewModelAdmin
            {

                Tickets = tickets,
                Statuses = statusSelectList,
                Modules = moduleSelectList,
                SelectedStatus = status,
                SelectedModuleId = moduleId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AdminChat(string ticketId)
        {

            bool AbleTOSendMsg = true;
            string UserId = _ticketManager.GetIdfromTicketId(ticketId);
            string status = await _ticketManager.getStatusFromTicketId(ticketId);
            if (status == "Closed" || status == "Resolved")
            {
                AbleTOSendMsg = false;
            }

            ViewData["AbleTOSendMsg"] = AbleTOSendMsg;

            var chatMessages = await _ticketManager.GetChatMessagesAsync(ticketId);
            ViewData["TicketId"] = ticketId;
            ViewData["UserId"] = UserId;
            return View(chatMessages);



        }

        /*public async Task<IActionResult> Reply(string ticketId, string message)
        {
            bool replyAdded = false;
            if (User.IsInRole("RegularUser"))
            {
                replyAdded = await _ticketManager.AddReply("RegularUser", ticketId, message);
            }
            else if (User.IsInRole("admin"))
            {
                string sender = "admin";
                await _ticketManager.UpdateStatus("InProgress", ticketId);
                replyAdded = await _ticketManager.AddReply(sender, ticketId, message);
                await _chatContext.Clients.All.SendAsync("ReceiveMessageFromAdmin", sender, message);
            }
            if (replyAdded)
            {
                return RedirectToAction("AdminChat", new { ticketId = ticketId });
            }
            else
            {
                return View();
            }*/

        [HttpGet]
        public async Task<IActionResult> SendMessage(string message, string sender, string ticketId)
        {
            bool replyAdded = false;
            try
            {


                /*replyAdded = await _ticketManager.AddReply(sender, ticketId, message);*/
                await _chatContext.Clients.All.SendAsync("ReceiveMessageFromAdmin", sender, message);

                return Json(new { success = true });
                
                
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        

        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string status, string ticketId)
        {
            try
            {
                bool isUpdated = false;


                if (ticketId != null)
                {
                     isUpdated =await _ticketManager.UpdateStatus(status, ticketId);
                }
                if (isUpdated)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}


