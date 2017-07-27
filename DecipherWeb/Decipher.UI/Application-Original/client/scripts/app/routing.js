var routingFunction = function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider, $urlMatcherFactory) {
    $stateProvider
    .state('Identify', {
        url: '/app-original/user/identify',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/user/identify.html';
                }
            }
        }
    })
    .state('Find', {
        url: '/app-original/find',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/find/index.html';
                }
            }
        }
    })
    .state('Data', {
        url: '/app-original/data',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/data/index.html';
                }
            }
        }
    })
    .state('ReviewSubmit', {
        url: '/app-original/review/submit/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/review/submit.html';
                }
            }
        }
    })
    .state('ReviewSummary', {
        url: '/app-original/review/summary/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/review/summary.html';
                }
            }
        }
    })
    .state('ReviewDetail', {
        url: '/app-original/review/detail/{reviewID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/review/detail.html';
                }
            }
        }
    })
    .state('Page', {
        url: '/app-original/page/{pageID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/home/page.html';
                }
            }
        }
    })
    .state('Home', {
        url: '/app-original',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application-Original/client/views/home/index.html';
                }
            }
        }
    });
};

routingFunction.$inject = ['$stateProvider', '$httpProvider', '$locationProvider', '$urlRouterProvider', '$urlMatcherFactoryProvider'];

app.config(routingFunction);