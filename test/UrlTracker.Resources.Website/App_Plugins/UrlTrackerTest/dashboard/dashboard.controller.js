(function () {

    controller.$inject = ['$http', '$location'];
    function controller($http, $location) {

        let $this = this;
        $this.loading = 0;
        $this.redactionScores = [];
        $this.rsconstant = 1;
        $this.vsconstant = 1;
        $this.tsconstant = 1;
        $this.results = [];

        $this.dateTimeConfig = {

            enableTime: true,
            dateFormat: "d/m/Y H:i",
            time_24hr: true
        };

        $this.newUrl = "";
        $this.newVariableScore = 1;
        let curDate = new Date();
        $this.newUpdateDate = `${curDate.getDate()}/${curDate.getMonth()}/${curDate.getFullYear()} ${curDate.getHours()}:${curDate.getMinutes}`;

        $this.update = update;
        $this.fetch = fetch;
        $this.getStrategy = getStrategy;
        $this.getScore = getScore;
        $this.getCurrentLocation = getCurrentLocation;
        $this.submit = submit;
        $this.clear = clear;
        $this.dtChange = dtChange;
        $this.seed = seed;
        $this.getDayDiff = getDayDiff;

        init();

        function seed() {
            $this.loading++;
            var baseUrl = getCurrentLocation();
            $http.post(`/umbraco/backoffice/api/urltrackertest/generaterandomrecommendations`, { baseurl: baseUrl })
                .then(function (result) {
                    fetch();
                })
                .finally(function () {
                    $this.loading--;
                });
        }

        function clear() {
            $http.post(`/umbraco/backoffice/api/urltrackertest/clearrecommendations`)
                .then(function (result) {
                    fetch();
                });
        }

        function update(key, score) {
            $http.post(`/umbraco/backoffice/api/urltrackertest/setredactionscore?id=${key}`, score)
                .then(function (result) {
                    fetch();
                });
        }

        function fetch() {
            $this.loading++;
            $http.get(`/umbraco/backoffice/api/urltrackertest/getresults?c1=${$this.rsconstant}&c2=${$this.vsconstant}&c3=${$this.tsconstant}`)
                .then(function (result) {

                    $this.results = result.data;
                })
                .finally(function () {
                    $this.loading--;
                });
        }

        function dtChange(selectedDates, dateStr, instance) {

            $this.newUpdateDate = dateStr;
        }

        function submit() {

            let url = getCurrentLocation() + $this.newUrl;
            if (!url.endsWith('/')) url += '/';

            let variableScore = $this.newVariableScore;
            let date = convertToDateTime($this.newUpdateDate);

            $http.post('/umbraco/backoffice/api/urltrackertest/setrecommendation', {
                Url: url,
                Visits: variableScore,
                DateTime: date.toISOString()
            }).then(function (result) {

                fetch();
            });
        }

        function getStrategy(recommendation) {
            return $this.redactionScores.find(function (e) { return e.Id === recommendation.Strategy });
        }

        function getDayDiff(recommendation) {

            let date = Date.parse(recommendation.UpdateDate);
            return Math.round((new Date() - date) / (1000 * 60 * 60 * 24));
        }

        function getScore(recommendation) {
            let strategy = getStrategy(recommendation);
            let daydiff = getDayDiff(recommendation);
            return ((($this.vsconstant * recommendation.VariableScore) +
                ($this.rsconstant * strategy.Score)) *
                (0.5 ** (daydiff / $this.tsconstant))).toFixed(4);
        }

        function getCurrentLocation() {

            let protocol = $location.protocol();
            let host = $location.host();
            let port = $location.port();

            let url = protocol + "://" + host;
            if (port != 80 && port != 443) url += ":" + port;

            url += "/"
            return url;
        }

        function convertToDateTime(sDateTime) {
            sDateTime = sDateTime.split(" ");

            var date = sDateTime[0].split("/");
            var yyyy = date[2];
            var mm = date[1] - 1;
            var dd = date[0];

            var time = sDateTime[1].split(":");
            var h = time[0];
            var m = time[1];

            return new Date(yyyy, mm, dd, h, m, 0);
        }

        function init() {
            $this.loading++;

            $http.get('/umbraco/backoffice/api/urltrackertest/getredactionscores')
                .then(function (result) {

                    $this.redactionScores = result.data;
                    fetch();
                })
                .finally(function () {
                    $this.loading--;
                })
        }
    }

    angular.module('umbraco').controller('urltrackertest.controller', controller);
})();