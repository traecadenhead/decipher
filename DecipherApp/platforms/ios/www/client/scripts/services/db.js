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
  console.log("checking connection");
  if(HasConnection()){
  console.log("found connection");
  $http(req).then(function (d) {
                  console.log(d);
                  deferred.resolve(d.data);
                  });
  }
  else{
  console.log("no connection");
  Alert("Sorry, you'll need an internet connection to use this app. Please connect and try again.");
  deferred.reject("no connection");
  }
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
  if(HasConnection()){
  $http(req).then(function (d) {
                  deferred.resolve(d.data);
                  });
  }
  else{
  Alert("Sorry, you'll need an internet connection to use this app. Please connect and try again.");
  deferred.reject("no connection");
  }
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
  if(HasConnection()){
  return $http(req).then(function (d) {
                         deferred.resolve(d.data);
                         });
  }
  else{
  Alert("Sorry, you'll need an internet connection to use this app. Please connect and try again.");
  deferred.reject("no connection");
  }
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
  if(HasConnection()){
  $http(req).then(function (d) {
                  data = d.data;
                  // cache return data
                  if (noStore == undefined || noStore == null || noStore == false) {
                  AddCall(req.url, data);
                  }
                  deferred.resolve(data);
                  });
  }
  else{
  Alert("Sorry, you'll need an internet connection to use this app. Please connect and try again.");
  deferred.reject("no connection");
  }
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
  
		var HasConnection = function () {
  var deviceType = amplify.store("DeviceType");
  if(deviceType == 'Web'){
  return navigator.onLine;
  }
  else{
  if(navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN){
  return false;
  }
  else{
  return true;
  }
  }
		};
  
		var Alert = function (message, title, buttons) {
  var deviceType = amplify.store("DeviceType");
  var deferred = $q.defer();
  if (deviceType != 'Web') {
  if (title == undefined || title == null) {
  title = "Decipher City";
  }
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
  else {
  alert(message);
  deferred.resolve(true);
  }
  return deferred.promise;
		};
  
  return {
  Post: Post,
  Save: Save,
  Delete: Delete,
  Get: Get,
  GetUrl: GetUrl,
  List: List,
  GetCallUrl: GetCallUrl,
  AddCall: AddCall
  };
  };
  db.$inject = ["$http", "oh", "root", "$q", "$rootScope"];
  app.factory("db", db);
  }(angular.module('app')));
