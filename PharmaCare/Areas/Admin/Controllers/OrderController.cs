using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaCare.Data;
using PharmaCare.Models;
using PharmaCare.Models.ViewModels;
using PharmaCare.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsVM OrderDetailsVm { get; private set; }

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole("Admin"))
            {
                orderHeaderList = _context.OrderHeaders.Include(o => o.ApplicationUser).ToList();
            }
            else
            {
                orderHeaderList = _context.OrderHeaders.Where(o => o.ApplicationUserId == claim.Value).Include(o => o.ApplicationUser).ToList();
            }

            return View(orderHeaderList);
        }

        public IActionResult Details(int id)
        {
            OrderDetailsVm = new OrderDetailsVM()
            {
                OrderHeader = _context.OrderHeaders.Include(o => o.ApplicationUser).FirstOrDefault(oh => oh.Id == id),
                OrderDetailsList = _context.OrderDetails.Where(od => od.OrderId == id).Include(o => o.Product).ToList()
            };
            return View(OrderDetailsVm);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);
            orderHeader.OderStatus = SD.OrderStatusInProcess;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ShipOrder(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);
            orderHeader.TrackingNumber = OrderDetailsVm.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailsVm.OrderHeader.Carrier;
            orderHeader.OderStatus = SD.OrderStatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                

                orderHeader.OderStatus = SD.OrderStatusRefunded;
                orderHeader.PaymentStatus = SD.OrderStatusRefunded;
            }
            else
            {
                orderHeader.OderStatus = SD.OrderStatusCancelled;
                orderHeader.PaymentStatus = SD.OrderStatusCancelled;
            }



            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
