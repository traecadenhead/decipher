function StartReview(placeID) {
    var inj = GetInjector();
    inj.invoke(['deviceSvc', function (deviceSvc) {
        deviceSvc.CallAction('StartReview', { PlaceID: placeID });
    }]);
};

function SeeReviews(placeID) {
    var inj = GetInjector();
    inj.invoke(['deviceSvc', function (deviceSvc) {
        deviceSvc.CallAction('LoadReviews', { PlaceID: placeID });
    }]);
};

function GetInjector() {
    return angular.element(document.getElementById('app')).injector();
}