// Find
(function (app) {
    var FindIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, uiGmapGoogleMapApi) {
        $scope.lists = [];
        $scope.view = null;
        $scope.resultView = null;
        $scope.search = { CurrentLocation: false };
        $scope.result = null;
        $scope.markers = [];
        $scope.customStrings = [];
        $scope.map = null;
        $scope.searchView = null;

        var Load = function () {
            db.Get("list").then(function (data) {
                $scope.lists = data;
                $scope.view = 'search';
                // need type to default to something
                $scope.search.TypeID = 'cafe';
            });
            deviceSvc.GetCustomStrings().then(function (data) {    
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
            // toggle to basic to start
            $timeout(function () {
                $scope.ToggleSearchView();
            }, 1);            
        };

        Load();

        $scope.ToggleSearchView = function () {
            if ($scope.searchView == 'basic') {
                $scope.searchView = 'additional';
            }
            else {
                $scope.searchView = 'basic';
                $scope.UseCurrentLocation(true);
            }
        };

        $scope.UseCurrentLocation = function (force) {
            if ($scope.search.CurrentLocation == true && (force == undefined || force == null || force == false)) {
                // if there's an address, use that here
                $scope.ApplyLocation();
            }
            else {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(
                        function (location) {
                            $scope.search.Location = {
                                Latitude: location.coords.latitude,
                                Longitude: location.coords.longitude
                            };
                            $scope.search.CurrentLocation = true;
                            $scope.search.Address = '';
                            $scope.$apply();
                            console.log("set current location");
                        }
                    );
                }
            }
        };

        $scope.ApplyLocation = function () {
            console.log("applying location");
            $scope.search.Location = null;
            $scope.search.CurrentLocation = false;
            if ($scope.search.Address != null && $scope.search.Address != '') {
                db.Get("place", $scope.search.Address, false, "location").then(function (location) {
                    if (location != null) {
                        $scope.search.Location = location;
                    }
                    else {
                        deviceSvc.Alert("alert", $scope.customStrings['Find-AddressAlert']);
                    }
                });
            }
        };

        $scope.Search = function () {
            if ($scope.search.Location == null) {
                deviceSvc.Alert("alert", $scope.customStrings['Find-LocationAlert']);
            }
            else {
                db.Post("place", "search", $scope.search).then(function (data) {
                    $scope.result = data;
                    $scope.ViewResults('list');
                });
            }
        };

        $scope.MoreResults = function (nextToken) {
            $scope.search.Token = nextToken;
            db.Post("place", "search", $scope.search).then(function (data) {
                $scope.result = data;
                if ($scope.resultView == 'map') {
                    // need to reset with new results on map
                    LoadMap();
                }
            });
        };

        uiGmapGoogleMapApi.then(function (maps) {
        });

        var LoadMap = function () {
            // set data for map here
            if ($scope.result.Search != null) {
                $scope.map = {
                    center: {
                        latitude: $scope.result.Search.Location.Latitude,
                        longitude: $scope.result.Search.Location.Longitude
                    },
                    zoom: $scope.result.Zoom
                };
                $scope.markers = [];
                angular.forEach($scope.result.Results, function (item) {
                    $scope.markers.push(CreateMarker(item));
                });
            }
        };

        var CreateMarker = function (item) {
            var marker = {
                id: item.PlaceID,
                name: item.Name,
                place: item,
                coords: {
                    latitude: item.Latitude,
                    longitude: item.Longitude
                },
                customStrings: $scope.customStrings,
                show: false
            };
            marker.onClick = function () {
                marker.show = !marker.show;
            };
            return marker;
        };

        $scope.SetView = function (view) {
            if (view == 'search') {
                $scope.search.Token = null;
            }
            $scope.view = view;
        };

        $scope.ViewResults = function (type) {            
            if (type == 'map') {
                LoadMap();
            }
            $scope.resultView = type;
            $scope.SetView('results');
        };

        $scope.LoadReview = function (place) {
            // first save the place to db
            db.Save("place", place).then(function (result) {
                if (result == true) {
                    $state.go("ReviewSubmit", { "placeID": place.PlaceID });
                }
            });
        };

        $scope.LoadSummary = function (place) {
            if (place.HasReviews) {
                $state.go("ReviewSummary", { "placeID": place.PlaceID });
            }
            else {
                deviceSvc.Alert("alert", $scope.customStrings['Find-NoReviewsAlert']);
            }
        };

        $scope.$on('LoadReviews', function (e, data) {
            angular.forEach($scope.result.Results, function (item) {
                if (item.PlaceID == data.PlaceID) {
                    $scope.LoadSummary(item);
                }
            });
        });

        $scope.$on('StartReview', function (e, data) {
            angular.forEach($scope.result.Results, function (item) {
                if (item.PlaceID == data.PlaceID) {
                    $scope.LoadReview(item);
                }
            });
        });

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    FindIndex.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "uiGmapGoogleMapApi"];
    app.controller("FindIndex", FindIndex);
}(angular.module("app")));