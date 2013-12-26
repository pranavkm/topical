(function () {
    angular.module("filters", ["utils"])
        .filter("topicLink", ["utils", function (utils) {
            return function (topic) {
                return topic.url || utils.topicCommentsLink(topic);
            }
        }])
        .filter("topicCommentsLink", ["utils", function (utils) {
            return function (topic) {
                return utils.topicCommentsLink(topic);
            }
        }])
        .filter("markdown", ["$sce", function ($sce) {
            var converter = new Showdown.converter();
            return function (text) {
                return text && $sce.trustAsHtml(converter.makeHtml(text));
            }
        }])
        .filter("ago", function () {
            return function (text) {
                return text && moment(text).fromNow();
            }
        })
})();





