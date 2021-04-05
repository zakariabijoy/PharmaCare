using Microsoft.AspNetCore.Identity;
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

namespace PharmaCare.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ShoppingCartVM ShoppingCartVm { get; set; }

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            ShoppingCartVm = new ShoppingCartVM()
            {
                ShoppingCarts = _context.ShoppingCarts
                                    .Where(s => s.ApplicationUserId == claim.Value).Include( s => s.Product).ToList(),
                                       

                OrderHeader = new OrderHeader()
            };

            ShoppingCartVm.OrderHeader.OrderTotal = 0;
            ShoppingCartVm.OrderHeader.ApplicationUser = _context.Users
                                                             .FirstOrDefault(a => a.Id == claim.Value);

            foreach (var shoppingCart in ShoppingCartVm.ShoppingCarts)
            {
                shoppingCart.Price = SD.GetPriceBasedOnQuatity(shoppingCart.Count, shoppingCart.Product.Price,
                                                    shoppingCart.Product.Price50, shoppingCart.Product.Price100);

                ShoppingCartVm.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);

            }

            return View(ShoppingCartVm);
        }
    }
}
