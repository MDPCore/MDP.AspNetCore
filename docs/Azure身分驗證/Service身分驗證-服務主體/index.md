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

## 運作流程

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。下列兩個運作流程，說明AzureAD的憑證發放流程、服務驗證流程。(內容為簡化說明，完整細節可參考AzureAD文件)

### 憑證發放

![Service身分驗證-憑證發放.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/Service身分驗證-憑證發放.png)

1.開發人員至AzureAD，建立API Client的應用程式註冊。

2.開發人員從AzureAD，取得API Client的身分憑證，內容包含：TenantId、ClientId、ClientSecret。

3.開發人員將API Client的身分憑證，設定在API Client應用程式的Config參數。

4.開發人員至AzureAD，建立API Provider的應用程式註冊。

5.開發人員從AzureAD，取得API Provider的身分憑證，內容包含：TenantId、ClientId。(沒有ClientSecret)

6.開發人員將API Provider的身分憑證，設定在API Provider應用程式的Config參數。

### 服務驗證

![Service身分驗證-服務驗證.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/Service身分驗證-服務驗證.png)

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

1.註冊並登入[Microsoft Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊應用程式註冊後，進入應用程式註冊頁面。

![01.建立Application01.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/01.建立Application01.png)

2.於應用程式註冊頁面，點擊新增註冊按鈕，依照頁面提示建立一個Application，命名為ApiProvider，並編輯「支援的帳戶類型」。(支援的帳戶類型=僅此組織目錄中的帳戶)

![02.註冊Application01.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/02.註冊Application01.png)

![02.註冊Application02.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/02.註冊Application02.png)

3.於Application頁面，取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」。接著點擊新增應用程式識別碼 URI按鈕，進入公開API頁面，然後點擊新增，依照頁面提示建立並取得一個「應用程式識別碼 URI」。

![03.取得參數01.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/03.取得參數01.png)

![03.取得參數02.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/03.取得參數02.png)

![03.取得參數03.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/03.取得參數03.png)

![03.取得參數04.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/03.取得參數04.png)


### 加入專案

服務申請完成之後，就可以開始建立專案並且加入模組。MDP.AspNetCore.Authentication.AzureAD.Services預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication.AzureAD.Services的專案。

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
      "TenantId": "Xxxxx",
      "ClientId": "Xxxxx"
    }
  }
}

- 命名空間：Authentication
- 掛載的身分驗證模組：AzureAD.Services
- API Provider的租戶編號：TenantId="Xxxxx"。(Xxxxx填入目錄 (租用戶) 識別碼)
- API Provider的客戶編號：ClientId="Xxxxx"。(Xxxxx填入應用程式 (用戶端) 識別碼)
```


## 模組使用-API客戶端(API Client)

### 申請服務

MDP.AspNetCore.Authentication.AzureAD.Services使用AzureAD提供的OAuth服務，透過Client Credentials流程來進行Service身分驗證。依照下列操作步驟，即可申請AzureAD提供給API客戶端(API Client)的身分憑證。

1.註冊並登入[Microsoft Azure Portal](https://portal.azure.com/)。於首頁左上角的選單裡，點擊應用程式註冊後，進入應用程式註冊頁面。

![11.建立Application01.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/11.建立Application01.png)

2.於應用程式註冊頁面，點擊新增註冊按鈕，依照頁面提示建立一個Application，命名為ApiClient，並編輯「支援的帳戶類型」。(支援的帳戶類型=僅此組織目錄中的帳戶)

![12.註冊Application01.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/12.註冊Application01.png)

![12.註冊Application02.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/12.註冊Application02.png)

3.於Application頁面，取得「目錄 (租用戶) 識別碼」、「應用程式 (用戶端) 識別碼」。接著點擊新增憑證或祕密按鈕，進入憑證及祕密頁面，然後點擊新增用戶端密碼，依照頁面提示建立並取得一個「用戶端密碼」。(記得要取「值」的內容)

![13.取得參數01.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/13.取得參數01.png)

![13.取得參數02.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/13.取得參數02.png)

![13.取得參數03.png](https://clark159.github.io/MDP.Lab/Azure身分驗證/Service身分驗證-服務主體/13.取得參數03.png)

### 加入專案

服務申請完成之後，就可以開始建立專案並且加入模組。Microsoft.Identity.Web預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入Microsoft.Identity.Web的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n ApiClient
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
Microsoft.Identity.Web
```

### 使用憑證

建立包含Microsoft.Identity.Web的專案之後，就可以使用憑證，建立代表客戶端身分的AccessToken，來通過ApiProvider的Service身分驗證。

```
(施工中)
```


## 模組範例

提供AzureAD的Service身分驗證，用來進行 Service to Service 之間的身分驗證，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.AzureAD.Services，逐步完成必要的設計和實作。

- 範例下載：[ApiClient.zip](https://clark159.github.io/MDP.Lab/Azure身分驗證/Services身分驗證-服務主體/ApiClient.zip)

- 範例下載：[ApiProvider.zip](https://clark159.github.io/MDP.Lab/Azure身分驗證/Services身分驗證-服務主體/ApiProvider.zip)

### 操作步驟

(施工中)
