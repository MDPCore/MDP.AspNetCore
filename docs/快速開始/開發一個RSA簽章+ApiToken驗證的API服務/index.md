---
layout: default
title: 開發一個RSA簽章+ApiToken驗證的API服務
parent: 快速開始
grand_parent: 身分驗證
nav_order: 4
has_children: false
---

# 開發一個RSA簽章+JwtBearer驗證的API服務

使用資料庫驗證帳號及密碼之後，發行使用RSA簽章的JWT給用戶，讓用戶放到HTTP Request封包的x-api-token表頭來進行身分驗證，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.Jwt，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個RSA簽章+JwtBearer驗證的API服務/WebApplication1.zip)

## 操作步驟

1.使用Visual Studio建立Console專案，執行下列程式碼來產生RSA公私鑰。

```
using System;
using System.Security.Cryptography;

namespace ConsoleApp1
{
    public class Program
    {
        // Methods
        public static void Main(string[] args)
        {
            // RsaKey
            string publicKey = string.Empty;
            string privateKey = string.Empty;
            using (RSA rsa = RSA.Create(2048))
            {
                // PublicKey
                publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                publicKey = $"-----BEGIN RSA PUBLIC KEY-----\n{publicKey}\n-----END RSA PUBLIC KEY-----";
                publicKey = publicKey.Replace("\r", "").Replace("\n", "");

                // PrivateKey
                privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                privateKey = $"-----BEGIN RSA PRIVATE KEY-----\n{privateKey}\n-----END RSA PRIVATE KEY-----";
                privateKey = privateKey.Replace("\r", "").Replace("\n", "");
            }

            // Display
            Console.WriteLine("RSA Public Key:");
            Console.WriteLine(publicKey);
            Console.WriteLine();
            Console.WriteLine("RSA Private Key:");
            Console.WriteLine(privateKey);
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
```

![01.GetKey01.png](https://clark159.github.io/MDP.AspNetCore.Authentication/快速開始/開發一個HMAC簽章+JwtBearer驗證的API服務/01.GetKey01.png)
