(function () {
    angular.module("filters", [])
        .filter("topicLink", function () {
            return function (topic) {
                return topic.url || topicCommentsLink(topic);

            }
        })
        .filter("topicCommentsLink", function () {
            return function (topic) {
                return _topicCommentsLink(topic);
            }
        })
})();





