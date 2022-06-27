using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using DigitalSignageClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace DigitalSignageClient.Controllers
{
    [Produces("application/json")]
    [Route("api/CurrencyFeeds")]
    public class CurrenciesController : Controller
    {
        private readonly ICurrencyViewRepository _currencyViewRepository;

        public CurrenciesController(ICurrencyViewRepository currencyViewRepository)
        {
            _currencyViewRepository = currencyViewRepository;
        }

        [HttpGet]
        [Route("GetAllCurrencies")]
        public IActionResult GetAllCurrencies()
        {
            List<CurrencyView> currencyViews = new List<CurrencyView>();
            currencyViews = _currencyViewRepository.Where(x => x.Id != 0).ToList();
            return Ok(currencyViews.Select(x => new { id = x.Id, x.Buying, x.Selling, x.UpdateDate, x.Abbreviation, x.Currency}));
        }

        [HttpGet]
        [Route("DownloadFormat")]
        public async Task<IActionResult> DownloadCurrencyFormat()
        {

            List<CurrencyView> currencyFeeds = _currencyViewRepository.Where(x => x.Id != 0).OrderBy(x => x.Id).ToList();
            string fileName = @"Exchange Rate.xlsx";
            FileInfo file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\", fileName));
            if (file.Exists)
            {
                file.Delete();
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Currency");
                worksheet.Cells[1, 1].Value = "Exchange Rate";
                worksheet.Cells[2, 1].Value = "Country";
                worksheet.Cells[2, 2].Value = "Abbreviation";
                worksheet.Cells[2, 3].Value = "Buying";
                worksheet.Cells[2, 4].Value = "Selling";
                for (int i = 3; i < currencyFeeds.Count() + 3; i++)
                {
                    worksheet.Cells[i, 1].Value = currencyFeeds[i - 3].Currency;
                    worksheet.Cells[i, 2].Value = currencyFeeds[i - 3].Abbreviation;
                }
                package.Save();
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\", fileName);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/xlsx", fileName);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Credential model)
        {
            if(model.UserName == "admin" && model.Password == "12345678")
            {
                return Ok(new { Success = true });
            }
            else
            {
                return Ok(new { Success = false });
            }
        }

        [HttpPost]
        [RequestSizeLimit(1_074_790_400)]
        //[DisableRequestSizeLimit]
        [Route("UploadCurrency")]
        public async Task<IActionResult> UploadCurrency()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var file = form.Files.FirstOrDefault();

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string ext = Path.GetExtension(fileName);
                    if (ext != ".xlsx")
                    {
                        return Ok("Exchange Rate file type is '.xlsx'. please make sure you uploaded the right file");
                    }
                    List<CurrencyView> currencyFeeds = _currencyViewRepository.Where(x =>x.Id != 0).OrderBy(x => x.Id).ToList();
                    List<CurrencyView> updatedCurrencyFeeds = new List<CurrencyView>();
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Currency"];
                            var rowCount = worksheet.Dimension.Rows;
                            for (int row = 3; row < currencyFeeds.Count() + 3; row++)
                            {
                                CurrencyView currencyFeed = currencyFeeds.FirstOrDefault(x => worksheet.Cells[row, 2].Value.ToString().Contains(x.Abbreviation));
                                if (currencyFeed != null)
                                {
                                    decimal buying = 0;
                                    decimal.TryParse(worksheet.Cells[row, 3].Value.ToString(), out buying);
                                    //decimal buying = decimal.Parse(worksheet.Cells[row, 3].Value.ToString());
                                    currencyFeed.Difference = buying - currencyFeed.Buying;
                                    currencyFeed.Buying = buying;
                                    decimal selling = 0;
                                    decimal.TryParse(worksheet.Cells[row, 4].Value.ToString(), out selling);
                                    currencyFeed.Selling = selling;

                                    //currencyFeed.TransactionalSelling = decimal.Parse(worksheet.Cells[row, 6].Value.ToString());
                                    currencyFeed.UpdateDate = DateTime.Now;
                                    updatedCurrencyFeeds.Add(currencyFeed);
                                }
                            }
                        }
                    }
                    if (updatedCurrencyFeeds.Count() > 0)
                    {
                        _currencyViewRepository.UpdateRange(updatedCurrencyFeeds);
                    }
                    return Ok("SUCCESS");
                }

            }
            catch (Exception ex)
            {
                return Ok("Unable to parse excel file");
            }

            return this.Ok("SUCCESS");
        }
    }
}
