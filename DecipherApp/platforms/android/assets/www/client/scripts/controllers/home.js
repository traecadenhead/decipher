// Index
(function (app) {
    var HomeIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $window, ga) {
        $scope.pages = [];
        $scope.customStrings = [];
        $scope.city = { DisplayName: "City"};
        $scope.languages = [];
        $scope.user = {};

        var Load = function () {
            ga.TrackScreen("HomeIndex");
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
            if($scope.languages == []){
                db.List("language").then(function (data) {
                    $scope.languages = data;
                    if ($scope.languages.length > 1) {
                        // update height if language list will show
                        $scope.height = $scope.height - 42;
                    }
                    $scope.user.Language = deviceSvc.GetLanguage();
                });
            }
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (location) {
                        var loc = {
                            Latitude: location.coords.latitude,
                            Longitude: location.coords.longitude
                        };
                        db.Post("city", "determine", loc).then(function(data){
                            $scope.city = data;
                            amplify.store("City", data);
                        });
                    }, function (err) {
                        db.Get("city", null, false, "default").then(function (data) {
                            $scope.city = data;
                            amplify.store("City", data);
                        });
                    }
                );
            }
            else{
                db.Get("city", null, false, "default").then(function (data) {
                    $scope.city = data;
                    amplify.store("City", data);
                });
            }
        };

        Load();

        $scope.LoadPage = function (pageID) {
            $state.go("Page", { "pageID": pageID });
        }

        $scope.ChangeLanguage = function () {
            ga.TrackEvent("Preferences", "LanguageChange", "Language", $scope.user.Language);
            deviceSvc.SetLanguage($scope.user.Language);
            if (amplify.store("UserID") != null) {
                // save user's language
                var city = amplify.store("City");
                var user = {
                    UserID: amplify.store("UserID"),
                    Language: $scope.user.Language,
                    CityID: city.CityID
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

        // move to top of screen when view is loaded
        $timeout(function () {
            $window.scrollTo(0, 0);
        });
    };

    HomeIndex.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$window", "ga"];
    app.controller("HomeIndex", HomeIndex);
}(angular.module("app")));

// Page
(function (app) {
    var HomePage = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams, $window, ga) {
        $scope.page = {};
        $scope.customStrings = [];

        Load = function () {
            $rootScope.$broadcast("PageLoad", $stateParams.pageID);
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
            db.Get("page", $stateParams.pageID).then(function (data) {
                $scope.page = data;
                ga.TrackScreen(data.Title);
                $scope.page.Content = $sce.trustAsHtml(data.Content);
            });
        };

        Load();

        $rootScope.$on("Refresh", function () {
            Load();
        });

        // move to top of screen when view is loaded
        $timeout(function () {
            $window.scrollTo(0, 0);
        });
    };

    HomePage.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams", "$window", "ga"];
    app.controller("HomePage", HomePage);
}(angular.module("app")));