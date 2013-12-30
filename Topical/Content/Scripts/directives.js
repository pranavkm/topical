angular.module("directives", [])
       .directive('ngFocus', ["$timeout", function ($timeout) {
           return {
               link: function (scope, element, attrs) {
                   scope.$watch(attrs.ngFocus, function (val) {
                       if (angular.isDefined(val) && val) {
                           $timeout(function () { element[0].focus(); });
                       }
                   }, true);

                   element.bind('blur', function () {
                       if (angular.isDefined(attrs.ngFocusLost)) {
                           scope.$apply(attrs.ngFocusLost);

                       }
                   });
               }
           };
       }])
       .directive('ngCompare', function () {
           return {
               require: 'ngModel',
               link: function (scope, elem, attrs, ctrl) {
                   var password = this. $('#' + attrs.ngCompare);
                   elem.add(password).on('keyup', function () {
                       scope.$apply(function () {
                           var valid = elem.val() === password.val();
                           ctrl.$setValidity('ngCompare', valid);
                           return undefined;
                       });
                   });
               }
           };
       })