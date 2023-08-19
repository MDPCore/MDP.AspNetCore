// MDP
var mdp = mdp || {};

// MDP.Liff
mdp.liff = {

    // init
    init: function (liffId, returnUrl) {

        // liff
        return liff.init({

            // liffId
            liffId: liffId
        })
        .then(() => {

            // login
            if (liff.isLoggedIn() == false) { liff.login({ redirectUri: window.location.href }); return; }
            var accessToken = liff.getAccessToken();
            var idToken = liff.getIDToken();
            if (liff.isInClient() == true) { liff.logout(); }

            // signin
            var signinURL = new URL("/signin-liff", window.location.href);
            signinURL.searchParams.append("returnUrl", returnUrl);
            signinURL.searchParams.append("access_token", accessToken);
            signinURL.searchParams.append("id_token", idToken);            
            window.location = signinURL.href;
        });
    }
}