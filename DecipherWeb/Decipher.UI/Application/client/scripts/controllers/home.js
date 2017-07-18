// Index
(function (app) {
    var HomeIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        $scope.pages = [];
        $scope.customStrings = [];

        var Load = function () {
            db.List("page").then(function (data) {
                console.log("got " + data.length + " pages");
                $scope.pages = data;
                try {
                    $scope.height = parseInt(window.innerHeight - ($scope.pages.length * 75) - 75);
                }
                catch (e) { console.log(e); }
            });
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
        };

        Load();

        $scope.LoadPage = function (pageID) {
            $state.go("Page", { "pageID": pageID });
        }
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

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    HomePage.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("HomePage", HomePage);
}(angular.module("app")));