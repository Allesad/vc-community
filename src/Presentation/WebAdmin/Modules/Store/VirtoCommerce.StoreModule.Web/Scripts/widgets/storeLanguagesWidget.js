﻿angular.module('virtoCommerce.storeModule.widgets')
.controller('storeLanguagesWidgetController', ['$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.widget.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "storeChildBlade",
            itemId: blade.itemId,
            title: blade.title,
            subtitle: 'Manage languages',
            controller: 'storeLanguagesListController',
            template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/blades/store-languages-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);