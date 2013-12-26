angular.module("controllers", ["utils"])
    .controller("listTopics", ["$scope", "services", "$routeParams", function ($scope, services, $routeParams) {
        var tags = $routeParams.tags && $routeParams.tags.split(' ');
        $scope.topics = services.topic.query({ tags: tags });
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
               $scope.submitted = false;
               $scope.comment.topic_id = $scope.topic.id;
               services.comment.save($scope.comment, function (savedComment) {
                   $scope.addCommentForm.$setPristine();
                   $scope.comments.push(savedComment);
               });
           }
       }
   }])
   .controller("replyComment", ["$scope", "services", function ($scope, services) {
       $scope.startReply = function () {
           $scope.showReplyComment = true;
           return false;
       }

       $scope.sendForm = function () {
           $scope.submitted = true;
           if ($scope.replyCommentForm.$valid) {
               $scope.submitted = false;
               $scope.replyComment.topic_id = $scope.topic.id;
               $scope.replyComment.parent_id = $scope.comment.id;
               services.comment.save($scope.replyComment, function (savedComment) {
                   $scope.showReplyComment = false;
                   $scope.replyCommentForm.$setPristine();
                   if (!$scope.comment.children) {
                       $scope.comment.children = [];
                   }
                   $scope.comment.children.push(savedComment);
               });
           }
       }
   }])