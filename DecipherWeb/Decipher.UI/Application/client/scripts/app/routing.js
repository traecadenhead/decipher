var routingFunction = function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider, $urlMatcherFactory) {
    $stateProvider
    .state('Identify', {
        url: '/app/user/identify',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/user/identify.html';
                }
            }
        }
    })
    .state('Find', {
        url: '/app/find',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/find/index.html';
                }
            }
        }
    })
    .state('Data', {
        url: '/app/data',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/data/index.html';
                }
            }
        }
    })
    .state('ReviewSubmit', {
        url: '/app/review/submit/{placeID}',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/submit.html';
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