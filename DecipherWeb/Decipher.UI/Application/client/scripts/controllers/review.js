// Review
(function (app) {
    var ReviewSubmit = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {
        $scope.place = null;
        $scope.question = {};
        $scope.review = null;
        $scope.view = null;

        var Load = function () {
            if (amplify.store("UserID") == null) {
                $state.go("Identify");
            } else if($stateParams.placeID == undefined || $stateParams.placeID == null || $stateParams.placeID == ''){
                $state.go("Find");
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
                    console.log("get next question");
                    $scope.NextQuestion();
                });
            }
        };

        Load();

        $scope.NextQuestion = function () {
            var nextIndex = 0;
            if ($scope.question != null) {
                angular.forEach($scope.place.Questions, function (item, index) {
                    if (item.QuestionID == $scope.question.QuestionID) {
                        nextIndex = index + 1;
                    }
                });
            }
            console.log($scope.place.Questions.length + " questions");
            console.log("nextIndex: " + nextIndex);
            if (nextIndex < $scope.place.Questions.length) {
                $scope.question = $scope.place.Questions[nextIndex];
                console.log("set the question: " + $scope.question.QuestionID);
            }
            else
            {
                $scope.question = {};
                $scope.view = 'thanks';
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

        $scope.Report = function () {
            if ($scope.review.Report == true) {
                $scope.review.Report = false;
            }
            else {
                $scope.review.Report = true;
            }
        };
    };

    ReviewSubmit.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewSubmit", ReviewSubmit);
}(angular.module("app")));

// Review Details
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
    };

    ReviewDetail.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewDetail", ReviewDetail);
}(angular.module("app")));

// Review Summary
(function (app) {
    var ReviewSummary = function ($scope, db, oh, $state, root, deviceSvc, $sce, $timeout, $rootScope, $stateParams) {

        $scope.entity = { UserDescriptors: [] };

        var Load = function () {
            $timeout(function () {
                $scope.Select();
            }, 1);
        };

        Load();

        $scope.Select = function (item) {
            if (item != undefined && item != null) {
                if (item.Selected == true) {
                    console.log("deselecting");
                    item.Selected = false;
                }
                else {
                    console.log("selecting");
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
            $state.go("ReviewDetail", {"reviewID": review.ReviewID});
        };
    };

    ReviewSummary.$inject = ["$scope", "db", "oh", "$state", "root", "deviceSvc", "$sce", "$timeout", "$rootScope", "$stateParams"];
    app.controller("ReviewSummary", ReviewSummary);
}(angular.module("app")));