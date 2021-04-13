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
    public class IndexModel : PageModel
    {
        private readonly SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext _context;

        public IndexModel(SSLCertificateTrackingWebApp.Data.SSLCertificateTrackingWebAppContext context)
        {
            _context = context;
        }

        public IList<CertificateInfo> CertificateInfo { get; set; }

        public IList<MinimalCertificateInfo> NewCertificateInfoList { get; set; }

        public string CategorySelectedId { get; set; }

        public MinimalCertificateInfo MinimalCertificateInfo { get; set; }

        public string CategorySelectedName;

        public async Task OnGetAsync()
        {
            CertificateInfo = await _context.CertificateInfo.ToListAsync();        
     
            NewCertificateInfoList = new List<MinimalCertificateInfo>();

            for (int i = 0; i < CertificateInfo.Count; i++)
            {
                CategorySelectedId = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo[i].CertificateCategoryID)).Select(a => a.CertificateCategoryID).FirstOrDefault().ToString();

                if (CertificateInfo[i].CertificateCategoryID == Convert.ToInt32(CategorySelectedId))
                {
                    CategorySelectedName = _context.CertificateCategory.Where(a => a.CertificateCategoryID == Convert.ToInt32(CertificateInfo[i].CertificateCategoryID)).Select(a => a.CertificateCategoryName).FirstOrDefault().ToString();
                    MinimalCertificateInfo = new MinimalCertificateInfo();
                    MinimalCertificateInfo.MinimalCertId = CertificateInfo[i].CertificateID.ToString();
                    MinimalCertificateInfo.MinimalCategorySelectedName = CategorySelectedName;
                    MinimalCertificateInfo.MinimalCertWorkOrderNumber = CertificateInfo[i].WorkOrderNumber.ToString();
                    MinimalCertificateInfo.MinimalCertExpireDate = CertificateInfo[i].CertificateExpirationDate.ToString(("MM/dd/yyyy"));
                    MinimalCertificateInfo.MinimalCertNotes = CertificateInfo[i].Notes;


                    NewCertificateInfoList.Add(MinimalCertificateInfo);
                }
            }
        }
    }

    public class MinimalCertificateInfo
    {
        public string MinimalCertId { get; set; }
        public string MinimalCategorySelectedName { get; set; }
        public string MinimalCertWorkOrderNumber { get; set; }
        public string MinimalCertExpireDate { get; set; }
        public string MinimalCertNotes { get; set; }
    }
}
