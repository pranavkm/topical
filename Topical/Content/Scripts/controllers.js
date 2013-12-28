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
   .controller("topicTags", ["$scope", "services", function ($scope, services) {
       $scope.onTagHover = function () {
           if (!$scope.tagDataReceived) {
               $scope.tagDataReceived = true;
               services.topic.getTags({ id: $scope.topic.id }, function (tags) {
                   $scope.tagDataReceived = true;
                   var map = [];
                   tags.forEach(function (t) {
                       map[t.tag_id] = t;
                   });
                   for (var i = 0; i < $scope.topic.tags.length; i++) {
                       $scope.topic.tags[i] = map[$scope.topic.tags[i]];
                   }
               });
           }
       }
       $scope.tagColor = function (tag) {
           if (tag.fit || tag.unfit) {
               var total = (tag.fit + tag.unfit),
                   r = Math.ceil(tag.unfit / total),
                   g = Math.ceil(tag.fit / total);

               return 'rgb(' + r + ',' + g + ',0}';
           }
           return "";
       }

       $scope.vote = function (tag, vote) {
           services.topic.voteTag({ id: $scope.topic.id, tag_id: tag.tag_id, vote: vote }, function (updatedTag) {
               var tags = $scope.topic.tags;
               for (var i = 0; i < tags.length; i++) {
                   if (tags[i].tag_id === updatedTag.tag_id) {
                       tags[i].fit = updatedTag.fit;
                       tags[i].unfit = updatedTag.unfit;
                       return;
                   }
               }
           });
       }
   }])
    