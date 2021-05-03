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
using Microsoft.Extensions.Configuration;
using SSLCertificateTrackingWebApp.Data;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Pages.EmailServer
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
        public EmailServerConfiguration EmailServerConfiguration { get; set; }

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

                    EmailServerConfiguration = await _context.EmailServerConfiguration.FirstOrDefaultAsync(m => m.ID == id);

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

                    EmailServerConfiguration = await _context.EmailServerConfiguration.FirstOrDefaultAsync(m => m.ID == id);

                    return Page();
                }
            }

            return RedirectToPage("/Error");
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            if (EmailServerConfiguration.ID == 0)
            {
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
