// Header
(function (app) {
    var CommonHeader = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $transitions) {
        $scope.pages = [];
        $scope.showNav = false;
        $scope.current = "Home";

        Load = function () {
            console.log("loading page list");
            db.List("page").then(function (data) {
                console.log("got " + data.length + " pages");
                $scope.pages = data;
            });
        };

        Load();

        $scope.ToggleNav = function () {
            if ($scope.showNav == true) {
                $scope.showNav = false;
            }
            else {
                $scope.showNav = true;
            }
        }

        $scope.LoadState = function (state) {
            $scope.showNav = false;
            $state.go(state);
        };

        $scope.LoadPage = function (pageID) {
            $scope.showNav = false;
            $state.go("Page", { "pageID": pageID });
        };

        $rootScope.$on("PageLoad", function (e, pageID) {
            $scope.current = "Page" + pageID;
        });

        $transitions.onSuccess({ to: "*" }, function (trans) {
            var to = trans.to().name;
            if (to != "Page") {
                $scope.current = to;
            }
        });
    };

    CommonHeader.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$transitions"];
    app.controller("CommonHeader", CommonHeader);
}(angular.module("app")));