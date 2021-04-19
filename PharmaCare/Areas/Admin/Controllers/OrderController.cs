using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaCare.Data;
using PharmaCare.Models;
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
    }
}
