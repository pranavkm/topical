angular.module("services", ["ngResource"])
       .factory("services", function ($resource) {
           return {
               topic: $resource("/api/topics/:id", { id: "@id", tagId : "@tag_id" }, {
                   getTags: {
                       method: "GET",
                       url: "/api/topics/:id/tags/",
                       isArray: true
                   },
                   voteTag: {
                       method: "PUT",
                       url: "/api/topics/:id/tags/:tagId"
                   }
               }),
               comment: $resource("/api/topics/:topicId/comments/:id", { id: "@id", topicId: "@topic_id" }),
               user: $resource("/api/user/:id", { id: "@user_name" })
           };
       });