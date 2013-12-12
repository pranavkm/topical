angular.module("topicalApp", [
    "ngRoute",
    "controllers",
    "services",
    "filters"
])
    .config(["$routeProvider", "$locationProvider",
        function ($routeProvider, $locationProvider) {
            $locationProvider.html5Mode(true)
                             .hashPrefix("!");
            $routeProvider
            .when("/", {
                templateUrl: "content/partials/topic-list.html",
                controller: "listTopics"
            })
            .when("/topic/create", {
                templateUrl: "content/partials/topic-create.html",
                controller: "createTopic"
            })
            .when("/topic/:topicId/:topicTitle", {
                templateUrl: "content/partials/topic-view.html",
                controller: "viewTopic"
            })
        }
    ])

