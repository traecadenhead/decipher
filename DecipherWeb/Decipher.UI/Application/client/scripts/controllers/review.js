// Index
(function (app) {
    var ReviewIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        $scope.customStrings = [];

        var Load = function () {
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
        };

        Load();
    };

    ReviewIndex.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("ReviewIndex", ReviewIndex);
}(angular.module("app")));