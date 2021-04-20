using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificatesInfo
{
    public class CreateModel : PageModel
    {
         private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public CreateModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<SelectListItem> CertificateCategoryList { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public CertificateInfo CertificateInfo { get; set; }

        public int wo;

        public IActionResult OnGet()
        {

            CertificateCategoryList = _context.CertificateCategory.Select(a =>
                                 new SelectListItem
                                 {
                                     Value = a.CertificateCategoryID.ToString(),
                                     Text = a.CertificateCategoryName
                                 }).ToList();

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

            string categorySelected = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo.CertificateCategoryID)).Select(a => a.CertificateCategoryID).FirstOrDefault().ToString();

            CertificateInfo.CertificateCategoryID = Convert.ToInt32(categorySelected);
            
            _context.CertificateInfo.Add(CertificateInfo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
