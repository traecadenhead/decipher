// Header
(function (app) {
    var CommonHeader = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $transitions) {
        $scope.pages = [];
        $scope.showNav = false;
        $scope.current = "Home";
        $scope.customStrings = [];
        $scope.languages = [];
        $scope.user = {};

        var Load = function () {
            console.log("loading page list");
            db.List("language").then(function (data) {
                $scope.languages = data;
                $scope.user.Language = deviceSvc.GetLanguage();
            });
            db.List("page").then(function (data) {
                console.log("got " + data.length + " pages");
                $scope.pages = data;
            });
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
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

        $scope.ChangeLanguage = function () {
            deviceSvc.SetLanguage($scope.user.Language);
            if (amplify.store("UserID") != null) {
                // save user's language
                var user = {
                    UserID: amplify.store("UserID"),
                    Language: $scope.user.Language
                };
                db.Save("user", user, "language");
            }
            // Refresh open views so that data is loaded in correct language
            console.log("language changed - get custom strings");
            deviceSvc.GetCustomStrings().then(function (data) {
                // get the data first before we use these methods that refresh everything
                console.log("refresh views");
                $rootScope.$broadcast("Refresh");
                console.log("reload header");
                Load();
            });            
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

// Footer
(function (app) {
    var CommonFooter = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {

        $scope.customStrings = [];

        var Load = function () {
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
        };

        Load();

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    CommonFooter.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("CommonFooter", CommonFooter);
}(angular.module("app")));