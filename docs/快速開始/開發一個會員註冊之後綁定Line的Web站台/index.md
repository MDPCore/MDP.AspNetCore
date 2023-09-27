---
layout: default
title: 開發一個會員註冊之後綁定Line的Web站台
parent: 快速開始
grand_parent: 身分驗證
nav_order: 4
has_children: false
---

# 開發一個會員註冊之後綁定Line的Web站台

專案開發過程，常會需要在會員註冊並使用之後，才進行Line身分綁定。完成綁定之後，客戶就能使用Line進行OAuth快速登入(系統也能取得Line的UID來訊息推送)。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.Line，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/WebApplication1.zip)

- 模組清單：Line綁定可替換為其他OAuth身分驗證，請參考已支援的[OAuth身分認證清單](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/)。


### 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

2.使用Visual Studio開啟WebApplication1專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.Line
```

3.依照[服務申請](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Line身分驗證/#服務申請)的步驟流程，申請Line身分驗證服務，並取得「Channel ID」、「Channel Secret」。

![01.申請服務01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/01.申請服務01.png)

4.於專案內改寫appsettings.json，填入「Channel ID」、「Channel Secret」，用以掛載Line身分驗證。

```
{
  "Authentication": {
    "Line": {
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}
```

5.在專案裡加入Modules\Member.cs、Modules\MemberRepository.cs，並改寫appsettings.json。用來掛載並實做模擬會員資料庫，提供會員資料的新增、修改、查詢。

```
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.Members
{
    public class Member
    {
        // Properties
        public string MemberId { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;

        public string Mail { get; set; } = String.Empty;

        public string Nickname { get; set; } = String.Empty;

        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();


        // Methods
        public Member Clone()
        {
            // Create
            var member = new Member();
            member.MemberId = this.MemberId;
            member.Name = this.Name;
            member.Mail = this.Mail;
            member.Nickname = this.Nickname;
            member.Links = this.Links.ToDictionary(o => o.Key, o => o.Value);

            // Return
            return member;
        }
    }
}
```

```
using MDP.Registration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.Members
{
    [Service<MemberRepository>(singleton: true)]
    public class MemberRepository
    {
        // Fields
        private readonly List<Member> _memberList = new List<Member>();


        // Methods
        public void Add(Member member)
        {
            // Add
            _memberList.RemoveAll(o => o.MemberId == member.MemberId);
            _memberList.Add(member);
        }

        public void Update(Member member)
        {
            // Update
            _memberList.RemoveAll(o => o.MemberId == member.MemberId);
            _memberList.Add(member);
        }

        public Member FindByMemberId(string memberId)
        {
            // Return
            return _memberList.FirstOrDefault(o => o.MemberId == memberId)?.Clone();
        }

        public Member FindByName(string name)
        {
            // Return
            return _memberList.FirstOrDefault(o => o.Name == name)?.Clone();
        }

        public Member FindByLink(string linkType, string linkId)
        {
            // Return
            return _memberList.FirstOrDefault(o => o.Links.ContainsKey(linkType) && o.Links[linkType] == linkId)?.Clone();
        }
    }
}
```

```
"MDP.Members": {
  "MemberRepository": {}
}
```

6.在專案裡加入Modules\MemberAuthenticationProvider.cs、Modules\MemberExtensions.cs、改寫appsettings.json，用來掛載並實做MemberAuthenticationProvider，提供Login(OAuth身分登入為會員)、Link(OAuth身分綁定至會員)。

```
using MDP.AspNetCore.Authentication;
using MDP.Members;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MDP.Security.Claims;
using System.Xml.Schema;

namespace MDP.Members
{
    [MDP.Registration.Service<AuthenticationProvider>(singleton: true)]
    public class MemberAuthenticationProvider : AuthenticationProvider
    {
        // Fields
        private readonly MemberRepository _memberRepository;


        // Constructors
        public MemberAuthenticationProvider(MemberRepository memberRepository)
        {
            #region Contracts

            if (memberRepository == null) throw new ArgumentException($"{nameof(memberRepository)}=null");

            #endregion

            // Default
            _memberRepository = memberRepository;
        }


        // Methods
        public override ClaimsIdentity RemoteExchange(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // Member
            var linkType = remoteIdentity.AuthenticationType;
            var linkId = remoteIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
            var member = _memberRepository.FindByLink(linkType, linkId);
            if (member == null) return null;

            // Return
            return member.ToIdentity(remoteIdentity.AuthenticationType);
        }

        public override void RemoteLink(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");
            if (localIdentity == null) throw new ArgumentException($"{nameof(localIdentity)}=null");

            #endregion

            // Member
            var memberId = localIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
            var member = _memberRepository.FindByMemberId(memberId);
            if (member == null) throw new InvalidOperationException($"{nameof(member)}=null");

            // MemberLink
            var linkType = remoteIdentity.AuthenticationType;
            var linkId = remoteIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
            member.Links.Remove(linkType);
            member.Links.Add(linkType, linkId);

            // Update
            _memberRepository.Update(member);
        }
    }
}
```

```
using MDP.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MDP.Members
{
    public static class MemberExtensions
    {
        // Methods
        public static ClaimsIdentity ToIdentity(this Member member, string authenticationType)
        {
            #region Contracts

            if (member == null) throw new ArgumentException($"{nameof(member)}=null");
            if (string.IsNullOrEmpty(authenticationType) == true) throw new ArgumentException($"{nameof(authenticationType)}=null");

            #endregion

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, member.MemberId),
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.Email, member.Mail),
                new Claim("Nickname", member.Nickname)
            }, authenticationType);

            // Links
            var linksValue = string.Empty;
            foreach (var link in member.Links)
            {
                linksValue += $"{link.Key}:{link.Value};";
            }
            claimsIdentity.AddClaim(new Claim("Links", linksValue));

            // Return
            return claimsIdentity;
        }
    }
}
```

```
"MDP.Members": {
  "MemberAuthenticationProvider": {}
}
```

7.在專案裡加入Controllers\AccountController.cs、Views\Account\Register.cshtml，用來提供會員註冊功能頁面。

```
using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.Line;
using MDP.Members;
using MDP.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1
{
    public partial class AccountController : Controller
    {
        // Methods
        [AllowAnonymous]
        public async Task<ActionResult> Register()
        {
            // DefaultValue
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity != null)
            {                
                this.ViewBag.Name = remoteIdentity?.GetClaimValue(ClaimTypes.Name);
                this.ViewBag.Mail = remoteIdentity?.GetClaimValue(ClaimTypes.Email);
                this.ViewBag.Nickname = remoteIdentity?.GetClaimValue(ClaimTypes.Name);
            }

            // Return
            return this.View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> RegisterMember(string name, string mail, string nickname, string password, string returnUrl = null)
        {
            // Member
            var member = new Member();
            member.MemberId = Guid.NewGuid().ToString();
            member.Name = name;
            member.Mail = mail;
            member.Nickname = nickname;

            // MemberLink
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity != null)
            {
                var linkType = remoteIdentity.AuthenticationType;
                var linkId = remoteIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
                member.Links.Add(linkType, linkId);
            }

            // Add
            _memberRepository.Add(member);

            // Return
            return await this.LoginAsync(member.ToIdentity("Password"), returnUrl);
        }
    }
}
```

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@{

}
<!DOCTYPE html>

<html>
<head>
    <!-- title -->
    <title>Register</title>

    <!-- meta -->
    <meta charset="utf-8" />
</head>
<body>

    <!--Title-->
    <h2>Register</h2>
    <hr />

    <!--Message-->
    <h3 style="color:red">@ViewBag.Message</h3>

    <!--RegisterMember-->
    <form asp-controller="Account" asp-action="RegisterMember" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        Name:<input type="text" name="name" value="@ViewBag.Name" /><br />
        Mail:<input type="text" name="mail" value="@ViewBag.Mail" /><br />
        Nickname:<input type="text" name="nickname" value="@ViewBag.Nickname" /><br />
        Password:<input type="text" name="password" value="" /><br />
        <input type="submit" value="RegisterMember" /><br />
        <br />
    </form>
    <hr />

</body>
</html>
```

8.在專案裡加入Controllers\AccountController.cs、Views\Account\Login.cshtml、Views\Account\AccessDenied.cshtml，用來提供基本的登入、登出、拒絕存取等功能頁面。

```
namespace WebApplication1
{
    public partial class AccountController : Controller
    {
        // Fields
        private readonly MemberRepository _memberRepository;


        // Constructors
        public AccountController(MemberRepository memberRepository)
        {
            // Default
            _memberRepository = memberRepository;
        }


        // Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            // Return
            return await this.LogoutAsync();
        }

        [AllowAnonymous]
        public Task<ActionResult> LoginByLine(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(LineDefaults.AuthenticationScheme, returnUrl);
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginByPassword(string username, string password, string returnUrl = null)
        {
            // Member: Ckeck Username + Password (for demo)
            var member = _memberRepository.FindByName(username);
            if (member == null)
            {
                // Message
                this.ViewBag.Message = "Login failed";

                // Return
                return this.View("Login");
            }

            // Return
            return await this.LoginAsync(member.ToIdentity("Password"), returnUrl);
        }
    }
}
```

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@{

}
<!DOCTYPE html>

<html>
<head>
    <!-- title -->
    <title>Login</title>

    <!-- meta -->
    <meta charset="utf-8" />
</head>
<body>

    <!--Title-->
    <h2>Login</h2>
    <hr />

    <!--Message-->
    <h3 style="color:red">@ViewBag.Message</h3>

    <!--LoginByPassword-->
    <form asp-controller="Account" asp-action="LoginByPassword" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        Username:<input type="text" name="username" value="Clark" /><br />
        Password:<input type="text" name="password" value="" /><br />
        <input type="submit" value="LoginByPassword" /><br />
        <br />
    </form>
    <hr />    

    <!--LoginByLine-->
    <form asp-controller="Account" asp-action="LoginByLine" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        <input type="submit" value="LoginByLine" /><br />
        <br />
    </form>
    <hr />

    <!--Register-->
    <form asp-controller="Account" asp-action="Register" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        <input type="submit" value="Register" /><br />
        <br />
    </form>
    <hr />

</body>
</html>
```

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@{

}
<!DOCTYPE html>

<html>
<head>
    <!-- title -->
    <title>AccessDenied</title>

    <!-- meta -->
    <meta charset="utf-8" />
</head>
<body>

    <!--Title-->
    <h2>AccessDenied</h2>
    <hr />

    <!--Login-->
    <form asp-controller="Account" asp-action="Login" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        <input type="submit" value="Login" /><br />
        <br />
    </form>
    <hr />

</body>
</html>
```

9.改寫專案內的Controllers\HomeController.cs、Views\Home\Index.cshtml，提供需登入才能進入的Home頁面，並於該頁面顯示目前登入User的身分資料。另外，此頁也提供了LinkToLine的功能按鈕，用來提供用戶於登入之後綁定Line帳號。

```
using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.Line;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1
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

        [AllowAnonymous]
        public Task<ActionResult> LinkByLine(string returnUrl = null)
        {
            // Return
            return this.LinkAsync(LineDefaults.AuthenticationScheme, returnUrl);
        }
    }
}
```

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using System.Security.Claims
@{
    string GetClaimValue(string claimType)
    {
        return (User?.Identity as ClaimsIdentity)?.FindFirst(claimType)?.Value;
    }
}
<!DOCTYPE html>

<html>
<head>
    <!-- title -->
    <title>Home</title>

    <!-- meta -->
    <meta charset="utf-8" />
</head>
<body>

    <!--Title-->
    <h2>Home</h2>
    <hr />

    <!--Identity-->
    AuthenticationType=@User?.Identity?.AuthenticationType<br />
    UserId=@GetClaimValue(ClaimTypes.NameIdentifier)<br />
    Username=@GetClaimValue(ClaimTypes.Name)<br />
    Mail=@GetClaimValue(ClaimTypes.Email)<br />    
    Nickname=@GetClaimValue("Nickname")<br />    
    Links=@GetClaimValue("Links")<br />    
    <br />
    <hr />

    <!--LinkByLine-->
    <form asp-controller="Home" asp-action="LinkByLine">
        <input type="submit" value="LinkByLine" /><br />
        <br />
    </form>
    <hr />

    <!--Logout-->
    <form asp-controller="Account" asp-action="Logout">
        <input type="submit" value="Logout" /><br />
        <br />
    </form>
    <hr />

</body>
</html>
```

10.執行專案，於開啟的Browser視窗內，可以看到系統畫面進入到Login頁面。(預設是開啟Home頁面，但是因為還沒登入，所以跳轉到Login頁面)

![02.LoginPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/02.LoginPage01.png)

11.於Login頁面，點擊Register按鈕。Browser視窗會跳轉至會員註冊的頁面，進行會員資料註冊。

![03.RegisterPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/03.RegisterPage01.png)

12.完成會員資料註冊之後，Browser視窗會跳轉回Home頁面，並且顯示目前User的身分資料。(無綁定身分)

![04.HomePage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/04.HomePage01.png)

13.於Home頁面，點擊LinkByLine按鈕。Browser視窗會跳轉至Line身分驗證服務的頁面，進行OAuth身分驗證。完成後進行會員身分綁定，最後再回到Home頁面，並於畫面上顯示會員資料的綁定身分。

![05.OAuthPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/05.OAuthPage01.png)

![05.OAuthPage02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/05.OAuthPage02.png)

14.完成上述步驟之後，於Home頁面點擊Logout按鈕。回到Login頁面，使用LoginByLine、LoginByPassword都可以登入已註冊的會員資料。(提醒：範例用程式裡LoginByPassword是用會員的Name屬性登入，並且不會檢查密碼。)

![06.ResultPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/06.ResultPage01.png)

![06.ResultPage02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/06.ResultPage02.png)