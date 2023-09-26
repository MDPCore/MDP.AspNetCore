---
layout: default
title: AzureAD身分驗證
parent: OAuth身分驗證
grand_parent: 身分驗證
nav_order: 5
has_children: false
---

# MDP.AspNetCore.Authentication.AzureAD

MDP.AspNetCore.Authentication.AzureAD擴充ASP.NET Core既有的身分驗證，加入AzureAD身分驗證功能。開發人員可以透過Config設定，掛載在專案裡使用的AzureAD身分驗證。

- 說明文件：[https://clark159.github.io/MDP.AspNetCore.Authentication/](https://clark159.github.io/MDP.AspNetCore.Authentication/)

- 程式源碼：[https://github.com/Clark159/MDP.AspNetCore.Authentication/](https://github.com/Clark159/MDP.AspNetCore.Authentication/)


## 模組使用

### 申請服務

MDP.AspNetCore.Authentication.AzureAD使用Azure官方提供的OAuth服務來進行身分驗證，依照下列操作步驟，即可申請官方所提供的OAuth服務。

1.註冊並登入[AzureAD Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊應用程式註冊後，進入應用程式註冊頁面。

![01.建立Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/01.建立Application01.png)

2.於應用程式註冊頁面，點擊新增註冊按鈕，依照頁面提示建立一個Application，並編輯「支援的帳戶類型」及「重新導向 URI」。(支援的帳戶類型=僅此組織目錄中的帳戶、重新導向平台=Web、重新導向 URI=「程式執行網址」+「/.auth/login/aad/callback」)

![02.註冊Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/02.註冊Application01.png)

![02.註冊Application02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/02.註冊Application02.png)

![02.註冊Application03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/02.註冊Application03.png)

3.於Application頁面，取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」。接著點擊新增憑證或祕密按鈕，進入憑證及祕密頁面，然後點擊新增用戶端密碼，依照頁面提示建立並取得一個「用戶端密碼」。(記得要取「值」的內容)

![03.取得參數01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/03.取得參數01.png)

![03.取得參數02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/03.取得參數02.png)

![03.取得參數03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/03.取得參數03.png)

### 加入專案

服務申請完成之後，就可以開始建立專案並且加入模組。MDP.AspNetCore.Authentication.AzureAD預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication.AzureAD的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
// 建立API服務、Web站台
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.AzureAD
```

### 設定參數

建立包含MDP.AspNetCore.Authentication.AzureAD的專案之後，就可以透過Config設定，掛載在專案裡使用的AzureAD身分驗證。

```
// Config設定
{
  "Authentication": {
    "AzureAD": {
      "TenantId": "Xxxxx",
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}

- 命名空間：Authentication
- 掛載的身分驗證模組：AzureAD
- AzureAD身分驗證模組的租戶編號：TenantId="Xxxxx"。(Xxxxx填入目錄 (租用戶) 識別碼)
- AzureAD身分驗證模組的客戶編號：ClientId="Xxxxx"。(Xxxxx填入應用程式 (用戶端) 識別碼)
- AzureAD身分驗證模組的客戶密碼：ClientSecret="Xxxxx"。(Xxxxx填入用戶端密碼)
```


## 模組範例

專案開發過程，需要提供AzureAD身分驗證，讓使用者能夠快速登入系統。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.AzureAD，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/WebApplication1.zip)

### 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

2.使用Visual Studio開啟WebApplication1專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.AzureAD
```

3.依照[服務申請](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/#服務申請)的步驟流程，申請AzureAD身分驗證服務，並取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「用戶端密碼」。

![05.申請服務01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/05.申請服務01.png)

4.於專案內改寫appsettings.json，填入「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「用戶端密碼」，用以掛載AzureAD身分驗證。

```
{
  "Authentication": {
    "AzureAD": {
      "TenantId": "Xxxxx",
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}
```
    
5.改寫專案內的Controllers\AccountController.cs、Views\Account\Login.cshtml，提供Login頁面及AzureAD身分驗證功能。

```
using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.AzureAD;
using AzureAD.AspNetCore.Authorization;
using AzureAD.AspNetCore.Mvc;
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
        public Task<ActionResult> LoginByAzureAD(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(AzureADDefaults.AuthenticationScheme, returnUrl);
        }
    }
}
```

```
@addTagHelper *, AzureAD.AspNetCore.Mvc.TagHelpers
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

    <!--LoginByAzureAD-->
    <form asp-controller="Account" asp-action="LoginByAzureAD" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        <input type="submit" value="LoginByAzureAD" /><br />
        <br />
    </form>
    <hr />

</body>
</html>
```

6.改寫專案內的Controllers\HomeController.cs、Views\Home\Index.cshtml，提供需登入才能進入的Home頁面，並於該頁面顯示目前登入User的身分資料。

```
using AzureAD.AspNetCore.Authorization;
using AzureAD.AspNetCore.Mvc;

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
@addTagHelper *, AzureAD.AspNetCore.Mvc.TagHelpers
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

![06.LoginPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/06.LoginPage01.png)

8.於Login頁面，點擊LoginByAzureAD按鈕。Browser視窗會跳轉至AzureAD身分驗證服務的頁面，進行OAuth身分驗證。

![07.OAuthPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/07.OAuthPage01.png)

9.於AzureAD身分驗證服務完成身分驗證之後，Browser視窗會跳轉回原系統的Home頁面，並且顯示目前User的身分資料。(經由AzureAD身分驗證登入)

![08.HomePage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/AzureAD身分驗證/08.HomePage01.png)