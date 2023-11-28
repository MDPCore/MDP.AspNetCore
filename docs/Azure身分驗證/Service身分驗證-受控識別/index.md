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

2.建立完畢後，於Application頁面，點擊新增應用程式識別碼 URI按鈕，進入公開API頁面，然後點擊新增，依照頁面提示建立一個「應用程式識別碼 URI」。建立完畢後，於Application頁面取得：API服務端的「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」、「應用程式識別碼 URI」。

![03.取得參數01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數01.png)

![03.取得參數02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數02.png)

![03.取得參數03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數03.png)

![03.取得參數04.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/03.取得參數04.png)

3.於Application頁面，點擊左側選單的應用程式角色，進入應用程式角色頁面。然後點擊建立應用程式角色，依照頁面提示建立一個「應用程式角色」。建立完畢後，於應用程式角色頁面取得：API服務端的「應用程式角色識別碼」。

![04.建立角色01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色01.png)

![04.建立角色02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色02.png)

![04.建立角色03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/04.建立角色03.png)

4.於Application頁面，點擊中頁面內容的本機目錄中受控的應用程式，進入企業應用程式頁面。並於企業應用程式頁面，取得：API服務端的「物件識別碼」。

![05.取得識別碼01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/05.取得識別碼01.png)

![05.取得識別碼02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/05.取得識別碼02.png)

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
- API服務端的租戶編號：TenantId="xxxxx"。(xxxxx填入API服務端的「目錄 (租用戶) 識別碼」)
- API服務端的客戶編號：ClientId="xxxxx"。(xxxxx填入API服務端的「應用程式 (用戶端) 識別碼」)
```


## 模組使用-API客戶端(API Client)

### 申請服務(API Client)

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。依照下列操作步驟，即可申請API客戶端(API Client)的受控識別，用以提供身分憑證。

1.建立Azure資源(例如：Azure VM、App Service、Container Apps)，用來執行API客戶端程式。

![11.建立受控識別01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/11.建立受控識別01.png)

2.執行API客戶端程式的Azure資源，建立完畢之後。進入該Azure資源頁面，點擊左側選單的身分識別，進入身分識別頁面。然後點擊開啟，依照頁面提示開啟系統指派的受控識別，並取得API客戶端的「物件 (主體) 識別碼」。

![11.建立受控識別02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/11.建立受控識別02.png)

![11.建立受控識別03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/11.建立受控識別03.png)

3.回到[Azure Portal](https://portal.azure.com/)。於右上角的選單裡，點擊Cloud Shell按鈕後，開啟Cloud Shell視窗。於Cloud Shell視窗，切換至Bash並執行下列指令，新增API服務端的「應用程式角色」給API客戶端。

```
az rest \
-m POST \
--headers "Content-Type=application/json" \
-u "https://graph.microsoft.com/v1.0/servicePrincipals/xxxClient-PrincipalIdxxx/appRoleAssignments" \
-b "{
    \"principalId\": \"xxxClient-PrincipalIdxxx\", 
    \"resourceId\": \"xxxProvider-ResourceIdxxx\", 
    \"appRoleId\": \"xxxProvider-AppRoleIdxxx\"
}"

- API客戶端的物件識別碼：xxxClient-PrincipalIdxxx。(xxxClient-PrincipalIdxxx填入先前取得的API客戶端「物件 (主體) 識別碼」。注意!有兩個地方要改。)
- API服務端的物件識別碼：xxxProvider-ResourceIdxxx。(xxxProvider-ResourceIdxxx填入先前取得的API服務端「物件識別碼」)
- API服務端的角色識別碼：xxxProvider-AppRoleIdxxx。(xxxProvider-AppRoleIdxxx填入先前取得的API服務端「應用程式角色識別碼」)
```

![12.建立授權01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/12.建立授權01.png)

![12.建立授權02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/12.建立授權02.png)

4.回到[Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊企業應用程式後，進入企業應用程式頁面。於企業應用程式頁面，可以找到API客戶端(API Client)的受控識別。點擊後，進入API客戶端(API Client)的受控識別頁面，選擇左側選單裡的權限頁籤，可以看到授權給API客戶端的權限。(這頁也可以撤銷權限)

![13.檢視授權01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/13.檢視授權01.png)

![13.檢視授權02.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/13.檢視授權02.png)

![13.檢視授權03.png](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Service身分驗證-受控識別/13.檢視授權03.png)

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

建立包含Azure.Identity的專案之後，就可以在程式碼裡使用受控識別，建立代表API客戶端身分的AccessToken，用來通過API服務端的Service身分驗證。

```
// 參數設定
var azureCredential = new DefaultAzureCredential();
var apiProviderURI= "api://xxxxx";
var apiProviderEndpoint= "https://localhost:7146/Home/Index";

- API服務端的應用程式識別碼URI: apiProviderURI= "api://xxxxx"。(xxxxx填入API服務端的「應用程式識別碼URI」)
- API服務端的API服務端點: apiProviderEndpoint= "https://localhost:7146/Home/Index"。(https://localhost:7146/Home/Index替換為API服務端的「API服務端點」)
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

- 範例下載：[ApiClient.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Services身分驗證-受控識別/ApiClient.zip)

- 範例下載：[ApiProvider.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/Azure身分驗證/Services身分驗證-受控識別/ApiProvider.zip)

- 特別說明1：本篇範例的 API客戶端必需在Azure環境佈署使用、API服務端不限制在Azure環境部署使用。

- 特別說明2：本篇範例的 API客戶端、API服務端，兩者皆無需持有Secret。

### 建立API服務端(API Provider)

### 建立API客戶端(API Client)

### 範例執行