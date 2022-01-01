using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstate.Model;
using RealEstate.Service;
using RealEstate.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRealEstateService realEstateService;
        private readonly IRealEstateOwnerService realEstateOwnerService;

        public HomeController(ILogger<HomeController> logger, IRealEstateService _realEstateService, IRealEstateOwnerService _realEstateOwnerService)
        {
            _logger = logger;
            realEstateService = _realEstateService;
            realEstateOwnerService = _realEstateOwnerService;
        }

        public IActionResult Index()
        {
            return View(realEstateService.GetRealEstate().List);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            var model = realEstateOwnerService.Login(login);

            if (!model.IsSuccess)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult InsertRealEstate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertRealEstate(RealEstateViewModel realEstate)
        {
            var model = realEstateService.Insert(realEstate);

            if (!model.IsSuccess)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteRealEstate(int id)
        {
            var model = realEstateService.Delete(id);

            if (!model.IsSuccess)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
