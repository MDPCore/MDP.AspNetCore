// mdp
var mdp = mdp || {};

// mdp.liff
mdp.liff = (function () {

    // fields
    var _isInitialized = false;


    // methods
    var execute = function (config, action) {

        // execute
        if (_isInitialized == false) {

            // flag
            _isInitialized = true;

            // liff.init
            return liff.init({ liffId: config.liffId }).then(action);
        }
        else {

            // liff.ready
            return liff.ready.then(action);
        }
    }

    var init = function (config) {
        return execute(config, () => {
            
            // redirect
            window.location.href = config.returnUrl;
        });
    };

    var login = function (config) {
        return execute(config, () => {
            
            // redirectUri
            var redirectUri = new URL("/.auth/init/liff", window.location.href);
            redirectUri += "?returnUrl=" + encodeURIComponent(window.location.href);

            // liff.login
            if (liff.isLoggedIn() == false) { liff.login({ redirectUri: redirectUri }); return; }
            var accessToken = liff.getAccessToken();
            var idToken = liff.getIDToken();
            if (liff.isInClient() == false) { liff.logout(); }

            // signin
            var signinUrl = new URL("/.auth/login/liff/callback", window.location.href);
            signinUrl.searchParams.append("returnUrl", config.returnUrl);
            signinUrl.searchParams.append("access_token", accessToken);
            signinUrl.searchParams.append("id_token", idToken);
            window.location.href = signinUrl.href;
        });
    };

    var sendMessages = function (config) {

        // liff.isInClient
        if (liff.isInClient() == false) {

            // redirect
            window.location.href = config.returnUrl;
        }

        // execute
        return execute(config, () => {

            // liff.sendMessages
            return liff.sendMessages(config.messages).then(() => { window.location.href = config.returnUrl; });
        });
    };
    
    // return
    return {
        init: init,
        login: login,
        sendMessages: sendMessages
    };
})();
