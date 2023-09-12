---
layout: default
title: 開發一個依賴注入+參數管理的Console程式
parent: 快速開始
grand_parent: 身分驗證
nav_order: 3
has_children: false
---

# 開發一個依賴注入+參數管理的Console程式

從零開始，開發一個依賴注入+參數管理的Console程式，是難度不高但繁瑣的工作項目。本篇範例協助開發人員使用MDP.Net，逐步完成必要的設計和實作。

- 範例下載：[ConsoleApp1.zip](https://clark159.github.io/MDP.Net/快速開始/開發一個依賴注入+參數管理的Console程式/ConsoleApp1.zip)


## 操作步驟

1.開啟命令提示字元，輸入下列指令。用以安裝MDP.ConsoleApp範本、並且建立一個名為ConsoleApp1的Console程式。

```
dotnet new install MDP.ConsoleApp
dotnet new MDP.ConsoleApp -n ConsoleApp1
```

2.使用Visual Studio開啟ConsoleApp1專案。並於專案內加入Modules\MessageRepository.cs，做為注入的Interface。

```
namespace ConsoleApp1
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

namespace ConsoleApp1
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
  "ConsoleApp1": {
    "ConfigMessageRepository": { "Message": "Hello World" }
  }
}
```

5.改寫專案內的Program.cs，注入並使用MessageRepository。

```
using System;

namespace ConsoleApp1
{
    public class Program
    {
        // Methods
        public static void Run(MessageRepository messageRepository)
        {
            // Message
            var message = messageRepository.GetValue();

            // Display
            Console.WriteLine(message);
        }

        public static void Main(string[] args)
        {
            // Host
            MDP.NetCore.Host.Run<Program>(args);
        }
    }
}
```

6.執行專案，於開啟的Console視窗內，可以看到由MessageRepository所提供的Hello World。

![01.執行結果01.png](https://clark159.github.io/MDP.Net/快速開始/開發一個依賴注入+參數管理的Console程式/01.執行結果01.png)
