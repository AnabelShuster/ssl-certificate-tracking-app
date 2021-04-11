using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificateCategories
{
    public class EditModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public EditModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CertificateCategory CertificateCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CertificateCategory = await _context.CertificateCategory.FirstOrDefaultAsync(m => m.CertificateCategoryID == id);

            if (CertificateCategory == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CertificateCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateCategoryExists(CertificateCategory.CertificateCategoryID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CertificateCategoryExists(int id)
        {
            return _context.CertificateCategory.Any(e => e.CertificateCategoryID == id);
        }
    }
}
