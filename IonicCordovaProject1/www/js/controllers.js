angular.module('starter.controllers', [])

.controller('FormsCtrl', function ($scope, Forms) {
    $scope.forms = Forms.all();
})

.controller('EditCtrl', function ($scope, $stateParams, Forms) {
    $scope.form = Forms.get($stateParams.id);
    $scope.fields = Forms.fields($stateParams.id);
})

;
