var routingFunction = function ($stateProvider, $httpProvider, $locationProvider, $urlRouterProvider, $urlMatcherFactory) {
    $stateProvider
    .state('ReviewIdentify', {
        url: '/app/review/identify',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/identify.html';
                }
            }
        }
    })
    .state('Review', {
        url: '/app/review',
        views: {
            "mainContainer": {
                templateUrl: function ($stateParams) {
                    return '/Application/client/views/review/index.html';
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