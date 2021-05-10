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

namespace SSLCertificateTrackingWebApp.Pages.CertificatesInfo
{
    public class DeleteModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;
        public IConfiguration Configuration { get; }

        private readonly IConfiguration _configuration;
        public DeleteModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context, IConfiguration iconfig)
        {
            _context = context;
             _configuration = iconfig;
        }

        [BindProperty]
        public CertificateInfo CertificateInfo { get; set; }
        public string CategorySelectedId { get; set; }
        public string CategorySelectedName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string currentUser = HttpContext.User.Identity.Name.ToUpper();
            string[] currentUserFormat = currentUser.Split("\\");
            string currentNameOnly = currentUserFormat[1];

            string[] fullAccessUsers = _configuration.GetValue<string>("FullAccess").ToUpper().Split(";");

            foreach (var fullAccessUser in fullAccessUsers)
            {
                if (currentNameOnly == fullAccessUser)
                {

                    if (id == null)
                    {
                        return NotFound();
                    }

                    CertificateInfo = await _context.CertificateInfo.FirstOrDefaultAsync(m => m.CertificateID == id);
                    CategorySelectedId = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo.CertificateCategoryID)).Select(a => a.CertificateCategoryID).FirstOrDefault().ToString();
                    CategorySelectedName = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo.CertificateCategoryID)).Select(a => a.CertificateCategoryName).FirstOrDefault().ToString();

                    if (CertificateInfo == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }
            }

            return RedirectToPage("/Error");
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
