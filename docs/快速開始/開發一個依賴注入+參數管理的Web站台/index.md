---
layout: default
title: 開發一個依賴注入+參數管理的Web站台
parent: 快速開始
grand_parent: 身分驗證
nav_order: 2
has_children: false
---

# 開發一個依賴注入+參數管理的Web站台

從零開始，開發一個依賴注入+參數管理的Web站台，是難度不高但繁瑣的工作項目。本篇範例協助開發人員使用MDP.Net，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.Net/快速開始/開發一個依賴注入+參數管理的Web站台/WebApplication1.zip)


## 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

2.使用Visual Studio開啟WebApplication1專案。並於專案內加入Modules\MessageRepository.cs，做為注入的Interface。

```
namespace WebApplication1
{
    public interface MessageRepository
    {
        // Methods
        string GetValue();
    }
}
```

3.於專案內加入Modules\ConfigMessageRepository.cs，做為注入的Implement。程式碼中的``` Service<MessageRepository>() ```，將ConfigMessageRepository註冊為MessageRepository。

```
using MDP.Registration;

namespace WebApplication1
{
    [Service<MessageRepository>()]
    public class ConfigMessageRepository : MessageRepository
    {
        // Fields
        private readonly string _message;


        // Constructors
        public ConfigMessageRepository(string message)
        {
            // Default
            _message = message;
        }


        // Methods
        public string GetValue()
        {
            // Return
            return _message;
        }
    }
}
```

4.改寫專案內的appsettings.json，加入ConfigMessageRepository的參數設定。參數檔中的``` "ConfigMessageRepository": { "Message": "Hello World" } ```，設定生成ConfigMessageRepository的時候，將Hello World帶入建構子的Message參數。

```
{
  "WebApplication1": {
    "ConfigMessageRepository": { "Message": "Hello World" }
  }
}
```

5.改寫專案內的Controllers\HomeController.cs、Views\Home\Index.cshtml，注入並使用MessageRepository。

```
using System;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1
{
    public class HomeController : Controller
    {
        // Fields
        private readonly MessageRepository _messageRepository = null;


        // Constructors
        public HomeController(MessageRepository messageRepository)
        {
            // Default
            _messageRepository = messageRepository;
        }


        // Methods
        public ActionResult Index()
        {
            // ViewBag
            this.ViewBag.Message = _messageRepository.GetValue();

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
    <title>WebApplication1</title>
</head>
<body>

    <!--Title-->
    <h2>WebApplication1</h2>
    <hr />

    <!--Message-->
    <h3>@ViewBag.Message</h3>

</body>
</html>
```

6.執行專案，於開啟的Browser視窗內，可以看到由MessageRepository所提供的Hello World。

![01.執行結果01.png](https://clark159.github.io/MDP.Net/快速開始/開發一個依賴注入+參數管理的Web站台/01.執行結果01.png)
