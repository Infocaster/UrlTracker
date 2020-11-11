(function () {
    "use strict";
    angular.module("umbraco").factory("UrlTrackerEntryService",["$http", "$log", function($http, $log) {
        var getRedirects = function (scope, skip, amount) {
            if (scope.loading != undefined) {
                scope.loading = true;
            }
            return $http({
                url: "/umbraco/api/UrlTrackerManager/GetRedirects",
                method: "GET",
                params: {
                    skip: skip,
                    amount: amount
                }
            }).then(function (response) {
                scope.items = response.data.Entries;
                scope.numberOfItems = response.data.NumberOfEntries;
                scope.pagination.totalPages = (response.data.NumberOfEntries / amount);

                if (scope.loading != undefined) {
                    scope.loading = false;
                }
            }).catch(function (data) {
	            $log.log(data);

                if (scope.loading != undefined) {
                    scope.loading = false;
                }
            });
        }

        var getNotFounds = function (scope, skip, amount) {
	        if (scope.loading != undefined) {
		        scope.loading = true;
	        }
	        return $http({
		        url: "/umbraco/api/UrlTrackerManager/GetNotFounds",
		        method: "GET",
		        params: {
			        skip: skip,
			        amount: amount
		        }
	        }).then(function (response) {
		        scope.items = response.data.Entries;
		        scope.numberOfItems = response.data.NumberOfEntries;
		        scope.pagination.totalPages = (response.data.NumberOfEntries / amount);

		        if (scope.loading != undefined) {
			        scope.loading = false;
		        }
	        }).catch(function (data) {
		        $log.log(data);

		        if (scope.loading != undefined) {
			        scope.loading = false;
		        }
	        });
        }

        var searchRedirects = function (scope, skip, amount, query) {
	        if (scope.loading != undefined) {
		        scope.loading = true;
	        }
	        return $http({
                url: "/umbraco/api/UrlTrackerManager/GetRedirectsByFilter",
		        method: "GET",
		        params: {
			        skip: skip,
                    amount: amount,
			        query: query
		        }
	        }).then(function (response) {
		        if (scope.loading != undefined) {
			        scope.loading = false;
                }

		        scope.items = response.data.Entries;
                scope.pagination.totalPages = response.data.TotalPages;
                scope.pagination.totalPages = (response.data.NumberOfEntries / amount);
	        }).catch(function (data) {
		        if (scope.loading != undefined) {
			        scope.loading = false;
		        }
		        $log.log(data);
	        });
        }

        var saveEntry = function (scope, entry) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/SaveChanges",
                method: "POST",
                data: entry
            }).then(function () {
                if (scope.getItems != undefined) {
                    scope.getItems();
                }
            }).catch(function (data) {
                $log.log(data)
            });
        }

        var createEntry = function (scope,entry) {
            return $http({
                url: "/umbraco/api/UrlTrackerManager/Create",
                method: "POST",
                data: entry
            }).then(function () {
                if (scope.getItems != undefined) {
                    scope.getItems();
                }
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

        var search = function (scope, query, skip, amount, _404 = false) {
            if (scope.loading != undefined) {
                scope.loading = true;
            }
            return $http({
                url: "/umbraco/api/UrlTrackerManager/Search",
                method: "GET",
                params: {
                    query: query,
                    skip: skip,
                    ammount: amount,
                    _404: _404
                }
            }).then(function (response) {
                if (scope.loading != undefined) {
                    scope.loading = false;
                }
                scope.items = response.data.Entries;
                scope.pagination.totalPages = response.data.TotalPages;
            }).catch(function (data) {
                if (scope.loading != undefined) {
                    scope.loading = false;
                }
                $log.log(data)
            });
        }

        var UrlTrackerEntryService = {
            getRedirects: getRedirects,
            getEntryDetails: getEntryDetails,
            deleteEntry: deleteEntry,
            saveEntry: saveEntry,
            createEntry: createEntry,
            searchRedirects: searchRedirects
        };
        return UrlTrackerEntryService;
    }]);
})();