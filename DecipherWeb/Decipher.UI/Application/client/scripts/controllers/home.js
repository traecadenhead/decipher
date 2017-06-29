// Index
(function (app) {
    var HomeIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        var Load = function () {
            if (amplify.store("UserID") != null) {
                $state.go("Find");
            } else {
                $state.go("Identify");
            }            
        };

        Load();
    };

    HomeIndex.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("HomeIndex", HomeIndex);
}(angular.module("app")));

// Identify
(function (app) {
    var HomeIdentify = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        $scope.user = {};

        Load = function () {
            // get descriptors list
            var userID = amplify.store("UserID");
            db.Get("user", userID).then(function (data) {
                $scope.user = data;
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

    HomeIdentify.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("HomeIdentify", HomeIdentify);
}(angular.module("app")));

// Page
(function (app) {
    var HomePage = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        $scope.page = {};

        Load = function () {
            db.Get("page", $stateParams.pageID).then(function (data) {
                $scope.page = data;
                $scope.page.Content = $sce.trustAsHtml(data.Content);
            });
        };

        Load();
    };

    HomePage.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("HomePage", HomePage);
}(angular.module("app")));