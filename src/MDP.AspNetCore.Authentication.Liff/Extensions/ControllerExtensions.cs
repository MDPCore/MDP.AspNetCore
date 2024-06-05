using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Liff
{
    public static partial class ControllerExtensions
    {
        // Methods
        public static ActionResult SendMessages(this Controller controller, string message, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");
            if (string.IsNullOrEmpty(message) == true) throw new ArgumentNullException($"{nameof(message)}=null");

            #endregion

            // SendMessage
            return controller.SendMessages(new List<string> { message }, returnUrl);
        }

        public static ActionResult SendMessages(this Controller controller, List<string> messageList, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");
            if (messageList == null) throw new ArgumentNullException($"{nameof(messageList)}=null");

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
            if (messageList.Count==0) return controller.Redirect(returnUrl);
            
            // OptionsMonitor
            var optionsMonitor = controller.HttpContext.RequestServices.GetService<IOptionsMonitor<LiffOptions>>();
            if (optionsMonitor == null) throw new InvalidOperationException($"{nameof(optionsMonitor)}=null");

            // LiffOptions
            var liffOptions = optionsMonitor.Get(LiffDefaults.AuthenticationScheme);
            if (liffOptions == null) throw new InvalidOperationException($"{nameof(liffOptions)}=null");
            if (liffOptions.LiffId == null) throw new InvalidOperationException($"{nameof(liffOptions.LiffId)}=null");

            // ViewBag
            controller.ViewBag.LiffId = liffOptions.LiffId;
            controller.ViewBag.ReturnUrl = returnUrl;
            controller.ViewBag.Messages = $"[{string.Join(",", messageList.ToArray())}]";

            // Return
            return controller.View("_auth/liff/send-messages");
        }

    }
}