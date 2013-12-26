angular.module("services", ["ngResource"])
       .factory("services", function ($resource) {
           return {
               topic: $resource("/api/topics/:id", { id: "@id" }),
               comment: $resource("/api/topics/:topicId/comments/:id", { id: "@id", topicId: "@topic_id" })
           };
       });
