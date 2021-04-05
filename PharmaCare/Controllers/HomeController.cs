using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmaCare.Data;
using PharmaCare.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.newProducts = _context.Products.ToList().OrderByDescending(p => p.CreatedDate).Take(4);
            ViewBag.popularProducts = _context.Products.ToList().OrderByDescending(p => p.Star).Take(6);
            return View();
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            if(product == null)
            {
                return NotFound();
            }

            product.Star += 1;
            await _context.SaveChangesAsync();

            var cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };

            return View(cart);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult ProductDetails(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;

            if (ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                var shoppingCartFromDb = _context.ShoppingCarts.Include(s => s.Product).FirstOrDefault(s => s.ApplicationUserId == shoppingCart.ApplicationUserId && s.ProductId == shoppingCart.ProductId);

                if (shoppingCartFromDb == null)
                {
                    //no records exists in database for that product for that user, so we create new data 
                    _context.ShoppingCarts.Add(shoppingCart);
                }
                else
                {
                    shoppingCartFromDb.Count += shoppingCart.Count;

                    
                }

                _context.SaveChanges();

                var count = _context.ShoppingCarts.Where(s => s.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                // HttpContext.Session.SetObject(SD.Session_ShoppingCart, shoppingCartFromDb);     // we can store an object into session by using SetObject (session extension method)
                HttpContext.Session.SetInt32("Session_ShoppingCart", count);

                return RedirectToAction("Index");

            }
            else
            {
                var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == shoppingCart.ProductId);

                var cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };

                return View(cart);
            }


        }

        public async Task<IActionResult> ShowAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
