var routingFunction = function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider, $urlMatcherFactory) {
    $stateProvider
    .state('Review', {
        url: '/app/review/identify',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/identify.html';
                }
            }
        }
    })
    .state('ReviewBegin', {
        url: '/app/review/begin/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/begin.html';
                }
            }
        }
    })
    .state('ReviewQuestions', {
        url: '/app/review/questions/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/questions.html';
                }
            }
        }
    })
    .state('ReviewFinish', {
        url: '/app/review/finish/{reviewID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/finish.html';
                }
            }
        }
    })
    .state('ReviewSummary', {
        url: '/app/review/summary/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/summary.html';
                }
            }
        }
    })
    .state('ReviewDetail', {
        url: '/app/review/detail/{reviewID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/detail.html';
                }
            }
        }
    })
    .state('ReviewPick', {
        url: '/app/review/pick',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/pick.html';
                }
            }
        }
    })
    .state('Page', {
        url: '/app/page/{pageID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/home/page.html';
                }
            }
        }
    })
    .state('Home', {
        url: '/app',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/home/index.html';
                }
            }
        }
    });
};

routingFunction.$inject = ['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider', '$urlMatcherFactoryProvider'];

app.config(routingFunction);