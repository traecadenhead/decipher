// Identify
(function (app) {
    var UserIdentify = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        $scope.user = {};
        $scope.customStrings = [];

        Load = function () {
            // get descriptors list
            var userID = amplify.store("UserID");
            db.Get("user", userID).then(function (data) {
                $scope.user = data;
            });
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
        };

        Load();

        $scope.Select = function (item) {
            if (item.Selected == true) {
                item.Selected = false;
            }
            else {
                item.Selected = true;
            }
            console.log(item.Selected);
        };

        $scope.Submit = function () {
            // save descriptors list
            db.Save("user", $scope.user).then(function (result) {
                if (result > 0) {
                    amplify.store("UserID", result);
                    db.Get("user", result, true).then(function (data) {
                        $state.go("Find");
                    });
                }
            });
        };
    };

    UserIdentify.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("UserIdentify", UserIdentify);
}(angular.module("app")));