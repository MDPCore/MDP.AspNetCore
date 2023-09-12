---
layout: default
title: 開發一個依賴注入+參數管理的API服務
parent: 快速開始
grand_parent: 身分驗證
nav_order: 1
has_children: false
---

# 開發一個依賴注入+參數管理的API服務

從零開始，開發一個依賴注入+參數管理的API服務，是難度不高但繁瑣的工作項目。本篇範例協助開發人員使用MDP.Net，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.Net/快速開始/開發一個依賴注入+參數管理的API服務/WebApplication1.zip)


## 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的API服務。

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

5.改寫專案內的Controllers\HomeController.cs，注入並使用MessageRepository。

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
        public IndexResultModel Index()
        {
            // ResultModel
            var resultModel = new IndexResultModel();
            resultModel.Message = _messageRepository.GetValue();

            // Return
            return resultModel;
        }


        // Class
        public class IndexResultModel
        {
            // Properties
            public string Message { get; set; }
        }
    }
}
```

6.執行專案，於開啟的Console視窗內，可以看到由MessageRepository所提供的Hello World。

![01.執行結果01.png](https://clark159.github.io/MDP.Net/快速開始/開發一個依賴注入+參數管理的API服務/01.執行結果01.png)
