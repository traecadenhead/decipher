(function (app) {
    var db = function ($http, oh, root, $q, $rootScope) {

        var Post = function (controller, action, entity) {
            var deferred = $q.defer();
            entity.Language = GetLanguage();
            var req = {
                method: 'POST',
                url: root.GetBaseUrl() + '/api/' + controller + '/' + action,
                data: entity
            };
            $http(req).then(function (d) {
                deferred.resolve(d.data);
            });
            return deferred.promise;
        }

        var Save = function (type, entity, subset) {
            var deferred = $q.defer();
            if (subset == undefined || subset == null) {
                subset = '';
            }
            entity.Language = GetLanguage();
            var req = {
                method: 'PUT',
                url: root.GetBaseUrl() + '/api/' + type + '/save' + subset,
                data: entity
            };
            $http(req).then(function (d) {
                deferred.resolve(d.data);
            });
            return deferred.promise;
        };

        var Delete = function (type, id, subset) {
            if (subset == undefined || subset == null) {
                subset = '';
            }
            var url = root.GetBaseUrl() + '/api/' + type + '/delete';
            if (subset != '') {
                url += subset;
            }
            url += '?id=' + id;
            var req = {
                method: 'DELETE',
                url: url
            };
            return $http(req).then(function (d) {
                deferred.resolve(d.data);
            });
            return deferred.promise;
        };

        var Get = function (type, id, fresh, subset, additional, noStore) {
            if (subset == undefined || subset == null) {
                subset = '';
            }
            var str = "language=" + GetLanguage();
            if (id != undefined && id != null && id != '') {
                str += "&id=" + id;
            }
            if (additional != undefined && additional != null && additional != '') {
                str += "&" + additional;
            }
            var req = {
                method: 'GET',
                url: root.GetBaseUrl() + '/api/' + type + '/get' + subset + '?' + str
            };
            return DeferredGet(req, fresh, noStore);
        };

        var GetUrl = function (url) {
            var req = {
                method: 'GET',
                url: url
            };
            return DeferredGet(req, true);
        };

        var List = function (type, additional, fresh, subset, noStore) {
            if (subset == undefined || subset == null) {
                subset = '';
            }
            var str = 'language=' + GetLanguage();
            if (additional != undefined && additional != null && additional != '') {
                str += "&" + additional;
            }
            var req = {
                method: 'GET',
                url: root.GetBaseUrl() + '/api/' + type + '/list' + subset + '?' + str
            };
            return DeferredGet(req, fresh, noStore);
        };

        var Custom = function (type, action, entity, formMethod, fresh) {
            if (formMethod == undefined || formMethod == null) {
                formMethod = "PUT";
            }
            if (formMethod != undefined && formMethod != null && formMethod == "GET") {
                var req = {
                    method: formMethod,
                    url: root.GetBaseUrl() + '/api/' + type + '/' + action + '?language=' + GetLanguage() + '&' + entity
                };
                if (fresh == undefined || fresh == null) {
                    fresh = true;
                }
                return DeferredGet(req, fresh);
            }
            else {
                entity.Language = GetLanguage();
                var req = {
                    method: formMethod,
                    url: root.GetBaseUrl() + '/api/' + type + '/' + action,
                    data: entity
                };
                return $http(req);
            }
        };

        var GetSavedCalls = function () {
            var calls = amplify.store("Calls");
            if (calls == undefined || calls == null) {
                calls = [];
            }
            return calls;
        };

        var AddCall = function (url, data) {
            var calls = GetSavedCalls();
            if (calls.indexOf(url) < 0) {
                // Add call to list
                calls.push(url);
            }
            // cache for 3 hours
            amplify.store(url, data, { expires: 10800000 });
        };

        var GetCallUrl = function (method, type, id, subset, additional) {
            if (method == "Get") {
                if (subset == undefined || subset == null) {
                    subset = '';
                }
                var str = "";
                if (id != undefined && id != null && id != '') {
                    str += "id=" + id;
                }
                if (additional != undefined && additional != null && additional != '') {
                    if (str.length > 0) {
                        str += "&";
                    }
                    str += additional;
                }
                var url = root.GetBaseUrl() + '/api/' + type + '/get' + subset + '?' + str;
                return url;
            }
            return "";
        }

        var DeferredGet = function (req, fresh, noStore) {
            var deferred = $q.defer();
            var data = null;
            if (fresh == undefined || fresh == null || fresh == false) {
                // see if we already have this call cached
                data = amplify.store(req.url);
                if (data != null) {
                    // if so, return cached version
                    deferred.resolve(data);
                }
            }
            if (data == null) {
                $http(req).then(function (d) {
                    data = d.data;
                    // cache return data
                    if (noStore == undefined || noStore == null || noStore == false) {
                        AddCall(req.url, data);
                    }
                    deferred.resolve(data);
                });
            }
            // return promise
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
            Post: Post,
            Save: Save,
            Delete: Delete,
            Get: Get,
            GetUrl: GetUrl,
            List: List,
            Custom: Custom,
            GetCallUrl: GetCallUrl,
            AddCall: AddCall
        };
    };
    db.$inject = ["$http", "oh", "root", "$q", "$rootScope"];
    app.factory("db", db);
}(angular.module('app')));