(function (app) {
    var root = function () {
        var baseUrl = "https://deciphercity.com";

        var GetBaseUrl = function () {
            return baseUrl;
        };

        return {
            GetBaseUrl: GetBaseUrl
        };
    };
    root.$inject = [];
    app.factory("root", root);
}(angular.module('app')));