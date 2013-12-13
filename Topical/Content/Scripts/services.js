angular.module("services", ["ngResource"])
       .factory("services", function ($resource) {
           return {
               topic: $resource("/api/topics/:id", { id: "@id" }),
               comment: $resource("/api/comments/:id", { id: "@id" })
           };
       });
