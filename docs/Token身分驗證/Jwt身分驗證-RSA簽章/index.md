---
layout: default
title: Jwt身分驗證-RSA簽章
parent: Token身分驗證
grand_parent: 身分驗證
nav_order: 1
has_children: false
---

# MDP.AspNetCore.Authentication.Jwt for RSA

MDP.AspNetCore.Authentication.Jwt擴充ASP.NET Core既有的身分驗證，加入Jwt身分驗證功能。開發人員可以透過Config設定，掛載在專案裡使用的Jwt身分驗證，用以驗證使用RSA簽章的JWT。

- 說明文件：[https://mdpnetcore.github.io/MDP.AspNetCore/](https://mdpnetcore.github.io/MDP.AspNetCore/)

- 程式源碼：[https://github.com/MDPNetCore/MDP.AspNetCore/](https://github.com/MDPNetCore/MDP.AspNetCore/)


## 模組使用

### 加入專案

MDP.AspNetCore.Authentication.Jwt預設獨立在MDP.Net專案範本外，依照下列操作步驟，即可建立加入MDP.AspNetCore.Authentication.Jwt的專案。

- 在命令提示字元輸入下列指令，使用MDP.Net專案範本建立專案。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

- 使用Visual Studio開啟專案。在專案裡使用NuGet套件管理員，新增下列NuGet套件。

```
MDP.AspNetCore.Authentication.Jwt
```


### 設定參數

建立包含MDP.AspNetCore.Authentication.Jwt的專案之後，就可以透過Config設定，掛載在專案裡使用的Jwt身分驗證。

- 使用RSA簽章時，需要提供RSA公鑰用來對JWT進行驗證。開發人員可以使用Visual Studio建立Console專案，執行下列程式碼來產生RSA公私鑰。

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

![01.GetKey01.png](https://mdpnetcore.github.io/MDP.AspNetCore/Token身分驗證/Jwt身分驗證-RSA簽章/01.GetKey01.png)

- 取得RSA公鑰後，透過Config設定，掛載在專案裡使用的Jwt身分驗證。

```
// Config設定 - Jwt身分驗證模組
{
  "Authentication": {
    "Jwt": {
      "Credentials": [
        {
          "Scheme": "JwtBearer",
          "Header": "Authorization",
          "Prefix": "Bearer ",
          "Algorithm": "RS256",
          "SignKey": "12345678901234567890123456789012",
          "Issuer": "MDP"
        }
      ]
    }
  }
}
- 命名空間：Authentication
- 掛載的身分驗證模組：Jwt
- 憑證清單：Credentials
- 憑證名稱：Scheme="JwtBearer"。
- 憑證標頭：Header="Authorization"。(從HTTP Request的哪個Header取得Token)
- 憑證前綴：Prefix="Bearer "。(Token的前綴字)
- 簽章算法：Algorithm="RS256"。(Token所使用的簽章演算法，RSxxx=RSA+SHAxxx)
- 簽章金鑰：SignKey="12345..."。(Token所使用的簽章金鑰，PEM格式金鑰)
- 憑證發行：Issuer="MDP"。(檢核用，Token的核發單位)
```

## 模組範例

使用資料庫驗證帳號及密碼之後，發行代表身分資料並使用RSA簽章的JWT，讓系統放到HTTP Request封包的Authorization表頭來進行身分驗證，是開發系統時常見的功能需求。本篇範例協助開發人員使用MDP.AspNetCore.Authentication.Jwt，逐步完成必要的設計和實作。

- 範例下載：[WebApplication1.zip](https://mdpnetcore.github.io/MDP.AspNetCore/Token身分驗證/Jwt身分驗證-RSA簽章/WebApplication1.zip)


### 操作步驟

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

![01.GetKey01.png](https://mdpnetcore.github.io/MDP.AspNetCore/Token身分驗證/Jwt身分驗證-RSA簽章/01.GetKey01.png)

2.開啟命令提示字元，輸入下列指令。用以安裝MDP.WebApp範本、並且建立一個名為WebApplication1的Web站台。

```
dotnet new install MDP.WebApp
dotnet new MDP.WebApp -n WebApplication1
```

3.使用Visual Studio開啟WebApplication1專案，在專案裡用NuGet套件管理員新增下列NuGet套件。

```
MDP.Security.Tokens.Jwt
MDP.AspNetCore.Authentication.Jwt
```

4.於專案內改寫appsettings.json，用以掛載MDP.AspNetCore.Authentication.Jwt及MDP.Security.Tokens.Jwt。( Authentication…SignKey：填入先前步驟取得的RSA公鑰用來解碼、MDP.Security.Tokens.Jwt…SignKey：填入先前步驟取得的RSA私鑰用來編碼)

```
{
  "Authentication": {
    "Jwt": {
      "Credentials": [
        {
          "Scheme": "JwtBearer",
          "Header": "Authorization",
          "Prefix": "Bearer ",
          "Algorithm": "RS256",
          "SignKey": "xxxxxxxxxxxxxxxxxxxxxxxxx",
          "Issuer": "MDP"
        }
      ]
    }
  },

  "MDP.Security.Tokens.Jwt": {
    "SecurityTokenFactory": {
      "Credentials": [
        {
          "Name": "RsaToken",
          "Algorithm": "RS256",
          "SignKey": "xxxxxxxxxxxxxxxxxxxxxxxxx",
          "Issuer": "MDP",
          "ExpireMinutes": 30
        }
      ]
    }
  }
}
```

5.在專案裡加入Modules\Member.cs、Modules\MemberRepository.cs，並改寫appsettings.json。用來掛載模擬的會員資料庫，提供會員資料的查詢。

```
using System;

namespace MDP.Members
{
    public class Member
    {
        // Properties
        public string MemberId { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;

        public string Mail { get; set; } = String.Empty;
    }
}
```

```
using MDP.Registration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.Members
{
    [Service<MemberRepository>(singleton: true)]
    public class MemberRepository
    {
        // Fields
        private readonly List<Member> _memberList = new List<Member>();


        // Constructors
        public MemberRepository()
        {
            // DEMO用的假資料
            _memberList.Add(new Member() { MemberId = Guid.NewGuid().ToString(), Name = "Clark", Mail = "Clark@hotmail.com" });
            _memberList.Add(new Member() { MemberId = Guid.NewGuid().ToString(), Name = "Jane", Mail = "Jane@hotmail.com" });
        }


        // Methods
        public Member FindByPassword(string username, string password)
        {
            // DEMO用的範例。正式環境可以使用DB或AD進行驗證。(PS.儲存密碼記得雜湊處理)
            return _memberList.FirstOrDefault(o => o.Name == username);
        }
    }
}
```

```
{
  "MDP.Members": {
    "MemberRepository": {}
  }
}
```

6.改寫專案內的Controllers\HomeController.cs、Views\Home\Index.cshtml，用來提供使用Password進行身分驗證後取得Token、及使用Token進行身分驗證後取得身分資料，這兩個功能服務。

```
using MDP.Members;
using MDP.Security.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebApplication1
{
    public class HomeController : Controller
    {
        // Fields
        private readonly MemberRepository _memberRepository;

        private readonly SecurityTokenFactory _securityTokenFactory;


        // Constructors
        public HomeController(MemberRepository memberRepository, SecurityTokenFactory securityTokenFactory)
        {
            // Default
            _memberRepository = memberRepository;
            _securityTokenFactory = securityTokenFactory;
        }


        // Methods
        [AllowAnonymous]
        public ActionResult Index()
        {
            // Return
            return View();
        }

        [AllowAnonymous]
        public ActionResult GetTokenByPassword(string username, string password)
        {
            // Member
            var member = _memberRepository.FindByPassword(username, password);
            if (member == null)
            {
                // Message
                this.ViewBag.Message = "Login failed";

                // Return
                return this.View("Index");
            }

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, member.MemberId),
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.Email, member.Mail)
            }, "Password");

            // RsaToken
            var rsaToken = _securityTokenFactory.CreateSecurityToken("RsaToken", claimsIdentity);
            if (string.IsNullOrEmpty(rsaToken) == true) throw new InvalidOperationException($"{nameof(rsaToken)}=null");

            // Return
            {
                // Message
                this.ViewBag.Token = rsaToken;

                // Return
                return this.View("Index");
            }
        }

        [Authorize]
        public ActionResult<UserModel> GetUser()
        {
            // ClaimsIdentity
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");

            // UserModel
            var user = new UserModel();
            user.AuthenticationType = claimsIdentity.AuthenticationType!;
            user.UserId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            user.UserName = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value!;
            user.Mail = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value!;

            // Return
            return user;
        }


        // Class
        public class UserModel
        {
            // Properties
            public string AuthenticationType { get; set; } = string.Empty;

            public string UserId { get; set; } = string.Empty;

            public string UserName { get; set; } = string.Empty;

            public string Mail { get; set; } = string.Empty;
        }
    }
}
```

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
<!DOCTYPE html>
<html>
<head>
    <!-- Title -->
    <title>Home</title>

    <!-- meta -->
    <meta charset="utf-8" />

    <!-- script -->
    <script language="javascript">
        async function getUser() {
            try {
                const response = await fetch("/Home/GetUser", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": "Bearer " + document.getElementById("token").innerText
                    }
                });
                if (response.ok) {
                    document.getElementById("getUser.result").innerText = JSON.stringify(await response.json(), null, 2);
                } else {
                    document.getElementById("getUser.result").innerText = `getUser is Failed: ${response.status}`;
                }
            }
            catch (error) {
                document.getElementById("getUser.result").innerText = `getUser is Failed: ${error}`;
            }
        }
    </script>
</head>
<body>

    <!--Title-->
    <h2>Home</h2>
    <hr />
       
    <!--GetTokenByPassword-->
    <form asp-controller="Home" asp-action="GetTokenByPassword" method="post">
        Username:<input type="text" name="username" value="Clark" /><br />
        Password:<input type="text" name="password" value="" /><br />
        <input type="submit" value="GetTokenByPassword" /><br />
        <br />
        <div id="token">@ViewBag.Token</div><br />
        <h3 style="color:red">@ViewBag.Message</h3>
    </form>
    <hr />

    <!--GetUser-->
    <input type="button" value="GetUser" onclick="getUser()" /><br />
    <div id="getUser.result"></div><br />
    <hr />    

</body>
</html>
```

7.執行專案，於開啟的Browser視窗內，可以看到系統畫面進入到Home頁面。點擊GetTokenByPassword按鈕，進行Password身分驗證，系統會在完成身分驗證之後，於畫面上顯示取得的Token。

![02.GetToken01.png](https://mdpnetcore.github.io/MDP.AspNetCore/Token身分驗證/Jwt身分驗證-RSA簽章/02.GetToken01.png)

8.取得Token之後，點擊GetUser按鈕，進行JwtBearer身分驗證。系統會使用JavaScript，將Token放入HTTP Request封包的Authorization表頭進行身分驗證，並在完成身分驗證之後，於畫面上顯示取得的身分資料。

![03.GetUser01.png](https://mdpnetcore.github.io/MDP.AspNetCore/Token身分驗證/Jwt身分驗證-RSA簽章/03.GetUser01.png)