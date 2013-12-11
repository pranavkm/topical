angular.module("controllers", [])
    .controller("listTopics", ["$scope", "topicServices", function ($scope, topicService) {
        $scope.topics = topicService.topic.query();
    }])
    .controller("viewTopic", ["$scope", "topicServices", "$routeParams", function ($scope, topicService, $routeParams) {
        $scope.topic = topicService.topic.get({ id: $routeParams.topicId });
    }])
    .controller("createTopic", ["$scope", "topicServices", function ($scope, topicService) {
        $scope.topic = { tags: [] };
        $scope.submitted = false;
        $scope.addTag = function () {
            $scope.topic.tags.push($scope.newTag);
        }
        $scope.removeTag = function (index) {
            $scope.topic.tags.splice(index, 1);
        }
        $scope.sendForm = function () {
            $scope.submitted = true;
            if ($scope.createTopicForm.$valid) {
                topicService.topic.save($scope.topic, function(savedTopic) {
                    $location.path('/topic/')
                });
            }
        }
    }])