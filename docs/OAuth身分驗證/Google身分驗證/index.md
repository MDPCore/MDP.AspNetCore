---
layout: default
title: Google身分驗證
parent: OAuth身分驗證
grand_parent: 身分驗證
nav_order: 2
has_children: false
---

# MDP.AspNetCore.Authentication.Google

MDP.AspNetCore.Authentication.Google擴充ASP.NET Core既有的身分驗證，加入Google身分驗證功能。開發人員可以透過Config設定，掛載在專案裡使用的Google身分驗證。

- 說明文件：[https://clark159.github.io/MDP.AspNetCore.Authentication/](https://clark159.github.io/MDP.AspNetCore.Authentication/)

- 程式源碼：[https://google.com/Clark159/MDP.AspNetCore.Authentication/](https://google.com/Clark159/MDP.AspNetCore.Authentication/)


## 模組使用

### 申請服務

MDP.AspNetCore.Authentication.Google使用Google官方提供的OAuth服務來進行身分驗證，依照下列操作步驟，即可申請官方所提供的OAuth服務。

1.註冊並登入[Google Cloud Console](https://console.cloud.google.com/)。於首頁，點擊左上角的專案清單後，點擊建立專案按鈕，依照頁面提示建立一個Project。

![01.建立Project01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/01.建立Project01.png)

![01.建立Project02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/01.建立Project02.png)

2.於Project頁面，點選左上角的選單，進入API和服務/OAuth同意畫面頁面，依照頁面提示建立一個OAuth同意畫面。

![02.建立OAuth同意畫面01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/02.建立OAuth同意畫面01.png)

![02.建立OAuth同意畫面02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/02.建立OAuth同意畫面02.png)

![02.建立OAuth同意畫面03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/02.建立OAuth同意畫面03.png)

3.於OAuth同意畫面，點選左上角的選單，進入API和服務/憑證頁面，點選建立憑證選單後，選擇建立OAuth用戶端ID，依照頁面提示建立一個OAuth用戶端，並加入一個「已授權的重新導向 URI」。(重新導向URL=「程式執行網址」+「/.auth/login/google/callback」)

![03.建立OAuth用戶端01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/03.建立OAuth用戶端01.png)

![03.建立OAuth用戶端02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/03.建立OAuth用戶端02.png)

![03.建立OAuth用戶端03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/03.建立OAuth用戶端03.png)

![03.建立OAuth用戶端04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/03.建立OAuth用戶端04.png)

4.於OAuth用戶端建立完成頁面，取得「用戶端編號」、「用戶端密碼」。

![04.完成OAuth用戶端01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/04.完成OAuth用戶端01.png)

### 加入專案

服務申請完成之後，就可以開始建立專案並且加入模組。MDP.AspNetCore.Authentication.Google預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication.Google的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
// 建立API服務、Web站台
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.Google
```

### 設定參數

建立包含MDP.AspNetCore.Authentication.Google的專案之後，就可以透過Config設定，掛載在專案裡使用的Google身分驗證。

```
// Config設定
{
  "Authentication": {
    "Google": {
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}

- 命名空間：Authentication
- 掛載的身分驗證模組：Google
- Google身分驗證模組的客戶編號：ClientId="Xxxxx"。(Xxxxx填入用戶端編號)
- Google身分驗證模組的客戶密碼：ClientSecret="Xxxxx"。(Xxxxx填入用戶端密碼)
```


## 模組範例

提供Google身分驗證，讓使用者能夠快速登入系統，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.Google，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/WebApplication1.zip)

### 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

2.使用Visual Studio開啟WebApplication1專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.Google
```

3.依照[服務申請](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/#服務申請)的步驟流程，申請Google身分驗證服務，並取得「用戶端編號」、「用戶端密碼」。

![05.申請服務01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/05.申請服務01.png)

4.於專案內改寫appsettings.json，填入「用戶端編號」、「用戶端密碼」，用以掛載Google身分驗證。

```
{
  "Authentication": {
    "Google": {
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}
```
    
5.改寫專案內的Controllers\AccountController.cs、Views\Account\Login.cshtml，提供Login頁面及Google身分驗證功能。

```
using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.Google;
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
        public Task<ActionResult> LoginByGoogle(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(GoogleDefaults.AuthenticationScheme, returnUrl);
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

    <!--LoginByGoogle-->
    <form asp-controller="Account" asp-action="LoginByGoogle" asp-route-returnUrl="@Context.Request.Query["ReturnUrl"]" method="post">
        <input type="submit" value="LoginByGoogle" /><br />
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

![06.LoginPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/06.LoginPage01.png)

8.於Login頁面，點擊LoginByGoogle按鈕。Browser視窗會跳轉至Google身分驗證服務的頁面，進行OAuth身分驗證。

![07.OAuthPage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/07.OAuthPage01.png)

9.於Google身分驗證服務完成身分驗證之後，Browser視窗會跳轉回原系統的Home頁面，並且顯示登入的身分資料。(經由Google身分驗證登入)

![08.HomePage01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/Google身分驗證/08.HomePage01.png)