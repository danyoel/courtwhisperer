angular.module('starter.controllers', [])

.controller('FormsCtrl', function ($scope, Forms) {
    $scope.state = "WA";
    $scope.county = "King";
    //$scope.forms = Forms.all();

    $scope.states = Forms.states();
    $scope.counties = Forms.counties;
    $scope.byJurisdiction = Forms.byJurisdiction;
})

.controller('FormIntroCtrl', function ($scope, $stateParams, Forms) {
    $scope.id = $stateParams.id;
    $scope.form = Forms.get($stateParams.id);
})


.controller('FormItemCtrl', function ($scope, $stateParams, $state, Forms) {
    $scope.id = $stateParams.id;
    $scope.index = parseInt($stateParams.index);
    $scope.form = Forms.get($scope.id);
    $scope.item = Forms.fields($scope.id)[$scope.index];
    $scope.completePct = Math.round(100.0 * $scope.index / $scope.form.itemCount);

    $scope.skipQuestion = function () {
        $state.go("form-item", { "id": $scope.id, "index": $scope.index + 1 });
    };

    $scope.nextQuestion = function () {
        if ($scope.item.onChange) {
            $scope.item.onChange(Forms, $scope.id, $scope.item.response);
        }
        if ($scope.index < $scope.form.itemCount - 1)
            $state.go("form-item", { "id": $scope.id, "index": $scope.index + 1 });
        else
            $state.go("finish", { "id": $scope.id });
    };

    $scope.previousQuestion = function () {
        $state.go("form-item", { "id": $scope.id, "index": $scope.index - 1 });
    };

    /*$scope.finish = function () {
        $state.go("finish", { "id": $scope.id });
    }*/
})

.controller('FinishCtrl', function ($scope, $stateParams, $state, $http, $ionicPopup, Forms) {
    $scope.id = $stateParams.id;

    function getFields() {
        return Forms.fields($scope.id).map(function (field) {
            return { id: field.id, response: field.response };
        });
    }

    $scope.finish = function () {
        //$state.go("form-item", { "id": $scope.id, "index": $scope.index + 1 });
        $http({
            method: 'POST',
            url: 'https://legalvoice.azurewebsites.net/api/Form',
            data: {
                form: $scope.id,
                fields: getFields()
            }
        }).then(function (response) {
            $ionicPopup.show({
                title: 'Success',
                template: 'We\'ll email the completed form to you shortly.',
                buttons: [ { text: 'Done' } ]
            });
        },
        function (response) {
            $ionicPopup.show({
                title: 'Oops!',
                template: 'We had a problem submitting your form. The error code is ' + response.status + '.',
                buttons: [{ text: 'OK' }]
            });
        });
    };
})

;
