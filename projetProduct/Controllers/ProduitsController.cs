using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using projetProduct.Data;
using projetProduct.Models;

namespace projetProduct.Controllers
{
    [Authorize]
    public class ProduitsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
    

        public ProduitsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager=userManager;
            _context = context;
        }

        // GET: Produits
        public async Task<IActionResult> Index()
        {
              return _context.produits != null ? 
                          View(await _context.produits.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.produits'  is null.");
        }
        [AllowAnonymous]
        public async Task<IActionResult> HomeProduit()
        {
            return _context.produits != null ?
                          View(await _context.produits.ToListAsync()) :
                          Problem("Entity set 'TpContext.produits'  is null.");
        }
        public async Task<IActionResult> annuler(int Id)
        {
            var res=_context.ProduitUser.Where(p=>p.ProduitId == Id).First();
            _context.ProduitUser.Remove(res);
            _context.SaveChanges();
            return RedirectToAction("GetMyProduit");
        }
        // GET: Produits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.produits == null)
            {
                return NotFound();
            }

            var produit = await _context.produits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // GET: Produits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,image,description")] Produit produit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produit);
        }
        
        public async Task<IActionResult> GetMyProduit()
        {
            var userMail = User.FindFirstValue(ClaimTypes.Email);
            var user = _context.users.Where(p=>p.username==userMail).First();
            var listeProduit = _context.produits.Where(p => p.produiUsers.Any(s => s.UserId == user.Id));
     
            return View(listeProduit.ToList());
    
        }
        public async Task<IActionResult> generatePdf()
        {
            
            var userMail = User.FindFirstValue(ClaimTypes.Email);
            var user = _context.users.Where(p => p.username == userMail).First();
            var listeProduit = _context.produits.Where(p => p.produiUsers.Any(s => s.UserId == user.Id)).ToArray();
           
          
            
            string outFile = Environment.CurrentDirectory + "/facture.pdf";
            Document document= new Document();
            PdfWriter.GetInstance(document, new FileStream(outFile, FileMode.Create));
            document.Open();
            //palette couleur
            BaseColor bleu = new BaseColor(0, 75, 155);
            BaseColor gris = new BaseColor(0,0,0);
            BaseColor gris1 = new BaseColor(240,240,240);
            BaseColor blanc = new BaseColor(255,255,255);
            //police d'ecriture
            Font policetitre = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, gris);
            Font policeth = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f,1, gris);

            //page
            //creation de paragraphes
            Paragraph p1= new Paragraph("AiShop.com"+"\n\n"+"22 avenue shop"+ "\n\n"+"33700, Merignac"+"\n\n",policetitre);
            p1.Alignment = Element.ALIGN_LEFT;
            document.Add(p1);
            Paragraph p2 = new Paragraph(user.username + "\n\n" + "36 avenue bordeaux" + "\n\n" + "33000, bordeux"+"\n\n", policetitre);
            p2.Alignment = Element.ALIGN_RIGHT;
            document.Add(p2);
            Random random= new Random();
            Paragraph p3 = new Paragraph("facture N : 00212"+random.Next(100,2000)+ "\n\n", policetitre);
            p3.Alignment = Element.ALIGN_CENTER;
            document.Add(p3);
            //creation tableu 
            PdfPTable tableau = new PdfPTable(3);
            tableau.WidthPercentage= 100;
            //cellule
            PdfPCell cell = new PdfPCell(new Phrase("name",policeth));
            cell.Padding = 7;
            cell.BackgroundColor = bleu;
            cell.BorderColor = bleu;
            tableau.AddCell(cell);
            PdfPCell cell1 = new PdfPCell(new Phrase("description", policeth));
            cell1.Padding = 7;
            cell1.BackgroundColor = bleu;
            cell1.BorderColor = bleu;
            tableau.AddCell(cell1);
            PdfPCell cell3 = new PdfPCell(new Phrase("prix", policeth));
            cell3.Padding = 7;
            cell3.BackgroundColor = bleu;
            cell3.BorderColor = bleu;
            tableau.AddCell(cell3);
            
            //lister les produit
            foreach(var item in listeProduit)
            {
                PdfPCell cell4 = new PdfPCell(new Phrase(item.Name));
                cell4.Padding = 7;
                cell4.BackgroundColor = gris1;
                cell4.BorderColor = gris1;
                tableau.AddCell(cell4);
                PdfPCell cell5 = new PdfPCell(new Phrase(item.description));
                cell5.Padding = 7;
                cell5.BackgroundColor = gris1;
                cell5.BorderColor = gris1;
                tableau.AddCell(cell5);
                PdfPCell cell6 = new PdfPCell(new Phrase(item.Price.ToString()));
                cell6.Padding = 7;
                cell6.BackgroundColor = gris1;
                cell6.BorderColor = gris1;
                tableau.AddCell(cell6);
            }
            document.Add(tableau);
            document.Add(new Phrase("\n"));
            double? total=0;
            foreach(var item in listeProduit)
            {
                total += item.Price;
            }
            Paragraph p4 = new Paragraph("Total = " +total+"\n\n", policetitre);
            p4.Alignment = Element.ALIGN_RIGHT;
            
            document.Add(p4);

            document.Close();
            Process.Start(@"cmd.exe", @"/c " + outFile);


            return RedirectToAction("GetMyProduit");

        }
        
        public async Task<IActionResult> acheterProduit(int Id)
        {
            var userMail = User.FindFirstValue(ClaimTypes.Email);
            var user = _context.users.Where(p => p.username == userMail).First();
            ProduiUser produiUser = new ProduiUser(Id,user.Id);
            _context.Add(produiUser);
            _context.SaveChanges();
            return RedirectToAction("GetMyProduit");
        }

        // GET: Produits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.produits == null)
            {
                return NotFound();
            }

            var produit = await _context.produits.FindAsync(id);
            if (produit == null)
            {
                return NotFound();
            }
            return View(produit);
        }

        // POST: Produits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,image,description")] Produit produit)
        {
            if (id != produit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduitExists(produit.Id))
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
            return View(produit);
        }

        // GET: Produits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.produits == null)
            {
                return NotFound();
            }

            var produit = await _context.produits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // POST: Produits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.produits == null)
            {
                return Problem("Entity set 'ApplicationDbContext.produits'  is null.");
            }
            var produit = await _context.produits.FindAsync(id);
            if (produit != null)
            {
                _context.produits.Remove(produit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProduitExists(int id)
        {
          return (_context.produits?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
