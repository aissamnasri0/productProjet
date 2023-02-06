using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projetProduct.Data;
using projetProduct.Models;

namespace projetProduct.Controllers
{
    [Authorize (Roles = "admin")]
    public class ProduiUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProduiUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProduiUsers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProduitUser.Include(p => p.Produit).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProduiUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProduitUser == null)
            {
                return NotFound();
            }

            var produiUser = await _context.ProduitUser
                .Include(p => p.Produit)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produiUser == null)
            {
                return NotFound();
            }

            return View(produiUser);
        }

        // GET: ProduiUsers/Create
        public IActionResult Create()
        {
            ViewData["ProduitId"] = new SelectList(_context.produits, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.users, "Id", "Id");
            return View();
        }

        // POST: ProduiUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProduitId,UserId")] ProduiUser produiUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produiUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProduitId"] = new SelectList(_context.produits, "Id", "Id", produiUser.ProduitId);
            ViewData["UserId"] = new SelectList(_context.users, "Id", "Id", produiUser.UserId);
            return View(produiUser);
        }

        // GET: ProduiUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProduitUser == null)
            {
                return NotFound();
            }

            var produiUser = await _context.ProduitUser.FindAsync(id);
            if (produiUser == null)
            {
                return NotFound();
            }
            ViewData["ProduitId"] = new SelectList(_context.produits, "Id", "Id", produiUser.ProduitId);
            ViewData["UserId"] = new SelectList(_context.users, "Id", "Id", produiUser.UserId);
            return View(produiUser);
        }

        // POST: ProduiUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProduitId,UserId")] ProduiUser produiUser)
        {
            if (id != produiUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produiUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduiUserExists(produiUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProduitId"] = new SelectList(_context.produits, "Id", "Id", produiUser.ProduitId);
            ViewData["UserId"] = new SelectList(_context.users, "Id", "Id", produiUser.UserId);
            return View(produiUser);
        }

        // GET: ProduiUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProduitUser == null)
            {
                return NotFound();
            }

            var produiUser = await _context.ProduitUser
                .Include(p => p.Produit)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produiUser == null)
            {
                return NotFound();
            }

            return View(produiUser);
        }

        // POST: ProduiUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProduitUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProduitUser'  is null.");
            }
            var produiUser = await _context.ProduitUser.FindAsync(id);
            if (produiUser != null)
            {
                _context.ProduitUser.Remove(produiUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProduiUserExists(int id)
        {
          return (_context.ProduitUser?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
