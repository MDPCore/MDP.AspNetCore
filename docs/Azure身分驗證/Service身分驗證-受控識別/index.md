---
layout: default
title: Service身分驗證-受控識別
parent: Azure身分驗證
grand_parent: 身分驗證
nav_order: 3
has_children: false
---

# MDP.AspNetCore.Authentication.AzureAD.Services for Managed Identity

MDP.AspNetCore.Authentication.AzureAD.Services擴充ASP.NET Core既有的身分驗證，加入AzureAD提供的Service身分驗證功能。開發人員可以透過Config設定，掛載在專案裡使用的Service身分驗證，用以驗證Azure裡的受控識別(Managed Identity)。

- 說明文件：[https://clark159.github.io/MDP.AspNetCore.Authentication/](https://clark159.github.io/MDP.AspNetCore.Authentication/)

- 程式源碼：[https://github.com/Clark159/MDP.AspNetCore.Authentication/](https://github.com/Clark159/MDP.AspNetCore.Authentication/)

- 特別說明1：本篇範例的 API客戶端必需在Azure環境佈署使用、API服務端不限制在Azure環境部署使用。

- 特別說明2：本篇範例的 API客戶端、API服務端，兩者皆無需持有Secret。


## 運作流程

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。下列兩個運作流程，說明AzureAD的憑證發放流程、服務驗證流程。(內容為簡化說明，完整細節可參考AzureAD文件)

### 憑證發放

### 服務驗證


## 模組使用-API服務端(API Provider)

### 申請服務

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。依照下列操作步驟，即可申請AzureAD提供給API服務端(API Provider)的身分憑證。

1.註冊並登入[Microsoft Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊應用程式註冊後，進入應用程式註冊頁面。於應用程式註冊頁面，點擊新增註冊按鈕，依照頁面提示建立一個Application。

![01.建立Application01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/01.建立Application01.png)

![01.建立Application02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/01.建立Application02.png)

![01.建立Application03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/01.建立Application03.png)

2.建立完畢後，於Application頁面，點擊新增應用程式識別碼 URI按鈕，進入公開API頁面，然後點擊新增，依照頁面提示建立一個「應用程式識別碼 URI」。建立完畢後，於Application頁面取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「應用程式識別碼 URI」。

![03.取得參數01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數01.png)

![03.取得參數02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數02.png)

![03.取得參數03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數03.png)

![03.取得參數04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數04.png)

3.於Application頁面，點擊左側選單的應用程式角色，進入應用程式角色頁面。然後點擊建立應用程式角色，依照頁面提示建立並取得一個「應用程式角色」。

![04.建立角色01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色01.png)

![04.建立角色02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色02.png)

![04.建立角色03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色03.png)

![04.建立角色04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色04.png)

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

### 加入專案

### 使用憑證


## 模組範例

使用AzureAD提供的Service身分驗證功能，進行 Service to Service 之間的身分驗證，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.AzureAD.Services，逐步完成必要的設計和實作。

- 範例下載：[ApiClient.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Services身分驗證-受控識別/ApiClient.zip)

- 範例下載：[ApiProvider.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Services身分驗證-受控識別/ApiProvider.zip)

- 特別說明1：本篇範例的 API客戶端必需在Azure環境佈署使用、API服務端不限制在Azure環境部署使用。

- 特別說明2：本篇範例的 API客戶端、API服務端，兩者皆無需持有Secret。

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

3.依照[模組使用-API服務端(API Provider)-申請服務](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/#模組使用-api服務端api-provider)的步驟流程，申請AzureAD提供的OAuth服務，並取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「應用程式識別碼 URI」。

![21.申請服務01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/21.申請服務01.png)

4.於專案內改寫appsettings.json，填入「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」，用以掛載Service身分驗證。

```
{
  "Authentication": {
    "AzureAD.Services": {
      "TenantId": "xxxxx", // API Provider-目錄 (租用戶) 識別碼
      "ClientId": "xxxxx"  // API Provider-應用程式 (用戶端) 識別碼
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

### 建立API客戶端(API Client)

### 範例執行