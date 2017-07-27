(function (app) {
    var deviceSvc = function (db, $rootScope, root, $q, $state, $http) {

        var Alert = function (type, message, title, buttons) {
            var deviceType = amplify.store("DeviceType");
            var deferred = $q.defer();
            if (deviceType != 'Web') {
                if (title == undefined || title == null) {
                    if (type == "confirm") {
                        title = "Your Choice";
                    }
                    else {
                        title = GetRandomAdjective();
                    }
                }
                else if (title == 'negative') {
                    title = GetRandomAdjective('negative');
                }
                if (type == 'confirm') {
                    if (buttons == undefined || buttons == null) {
                        buttons = ["OK", "Cancel"];
                    }
                    navigator.notification.confirm(
 						message,
 						function (button) {
 						    if (button == 1) {
 						        deferred.resolve(true);
 						    }
 						    else {
 						        deferred.resolve(false);
 						    }
 						},
                        title,
                        buttons
                    );
                }
                else if (type == 'prompt'){
                    if (buttons == undefined || buttons == null) {
                        buttons = ["OK", "Cancel"];
                    }
                    navigator.notification.prompt(
                        message,
                        function (result){
                            if(result.buttonIndex == 1){
                                deferred.resolve(result.input1);
                            }
                            else{
                                deferred.reject();
                            }
                        },
                        title,
                        buttons
                    );
                }
                else if (type == 'multichoice') {
                    if (buttons == undefined || buttons == null) {
                        buttons = ["OK", "Cancel"];
                    }
                    navigator.notification.confirm(
                        message,
                        function (button) {
                            deferred.resolve(buttons[button - 1]);
                        },
                        title,
                        buttons
                    );
                }
                else {
                    if (buttons == undefined || buttons == null) {
                        buttons = ["OK"];
                    }
                    navigator.notification.alert(
                		message,
                		function () {
                		    deferred.resolve(true);
                		},
                		title,
                		buttons[0]
                	);
                }
            }
            else {
                if (type == 'confirm') {
                    if (confirm(message)) {
                        deferred.resolve(true);
                    }
                    else {
                        deferred.resolve(false);
                    }
                }
                else if (type == "prompt") {
                    var answer = prompt(message);
                    if (answer != null) {
                        deferred.resolve(answer);
                    }
                    else {
                        deferred.reject();
                    }
                }
                else {
                    alert(message);
                    deferred.resolve(true);
                }
            }
            return deferred.promise;
        };

        var OpenUrl = function (url) {
            var deviceType = amplify.store("DeviceType");
            if (deviceType == 'Web') {
                var win = window.open(url, '_blank');
                win.focus();
            }
            else {
                window.open(url, '_system', 'location=yes');
            }
        };

        var CallAction = function (action, parameters) {
            $rootScope.$broadcast(action, parameters);
        };

        var GetCustomStrings = function GetCustomStrings() {
            var deferred = $q.defer();
            // For now force a fresh version of the string list
            db.List("CustomString", null, true).then(function (data) {
                deferred.resolve(data);
            });
            return deferred.promise;
        };
        
        var GetCustomString = function GetCustomString(strID) {
            var deferred = $q.defer();
            db.List("CustomString").then(function (data) {
                angular.forEach(data, function (item) {
                    if (item.CustomStringID == strID) {
                        deferred.resolve(item.Text);
                    }
                });
            });
            return deferred.promise;
        };

        var SetLanguage = function (language) {
            amplify.store("Language", language);
        };

        var GetLanguage = function () {
            if (amplify.store("Language") != null) {
                return amplify.store("Language");
            }
            else {
                return "en";
            }
        };

        return {
            Alert: Alert,
            OpenUrl: OpenUrl,
            CallAction: CallAction,
            GetCustomStrings: GetCustomStrings,
            GetCustomString: GetCustomString,
            SetLanguage: SetLanguage,
            GetLanguage: GetLanguage
        };
    };
    deviceSvc.$inject = ["db", "$rootScope", "root", "$q", "$state", "$http"];
    app.factory("deviceSvc", deviceSvc);
}(angular.module('app')));
