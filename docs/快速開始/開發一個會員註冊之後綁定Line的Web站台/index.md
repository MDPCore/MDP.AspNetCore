---
layout: default
title: 開發一個會員註冊之後綁定Line的Web站台
parent: 快速開始
grand_parent: 身分驗證
nav_order: 1
has_children: false
---

# 開發一個會員註冊之後綁定Line的Web站台

專案開發過程，常會需要在會員註冊並使用之後，才進行Line身分綁定。完成綁定之後，客戶就能使用Line進行OAuth快速登入(系統也能取得Line的UID來訊息推送)。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.Line，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個會員註冊之後綁定Line的Web站台/WebApplication1.zip)

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


