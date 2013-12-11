angular.module("services", ["ngResource"])
       .factory("topicServices", function ($resource) {
           return {
               topic: $resource("/api/topics/:id", { id: "@id" })
           };
       });
