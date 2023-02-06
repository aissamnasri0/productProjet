using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace projetProduct.Controllers
{
    [Authorize(Roles = "admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var role= _roleManager.Roles.ToList();
            
            return View(role);
        }
        public IActionResult CreateRole() {
        
            return View(new IdentityRole());
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole role)
        {
            await _roleManager.CreateAsync(role);
            return RedirectToAction(nameof(Index));

        } 
    }
}
