using DotNetSelfHost.WinForms.Models;
using DotNetSelfHost.WinForms.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Net5SelfHost.WinForms.Controllers
{
    [Route("/getfile")]
    public class GreetingController
    {
        [HttpGet]
        public async Task<FileStatus> Get(string name)
        {
            //get Url
            var url = $"/reports/pers/departments/IsPed";
            // Get File
            var getFile = await FileService.JsonPostWithToken("secret", url, "GET", "Отчет");

            return getFile;
            //return new JsonResult(greeting);
        }
    }
}
