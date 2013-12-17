angular.module("controllers", ["utils"])
    .controller("listTopics", ["$scope", "services", function ($scope, services) {
        $scope.topics = services.topic.query();
    }])
    .controller("createTopic", ["$scope", "$location", "services", "utils", function ($scope, $location, services, utils) {
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
                services.topic.save($scope.topic, function (savedTopic) {
                    $location.path(utils.topicCommentsLink(savedTopic));
                });
            }
        }
    }])
   .controller("viewTopic", ["$scope", "services", "$routeParams", function ($scope, services, $routeParams) {
       $scope.topic = services.topic.get({ id: $routeParams.topicId });
       $scope.comments = services.comment.query({ topicId: $routeParams.topicId });
   }])
   .controller("addComment", ["$scope", "services", function ($scope, services) {
       $scope.sendForm = function () {
           $scope.submitted = true;
           if ($scope.addCommentForm.$valid) {
               $scope.processing = true;
               $scope.comment.topic_id = $scope.topic.id;
               services.comment.save($scope.comment, function (savedComment) {
                   $scope.processing = false;
                   $scope.addCommentForm.$setPristine();
                   $scope.comments.push(savedComment);
               });
           }
       }
   }])