(function () {
    'use strict';

    // Add the service to the module
    angular
        .module('app.auth')
        .factory('tokenService', tokenService);

    // Inject dependencies
    tokenService.$inject = ['$window'];

    // Define the service
    function tokenService($window) {

        // Define the service
        var service = {
            decodeAccessToken: decodeAccessToken,
            decodeBase64String: decodeBase64String,
            getTokenExpirationDate: getTokenExpirationDate,
            isTokenExpired: isTokenExpired
        };
        return service;

        // Decode access token
        function decodeAccessToken(accessToken) {
            var parts = accessToken.split('.');
            if (parts.length !== 3) {
                throw new Error('JWT must have 3 parts');
            }
            var decoded = decodeBase64String(parts[1]);
            if (!decoded) {
                throw new Error('Cannot decode the token');
            }
            return angular.fromJson(decoded);
        }

        // Decode base64 string
        function decodeBase64String(str) {
            var output = str.replace(/-/g, '+').replace(/_/g, '/');
            switch (output.length % 4) {
                case 0: { break; }
                case 2: { output += '=='; break; }
                case 3: { output += '='; break; }
                default: {
                    throw 'Illegal base64url string!';
                }
            }
            return $window.decodeURIComponent(escape($window.atob(output)));
        }

        // Determine whether the token has expired
        function getTokenExpirationDate(accessToken) {
            var decodedToken = decodeAccessToken(accessToken);
            if (typeof decodedToken.exp === "undefined") {
                return null;
            }
            var d = new Date(0);
            d.setUTCSeconds(decodedToken.exp);
            return d;
        }

        // Determine whether the token has expired
        function isTokenExpired(accessToken, offsetSeconds) {
            var d = getTokenExpirationDate(accessToken);
            offsetSeconds = offsetSeconds || 0;
            if (d == null) {
                return false;
            }
            return !(d.valueOf() > (new Date().valueOf() + (offsetSeconds * 1000)));
        }
    }
})();