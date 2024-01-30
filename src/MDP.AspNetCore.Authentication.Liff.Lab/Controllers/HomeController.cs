using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace MDP.AspNetCore.Authentication.Liff.Lab
{
    public class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult Index()
        {            
            // Return
            return this.View();
        }
       
        public ActionResult Landing()
        {
            // Return
            return this.View();
        }

        [Authorize]
        public ActionResult Member()
        {
            // Return
            return this.View();
        }

        public ActionResult SendMessages()
        {
            // Message
            var flexMessage = @"{""type"":""bubble"",""body"":{""type"":""box"",""layout"":""vertical"",""contents"":[{""type"":""text"",""text"":""預約登記"",""size"":""xl"",""color"":""#06C755"",""weight"":""bold"",""align"":""center"",""contents"":[],""offsetBottom"":""10px""},{""type"":""box"",""layout"":""horizontal"",""contents"":[{""type"":""text"",""text"":""客戶姓名："",""weight"":""bold"",""decoration"":""none"",""flex"":0},{""type"":""text"",""text"":""Clark""}]},{""type"":""box"",""layout"":""horizontal"",""contents"":[{""type"":""text"",""text"":""預約時間："",""flex"":0,""weight"":""bold""},{""type"":""text"",""text"":""2024-01-18(五) 14:00""}]},{""type"":""box"",""layout"":""horizontal"",""contents"":[{""type"":""text"",""text"":""服務人員："",""weight"":""bold"",""decoration"":""none"",""flex"":0},{""type"":""text"",""text"":""Jane""}]},{""type"":""box"",""layout"":""horizontal"",""contents"":[{""type"":""text"",""text"":""服務項目："",""weight"":""bold"",""decoration"":""none"",""flex"":0},{""type"":""text"",""text"":""1-2張畫作裱框""}]}]}}";
            var message = @"{""type"":""flex"",""altText"":""Flex Message"",""contents"":" + flexMessage + "}";

            // Return
            return this.SendMessages(message);
        }        
    }
}
