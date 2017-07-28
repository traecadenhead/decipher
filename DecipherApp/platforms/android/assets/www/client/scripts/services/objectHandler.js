(function (app) {
    var oh = function () {
        var ToBool = function (string) {
            if (string != undefined && string != null) {
                switch (string.toLowerCase()) {
                    case "true": case "yes": case "1": return true;
                    case "false": case "no": case "0": case null: return false;
                    default: false;
                }
            }
            else {
                return false;
            }
        }

        var ToObject = function (string) {
            try {
                return JSON.parse(string);
            }
            catch (e) {
                return null;
            }
        }

        var ToInt = function (string) {
            try {
                var i = parseInt(string);
                if (isNaN(i)) {
                    return 0;
                }
                else {
                    return i;
                }
            }
            catch (e) {
                return 0;
            }
        }

        var GetRandomNumber = function (max) {
            if (max == undefined || max == null) {
                max = 99999;
            }
            return Math.floor(Math.random() * max);
        };

        return {
            ToBool: ToBool,
            ToObject: ToObject,
            ToInt: ToInt,
            GetRandomNumber: GetRandomNumber
        };
    };
    app.factory("oh", oh);
}(angular.module('app')));