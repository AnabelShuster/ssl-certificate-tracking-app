using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSLCertificateTrackingWebApp.Migrations
{
    public partial class SSLCertificateTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateCategory",
                columns: table => new
                {
                    CertificateCategoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificateCategoryName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateCategory", x => x.CertificateCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "CertificateInfo",
                columns: table => new
                {
                    CertificateID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificateCategoryID = table.Column<int>(nullable: false),
                    WorkOrderNumber = table.Column<int>(nullable: false),
                    CertificateName = table.Column<string>(nullable: true),
                    CommonName = table.Column<string>(nullable: true),
                    CertificateType = table.Column<string>(nullable: true),
                    CertificateExpirationDate = table.Column<DateTime>(nullable: false),
                    ServerName = table.Column<string>(nullable: true),
                    CertificateTemplate = table.Column<string>(nullable: true),
                    Hosted = table.Column<string>(nullable: true),
                    SubjectAlternativeNames = table.Column<string>(nullable: true),
                    WebServer = table.Column<string>(nullable: true),
                    ServersInstalledOn = table.Column<string>(nullable: true),
                    OperatingSystem = table.Column<string>(nullable: true),
                    Organization = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Requester = table.Column<string>(nullable: true),
                    CertificateEffectiveDate = table.Column<DateTime>(nullable: false),
                    ApplicationName = table.Column<string>(nullable: true),
                    IssuesBy = table.Column<string>(nullable: true),
                    IssuedEmailAddress = table.Column<string>(nullable: true),
                    ServerType = table.Column<string>(nullable: true),
                    SpecialistWhoIssued = table.Column<string>(nullable: true),
                    CMCert = table.Column<bool>(nullable: false),
                    DomainMember = table.Column<bool>(nullable: false),
                    Approver = table.Column<string>(nullable: true),
                    IssuedFromNewPKI = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    RequestedDate = table.Column<DateTime>(nullable: false),
                    ApprovedDate = table.Column<DateTime>(nullable: false),
                    DeclinedDate = table.Column<DateTime>(nullable: false),
                    RevokedDate = table.Column<DateTime>(nullable: false),
                    ReplacedDate = table.Column<DateTime>(nullable: false),
                    DiscoveredDate = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfo", x => x.CertificateID);
                });

            migrationBuilder.CreateTable(
                name: "EmailServerConfiguration",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SMTPServer = table.Column<string>(nullable: false),
                    Port = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailServerConfiguration", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateCategory");

            migrationBuilder.DropTable(
                name: "CertificateInfo");

            migrationBuilder.DropTable(
                name: "EmailServerConfiguration");
        }
    }
}
