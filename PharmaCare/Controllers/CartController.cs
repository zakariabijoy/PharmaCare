using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize]
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

        public IActionResult Plus(int cartId)
        {
            var cart = _context.ShoppingCarts.Include(s => s.Product).FirstOrDefault(c => c.Id == cartId);
            cart.Count += 1;
            cart.Price = SD.GetPriceBasedOnQuatity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Minus(int cartId)
        {
            var cart = _context.ShoppingCarts.Include(s => s.Product).FirstOrDefault(c => c.Id == cartId);


            if (cart.Count == 1)
            {
                var cnt = _context.ShoppingCarts.Where(sC => sC.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _context.ShoppingCarts.Remove(cart);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("Session_ShoppingCart", cnt - 1);

            }
            else
            {
                cart.Count -= 1;
                cart.Price = SD.GetPriceBasedOnQuatity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                _context.SaveChanges();
            }


            return RedirectToAction(nameof(Index));

        }

        public IActionResult Remove(int cartId)
        {
            var cart = _context.ShoppingCarts.Include(s => s.Product).FirstOrDefault(c => c.Id == cartId);


            var cnt = _context.ShoppingCarts.Where(sC => sC.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("Session_ShoppingCart", cnt - 1);


            return RedirectToAction(nameof(Index));

        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVm = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ShoppingCarts = _context.ShoppingCarts.Include(s => s.Product).Where(c => c.ApplicationUserId == claim.Value).ToList()
            };

            ShoppingCartVm.OrderHeader.ApplicationUser =
                _context.Users
                .FirstOrDefault(u => u.Id == claim.Value);

            foreach (var list in ShoppingCartVm.ShoppingCarts)
            {
                list.Price = SD.GetPriceBasedOnQuatity(list.Count, list.Product.Price,
                    list.Product.Price50, list.Product.Price100);
                ShoppingCartVm.OrderHeader.OrderTotal += (list.Price * list.Count);
            }

            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.UserName;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
          

            return View(ShoppingCartVm);
        }
    }
}
