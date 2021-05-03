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

namespace SSLCertificateTrackingWebApp.Pages.CertificatesInfo
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
        public List<SelectListItem> CertificateCategoryList { get; set; }

        [BindProperty]
        public CertificateInfo CertificateInfo { get; set; }

        public string CategorySelectedId { get; set; }

        public string CategorySelectedName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            string currentUser = HttpContext.User.Identity.Name.ToUpper();
            string[] currentUserFormat = currentUser.Split("\\");
            string currentNameOnly = currentUserFormat[1];

            string modifyAccessUser = _configuration.GetValue<string>("ModifyAccess").ToUpper();
            string fullAccessUser = _configuration.GetValue<string>("FullAccess").ToUpper();


            if (currentNameOnly == fullAccessUser || currentNameOnly == modifyAccessUser)
            {
                if (id == null)
                {
                    return NotFound();
                }

                CertificateInfo = await _context.CertificateInfo.FirstOrDefaultAsync(m => m.CertificateID == id);

                CategorySelectedId = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo.CertificateCategoryID)).Select(a => a.CertificateCategoryID).FirstOrDefault().ToString();
                CategorySelectedName = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo.CertificateCategoryID)).Select(a => a.CertificateCategoryName).FirstOrDefault().ToString();

                //Creates List of Category Id and Category Name from the CertificateCategory table
                CertificateCategoryList = _context.CertificateCategory.Select(a =>
                     new SelectListItem
                     {
                         Value = a.CertificateCategoryID.ToString(),
                         Text = a.CertificateCategoryName,
                     }).ToList();

                for (int i = 0; i < CertificateCategoryList.Count; i++)
                {
                    if (CertificateCategoryList[i].Value == CategorySelectedId)
                    {
                        CertificateCategoryList[i].Selected = true;
                    }
                }

                if (CertificateInfo == null)
                {
                    return NotFound();
                }
                return Page();
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

            _context.Attach(CertificateInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateInfoExists(CertificateInfo.CertificateID))
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

        private bool CertificateInfoExists(int id)
        {
            return _context.CertificateInfo.Any(e => e.CertificateID == id);
        }
    }
}
