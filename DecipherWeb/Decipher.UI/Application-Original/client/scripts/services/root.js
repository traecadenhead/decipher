(function (app) {
    var root = function () {
        var baseUrl = "http://dev.decipheraustin.com";

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