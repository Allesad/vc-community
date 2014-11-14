﻿angular.module('catalogModule.blades.catalogsList', [
   'catalogModule.resources.catalogs'
])
.controller('catalogsListController', ['$injector', '$rootScope', '$scope', 'catalogs', 'bladeNavigationService', 'dialogService',
function ($injector, $rootScope, $scope, catalogs, bladeNavigationService, dialogService)
{
    var selectedNode = null;
    var preventCategoryListingOnce;

    $scope.blade.refresh = function ()
    {
        $scope.blade.isLoading = true;

        var searchResult = catalogs.getCatalogs({}, function (results)
        {
            $scope.blade.isLoading = false;

            $scope.objects = results;

            if (selectedNode != null)
            {
                //select the node in the new list
                angular.forEach(results, function (node)
                {
                    if (selectedNode.id === node.id)
                    {
                        selectedNode = node;
                    }
                });
            }
            setBreadcrumps();
        });

    };

    $scope.blade.onClose = function (closeCallback) {
    	if ($scope.blade.childrenBlades.length > 0) {
    		var callback = function () {
    			if ($scope.blade.childrenBlades.length == 0) {
    				closeCallback();
    			};
    		};
    		angular.forEach($scope.blade.childrenBlades, function (child) {
    			bladeNavigationService.closeBlade(child, callback);
    		});
    	}
    	else {
    		closeCallback();
    	}
    };

	//Breadcrumps
    function setBreadcrumps() {
    	//Clone array (angular.copy leave a same reference)
    	$scope.blade.breadcrumbs = $scope.blade.breadcrumbs.slice(0);

    	var breadCrumb = {
    		id: "Catalogs",
    		name: "Catalogs",
    	};

    	//prevent dublicate items
    	if (!_.some($scope.blade.breadcrumbs, function (x) { return x.id == breadCrumb.id })) {
    		$scope.blade.breadcrumbs.push(breadCrumb);
    	}

    	breadCrumb.navigate = function (breadcrumb) {
    		bladeNavigationService.closeBlade($scope.blade,
			function () {
				bladeNavigationService.showBlade($scope.blade);
				$scope.blade.refresh();
			});
    	};

    };

    $scope.refreshItems = function ()
    {
        if (preventCategoryListingOnce)
        {
            preventCategoryListingOnce = undefined;
        } else
        {
            var newBlade = {
                id: 'itemsList',
                level: 1,
                breadcrumbs: $scope.blade.breadcrumbs,
                title: 'Categories & Items',
                subtitle: 'Browsing ' + (selectedNode != null ? '"' + selectedNode.name + '"' : ''),
                catalogId: (selectedNode != null) ? selectedNode.id : null,
                catalog: selectedNode,
                controller: 'categoriesItemsListController',
                template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/categories-items-list.tpl.html',
            };
       
            bladeNavigationService.showBlade(newBlade, $scope.blade);
        }
    };

    $scope.editCatalog = function (catalog)
    {
        if (catalog.virtual)
        {
            showVirtualCatalogBlade(catalog.id, null, catalog.name);
        }
        else
        {
            showCatalogBlade(catalog.id, null, catalog.name);
        }
        preventCategoryListingOnce = true;
    };

    $scope.deleteCatalog = function (node)
    {
        var dialog = {
            id: "confirmDelete",
            title: "Delete confirmation",
            message: "Are you sure you want to delete catalog '" + node.name + "'?",
            callback: function (remove)
            {
                if (remove)
                {
                    catalogs.delete({ id: node.id }, function (data, headers)
                    {
                        $scope.blade.refresh();
                        $scope.refreshItems();
                    });
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);

        preventCategoryListingOnce = true;
    };

    function showCatalogBlade(id, data, title)
    {
        var newBlade = {
            currentEntityId: id,
            currentEntity: data,
            title: title,
            id: 'catalogEdit',
            subtitle: 'edit catalog',
            controller: 'catalogDetailController',
            template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/catalog-detail.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, $scope.blade);
    };

    function showVirtualCatalogBlade(id, data, title)
    {
        var newBlade = {
            currentEntityId: id,
            currentEntity: data,
            title: title,
            subtitle: 'Virtual catalog details',
            id: 'catalogEdit',
            controller: 'virtualCatalogDetailController',
            template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/virtual-catalog-detail.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, $scope.blade);
    };

    $scope.selectNode = function (node)
    {
        selectedNode = node;
        $scope.selectedNodeId = selectedNode.id;

        $scope.refreshItems();
    };

    $scope.bladeToolbarCommands = [
        {
            name: "Add", icon: 'icon-plus',
            executeMethod: function ()
            {
                var newBlade = {
                    id: 'listItemChild',
                    title: 'New catalog',
                    subtitle: 'choose new catalog type',
                    controller: 'catalogAddController',
                    template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/catalog-add.tpl.html'
                };

                bladeNavigationService.showBlade(newBlade, $scope.blade);
            },
            canExecuteMethod: function ()
            {
                return true;
            }
        },
    {
        name: "Manage", icon: 'icon-new-tab-2',
        executeMethod: function ()
        {
            $scope.editCatalog(selectedNode);
        },
        canExecuteMethod: function ()
        {
            return selectedNode;
        }
    },
      {
          name: "Delete", icon: 'icon-remove',
          executeMethod: function ()
          {
              $scope.deleteCatalog(selectedNode);
          },
          canExecuteMethod: function ()
          {
              return selectedNode;
          }
      }
    ];

    // actions on load
    $scope.blade.refresh();
}]);