// Find
(function (app) {
    var DataIndex = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $q) {
     
        $scope.customStrings = [];
        $scope.filterView = null;
        $scope.filterViewNext = false;
        $scope.filters = {};
        $scope.mayGenerate = false;

        var Load = function () {
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
                $scope.filterViews = [
                    {
                        ID: 'city',
                        Title: $scope.customStrings['Data-NavCity'],
                        Index: 0,
                        Available: false
                    }, {
                        ID: 'zip',
                        Title: $scope.customStrings['Data-NavZip'],
                        Index: 1,
                        Available: false
                    },
                    {
                        ID: 'places',
                        Title: $scope.customStrings['Data-NavPlaces'],
                        Index: 2,
                        Available: false
                    },
                    {
                        ID: 'identifiers',
                        Title: $scope.customStrings['Data-NavIdentifiers'],
                        Index: 3,
                        Available: false
                    }
                ];
            });

            $timeout(function () {
                $scope.NextFilter();
            }, 1);
        };

        Load();

        $scope.NextFilter = function (requestedIndex) {
            LoadFilters().then(function () {                
                var index = 0;
                if (requestedIndex != undefined && requestedIndex != null) {
                    index = requestedIndex;
                }
                else {
                    var next = false;
                    angular.forEach($scope.filterViews, function (filterView, key) {
                        if (filterView.Available && next) {
                            index = key;
                            next = false;
                        }
                        if (filterView.ID == $scope.filterView) {
                            next = true;
                        }
                    });
                }
                $scope.filterView = $scope.filterViews[index].ID;
                var next = false;
                var hasNext = true;
                angular.forEach($scope.filterViews, function (filterView, key) {
                    if (filterView.Available && next) {
                        hasNext = true;
                        next = false;
                    }
                    if (filterView.ID == $scope.filterView) {
                        next = true;
                    }
                });
                $scope.filterViewNext = hasNext;
            });
        };

        var LoadFilters = function () {
            var deferred = $q.defer();
            db.Post("review", "getfilters", $scope.filters).then(function (data) {
                $scope.filters = data;
                if ($scope.filters.Zips != null && $scope.filters.Zips.length > 0) {
                    // if there are zips, then we can generate a report
                    $scope.mayGenerate = true;
                }
                angular.forEach($scope.filterViews, function (filterView, key) {
                    if (filterView.ID == 'city') {
                        if ($scope.filters.Cities != null && $scope.filters.Cities.length > 0) {
                            filterView.Available = true;
                        }
                        else {
                            filterView.Available = false;
                        }
                    }
                    if (filterView.ID == 'zip') {
                        if ($scope.filters.Zips != null && $scope.filters.Zips.length > 0) {
                            filterView.Available = true;
                        }
                        else {
                            filterView.Available = false;
                        }
                    }
                    if (filterView.ID == 'places') {
                        if ($scope.filters.Places != null && $scope.filters.Places.length > 0) {
                            filterView.Available = true;
                        }
                        else {
                            filterView.Available = false;
                        }
                    }
                    if (filterView.ID == 'identifiers') {
                        if ($scope.filters.Descriptors != null && $scope.filters.Descriptors.length > 0) {
                            filterView.Available = true;
                        }
                        else {
                            filterView.Available = false;
                        }
                    }
                });
                deferred.resolve();
            });
            return deferred.promise;
        };

        $scope.Select = function (item) {
            if (item.Selected == true) {
                item.Selected = false;
            }
            else {
                item.Selected = true;
            }
        };

        $scope.GenerateReport = function () {
            var filters = {
                Descriptors: [],
                Places: [],
                Cities: [],
                Zips: []
            };
            angular.forEach($scope.filters.Cities, function (item) {
                if (item.Selected) {
                    filters.Cities.push(item);
                }
            });
            angular.forEach($scope.filters.Zips, function (item) {
                if (item.Selected) {
                    filters.Zips.push(item);
                }
            });
            angular.forEach($scope.filters.Places, function (item) {
                if (item.Selected) {
                    filters.Places.push(item);
                }
            });
            angular.forEach($scope.filters.Descriptors, function (item) {
                if (item.Selected) {
                    filters.Descriptors.push(item);
                }
            });
            db.Post("review", "summary", filters).then(function (data) {
                $scope.entity = data;
            });
        };

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    DataIndex.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$q"];
    app.controller("DataIndex", DataIndex);
}(angular.module("app")));