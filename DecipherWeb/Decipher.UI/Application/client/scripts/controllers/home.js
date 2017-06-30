// Index
(function (app) {
    var HomeIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        var timer = null;
        var timedOut = false;
        var loaded = false;

        Load = function () {
            try {
                $scope.height = parseInt(window.innerHeight - 100) + 'px';
            }
            catch (e) { }
            timer = $timeout(function () { EndTimer(); }, 3000);
            var userID = amplify.store("UserID");
            // get user identity
            db.Get("user", userID, true).then(function (data) {
                // get pages
                db.List("page", null, true).then(function (data) {
                    loaded = true;
                    if (timedOut) {
                        Resume();
                    }
                });
            });
        };

        Load();

        var EndTimer = function () {
            $timeout.cancel(timer);
            timedOut = true;
            if (loaded) {
                Resume();
            }
        };

        $scope.$on('datarefresh', function () {
            Load();
        });

        var Resume = function () {
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

// Page
(function (app) {
    var HomePage = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        $scope.page = {};

        Load = function () {
            $rootScope.$broadcast("PageLoad", $stateParams.pageID);

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