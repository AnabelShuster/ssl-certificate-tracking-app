using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificateCategories
{
    public class CreateModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public IConfiguration Configuration { get; }

        private readonly IConfiguration _configuration;

        public CreateModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context, IConfiguration iconfig)
        {
            _context = context;
            _configuration = iconfig;
        }

        public IActionResult OnGet()
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
                    return Page();
                }
            }

            foreach (var modifyAccessUser in modifyAccessUsers)
            {
                if (currentNameOnly == modifyAccessUser)
                {
                    return Page();
                }
            }

            return RedirectToPage("/Error");
        }

        [BindProperty]
        public CertificateCategory CertificateCategory { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CertificateCategory.Add(CertificateCategory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
