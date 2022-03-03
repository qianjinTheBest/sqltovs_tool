using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tool_common;
using tools_sqltonet.tool_deal;

namespace tools_sqltonet.Controllers
{
    [ApiController]
    [Route("api/producttest")]
    public class TestController : Controller
    {
        [HttpPost]
        [Route("index")]
        public IActionResult Index()
        {

            //SqlServerDealTool.WriteTbToCs("demo", "QJJob");
            MySqlDealTool.GetAllTbNameByDataBase("demo");
            return Json("index");
        }
    }
}