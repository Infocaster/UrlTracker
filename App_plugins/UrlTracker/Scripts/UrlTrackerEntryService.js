(function () {
    "use strict";
    angular.module("umbraco").factory("urlTrackerEntryService", ["$http", "$log", function ($http, $log) {
        var addRedirect = function (scope, entry) {
	        entry.is404 = false;
		    return $http({
			    url: "/umbraco/BackOffice/api/UrlTrackerManager/AddRedirect",
			    method: "POST",
			    data: entry
			}).then(function () {
			    if (scope.getItems != undefined) {
				    scope.getItems();
			    }
			}).catch(function (data) {
				$log.log(data);
				throw data.data.Message;
		    });
	    }

        var getRedirects = function (scope, skip, amount) {
            if (scope.loading != undefined) {
                scope.loading = true;
            }
            return $http({
                url: "/umbraco/BackOffice/api/UrlTrackerManager/GetRedirects",
                method: "GET",
                params: {
                    skip: skip,
                    amount: amount
                }
            }).then(function (response) {
                scope.items = response.data.Entries;
                scope.numberOfItems = response.data.NumberOfEntries;
				scope.pagination.totalPages = Math.ceil(response.data.NumberOfEntries / amount);

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
		        url: "/umbraco/BackOffice/api/UrlTrackerManager/GetNotFounds",
		        method: "GET",
		        params: {
			        skip: skip,
			        amount: amount
		        }
            }).then(function (response) {
		        scope.items = response.data.Entries;
		        scope.numberOfItems = response.data.NumberOfEntries;
				scope.pagination.totalPages = Math.ceil(response.data.NumberOfEntries / amount);

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

        var getRedirectsByFilters = function (scope, skip, amount, query, sortType = "CreatedDesc") {
	        if (scope.loading != undefined) {
		        scope.loading = true;
	        }
	        return $http({
                url: "/umbraco/BackOffice/api/UrlTrackerManager/GetRedirectsByFilter",
		        method: "GET",
		        params: {
			        skip: skip,
                    amount: amount,
                    query: query,
                    sortType: sortType
		        }
	        }).then(function (response) {
		        if (scope.loading != undefined) {
			        scope.loading = false;
                }
                
                scope.items = response.data.Entries;

                if (scope.pagination != null) {
					scope.numberOfItems = response.data.NumberOfEntries;
					scope.pagination.totalPages = Math.ceil(response.data.NumberOfEntries / amount);
                }
	        }).catch(function (data) {
		        if (scope.loading != undefined) {
			        scope.loading = false;
		        }
		        $log.log(data);
	        });
        }

        var getNotFoundsByFilters = function (scope, skip, amount, query, sortType = "LastOccurrenceDesc") {
	        if (scope.loading != undefined) {
		        scope.loading = true;
	        }
	        return $http({
                url: "/umbraco/BackOffice/api/UrlTrackerManager/GetNotFoundsByFilter",
		        method: "GET",
		        params: {
			        skip: skip,
			        amount: amount,
			        query: query,
			        sortType: sortType
		        }
	        }).then(function (response) {
		        if (scope.loading != undefined) {
			        scope.loading = false;
		        }

		        scope.items = response.data.Entries;

		        if (scope.pagination != null) {
					scope.numberOfItems = response.data.NumberOfEntries;
					scope.pagination.totalPages = Math.ceil(response.data.NumberOfEntries / amount);
		        }
	        }).catch(function (data) {
		        if (scope.loading != undefined) {
			        scope.loading = false;
		        }
		        $log.log(data);
	        });
        }

        var updateRedirect = function (scope, entry) {
            return $http({
                url: "/umbraco/BackOffice/api/UrlTrackerManager/UpdateRedirect",
                method: "POST",
                data: entry
            }).then(function () {
                if (scope.getItems != undefined) {
                    scope.getItems();
                }
            }).catch(function (data) {
	            $log.log(data);
            });
        }

        var deleteEntry = function (entryId, is404 = false) {
            return $http({
                url: "/umbraco/BackOffice/api/UrlTrackerManager/DeleteEntry?id=" + entryId + "&is404=" + is404,
                method: "POST"
            }).then(function (data) {
                return data;
            }).catch(function (data) {
				$log.log(data);
            });
        }

        var getLanguagesOutNodeDomains = function (scope, nodeId) {
	        return $http({
		        url: "/umbraco/BackOffice/api/UrlTrackerManager/GetLanguagesOutNodeDomains",
		        method: "GET",
		        params: {
			        nodeId: nodeId
		        }
	        }).then(function (response) {
		        scope.languages = response.data;
	        }).catch(function (response) {
		        $log.log(response);
	        });
		}

		var countNotFoundsThisWeek = function(scope) {
			return $http({
				url: "/umbraco/BackOffice/api/UrlTrackerManager/CountNotFoundsThisWeek",
				method: "GET",
			}).then(function (response) {
				scope.notFoundsThisWeek = response.data;
			}).catch(function (response) {
				$log.log(response);
			});
		}

        var UrlTrackerEntryService = {
            getRedirects: getRedirects,
            getNotFounds: getNotFounds,
            deleteEntry: deleteEntry,
			updateRedirect: updateRedirect,
            addRedirect: addRedirect,
            getRedirectsByFilters: getRedirectsByFilters,
			getNotFoundsByFilters: getNotFoundsByFilters,
			getLanguagesOutNodeDomains: getLanguagesOutNodeDomains,
			countNotFoundsThisWeek: countNotFoundsThisWeek
		};

        return UrlTrackerEntryService;
    }]);
})();