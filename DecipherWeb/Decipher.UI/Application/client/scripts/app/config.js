// initialize app with angular plugins
var app = angular.module('app', ['ui.router', 'angular-loading-bar', 'ngAnimate', 'ngSanitize', 'relativePathsInPartial', 'ui.bootstrap', 'uiGmapgoogle-maps', 'angulartics', 'angulartics.google.analytics']);

// configuration for angular app
var configFunction = function ($stateProvider, $httpProvider, $locationProvider, cfpLoadingBarProvider, $urlRouterProvider, $urlMatcherFactory, $compileProvider, relativePathsInterceptorProvider, uiGmapGoogleMapApiProvider) {
    // url settings
    $urlMatcherFactory.caseInsensitive(true);
    $urlMatcherFactory.strictMode(false);
    // ignore '/Application' in URL paths
    relativePathsInterceptorProvider.setInteceptionPrefix('/Application');
    // whitelist protocols for URLs
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(http|https|itms-services|mailto):/);

    // configure Google maps
    uiGmapGoogleMapApiProvider.configure({
        key: 'AIzaSyBPDR3iDnmyKJYDSVCdAdNKJQ7vGMuiI4w'
    });

    // loading bar settings
    cfpLoadingBarProvider.includeSpinner = false;

    // enable HTML5 mode for clean URLs
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    }).hashPrefix('!');
    
}
configFunction.$inject = ['$stateProvider', '$httpProvider', '$locationProvider', 'cfpLoadingBarProvider', '$urlRouterProvider', '$urlMatcherFactoryProvider', '$compileProvider', 'relativePathsInterceptorProvider', 'uiGmapGoogleMapApiProvider'];

// apply configuration to app
app.config(configFunction);