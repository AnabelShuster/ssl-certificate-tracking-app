using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificateCategories
{
    public class DeleteModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;
        public string Msg { get; set; }

        public IConfiguration Configuration { get; }

        private readonly IConfiguration _configuration;

        public DeleteModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context, IConfiguration iconfig)
        {
            _context = context;
            _configuration = iconfig;
        }

        [BindProperty]
        public CertificateCategory CertificateCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string currentUser = HttpContext.User.Identity.Name.ToUpper();
            string[] currentUserFormat = currentUser.Split("\\");
            string currentNameOnly = currentUserFormat[1];

            string fullAccessUser = _configuration.GetValue<string>("FullAccess").ToUpper();


            if (currentNameOnly == fullAccessUser)
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

            return RedirectToPage("/Error");
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
