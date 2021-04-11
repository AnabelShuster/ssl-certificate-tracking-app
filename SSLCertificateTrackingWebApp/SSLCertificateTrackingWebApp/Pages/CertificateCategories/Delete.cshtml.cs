using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificateCategories
{
    public class DeleteModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public DeleteModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CertificateCategory = await _context.CertificateCategory.FindAsync(id);

            if (CertificateCategory != null)
            {
                _context.CertificateCategory.Remove(CertificateCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
