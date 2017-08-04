(function (app) {
    var ga = function (db, $rootScope, root, $q, $state, $analytics) {

        var TrackEvent = function(category, action, label, value){
            $analytics.eventTrack(action, { category: category, label: label });
        };

        var TrackScreen = function(screen){
            // don't do anything - we'll lead angulartics handle this with its defaults
        };

        return {
            TrackEvent: TrackEvent,
            TrackScreen: TrackScreen
        };
    };
    ga.$inject = ["db", "$rootScope", "root", "$q", "$state", "$analytics"];
    app.factory("ga", ga);
}(angular.module('app')));
