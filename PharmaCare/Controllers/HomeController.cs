using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PharmaCare.Data;
using PharmaCare.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            product.Star += 1;
            await _context.SaveChangesAsync();

            return View(product);

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
