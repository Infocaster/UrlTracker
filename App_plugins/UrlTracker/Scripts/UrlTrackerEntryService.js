(function () {
    "use strict";
    angular.module("umbraco").factory("urlTrackerEntryService", ["$http", "$log", "Upload", function ($http, $log, Upload) {
        var addRedirect = function (scope, entry) {
	        entry.is404 = false;
		    return $http({
			    url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/AddRedirect",
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

		var addIgnore404 = function (id) {
	        return $http({
		        url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/AddIgnore404",
		        method: "POST",
		        data: id
	        }).catch(function (data) {
		        $log.log(data);
		        throw data.data.Message;
	        });
        }

		var getRedirects = function (scope, skip, amount, query, sortType = "CreatedDesc") {
            if (scope.loading != undefined)
                scope.loading = true;
            if (!sortType)
	            sortType = "CreatedDesc";

            return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/GetRedirects",
                method: "GET",
                params: {
                    skip: skip,
					amount: amount,
					query: query,
					sortType: sortType
                }
            }).then(function (response) {
				scope.items = response.data.Entries;

				if (scope.pagination != null) {
					scope.numberOfItems = response.data.NumberOfEntries;
					scope.pagination.totalPages = Math.ceil(response.data.NumberOfEntries / amount);
				}

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

		var getNotFounds = function (scope, skip, amount, query, sortType = "LastOccurredDesc") {
	        if (scope.loading != undefined) 
		        scope.loading = true;
	        if (!sortType)
				sortType = "LastOccurredDesc";

			console.log(sortType);

	        return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/GetNotFounds",
		        method: "GET",
		        params: {
			        skip: skip,
			        amount: amount,
			        query: query,
			        sortType: sortType
		        }
            }).then(function (response) {
				scope.items = response.data.Entries

				if (scope.pagination != null) {
					scope.numberOfItems = response.data.NumberOfEntries;
					scope.pagination.totalPages = Math.ceil(response.data.NumberOfEntries / amount);
				}

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

        var updateRedirect = function (scope, entry) {
            return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/UpdateRedirect",
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

        var deleteEntry = function (entryId, is404 = false) {
            return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/DeleteEntry?id=" + entryId + "&is404=" + is404,
                method: "POST"
            }).then(function (data) {
                return data;
            }).catch(function (data) {
				$log.log(data);
            });
        }

        var getLanguagesOutNodeDomains = function (scope, nodeId) {
	        return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/GetLanguagesOutNodeDomains",
		        method: "GET",
		        params: {
			        nodeId: nodeId
		        }
	        }).then(function (response) {
		        scope.domainLanguages = response.data;
	        }).catch(function (response) {
		        $log.log(response);
	        });
		}

		var countNotFoundsThisWeek = function(scope) {
			return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/CountNotFoundsThisWeek",
				method: "GET",
			}).then(function (response) {
				scope.notFoundsThisWeek = response.data;
			}).catch(function (response) {
				$log.log(response);
			});
		}

		var getSettings = function (scope) {
			return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/GetSettings",
				method: "GET",
			}).then(function (response) {
				scope.settings = response.data;
			}).catch(function (response) {
				$log.log(response);
			});
		}

		var importRedirects = function (file) {
			return Upload.upload({
				url: '/umbraco/BackOffice/UrlTracker/UrlTrackerManager/ImportRedirects',
				file: file
			})
		}

		var exportRedirects = function () {
			return $http({
				url: "/umbraco/BackOffice/UrlTracker/UrlTrackerManager/ExportRedirects",
				method: "GET",
			}).then(function (response) {
				var anchor = angular.element('<a/>').css({ display: 'none' });
				angular.element(document.body).append(anchor);
				anchor.attr({
					href: 'data:attachment/csv;charset=utf-8,' + encodeURI(response.data),
					target: '_blank',
					download: `urltracker-redirects-${new Date().getFullYear()}-${new Date().getMonth()}-${new Date().getDay()}-${new Date().getHours()}-${new Date().getMinutes()}.csv`
				})[0].click();
				anchor.remove();
			}).catch(function (response) {
				$log.log(response);
			});
		}

        var UrlTrackerEntryService = {
			getRedirects: getRedirects,
			addIgnore404: addIgnore404,
            getNotFounds: getNotFounds,
            deleteEntry: deleteEntry,
			updateRedirect: updateRedirect,
            addRedirect: addRedirect,
			getLanguagesOutNodeDomains: getLanguagesOutNodeDomains,
			countNotFoundsThisWeek: countNotFoundsThisWeek,
			getSettings: getSettings,
			importRedirects: importRedirects,
			exportRedirects: exportRedirects
		};

        return UrlTrackerEntryService;
    }]);
})();