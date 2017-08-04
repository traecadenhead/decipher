(function (app) {
    var ga = function (db, $rootScope, root, $q, $state) {

        var TrackEvent = function(category, action, label, value){
            window.analytics.trackEvent(category, action, label, value);
        };

        var TrackScreen = function(screen){
            window.analytics.trackView(screen);
        };

        return {
            TrackEvent: TrackEvent,
            TrackScreen: TrackScreen
        };
    };
    ga.$inject = ["db", "$rootScope", "root", "$q", "$state"];
    app.factory("ga", ga);
}(angular.module('app')));
