using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SelectPdf;
using System.Text;
using QRCoder;

namespace SelectPDFApp.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            var pdf = HtmlToPdf(HtmlBody(), GetFooter());
            FileResult fileResult = new FileContentResult(pdf, "application/pdf")
            {
                FileDownloadName = "Document.pdf"
            };
            return fileResult;
        }

        private byte[] HtmlToPdf(string htmlBody, string htmlFooter)
        {
            // get parameters
            string headerUrl = Startup.ServerPath + @"\wwwroot\templates\header.html";
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();
            // header settings
            converter.Options.DisplayHeader = true;
            converter.Header.Height = 20;
            PdfHtmlSection headerHtml = new PdfHtmlSection(headerUrl)
            {
                AutoFitHeight = HtmlToPdfPageFitMode.AutoFit
            };
            converter.Header.Add(headerHtml);
            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.Height = 165;
            PdfHtmlSection footerHtml = new PdfHtmlSection(htmlFooter, null)
            {
                AutoFitHeight = HtmlToPdfPageFitMode.AutoFit,
                AutoFitWidth = HtmlToPdfPageFitMode.AutoFit,
            };
            converter.Footer.Add(footerHtml);
            // read parameters from the webpage
            PdfPageSize pageSize = PdfPageSize.A4;
            PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;
            int webPageWidth = 793;
            int webPageHeight = 0;
            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            converter.Options.MarginLeft = 0;
            converter.Options.MarginRight = 0;
            //set document permissions
            converter.Options.SecurityOptions.CanAssembleDocument = false;
            converter.Options.SecurityOptions.CanCopyContent = true;
            converter.Options.SecurityOptions.CanEditAnnotations = false;
            converter.Options.SecurityOptions.CanEditContent = false;
            converter.Options.SecurityOptions.CanPrint = true;
            converter.Options.PdfDocumentInformation.Title = "CPD Activity Accreditation";
            converter.Options.PdfDocumentInformation.CreationDate = DateTime.UtcNow;
            converter.Options.PdfDocumentInformation.Subject = "CPD Activity Accreditation";
            converter.Options.PdfDocumentInformation.Keywords = "scfhs.org.sa";
            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlBody);
            // save pdf document
            byte[] pdf = doc.Save();
            // close pdf document
            doc.Close();
            // return resulted pdf document
            return pdf;

        }

        public string HtmlBody()
        {
            var htmlTemplateEnPath = Startup.ServerPath + @"\wwwroot\templates\certificateEn.html";
            if (System.IO.File.Exists(htmlTemplateEnPath))
            {
                var htmlTemplate = System.IO.File.ReadAllText(htmlTemplateEnPath);
                StringBuilder htmlBuilder = new StringBuilder(htmlTemplate);
                htmlBuilder.Replace("{ActivityName}", "Sample Name");
                StringBuilder table = new StringBuilder();
                table.Append("<table>");
                table.Append($"<tr><th>Provider name</th><td>Sample Text</td> </tr>");
                table.Append($"<tr><th>Activity Accreditation Number </th><td>Sample Text</td></tr>");
                table.Append($"<tr><th>Accreditation type</th><td>Sample Text</td></tr>");
                table.Append($"<tr><th> Accreditation date</th><td>Sample Text</td></tr>");
                table.Append($"<tr><th>Activity objective(s)</th><td> Sample Text</td></tr>");
                table.Append($"<tr><th>Target Audience</th><td>Sample Text</td> </tr>");
                table.Append($"<tr><th>Activity type</th> <td>Sample Text</td></tr>");
                table.Append($"<tr><th>Evaluation method of the practitioner’s level </th><td>Sample Text</td></tr>");
                table.Append($"<tr><th>The activity capacity</th><td>Sample Text</td></tr>");
                table.Append($"<tr><th>Activity Venue</th> <td>Sample Text</td></tr>");
                table.Append($"<tr><th> Activity date</th><td>Sample Text</td></tr> ");
                table.Append("</table>");
                htmlBuilder.Replace("{activitytable}", table.ToString());
                return htmlBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetFooter()
        {
            var footerEnPath = Startup.ServerPath + @"\wwwroot\templates\footer.html";
            if (System.IO.File.Exists(footerEnPath))
            {
                var footer = System.IO.File.ReadAllText(footerEnPath);
                StringBuilder footerBuilder = new StringBuilder(footer);
                //Genrating QR
                StringBuilder sb1 = new StringBuilder();
                sb1.Append("CPD provider: Sample\r\n");
                sb1.Append("Reference Number: 12345\r\n");
                QRCodeGenerator qrGeneratorbase64 = new QRCodeGenerator();
                QRCodeData qrCodeData1 = qrGeneratorbase64.CreateQrCode(sb1.ToString(), QRCodeGenerator.ECCLevel.Q);
                Base64QRCode qrCodeBase = new Base64QRCode(qrCodeData1);
                string qrCodeImageAsBase64 = qrCodeBase.GetGraphic(20);
                var imgType = Base64QRCode.ImageType.Jpeg;
                var htmlPictureTag = $"<img alt='QR' src=\"data:image/{imgType.ToString().ToLower()};base64,{qrCodeImageAsBase64}\" class='img-qr-code' />";
                //End QR Code
                footerBuilder.Replace("{qrImage}", htmlPictureTag);
                footerBuilder.Replace("{refNo}", "1234567890");

                return footerBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
