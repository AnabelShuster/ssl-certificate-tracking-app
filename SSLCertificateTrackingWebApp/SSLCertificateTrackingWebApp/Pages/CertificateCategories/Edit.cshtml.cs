using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificateCategories
{
    public class EditModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;
 
        public IConfiguration Configuration { get; }

        private readonly IConfiguration _configuration;
        public EditModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context, IConfiguration iconfig)
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

            string[] modifyAccessUsers = _configuration.GetValue<string>("ModifyAccess").ToUpper().Split(";");
            string[] fullAccessUsers = _configuration.GetValue<string>("FullAccess").ToUpper().Split(";");

            foreach (var fullAccessUser in fullAccessUsers)
            {
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
            }

            foreach (var modifyAccessUser in modifyAccessUsers)
            {
                if (currentNameOnly == modifyAccessUser)
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
            }

            return RedirectToPage("/Error");
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
