using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificatesInfo
{
    public class DeleteModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public DeleteModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CertificateInfo CertificateInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CertificateInfo = await _context.CertificateInfo.FirstOrDefaultAsync(m => m.CertificateID == id);

            if (CertificateInfo == null)
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

            CertificateInfo = await _context.CertificateInfo.FindAsync(id);

            if (CertificateInfo != null)
            {
                _context.CertificateInfo.Remove(CertificateInfo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
