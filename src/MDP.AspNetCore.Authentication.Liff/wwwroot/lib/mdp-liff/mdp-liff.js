// namespace
var mdp = mdp || {};
mdp.liff = mdp.liff || {};


// mdp.liff.liffManager
mdp.liff.liffManager = (function () {

    // fields
    var _isInitialized = false;


    // methods
    var initialize = function (config) {
        return execute(config, () => {

            // redirect
            window.location.href = config.returnUrl;
        });
    };

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

    
    // return
    return {
        initialize: initialize,
        execute: execute
    };
})();


// mdp.liff.authenticationManager
mdp.liff.authenticationManager = (function () {

    // methods
    var login = function (config) {
        return mdp.liff.liffManager.execute(config, () => {

            // redirectUri
            var redirectUri = new URL("/.auth/login/liff/init", window.location.href);
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


    // return
    return {
        login: login
    };
})();


// mdp.liff.messageManager
mdp.liff.messageManager = (function () {

    // methods
    var sendMessages = function (config) {

        // liff.isInClient
        if (liff.isInClient() == false) {

            // redirect
            window.location.href = config.returnUrl;
        }

        // execute
        return mdp.liff.liffManager.execute(config, () => {

            // liff.sendMessages
            return liff.sendMessages(config.messages).then(() => { window.location.href = config.returnUrl; });
        });
    };


    // return
    return {
        sendMessages: sendMessages
    };
})();
