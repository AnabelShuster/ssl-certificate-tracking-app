using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [BindProperty(SupportsGet = true)]
        public string CategorySearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string WorkOrderNumberSearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ExpireDateSearchString { get; set; }

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
                    MinimalCertificateInfo = new MinimalCertificateInfo
                    {
                        MinimalCertId = CertificateInfo[i].CertificateID.ToString(),
                        MinimalCertName = CertificateInfo[i].CertificateName,
                        MinimalCategorySelectedName = CategorySelectedName.ToUpper(),
                        MinimalCertWorkOrderNumber = CertificateInfo[i].WorkOrderNumber.ToString(),
                        MinimalCertExpireDate = CertificateInfo[i].CertificateExpirationDate.ToString(("MM/dd/yyyy")),
                        MinimalCertNotes = CertificateInfo[i].Notes
                    };


                    NewCertificateInfoList.Add(MinimalCertificateInfo);
                }
            }

            NewCertificateInfoList = (from e in NewCertificateInfoList orderby e.MinimalCertExpireDate ascending select e).ToList();

            var certList = from m in NewCertificateInfoList
                           select m;
            if (!string.IsNullOrEmpty(CategorySearchString))
            {
                certList = certList.Where(s => s.MinimalCategorySelectedName.Contains(CategorySearchString.ToUpper()));
                certList = from e in certList orderby e.MinimalCertExpireDate ascending select e;
                NewCertificateInfoList = certList.ToList();
            }

            if (!string.IsNullOrEmpty(WorkOrderNumberSearchString))
            {
                certList = certList.Where(s => s.MinimalCertWorkOrderNumber.Contains(WorkOrderNumberSearchString.ToUpper()));
                certList = from e in certList orderby e.MinimalCertWorkOrderNumber ascending select e;
                NewCertificateInfoList = certList.ToList();
            }

            if (!string.IsNullOrEmpty(ExpireDateSearchString))
            {
                DateTime aboutToExpireDate = DateTime.Now.AddDays(Convert.ToInt32(ExpireDateSearchString));

                certList = certList.Where(s => DateTime.Parse(s.MinimalCertExpireDate) <= aboutToExpireDate);
                certList = from e in certList orderby e.MinimalCertExpireDate ascending select e;
                NewCertificateInfoList = certList.ToList();
            }

        }
    }

    public class MinimalCertificateInfo
    {
        public string MinimalCertId { get; set; }
        public string MinimalCertName { get; set; }
        public string MinimalCategorySelectedName { get; set; }
        public string MinimalCertWorkOrderNumber { get; set; }
        public string MinimalCertExpireDate { get; set; }
        public string MinimalCertNotes { get; set; }
    }
}
