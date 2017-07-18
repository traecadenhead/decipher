// Index
(function (app) {
    var ReviewIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        $scope.customStrings = [];
        $scope.search = {};
        $scope.result = null;
        $scope.selected = null;

        var Load = function () {
            if (amplify.store("UserID") == null) {
                $state.go("ReviewIdentify");
            }
            else {
                $scope.search.User = {
                    UserID: amplify.store("UserID"),
                    Language: amplify.store("Language")
                };
            }
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
            $timeout(function () {
                $scope.UseCurrentLocation();
            }, 1);
        };

        Load();

        $scope.UseCurrentLocation = function () {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (location) {
                        $scope.search.Location = {
                            Latitude: location.coords.latitude,
                            Longitude: location.coords.longitude
                        };
                        $scope.search.CurrentLocation = true;
                        console.log("set current location");
                        Submit();
                    }
                );
            }
            else {
                deviceSvc.Alert($scope.customStrings["Review-NoLocation"]);
            }
        };

        $scope.Search = function () {
            if ($scope.search.Location == null) {
                $scope.UseCurrentLocation();
            }
            else {
                // already have the location
                Submit();
            }
        };

        $scope.ClearSearch = function () {
            $scope.search.Term = '';
            Submit();
        };

        $scope.Select = function (item) {
            if (item.PlaceID == $scope.selected) {
                $scope.selected = null;
            }
            else {
                $scope.selected = item.PlaceID;
            }
        };

        var Submit = function () {
            db.Post("place", "find", $scope.search).then(function (data) {
                $scope.result = data;
            });
        };

        $scope.Next = function () {
            if ($scope.selected != null) {
                $state.go("ReviewQuestions", { "placeID": $scope.selected });
            }
            else {
                deviceSvc.Alert($scope.customStrings["Review-NoPlace"]);
            }
        };
    };

    ReviewIndex.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("ReviewIndex", ReviewIndex);
}(angular.module("app")));

// Identify
(function (app) {
    var ReviewIdentify = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope) {
        $scope.user = {};
        $scope.customStrings = [];

        var Load = function () {
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
                        $state.go("Review");
                    });
                }
            });
        };

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    ReviewIdentify.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope"];
    app.controller("ReviewIdentify", ReviewIdentify);
}(angular.module("app")));