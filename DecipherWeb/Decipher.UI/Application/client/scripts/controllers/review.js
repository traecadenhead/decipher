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
                deviceSvc.Alert("alert", $scope.customStrings["Review-NoLocation"]);
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
            if ($scope.selected != null && item.PlaceID == $scope.selected.PlaceID) {
                $scope.selected = null;
            }
            else {
                $scope.selected = item;
            }
        };

        var Submit = function () {
            db.Post("place", "find", $scope.search).then(function (data) {
                $scope.result = data;
            });
        };

        $scope.Next = function () {
            if ($scope.selected != null) {
                db.Save("place", $scope.selected).then(function (result) {
                    if (result) {
                        $state.go("ReviewBegin", { "placeID": $scope.selected.PlaceID });
                    }
                    else {
                        console.log("couldn't save place");
                    }
                });                
            }
            else {
                deviceSvc.Alert("alert", $scope.customStrings["Review-NoPlace"]);
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

// Begin
(function (app) {
    var ReviewBegin = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        $scope.customStrings = [];

        var Load = function () {
            if ($stateParams.placeID != undefined && $stateParams.placeID != null && $stateParams.placeID != '' && amplify.store("UserID") != null) {
                deviceSvc.GetCustomStrings().then(function (data) {
                    angular.forEach(data, function (item) {
                        $scope.customStrings[item.CustomStringID] = item.Text;
                    });
                });
            }
            else {
                $state.go("Review");
            }
        };

        Load();

        $scope.Submit = function () {
            $state.go("ReviewQuestions", { "placeID": $stateParams.placeID });
        };

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    ReviewBegin.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewBegin", ReviewBegin);
}(angular.module("app")));

// Questions
(function (app) {
    var ReviewQuestions = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        $scope.place = null;
        $scope.question = {};
        $scope.review = null;
        $scope.view = null;
        $scope.customStrings = [];
        $scope.questionCount = 20;
        $scope.questionNum = 0;

        var Load = function () {
            if (amplify.store("UserID") == null) {
                $state.go("ReviewIdentify");
            } else if ($stateParams.placeID == undefined || $stateParams.placeID == null || $stateParams.placeID == '') {
                $state.go("Review");
            }
            else {
                db.Get("place", $stateParams.placeID, false, "forreview").then(function (data) {
                    $scope.place = data;
                    $scope.view = 'review';
                    $scope.review = {
                        PlaceID: $scope.place.PlaceID,
                        UserID: amplify.store("UserID"),
                        ReviewID: 0,
                        Responses: [],
                        Report: false
                    };
                    $scope.questionCount = $scope.place.Questions.length;                    
                    $scope.NextQuestion();
                });
            }
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
        };

        Load();

        $scope.NextQuestion = function () {
            $scope.questionNum = $scope.questionNum + 1;
            var nextIndex = 0;
            if ($scope.question != null) {
                angular.forEach($scope.place.Questions, function (item, index) {
                    if (item.QuestionID == $scope.question.QuestionID) {
                        nextIndex = index + 1;
                    }
                });
            }
            if (nextIndex < $scope.place.Questions.length) {
                $scope.question = $scope.place.Questions[nextIndex];
                if ($scope.question.index != null) {
                    $scope.question.index = 1;
                }
            }
            else
            {
                // we've reached the end
                $state.go("ReviewFinish", { "reviewID": $scope.review.ReviewID });
            }
        };

        $scope.Select = function (item) {
            if (item.Selected == true) {
                item.Selected = false;
            }
            else {
                item.Selected = true;
            }
        };

        $scope.Submit = function () {
            $scope.review.Responses = [];
            angular.forEach($scope.question.Descriptors, function (item) {
                if (item.Selected == true) {
                    var desc = {
                        DescriptorID: item.DescriptorID,
                        QuestionID: $scope.question.QuestionID,
                        Detail: $scope.question.Detail
                    };
                    $scope.review.Responses.push(desc);
                }
            });
            db.Save("review", $scope.review).then(function (result) {
                if (result > 0) {
                    $scope.review.ReviewID = result;
                    if ($scope.review.Report) {
                        $scope.review.Reported = true;
                    }
                    $scope.NextQuestion();
                }
            });
        };

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    ReviewQuestions.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewQuestions", ReviewQuestions);
}(angular.module("app")));

// Finish
(function (app) {
    var ReviewFinish = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        $scope.customStrings = [];
        $scope.review = null;

        var Load = function () {
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
            db.Get("review", $stateParams.reviewID, false, "forsubmit").then(function (data) {
                $scope.review = data;
            });            
        };

        Load();

        $scope.Select = function () {
            if ($scope.review.Reported == true) {
                $scope.review.Reported = false;
            }
            else {
                $scope.review.Reported = true;
            }
        };

        $scope.Submit = function () {
            db.Save("review", $scope.review, "submit").then(function (result) {
                if (result) {
                    $state.go("ReviewSummary", {"placeID": $scope.review.PlaceID});
                }
                else {
                    console.log("submit didn't work");
                }
            });
        };

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    ReviewFinish.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewFinish", ReviewFinish);
}(angular.module("app")));

// Detail
(function (app) {
    var ReviewDetail = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        
        $scope.entity = null;
        $scope.customStrings = [];

        var Load = function () {
            db.Get("review", $stateParams.reviewID).then(function (data) {
                $scope.entity = data;
            });
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

    ReviewDetail.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewDetail", ReviewDetail);
}(angular.module("app")));

// Review Summary
(function (app) {
    var ReviewSummary = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {

        $scope.entity = { UserDescriptors: [] };
        $scope.customStrings = [];
        $scope.showFilters = false;

        var Load = function () {
            deviceSvc.GetCustomStrings().then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.customStrings[item.CustomStringID] = item.Text;
                });
            });
            $timeout(function () {
                $scope.Select();
            }, 1);
        };

        Load();

        $scope.ToggleFilters = function () {
            if($scope.showFilters){
                $scope.showFilters = false;
            }
            else {
                $scope.showFilters = true;
            }
        };

        $scope.Select = function (item) {
            if (item != undefined && item != null) {
                if (item.Selected == true) {
                    item.Selected = false;
                }
                else {
                    item.Selected = true;
                }
            }
            var filters = {
                Descriptors: [],
                Places: [
                    { PlaceID: $stateParams.placeID }
                ]
            };
            angular.forEach($scope.entity.UserDescriptors, function (item) {
                if (item.Selected) {
                    filters.Descriptors.push(item);
                }
            });
            db.Post("review", "summary", filters).then(function (data) {
                $scope.entity = data;
            });
        };

        $scope.LoadReview = function (review) {
            $state.go("ReviewDetail", { "reviewID": review.ReviewID });
        };

        $rootScope.$on("Refresh", function () {
            Load();
        });
    };

    ReviewSummary.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewSummary", ReviewSummary);
}(angular.module("app")));