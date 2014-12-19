﻿angular.module('catalogModule.wizards.associationWizard', [
'catalogModule.wizards.associationWizard.associationGroup'
])
.controller('associationWizardController', ['$scope', 'bladeNavigationService', 'dialogService', 'items', function ($scope, bladeNavigationService, dialogService, items) {

    $scope.create = function () {
        $scope.blade.isLoading = true;
        var entriesCopy = $scope.blade.currentEntities.slice();

        _.each($scope.blade.selection, function (item) {
            if (_.every(entriesCopy, function (x) { return x.productId != item.id; })) {
                var newEntry = {
                    name: $scope.blade.groupName,
                    productId: item.id
                };
                entriesCopy.push(newEntry);
            }
        });

        items.updateitem({ id: $scope.blade.parentBlade.currentEntityId, associations: entriesCopy }, function () {
            $scope.bladeClose();
            $scope.blade.parentBlade.refresh();
        });
    }

    $scope.openBlade = function (type) {
        $scope.blade.onClose(function () {
            var newBlade = null;
            switch (type) {
                case 'group':
                    newBlade = {
                        id: "associationGroup",
                        title: 'Association Group',
                        controller: 'associationGroupSelectController',
                        groupNames: ['Accessories', 'Related Items'],
                        // bladeActions: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/common/wizard-ok-action.tpl.html',
                        template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/newAssociation/association-wizard-group-step.tpl.html'
                    };

                    if ($scope.blade.groupName && !_.contains(newBlade.groupNames, $scope.blade.groupName)) {
                        newBlade.groupNames.splice(0, 0, $scope.blade.groupName);
                    }

                    break;
                case 'products':
                    newBlade = {
                        id: 'selectCatalog',
                        title: 'Select Catalog',
                        subtitle: 'Adding Associations to product',
                        controller: 'catalogsSelectController',
                        // bladeActions: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/common/wizard-ok-action.tpl.html',
                        template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/catalogs-select.tpl.html'
                    };

                    break;
            }

            if (newBlade != null) {
                bladeNavigationService.showBlade(newBlade, $scope.blade);
            }
        });
    }

    $scope.blade.onAfterCatalogSelected = function (selectedNode) {
        var newBlade = {
            id: 'itemsList55',
            level: 55,
            mode: 'newAssociation',
            breadcrumbs: [],
            title: 'Choose Items to associate',
            subtitle: 'Adding Associations to product',
            catalogId: selectedNode.id,
            catalog: selectedNode,
            controller: 'categoriesItemsListController',
            template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/categories-items-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    };

    $scope.blade.updateSelection = function (listItem) {
        if (listItem.selected &&
                _.every($scope.blade.selection, function (x) { return x.id != listItem.id; }) &&
                _.every($scope.blade.currentEntities, function (x) { return x.id != listItem.id; })) {
            $scope.blade.selection.push(listItem);
        } else if (!listItem.selected) {
            var existingItem = _.findWhere($scope.blade.selection, { id: listItem.id });
            if (existingItem) {
                var idx = $scope.blade.selection.indexOf(existingItem);
                $scope.blade.selection.splice(idx, 1);
            }
        }
    }

    $scope.blade.onClose = function (closeCallback) {
        closeChildrenBlades();
        closeCallback();
    };

    function closeChildrenBlades() {
        angular.forEach($scope.blade.childrenBlades.slice(), function (child) {
            bladeNavigationService.closeBlade(child);
        });
    }

    $scope.blade.selection = [];
    $scope.blade.isLoading = false;
}]);

