# MDP.AspNetCore

MDP.AspNetCore是開源的.NET開發套件，協助開發人員快速建立整合ASP.NET Core身分驗證的應用系統。提供OAuth身分驗證模組、Token身分驗證模組、Azure身分驗證模組，用以簡化開發流程並滿足多變的商業需求。

- 說明文件：[https://mdpnetcore.github.io/MDP.AspNetCore/](https://mdpnetcore.github.io/MDP.AspNetCore/)

- 程式源碼：[https://github.com/MDPNetCore/MDP.AspNetCore/](https://github.com/MDPNetCore/MDP.AspNetCore/)


## 快速開始

- [開發一個Line登入之後註冊會員的Web站台](https://mdpnetcore.github.io/MDP.AspNetCore/快速開始/開發一個Line登入之後註冊會員的Web站台/)

- [開發一個會員註冊之後綁定Line的Web站台](https://mdpnetcore.github.io/MDP.AspNetCore/快速開始/開發一個會員註冊之後綁定Line的Web站台/)

- [開發一個HMAC簽章+JwtBearer驗證的API服務](https://mdpnetcore.github.io/MDP.AspNetCore/快速開始/開發一個HMAC簽章+JwtBearer驗證的API服務/)

- [開發一個RSA簽章+ApiToken驗證的API服務](https://mdpnetcore.github.io/MDP.AspNetCore/快速開始/開發一個RSA簽章+ApiToken驗證的API服務/)


## 模組功能

![MDP.AspNetCore.Authentication-模組功能.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/MDP.AspNetCore.Authentication-模組功能.png)

### 模組掛載

MDP.AspNetCore.Authentication擴充ASP.NET Core既有的身分驗證，加入OAuth身分驗證模組、Token身分驗證模組、Azure身分驗證模組的掛載功能。開發人員可以透過Config設定，掛載在執行階段使用的身分認證。

- OAuth身分驗證：[OAuth身分認證模組清單](https://mdpnetcore.github.io/MDP.AspNetCore/OAuth身分驗證/)。

- Token身分驗證：[Token身分認證模組清單](https://mdpnetcore.github.io/MDP.AspNetCore/Token身分驗證/)。

- Azure身分驗證：[Azure身分認證模組清單](https://mdpnetcore.github.io/MDP.AspNetCore/Azure身分驗證/)。

```
// Config設定 - Line身分驗證模組
{
  "Authentication": {
    "Line": {
      "ClientId": "xxxxx",
      "ClientSecret": "xxxxx"
    }
  }
}
- 命名空間：Authentication
- 掛載的身分驗證模組：Line
- Line身分驗證模組的客戶編號：ClientId="xxxxx"。(xxxxx填入Channel ID)
- Line身分驗證模組的客戶密碼：ClientSecret="xxxxx"。(xxxxx填入Channel Secret)
```

```
// Config設定 - Jwt身分驗證模組
{
  "Authentication": {
    "Jwt": {
      "Credentials": [
        {
          "Scheme": "JwtBearer",
          "Header": "Authorization",
          "Prefix": "Bearer ",
          "Algorithm": "HS256",
          "SignKey": "12345678901234567890123456789012",
          "Issuer": "MDP"
        }
      ]
    }
  }
}
- 命名空間：Authentication
- 掛載的身分驗證模組：Jwt
- 憑證清單：Credentials
- 憑證名稱：Scheme="JwtBearer"。
- 憑證標頭：Header="Authorization"。(從HTTP Request的哪個Header取得Token，常見：Authorization、x-api-token)
- 憑證前綴：Prefix="Bearer "。(Token的前綴字，常見："Bearer "、"")
- 簽章算法：Algorithm="HS256"。(Token所使用的簽章演算法，支持：HSxxx、RSxxx)
- 簽章金鑰：SignKey="12345..."。(Token所使用的簽章金鑰，支持：Base64格式金鑰、PEM格式金鑰)
- 憑證發行：Issuer="MDP"。(檢核用，Token的核發單位)
```

```
// Config設定 - Azure身分驗證模組
{
  "Authentication": {
    "AzureAD.Users": {
      "TenantId": "xxxxx",
      "ClientId": "xxxxx",
      "ClientSecret": "xxxxx"
    }
  }
}

- 命名空間：Authentication
- 掛載的身分驗證模組：AzureAD.Users
- AzureAD.Users身分驗證模組的租戶編號：TenantId="xxxxx"。(xxxxx填入目錄 (租用戶) 識別碼)
- AzureAD.Users身分驗證模組的客戶編號：ClientId="xxxxx"。(xxxxx填入應用程式 (用戶端) 識別碼)
- AzureAD.Users身分驗證模組的客戶密碼：ClientSecret="xxxxx"。(xxxxx填入用戶端密碼)
```

### Remote身分驗證

![MDP.AspNetCore.Authentication-Remote身分驗證.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/MDP.AspNetCore.Authentication-Remote身分驗證.png)

MDP.AspNetCore.Authentication擴充ASP.NET Core既有的身分驗證，加入Remote身分驗證流程。用來確認通過OAuth身分驗證的身分資料，是否為已知用戶、是否引導註冊，並於最終完成登入。

- MDP.AspNetCore.Authentication加入Controller的擴充方法LoginAsync，用來發起Remote身分驗證流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationControllerExtensions

// 擴充方法
public static async Task<ActionResult> LoginAsync(this Controller controller, string scheme, string returnUrl = null)
- controller：執行的Controller物件。
- scheme：OAuth身分驗證的名稱。
- returnUrl：完成Remote身分驗證之後，要跳轉的功能頁面路徑。
- Task<ActionResult>：回傳值，流程跳轉頁面。
```

- 開發人員使用LoginAsync發起Remote身分驗證流程之後，系統就會依照輸入的scheme名稱進行OAuth身分驗證。完成之後，系統會將取得的身分資料拿來執行Remote身分登入，將身分資料寫入Cookie提供後續流程使用。(在這個階段還沒完成登入，開發人員在這個階段可以使用Controller的擴充方法RemoteAuthenticateAsync，來取得目前Remote身分登入的身分資料)

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationControllerExtensions

// 擴充方法
public static Task<ClaimsIdentity> RemoteAuthenticateAsync(this Controller controller)
- controller：執行的Controller物件。
- Task<ClaimsIdentity>：回傳值，目前Remote身分登入的身分資料。
```

- 完成Remote身分登入之後，系統會檢查是否有覆寫AuthenticationProvider的實作存在。有的話，會使用該實作覆寫的RemoteExchange方法，將Remote身分登入的身分資料轉換為本地的身分資料。轉換過程，可以依照專案需求比對會員資料庫、比對AD...用來確認身分資料。能確認身分資料的就回傳本地的身分資料進行Local身分登入，不能確認身分資料的則是回傳null進行後續流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationProvider

// 類別方法：
public virtual ClaimsIdentity RemoteExchange(ClaimsIdentity remoteIdentity)
- remoteIdentity：Remote身分登入的身分資料。
- Task<ClaimsIdentity>：回傳值，經過比對之後回傳本地的身分資料。
```

- Remote身分登入的身分資料轉換為本地的身分資料的過程中，發現不能確認身分資料的時候。系統會參考Config設定，沒有設定RegisterPath的系統則是會跳轉至拒絕存取頁面；有設定RegisterPath的系統會跳轉至該註冊頁面，並於註冊完畢將取得的身分資料拿來執行Local身分登入。

```
// Config設定
{
  "Authentication": {
    "RegisterPath": "/Account/Register"
  }
}
- 命名空間：Authentication
- 註冊頁面路徑：RegisterPath="/Account/Register"。(null是預設值，代表不須跳轉至註冊頁面)
```

- 當系統將取得的身分資料拿來執行Local身分登入，會將身分資料寫入Cookie提供後續流程使用。(在這個階段已經完成登入，開發人員在這個階段可以使用Controller的擴充方法RemoteAuthenticateAsync，來取得目前Remote身分登入的身分資料)

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationControllerExtensions

// 擴充方法
public static Task<ClaimsIdentity> LocalAuthenticateAsync(this Controller controller)
- controller：執行的Controller物件。
- Task<ClaimsIdentity>：回傳值，目前Local身分登入的身分資料。
```

### Remote身分綁定

![MDP.AspNetCore.Authentication-Remote身分綁定.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/MDP.AspNetCore.Authentication-Remote身分綁定.png)

完成登入之後，開發人員可以使用MDP.AspNetCore.Authentication提供的Remote身分綁定流程，用來綁定用戶所擁有的其他OAuth身分驗證。

- MDP.AspNetCore.Authentication加入Controller的擴充方法LinkAsync，用來發起Remote身分綁定流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationControllerExtensions

// 擴充方法
public static async Task<ActionResult> LinkAsync(this Controller controller, string scheme, string returnUrl = null)
- controller：執行的Controller物件。
- scheme：OAuth身分驗證的名稱。
- returnUrl：完成Remote身分綁定之後，要跳轉的功能頁面路徑。
- Task<ActionResult>：回傳值，流程跳轉頁面。
```

- 開發人員使用LinkAsync發起Remote身分綁定流程之後，系統就會依照輸入的scheme名稱進行OAuth身分驗證。完成之後，系統會將取得的身分資料拿來執行Remote身分登入，將身分資料寫入Cookie提供後續流程使用。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationControllerExtensions

// 擴充方法
public static Task<ClaimsIdentity> RemoteAuthenticateAsync(this Controller controller)
- controller：執行的Controller物件。
- Task<ClaimsIdentity>：回傳值，目前Remote身分登入的身分資料。
```

- 完成Remote身分登入之後，系統會檢查是否有覆寫AuthenticationProvider的實作存在。有的話，會使用該實作覆寫的RemoteLink方法，將Remote身分登入的身分資料與本地的身分資料進行綁定。綁定過程，可以依照專案需求將綁定資料寫入會員資料庫、寫入AD欄位...用來提供下次Remote身分驗證時使用。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationProvider

// 類別方法：
public virtual void RemoteLink(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
- remoteIdentity：Remote身分登入的身分資料。
- localIdentity：Local身分登入的身分資料。
```

### Local身分驗證

![MDP.AspNetCore.Authentication-Local身分驗證.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/MDP.AspNetCore.Authentication-Local身分驗證.png)

MDP.AspNetCore.Authentication擴充ASP.NET Core既有的身分驗證，加入Local身分驗證流程。用來讓開發人員使用資料庫、AD服務...驗證帳號及密碼之後，取得身分資料來執行Local身分登入，將身分資料寫入Cookie提供後續流程使用。

- MDP.AspNetCore.Authentication加入Controller的擴充方法LoginAsync，用來發起Local身分驗證流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class AuthenticationControllerExtensions

// 擴充方法
public static async Task<ActionResult> LoginAsync(this Controller controller, ClaimsIdentity localIdentity, string returnUrl = null)
- controller：執行的Controller物件。
- localIdentity：Local身分登入的身分資料。
- returnUrl：完成Remote身分驗證之後，要跳轉的功能頁面路徑。
- Task<ActionResult>：回傳值，流程跳轉頁面。
```

### Token身分驗證

![MDP.AspNetCore.Authentication-Token身分驗證.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/MDP.AspNetCore.Authentication-Token身分驗證.png)

MDP.AspNetCore.Authentication擴充ASP.NET Core既有的身分驗證，加入Token身分驗證流程。用來在HTTP Request封包內容通過Token身分驗證之後，取得身分資料來執行Token身分登入，將身分資料提供後續流程使用。(當次封包處理流程有效)

- 開發人員可以在HTTP Request封包裡加入代表身分資料的Token，用來發起Token身分驗證流程。

```
// HTTP headers - JwtBearer
Authorization:Bearer xxxxxxxxxxxxxxxx

// HTTP headers - ApiToken
X-Api-Token:xxxxxxxxxxxxxxxx
```


## 模組使用

### 加入專案

MDP.AspNetCore.Authentication預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication
```

### 設定參數

建立包含MDP.AspNetCore.Authentication模組的專案之後，在專案裡可以透過Config設定，掛載在執行階段使用的身分驗證及相關參數。

```
// Config設定 - Line身分驗證模組
{
  "Authentication": {
    "Line": {
      "ClientId": "xxxxx",
      "ClientSecret": "xxxxx"
    }
  }
}
- 命名空間：Authentication
- 掛載的身分驗證模組：Line
- Line身分驗證模組的客戶編號：ClientId="xxxxx"。(xxxxx填入Channel ID)
- Line身分驗證模組的客戶密碼：ClientSecret="xxxxx"。(xxxxx填入Channel Secret)
```

```
// Config設定
{
  "Authentication": {
    "RegisterPath": "/Account/Register"
  }
}
- 命名空間：Authentication
- 註冊頁面路徑：RegisterPath="/Account/Register"。(null是預設值，代表不須跳轉至註冊頁面)
```

### 註冊AuthenticationProvider

建立包含MDP.AspNetCore.Authentication模組的專案之後，在專案裡可以註冊AuthenticationProvider實作，來覆寫RemoteExchange、RemoteLink。

```
using MDP.AspNetCore.Authentication;

namespace MDP.Members
{
    [MDP.Registration.Service<AuthenticationProvider>(singleton: true)]
    public class MemberAuthenticationProvider : AuthenticationProvider
    {
        // Methods
        public override ClaimsIdentity RemoteExchange(ClaimsIdentity remoteIdentity)
        {
            // ...
        }

        public override void RemoteLink(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            // ...
        }
    }
}
```

```
{
  "MDP.Members": {
    "MemberAuthenticationProvider": {}
  }
}
```

## 模組範例

使用資料庫驗證帳號及密碼之後，取得身分資料來登入系統，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/WebApplication1.zip)

### 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

2.使用Visual Studio開啟WebApplication1專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.AspNetCore.Authentication
```

3.於專案內改寫appsettings.json，用以掛載MDP.AspNetCore.Authentication。

```
{
  "Authentication": {    
    
  }
}
```

4.在專案裡加入Modules\Member.cs、Modules\MemberRepository.cs，並改寫appsettings.json。用來掛載模擬的會員資料庫，提供會員資料的查詢。

```
using System;

namespace MDP.Members
{
    public class Member
    {
        // Properties
        public string MemberId { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;

        public string Mail { get; set; } = String.Empty;
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


        // Constructors
        public MemberRepository()
        {
            // DEMO用的假資料
            _memberList.Add(new Member() { MemberId = Guid.NewGuid().ToString(), Name = "Clark", Mail = "Clark@hotmail.com" });
            _memberList.Add(new Member() { MemberId = Guid.NewGuid().ToString(), Name = "Jane", Mail = "Jane@hotmail.com" });
        }


        // Methods
        public Member FindByPassword(string username, string password)
        {
            // DEMO用的範例。正式環境可以使用DB或AD進行驗證。(PS.儲存密碼記得雜湊處理)
            return _memberList.FirstOrDefault(o => o.Name == username);
        }
    }
}
```

```
{
  "MDP.Members": {
    "MemberRepository": {}
  }
}
```

5.在專案裡加入Controllers\AccountController.cs、Views\Account\Login.cshtml，用來提供Password登入功能頁面。

```
using MDP.AspNetCore.Authentication;
using MDP.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class AccountController : Controller
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
        public Task<ActionResult> Logout()
        {
            // Return
            return this.LogoutAsync();
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginByPassword(string username, string password, string returnUrl = null)
        {
            // Member
            var member = _memberRepository.FindByPassword(username, password);
            if (member == null)
            {
                // Message
                this.ViewBag.Message = "Login failed";

                // Return
                return this.View("Login");
            }

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, member.MemberId),
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.Email, member.Mail)
            }, "Password");

            // Return
            return await this.LoginAsync(claimsIdentity, returnUrl);
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

![01.LoginPage01.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/01.LoginPage01.png)

8.於Login頁面，點擊LoginByPassword按鈕，進行Password身分驗證。在完成身分驗證之後，Browser視窗會跳轉回Home頁面，並且顯示登入的身分資料。

![02.HomePage01.png](https://mdpnetcore.github.io/MDP.AspNetCore/功能說明/02.HomePage01.png)


## 版本更新

### MDP.AspNetCore.Authentication 6.1.13

- 跟隨MDP.NetCore 6.1.13版本更新。

### MDP.AspNetCore.Authentication 6.1.11

- 跟隨MDP.NetCore 6.1.11版本更新。

### MDP.AspNetCore.Authentication 6.1.8.4

- 加入MDP.AspNetCore.Authentication.AzureAD.Users，用來驗證AzureAD裡的使用者。

- 加入MDP.AspNetCore.Authentication.AzureAD.Services，用來驗證AzureAD裡的服務主體、受控識別。

### MDP.AspNetCore.Authentication 6.1.8.3

- 重構MDP.AspNetCore.Authentication.Jwt，調整RSA金鑰格式為PEM格式字串。

### MDP.AspNetCore.Authentication 6.1.8.2

- 重構MDP.AspNetCore.Authentication.Jwt，加入支援多組憑證的功能。

### MDP.AspNetCore.Authentication 6.1.8.1

- 重構AuthenticationProvider，讓他更容易被理解。

### MDP.AspNetCore.Authentication 6.1.8

- 加入Microsoft身分驗證。

- 加入AzureAD身分驗證。

- 重構MDP.AspNetCore.Authentication，簡化登入邏輯與流程。

### MDP.AspNetCore.Authentication 6.1.5

- 跟隨MDP.NetCore 6.1.5版本更新。