using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.CertificatesInfo
{
    public class CreateModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        [BindProperty]
        public List<SelectListItem> CertificateCategoryList { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public CertificateInfo CertificateInfo { get; set; }

        public int wo;

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
                    CertificateCategoryList = _context.CertificateCategory.Select(a =>
                    new SelectListItem
                    {
                        Value = a.CertificateCategoryID.ToString(),
                        Text = a.CertificateCategoryName
                    }).ToList();

                    return Page();
                }
            }

            foreach (var modifyAccessUser in modifyAccessUsers)
            {
                if (currentNameOnly == modifyAccessUser)
                {
                    CertificateCategoryList = _context.CertificateCategory.Select(a =>
                    new SelectListItem
                    {
                        Value = a.CertificateCategoryID.ToString(),
                        Text = a.CertificateCategoryName
                    }).ToList();

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

            string categorySelected = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo.CertificateCategoryID)).Select(a => a.CertificateCategoryID).FirstOrDefault().ToString();

            CertificateInfo.CertificateCategoryID = Convert.ToInt32(categorySelected);

            _context.CertificateInfo.Add(CertificateInfo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
