---
layout: default
title: Facebook身分驗證
parent: OAuth身分驗證
grand_parent: 身分驗證
nav_order: 3
has_children: false
---

# MDP.AspNetCore.Authentication.Facebook

MDP.AspNetCore.Authentication.Facebook擴充ASP.NET Core既有的身分驗證，加入Facebook身分驗證功能。開發人員可以透過Config設定，掛載在專案裡使用的Facebook身分驗證。

- 說明文件：[https://clark159.github.io/MDP.AspNetCore.Authentication/](https://clark159.github.io/MDP.AspNetCore.Authentication/)

- 程式源碼：[https://Facebook.com/Clark159/MDP.AspNetCore.Authentication/](https://Facebook.com/Clark159/MDP.AspNetCore.Authentication/)


## 模組使用

### 申請服務

MDP.AspNetCore.Authentication.Facebook使用Facebook官方提供的OAuth服務來進行身分驗證，依照下列操作步驟，即可申請官方所提供的OAuth服務。

1.註冊並登入[Facebook Developers Console](https://developers.facebook.com/apps/)。於應用程式頁面，點擊右上角的建立應用程式按鈕，依照頁面提示建立一個Application。

![01.建立Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/01.建立Application01.png)

![01.建立Application02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/01.建立Application02.png)

![01.建立Application03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/01.建立Application03.png)

2.於Application頁面，點擊左側選單，進入產品頁面後，點擊Facebook登入的設定按鈕，依照頁面提示設定「有效的OAuth重新導向URI」。(有效的OAuth重新導向URI=「程式執行網址」+「/.auth/login/facebook/callback」)

![02.設定Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/02.設定Application01.png)

![02.設定Application02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/02.設定Application02.png)

![02.設定Application03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/02.設定Application03.png)

3.同樣於Application頁面，點擊左側選單，進入使用案例頁面後，點擊驗證和帳號建立流程的編輯按鈕，依照頁面提示新增email權限。

![03.設定Scope01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/03.設定Scope01.png)

![03.設定Scope02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/03.設定Scope02.png)

4.同樣於Application頁面，點擊左側選單，進入應用程式設定/基本資料頁面後，依照頁面提示取得「應用程式編號」、「應用程式密鑰」。

![04.取得參數01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/04.取得參數01.png)

### 加入專案

服務申請完成之後，就可以開始建立專案並且加入模組。MDP.AspNetCore.Authentication.Facebook預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication.Facebook的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
// 建立API服務、Web站台
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.Facebook
```

### 設定參數

建立包含MDP.AspNetCore.Authentication.Facebook的專案之後，就可以透過Config設定，掛載在專案裡使用的Facebook身分驗證。

```
// Config設定
{
  "Authentication": {
    "Facebook": {
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}

- 命名空間：Authentication
- 掛載的身分驗證模組：Facebook
- Facebook身分驗證模組的客戶編號：ClientId="Xxxxx"。(Xxxxx填入應用程式編號)
- Facebook身分驗證模組的客戶密碼：ClientSecret="Xxxxx"。(Xxxxx填入應用程式密鑰)
```


## 模組範例

提供Facebook身分驗證，讓使用者能夠快速登入系統，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.Facebook，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/WebApplication1.zip)

### 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

2.使用Visual Studio開啟WebApplication1專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.Facebook
```

3.依照[服務申請](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/#服務申請)的步驟流程，申請Facebook身分驗證服務，並取得「應用程式編號」、「應用程式密鑰」。

![05.申請服務01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/05.申請服務01.png)

4.於專案內改寫appsettings.json，填入「應用程式編號」、「應用程式密鑰」，用以掛載Facebook身分驗證。

```
{
  "Authentication": {
    "Facebook": {
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}
```
    
5.改寫專案內的Controllers\AccountController.cs、Views\Account\Login.cshtml，提供Login頁面及Facebook身分驗證功能。

```
using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class AccountController : Controller
    {
        // Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public Task<ActionResult> Logout()
        {
            // Return
            return this.LogoutAsync();
        }

        [AllowAnonymous]
        public Task<ActionResult> LoginByFacebook(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(FacebookDefaults.AuthenticationScheme, returnUrl);
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

    <!--LoginByFacebook-->
    <form asp-controller="Account" asp-action="LoginByFacebook" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        <input type="submit" value="LoginByFacebook" /><br />
        <br />
    </form>
    <hr />

</body>
</html>
```

6.改寫專案內的Controllers\HomeController.cs、Views\Home\Index.cshtml，提供需登入才能進入的Home頁面，並於該頁面顯示目前登入的身分資料。

```
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    <br />
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

7.執行專案，於開啟的Browser視窗內，可以看到系統畫面進入到Login頁面。(預設是開啟Home頁面，但是因為還沒登入，所以跳轉到Login頁面)

![06.LoginPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/06.LoginPage01.png)

8.於Login頁面，點擊LoginByFacebook按鈕。Browser視窗會跳轉至Facebook身分驗證服務的頁面，進行OAuth身分驗證。

![07.OAuthPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/07.OAuthPage01.png)

9.於Facebook身分驗證服務完成身分驗證之後，Browser視窗會跳轉回原系統的Home頁面，並且顯示登入的身分資料。(經由Facebook身分驗證登入)

![08.HomePage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Facebook身分驗證/08.HomePage01.png)