(function () {

    controller.$inject = ['$http'];
    function controller($http) {

        let $this = this;

        $this.generate = generate;
        $this.clear = clear;
        $this.data = null;

        init();

        function init() {

            return $http.get('/umbraco/backoffice/urltracker/redirects/list', {
                params: {
                    page: 1,
                    pageSize: 100
                }
            }).then(function (response) {

                $this.data = response.data;
            });
        }

        function generate() {

            $http.post('/umbraco/backoffice/api/urltrackerredirectgenerator/generate')
                .then(init);
        }

        function clear() {

            $http.post('/umbraco/backoffice/api/urltrackerredirectgenerator/clear')
                .then(init);
        }
    }

    angular.module('umbraco')
        .controller('urltrackertest.redirectgenerator.controller', controller);

})()