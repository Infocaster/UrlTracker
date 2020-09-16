(function () {
    "use strict";
    angular.module("umbraco").factory("UrlTrackerEntryService",["$http", "$log", function($http, $log) {
        var getEntries = function (scope,skip, ammount) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/Index",
                method: "GET",
                params: {
                    skip: skip,
                    ammount: ammount
                }
            }).then(function (response) {
                scope.items = response.data.Entries;
                scope.pagination.totalPages = response.data.TotalPages;
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var saveEntry = function (entry) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/SaveChanges",
                method: "POST",
                data: entry
            }).then(function () {
                //return data;
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var createEntry = function (entry) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/Create",
                method: "POST",
                data: entry
            }).then(function () {
                //return data;
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var deleteEntry = function (entryId) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/Delete?id="+entryId,
                method: "POST"
            }).then(function (data) {
                return data;
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var getEntryDetails = function (scope,entryId) {
            return $http({
                url: "api/UrlTrackerManager/Details",
                method: "GET",
                params: {
                    id: entryId
                }
            }).then(function (data) {
                scope.entry = data.response;
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var search = function (scope, query ,skip, ammount) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/Search",
                method: "GET",
                params: {
                    query: query,
                    skip: skip,
                    ammount: ammount
                }
            }).then(function (response) {
                scope.items = response.data.Entries;
                scope.pagination.totalPages = response.data.TotalPages;
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var UrlTrackerEntryService = {
            getEntries: getEntries,
            getEntryDetails: getEntryDetails,
            deleteEntry: deleteEntry,
            saveEntry: saveEntry,
            createEntry: createEntry,
            search:search
        };
        return UrlTrackerEntryService;
    }]);
})();