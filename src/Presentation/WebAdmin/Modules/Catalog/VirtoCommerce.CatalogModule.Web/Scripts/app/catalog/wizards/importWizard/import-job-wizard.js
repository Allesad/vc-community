﻿angular.module('catalogModule.wizards.newImportJobWizard', [
       'angularFileUpload'
])
.controller('importJobWizardController', ['$scope', 'bladeNavigationService', 'dialogService', 'imports', 'FileUploader', function ($scope, bladeNavigationService, dialogService, imports, FileUploader)
{
    $scope.blade.isLoading = false;

    $scope.createItem = function ()
    {
        $scope.blade.item.$create(null,
            function (dbItem)
            {
                $scope.bladeClose();
                $scope.blade.parentBlade.refresh();
            });

    }

    $scope.updateItem = function ()
    {
        $scope.blade.item.$update(null,
            function (dbItem)
            {
                $scope.bladeClose();
                $scope.blade.parentBlade.refresh();
            });
    }

    function initialize()
    {
        if (!$scope.uploader)
        {
            // Creates a uploader
            var uploader = $scope.uploader = new FileUploader({
                scope: $scope,
                headers: { Accept: 'application/json' },
                url: 'api/assets/',
                method: 'PUT',
                autoUpload: true,
                removeAfterUpload: true
            });

            // ADDING FILTERS
            // Images only
            uploader.filters.push({
                name: 'csvFilter',
                fn: function (i /*{File|FileLikeObject}*/, options)
                {
                    var type = '|' + i.type.slice(i.type.lastIndexOf('/') + 1) + '|';
                    return '|csv|vnd.ms-excel|'.indexOf(type) !== -1;
                }
            });


            uploader.onSuccessItem = function (fileItem, asset, status, headers)
            {
                $scope.blade.item.templatePath = asset[0].url;
            };
        }
    };

    $scope.canMapColumns = function ()
    {
        return $scope.blade.item.templatePath &&
            $scope.blade.item.catalogId &&
            $scope.formScope &&
            $scope.formScope.$valid &&
            $scope.blade.item.entityImporter;
    }

    $scope.openBlade = function (type)
    {
        $scope.blade.onClose(function ()
        {
            var newBlade = null;
            switch (type)
            {
                case 'importer':
                    newBlade = {
                        id: "newProductProperties",
                        item: $scope.blade.item,
                        title: $scope.blade.item.name,
                        bladeActions: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/newProduct/new-product-wizard-ok-action.tpl.html',
                        subtitle: 'item properties',
                        controller: 'newProductWizardPropertiesController',
                        template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/item-property-detail.tpl.html'
                    };
                    break;
                case 'catalogs':
                    newBlade = {
                        id: "importJobCatalogs",
                        item: $scope.blade.item,
                        title: $scope.blade.item.name,
                        subtitle: 'Catalogs',
                        controller: 'importJobCatalogsController',
                        template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/importWizard/import-job-wizard-catalogs-step.tpl.html'
                    };
                    break;
                case 'other':
                    newBlade = {
                        id: "newProductSeoDetail",
                        seoInfos: $scope.blade.item.seoInfos,
                        title: $scope.blade.item.name,
                        bladeActions: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/newProduct/new-product-wizard-ok-action.tpl.html',
                        subtitle: 'Seo details',
                        controller: 'newProductSeoDetailController',
                        template: 'Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/blades/seo-detail.tpl.html'
                    };
                    break;
                case 'mapping':

                    break;
            }

            if (newBlade != null)
            {
                bladeNavigationService.showBlade(newBlade, $scope.blade);
            }
        });
    }


    $scope.setForm = function (form)
    {
        $scope.formScope = form;
    }

    $scope.blade.onClose = function (closeCallback)
    {

        if ($scope.blade.childrenBlades.length > 0)
        {
            var callback = function ()
            {
                if ($scope.blade.childrenBlades.length == 0)
                {
                    closeCallback();
                };
            };
            angular.forEach($scope.blade.childrenBlades, function (child)
            {
                bladeNavigationService.closeBlade(child, callback);
            });
        }
        else
        {
            closeCallback();
        }
    };

    initialize();


}]);


