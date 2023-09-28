---
layout: default
title: 功能說明
parent: 身分驗證
nav_order: 1
permalink: /
---


# MDP.AspNetCore.Authentication

MDP.AspNetCore.Authentication是開源的.NET開發套件，協助開發人員快速建立整合ASP.NET Core身分驗證的應用系統。提供Line、Google、Facebook等OAuth身分驗證模組，及Remote身分驗證、Local身分驗證、Token身分驗證等功能服務，用以簡化開發流程並滿足多變的商業需求。

- 說明文件：[https://clark159.github.io/MDP.AspNetCore.Authentication/](https://clark159.github.io/MDP.AspNetCore.Authentication/)

- 程式源碼：[https://github.com/Clark159/MDP.AspNetCore.Authentication/](https://github.com/Clark159/MDP.AspNetCore.Authentication/)


## 快速開始

- [開發一個Line登入之後註冊會員的Web站台](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個Line登入之後註冊會員的Web站台/)

- [開發一個會員註冊之後綁定Line的Web站台](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/)


## 模組功能

![MDP.AspNetCore.Authentication-模組功能.png](https://clark159.github.io/MDP.AspNetCore.Authentication/功能說明/MDP.AspNetCore.Authentication-模組功能.png)

### 模組掛載

MDP.AspNetCore.Authentication擴充ASP.NET Core既有的身分驗證，加入Line、Google、Facebook等功能模組的掛載功能。開發人員可以透過Config設定，掛載在執行階段使用的身分認證。

- 模組清單：[OAuth身分認證清單](https://clark159.github.io/MDP.AspNetCore.Authentication/OAuth身分驗證/)。

```
// Config設定
{
  "Authentication": {
    "Line": {
      "ClientId": "Xxxxx",
      "ClientSecret": "Xxxxx"
    }
  }
}
- 命名空間：Authentication
- 掛載的身分驗證模組：Line
- Line身分驗證模組的客戶編號：ClientId="Xxxxx"。(Xxxxx填入Channel ID)
- Line身分驗證模組的客戶密碼：ClientSecret="Xxxxx"。(Xxxxx填入Channel Secret)
```

### Remote身分驗證

![MDP.AspNetCore.Authentication-Remote身分驗證.png](https://clark159.github.io/MDP.AspNetCore.Authentication/功能說明/MDP.AspNetCore.Authentication-Remote身分驗證.png)

MDP.AspNetCore.Authentication擴充ASP.NET Core既有的身分驗證，加入Remote身分驗證流程。用來確認通過OAuth身分驗證的用戶，是否為已知用戶、是否需要引導註冊、是否拒絕存取，並於最終完成登入。

- MDP.AspNetCore.Authentication加入Controller的擴充方法LoginAsync，用來發起Remote身分驗證流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class ControllerExtensions

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
public class ControllerExtensions

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
public class ControllerExtensions

// 擴充方法
public static Task<ClaimsIdentity> LocalAuthenticateAsync(this Controller controller)
- controller：執行的Controller物件。
- Task<ClaimsIdentity>：回傳值，目前Local身分登入的身分資料。
```

### Remote身分綁定

![MDP.AspNetCore.Authentication-Remote身分綁定.png](https://clark159.github.io/MDP.AspNetCore.Authentication/功能說明/MDP.AspNetCore.Authentication-Remote身分綁定.png)

完成登入之後，開發人員可以使用MDP.AspNetCore.Authentication提供的Remote身分綁定流程，用來綁定用戶所擁有的其他OAuth身分驗證。

- MDP.AspNetCore.Authentication加入Controller的擴充方法LinkAsync，用來發起Remote身分綁定流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class ControllerExtensions

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
public class ControllerExtensions

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

![MDP.AspNetCore.Authentication-Local身分驗證.png](https://clark159.github.io/MDP.AspNetCore.Authentication/功能說明/MDP.AspNetCore.Authentication-Local身分驗證.png)

MDP.AspNetCore.Authentication也加入Local身分驗證流程。用來讓開發人員透過資料庫帳號密碼驗證、或是AD帳號密碼認證之後，直接建立身分資料來執行Local身分登入，將身分資料寫入Cookie提供後續流程使用。

- MDP.AspNetCore.Authentication加入Controller的擴充方法LoginAsync，用來發起Local身分驗證流程。

```
// 命名空間：
MDP.AspNetCore.Authentication

// 類別定義：
public class ControllerExtensions

// 擴充方法
public static async Task<ActionResult> LoginAsync(this Controller controller, ClaimsIdentity localIdentity, string returnUrl = null)
- controller：執行的Controller物件。
- localIdentity：Local身分登入的身分資料。
- returnUrl：完成Remote身分驗證之後，要跳轉的功能頁面路徑。
- Task<ActionResult>：回傳值，流程跳轉頁面。
```

## 模組使用

### 加入專案

MDP.AspNetCore.Authentication預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
// 建立API服務、Web站台
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication
```

### 設定參數

建立包含MDP.AspNetCore.Authentication模組的專案之後，在專案裡可以透過Config設定，掛載在執行階段使用的身分驗證。

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

建立包含MDP.AspNetCore.Authentication模組的專案之後，就可以註冊AuthenticationProvider實作，來覆寫RemoteExchange、RemoteLink。

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
"MDP.Members": {
  "MemberAuthenticationProvider": {}
}
```


## 版本更新

### MDP.AspNetCore.Authentication 6.1.8.1

- 重構AuthenticationProvider，讓他更容易被理解。

### MDP.AspNetCore.Authentication 6.1.8

- 加入Microsoft身分驗證。

- 加入AzureAD身分驗證。

- 重構MDP.AspNetCore.Authentication，簡化登入邏輯與流程。

### MDP.AspNetCore.Authentication 6.1.5

- 跟隨 MDP.Net進版。