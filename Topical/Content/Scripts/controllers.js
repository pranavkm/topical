angular.module("controllers", ["utils"])
    .controller("listTopics", ["$scope", "topicServices", function ($scope, topicService) {
        $scope.topics = topicService.topic.query();
    }])
    .controller("createTopic", ["$scope", "$location", "topicServices", "utils", function ($scope, $location, topicService, utils) {
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
                topicService.topic.save($scope.topic, function (savedTopic) {
                    $location.path(utils.topicCommentsLink(savedTopic));
                });
            }
        }
    }])
   .controller("viewTopic", ["$scope", "topicServices", "$routeParams", function ($scope, topicService, $routeParams) {
       $scope.topic = topicService.topic.get({ id: $routeParams.topicId });
   }])
   .controller("addComment", ["$scope", function ($scope) {
       $scope.sendForm = function () {
           $scope.submitted = true;
           if ($scope.createTopicForm.$valid) {
               topicService.comment.save($scope.topic, function (savedTopic) {
                   $location.path(utils.topicCommentsLink(savedTopic));
               });
           }
       }
   }])