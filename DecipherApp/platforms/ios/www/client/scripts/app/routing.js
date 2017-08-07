var routingFunction = function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider, $urlMatcherFactory) {
    $stateProvider
    .state('ReviewIdentify', {
        url: 'review/identify/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/identify.html';
                }
            }
        }
    })
    .state('ReviewBegin', {
        url: 'review/begin/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/begin.html';
                }
            }
        }
    })
    .state('ReviewQuestions', {
        url: 'review/questions/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/questions.html';
                }
            }
        }
    })
    .state('ReviewFinish', {
        url: 'review/finish/{reviewID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/finish.html';
                }
            }
        }
    })
    .state('ReviewSummary', {
        url: 'review/summary/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/summary.html';
                }
            }
        }
    })
    .state('ReviewDetail', {
        url: 'review/detail/{reviewID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/detail.html';
                }
            }
        }
    })
    .state('Review', {
        url: 'review/pick',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/review/pick.html';
                }
            }
        }
    })
    .state('Page', {
        url: 'page/{pageID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/home/page.html';
                }
            }
        }
    })
    .state('Home', {
        url: '',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return 'client/views/home/index.html';
                }
            }
        }
    });
};

routingFunction.$inject = ['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider', '$urlMatcherFactoryProvider'];

app.config(routingFunction);