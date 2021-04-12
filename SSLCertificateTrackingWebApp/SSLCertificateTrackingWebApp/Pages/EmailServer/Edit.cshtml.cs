using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.EmailServer
{
    public class EditModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public EditModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmailServerConfiguration EmailServerConfiguration { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EmailServerConfiguration = await _context.EmailServerConfiguration.FirstOrDefaultAsync(m => m.ID == id);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            if (EmailServerConfiguration.ID == 0)
            {
                EmailServerConfiguration.ID = 1;
                EmailServerConfiguration.Password = PasswordEncryptionUtil.Encrypt(EmailServerConfiguration.Password);
                _context.EmailServerConfiguration.Add(EmailServerConfiguration);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Attach(EmailServerConfiguration).State = EntityState.Modified;

                try
                {
                    EmailServerConfiguration.Password = PasswordEncryptionUtil.Encrypt(EmailServerConfiguration.Password);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailServerConfigurationExists(EmailServerConfiguration.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EmailServerConfigurationExists(int id)
        {
            return _context.EmailServerConfiguration.Any(e => e.ID == id);
        }
    }
}
