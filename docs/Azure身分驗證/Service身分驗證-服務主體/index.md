---
layout: default
title: Service身分驗證-服務主體
parent: Azure身分驗證
grand_parent: 身分驗證
nav_order: 2
has_children: false
---
    
# MDP.AspNetCore.Authentication.AzureAD.Services for Service Principal

MDP.AspNetCore.Authentication.AzureAD.Services擴充ASP.NET Core既有的身分驗證，加入AzureAD提供的Service身分驗證功能。開發人員可以透過Config設定，掛載在專案裡使用的Service身分驗證，用以驗證Azure裡的服務主體(Service Principal)。

- 說明文件：[https://clark159.github.io/MDP.AspNetCore.Authentication/](https://clark159.github.io/MDP.AspNetCore.Authentication/)

- 程式源碼：[https://github.com/Clark159/MDP.AspNetCore.Authentication/](https://github.com/Clark159/MDP.AspNetCore.Authentication/)

- 特別說明1：本篇範例的 API客戶端、API服務端，兩者皆可以在Azure環境之外部署使用。

- 特別說明2：本篇範例的 API客戶端必需持有Secret、API服務端無需持有Secret。


## 運作流程

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。下列兩個運作流程，說明AzureAD的憑證發放流程、服務驗證流程。(內容為簡化說明，完整細節可參考AzureAD文件)

### 憑證發放

![Service身分驗證-憑證發放.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/Service身分驗證-憑證發放.png)

1.開發人員至AzureAD，建立API Client的應用程式註冊。

2.開發人員從AzureAD，取得API Client的身分憑證，內容包含：TenantId、ClientId、ClientSecret。

3.開發人員將API Client的身分憑證，設定在API Client應用程式的Config參數。

4.開發人員至AzureAD，建立API Provider的應用程式註冊。

5.開發人員從AzureAD，取得API Provider的身分憑證，內容包含：TenantId、ClientId。(沒有ClientSecret)

6.開發人員將API Provider的身分憑證，設定在API Provider應用程式的Config參數。

### 服務驗證

![Service身分驗證-服務驗證.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/Service身分驗證-服務驗證.png)

1.使用者開啟API Client提供的URL。

2.API Client從Config參數，取得API Client的身分憑證發送給AzureAD，內容包含：TenantId、ClientId、ClientSecret。

3.AzureAD依照API Client的身分憑證，使用內建的公私鑰加密機制，回傳代表API Client的AccessToken。

4.API Client調用API，並且將AccessToken放置在API Request封包的Header。

5.API Provider從Config參數，取得API Provider的身分憑證發送給AzureAD，內容包含：TenantId、ClientId。(沒有ClientSecret)

6.AzureAD依照API Provider的身分憑證，使用內建的公私鑰管理機制，回傳可以驗證AccessToken的公鑰(Public Key)。

7.API Provider使用Public Key驗證AccessToken簽章，確認合法就依照系統邏輯回傳API Response。(不合法回傳401 Unauthorized)

8.API Client依照API Response，回傳Page給使用者。


## 模組使用-API服務端(API Provider)

### 申請服務

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。依照下列操作步驟，即可申請AzureAD提供給API服務端(API Provider)的身分憑證。

1.註冊並登入[Microsoft Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊應用程式註冊後，進入應用程式註冊頁面。於應用程式註冊頁面，點擊新增註冊按鈕，依照頁面提示建立一個Application。

![01.建立Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/01.建立Application01.png)

![01.建立Application02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/01.建立Application02.png)

![01.建立Application03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/01.建立Application03.png)

2.建立完畢後，於Application頁面，點擊新增應用程式識別碼 URI按鈕，進入公開API頁面，然後點擊新增，依照頁面提示建立一個「應用程式識別碼 URI」。建立完畢後，於Application頁面取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「應用程式識別碼 URI」。

![03.取得參數01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/03.取得參數01.png)

![03.取得參數02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/03.取得參數02.png)

![03.取得參數03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/03.取得參數03.png)

![03.取得參數04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/03.取得參數04.png)

3.於Application頁面，點擊左側選單的應用程式角色，進入應用程式角色頁面。然後點擊建立應用程式角色，依照頁面提示建立並取得一個「應用程式角色」。

![04.建立角色01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/04.建立角色01.png)

![04.建立角色02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/04.建立角色02.png)

![04.建立角色03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/04.建立角色03.png)

![04.建立角色04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/04.建立角色04.png)

### 加入專案

申請服務完成之後，就可以開始建立專案並且加入模組。MDP.AspNetCore.Authentication.AzureAD.Services預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication.AzureAD.Services的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n ApiProvider
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.AzureAD.Services
```

### 設定參數

建立包含MDP.AspNetCore.Authentication.AzureAD.Services的專案之後，就可以透過Config設定，掛載在專案裡使用的Service身分驗證。

```
// Config設定
{
  "Authentication": {
    "AzureAD.Services": {
      "TenantId": "xxxxx",
      "ClientId": "xxxxx"
    }
  }
}

- 命名空間：Authentication
- 掛載的身分驗證模組：AzureAD.Services
- API服務端的租戶編號：TenantId="xxxxx"。(xxxxx填入目錄 (租用戶) 識別碼)
- API服務端的客戶編號：ClientId="xxxxx"。(xxxxx填入應用程式 (用戶端) 識別碼)
```


## 模組使用-API客戶端(API Client)

### 申請服務(API Client)

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。依照下列操作步驟，即可申請AzureAD提供給API客戶端(API Client)的身分憑證。

1.註冊並登入[Microsoft Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊應用程式註冊後，進入應用程式註冊頁面。於應用程式註冊頁面，點擊新增註冊按鈕，依照頁面提示建立一個Application。

![11.建立Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/11.建立Application01.png)

![11.建立Application02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/11.建立Application02.png)

![11.建立Application03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/11.建立Application03.png)

2.建立完畢後，於Application頁面，取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」。接著點擊新增憑證或祕密按鈕，進入憑證及祕密頁面，然後點擊新增用戶端密碼，依照頁面提示建立並取得一個「用戶端密碼」。(記得要取「值」的內容)

![13.取得參數01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/13.取得參數01.png)

![13.取得參數02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/13.取得參數02.png)

![13.取得參數03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/13.取得參數03.png)

3.於Application頁面，點擊左側選單的API權限，進入API權限頁面。然後點擊新增權限，依照頁面提示，新增「應用程式角色」給應用程式。

![14.設定角色01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/14.設定角色01.png)

![14.設定角色02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/14.設定角色02.png)

![14.設定角色03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/14.設定角色03.png)

![14.設定角色04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/14.設定角色04.png)

4.停留於API權限頁面，點擊代表xxxx授予管理員同意按鈕，依照頁面提示授予API權限。(xxxx為目錄名稱)

![15.同意授權01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/15.同意授權01.png)

![15.同意授權02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/15.同意授權02.png)

![15.同意授權03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/15.同意授權03.png)

### 加入專案

申請服務完成之後，就可以開始建立專案並且加入模組。Azure.Identity預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入Azure.Identity的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n ApiClient
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
Azure.Identity
```

### 使用憑證

建立包含Azure.Identity的專案之後，就可以在程式碼裡使用憑證，建立代表API客戶端身分的AccessToken，用來通過API服務端的Service身分驗證。

```
// 參數設定
var azureCredential = new ClientSecretCredential
(
    tenantId: "xxxxx",
    clientId: "xxxxx",
    clientSecret: "xxxxx"
);
var apiProviderURI= "api://xxxxx";
var apiProviderEndpoint= "https://localhost:7146/Home/Index";

- API客戶端的租戶編號：tenantId: "xxxxx"。(xxxxx填入目錄 (租用戶) 識別碼)
- API客戶端的客戶編號：clientId: "xxxxx"。(xxxxx填入應用程式 (用戶端) 識別碼)
- API客戶端的客戶密碼：clientSecret: "xxxxx"。(xxxxx填入用戶端密碼)
- API服務端的應用程式識別碼URI: apiProviderURI= "api://xxxxx"。(xxxxx填入應用程式識別碼URI)
- API服務端的API服務端點: apiProviderEndpoint= "https://localhost:7146/Home/Index"。
```

```
// 建立AccessToken
var accessToken = (await azureCredential.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { $"{apiProviderURI}/.default" }), default)).Token;
```

```
// 呼叫API服務端點
var responseContent = string.Empty;
using (var httpClient = new HttpClient())
{
    // Headers
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

    // Send
    var response = await httpClient.GetAsync(apiProviderEndpoint);
    responseContent = await response?.Content?.ReadAsStringAsync();
}
```


## 模組範例

使用AzureAD提供的Service身分驗證功能，進行 Service to Service 之間的身分驗證，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.AzureAD.Services，逐步完成必要的設計和實作。

- 範例下載：[ApiClient.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Services身分驗證-服務主體/ApiClient.zip)

- 範例下載：[ApiProvider.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Services身分驗證-服務主體/ApiProvider.zip)

- 特別說明1：本篇範例的 API客戶端、API服務端，兩者皆可以在Azure環境之外部署使用。

- 特別說明2：本篇範例的 API客戶端必需持有Secret、API服務端無需持有Secret。

### 建立API服務端(API Provider)

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為ApiProvider的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n ApiProvider
```

2.使用Visual Studio開啟ApiProvider專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.AzureAD.Services
```

3.依照[模組使用-API服務端(API Provider)-申請服務](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/#模組使用-api服務端api-provider)的步驟流程，申請AzureAD提供的OAuth服務，並取得API服務端的：「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「應用程式識別碼 URI」。

![21.申請服務01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/21.申請服務01.png)

4.於專案內改寫appsettings.json，填入「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」，用以掛載Service身分驗證。

```
{
  "Authentication": {
    "AzureAD.Services": {
      "TenantId": "xxxxx", // API服務端-目錄 (租用戶) 識別碼
      "ClientId": "xxxxx"  // API服務端-應用程式 (用戶端) 識別碼
    }
  }
}
```

5.改寫專案內的Controllers\HomeController.cs，提供一個必須通過身分驗證才能使用的\Home\Index API服務端點。

```
using MDP.AspNetCore.Authentication.AzureAD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace ApiProvider
{
    public class HomeController : Controller
    {
        // Methods
        [Authorize]
        public string Index()
        {
            // ClaimsIdentity
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");
            Console.WriteLine($"this.User.AuthenticationType = {claimsIdentity.AuthenticationType}");
            Console.WriteLine($"this.User.TenantId = {claimsIdentity.FindFirst(AzureServicesAuthenticationClaimTypes.TenantId)?.Value}");
            Console.WriteLine($"this.User.ClientId = {claimsIdentity.FindFirst(AzureServicesAuthenticationClaimTypes.ClientId)?.Value}");
            Console.WriteLine($"this.User.Roles = {String.Join(",", claimsIdentity.FindAll(System.Security.Claims.ClaimTypes.Role).Select(o => o.Value))}");
            Console.WriteLine();
            
            // Return
            return "Hello World";
        }
    }
}
```

6.改寫專案內的Properties\launchSettings.json，設定Visual Studio開啟執行專案時，使用https://localhost:7146做為Web站台入口、並且不開啟Browser。

```
{
  "profiles": {
    "ApiProvider": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7146"
    }
  }
}
```

### 建立API客戶端(API Client)

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為ApiClient的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n ApiClient
```

2.使用Visual Studio開啟ApiClient專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
Azure.Identity
```

3.依照[模組使用-API客戶端(API Client)-申請服務](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/#模組使用-api客戶端api-client)的步驟流程，申請AzureAD提供的OAuth服務，並取得API客戶端的：「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「用戶端密碼」。

![21.申請服務02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/21.申請服務02.png)

4.改寫專案內的Controllers\HomeController.cs、Views\Home\Index.cshtml，提供Home頁面。並於Home頁面，使用憑證，建立代表API客戶端身分的AccessToken，用來通過API服務端的Service身分驗證後，取得資料顯示於頁面。

```
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClient
{
    public class HomeController : Controller
    {
        // Methods
        public async Task<ActionResult> Index()
        {
            // Variables
            var azureCredential = new ClientSecretCredential
            (
                tenantId: "xxxxx",    // API客戶端-目錄 (租用戶) 識別碼
                clientId: "xxxxx",    // API客戶端-應用程式 (用戶端) 識別碼
                clientSecret: "xxxxx" // API客戶端-用戶端密碼
            );
            var apiProviderURI = "api://xxxxx";                            // API服務端-應用程式識別碼 URI
            var apiProviderEndpoint = "https://localhost:7146/Home/Index"; // API服務端-API服務端點

            // AccessToken
            var accessToken = (await azureCredential.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { $"{apiProviderURI}/.default" }), default)).Token;
            if (string.IsNullOrEmpty(accessToken) == true) throw new InvalidOperationException($"{nameof(accessToken)}=null");

            // Call API
            var responseContent = string.Empty;
            using (var httpClient = new HttpClient())
            {
                // Headers
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // Send
                var response = await httpClient.GetAsync(apiProviderEndpoint);
                responseContent = await response?.Content?.ReadAsStringAsync();
            }
            if (string.IsNullOrEmpty(responseContent) == true) throw new InvalidOperationException($"{nameof(responseContent)}=null");

            // ViewBag
            this.ViewBag.Message = responseContent;

            // Return
            return View();
        }
    }
}
```

```
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>ApiClient</title>
</head>
<body>

    <!--Title-->
    <h2>ApiClient</h2>
    <hr />

    <!--Message-->
    <h3>@ViewBag.Message</h3>

</body>
</html>
```

5.改寫專案內的Properties\launchSettings.json，設定Visual Studio開啟執行專案時，使用https://localhost:7147做為Web站台入口、並且開啟Browser。

```
{
  "profiles": {
    "ApiClient": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7147"
    }
  }
}
```

### 範例執行

1.使用Visual Studio開啟ApiProvider專案並執行。

2.使用Visual Studio開啟ApiClient專案並執行。

3.於ApiClient專案，執行所開啟的Browser視窗內，可以看到系統畫面進入到Home頁面，並且顯示API服務端回傳的"Hello World"。

![22.執行結果01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/22.執行結果01.png)

4.於ApiProvider專案，執行所開啟的Console視窗內，可以看到通過Service身分驗證的API客戶端身分資料(Controller.User屬性)。

![22.執行結果02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-服務主體/22.執行結果02.png)